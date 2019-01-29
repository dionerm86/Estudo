using Colosoft;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Glass.Otimizacao.Negocios.Componentes
{
    /// <summary>
    /// Fluxo de negócio da otimização.
    /// </summary>
    public class OtimizacaoFluxo : IOtimizacaoFluxo
    {
        /// <summary>
        /// Obtém o repositório da solução.
        /// </summary>
        private readonly IRepositorioSolucaoOtimizacao _repositorio;

        /// <summary>
        /// Obtém o provedor do plano de corte.
        /// </summary>
        private readonly Entidades.IProvedorPlanoCorte _provedorPlanoCorte;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="OtimizacaoFluxo"/>.
        /// </summary>
        /// <param name="repositorioSolucaoOtimizacao">Repositório das soluções de otimização.</param>
        /// <param name="provedorPlanoCorte">Provedor dos planos de corte.</param>
        public OtimizacaoFluxo(
            IRepositorioSolucaoOtimizacao repositorioSolucaoOtimizacao,
            Entidades.IProvedorPlanoCorte provedorPlanoCorte)
        {
            this._repositorio = repositorioSolucaoOtimizacao;
            this._provedorPlanoCorte = provedorPlanoCorte;
        }

        /// <summary>
        /// Recupera as entradas do estoque de chapas.
        /// </summary>
        /// <param name="idsProd">Identificadores do produtos para recuperar as chapas.</param>
        /// <returns>Entrada do estoque de chapas.</returns>
        private IEnumerable<IEntradaEstoqueChapa> ObterEntradasEstoqueChapas(IEnumerable<int> idsProd)
        {
            var idsProd2 = string.Join(",", idsProd);

            if (string.IsNullOrEmpty(idsProd2))
            {
                yield break;
            }

            // Recupera os identificadores dos produtos de baixa
            var idsProdBaixa = SourceContext.Instance.CreateQuery()
                .From<Data.Model.ProdutoBaixaEstoque>()
                .Where($"IdProd IN ({idsProd2})")
                .SelectDistinct("IdProdBaixa")
                .Execute()
                .Select(f => f.GetInt32(0))
                .ToList();

            var consultas = new List<Colosoft.Query.Queryable>();

            // Verifica se foram encontrados produtos de baixa para localizar os retalhos
            if (idsProdBaixa.Any())
            {
                var idsProdBaixa2 = string.Join(",", idsProdBaixa);

                consultas.Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.RetalhoProducao>("r")
                    .InnerJoin<Data.Model.Produto>("r.IdProd=p.IdProd", "p")
                    .InnerJoin<Data.Model.Produto>("p.IdProd=pref.IdProd OR p.IdProdOrig=pref.IdProd OR p.IdProdBase=pref.IdProd", "pref")
                    .LeftJoin<Data.Model.ProdutoPedidoProducao>("r.IdProdPedProducaoOrig=ppp.IdProdPedProducao", "ppp")
                    .LeftJoin(
                        SourceContext.Instance.CreateQuery()
                            .From<Data.Model.UsoRetalhoProducao>("ur1")
                            .GroupBy("ur1.IdRetalhoProducao")
                            .Select("MIN(ur1.IdUsoRetalhoProducao) AS IdUsoRetalhoProducao, ur1.IdRetalhoProducao"),
                        "r.IdRetalhoProducao=ur.IdRetalhoProducao",
                        "ur")
                    .Where($"r.Situacao=?situacaoRetalho AND pref.IdProd IN({idsProdBaixa2})")
                    .GroupBy("r.IdProd")
                    .Select("MIN(pref.CodOtimizacao) AS CodMaterial, MIN(p.Altura) AS Altura, MIN(p.Largura) AS Largura, COUNT(r.IdProd) AS Qtde, 1 AS Retalho"));
            }

            idsProd2 = string.Join(",", idsProd.Concat(idsProdBaixa));

            consultas.Add(SourceContext.Instance.CreateQuery()
                .From<Data.Model.Produto>("p")
                .InnerJoin<Data.Model.Produto>("p.IdProd=pref.IdProd OR p.IdProdOrig=pref.IdProd OR p.IdProdBase=pref.IdProd", "pref")
                .InnerJoin<Data.Model.SubgrupoProd>("p.IdSubgrupoProd=sg.IdSubgrupoProd AND (sg.TipoSubgrupo=?tipoChapaVidro OR sg.TipoSubgrupo=?tipoChapaVidroLaminado)", "sg")
                .LeftJoin<Data.Model.RetalhoProducao>("p.IdProd=rp.IdProd AND rp.Situacao=?situacaoRetalho", "rp")
                .InnerJoin<Data.Model.ProdutoLoja>("p.IdProd=pl.IdProd", "pl")
                .LeftJoin<Data.Model.Produto>("p.IdProdBase=pbase.IdProd", "pbase")
                .LeftJoin<Data.Model.Produto>("p.IdProdOrig=pbase.IdProd", "porig")
                .Where($"rp.IdRetalhoProducao IS NULL AND pref.IdProd IN({idsProd2})")
                .GroupBy("Altura, Largura")
                .Having("SUM(pl.QtdEstoque) > 0")
                .OrderBy("CodMaterial")
                .Add("?tipoChapaVidro", Data.Model.TipoSubgrupoProd.ChapasVidro)
                .Add("?tipoChapaVidroLaminado", Data.Model.TipoSubgrupoProd.ChapasVidroLaminado)
                .Select(@"MIN(pref.CodOtimizacao) AS CodMaterial,
                         p.Altura, p.Largura, SUM(pl.QtdEstoque) AS Qtde, 0 AS Retalho"));

            var consulta = consultas.First();

            if (consultas.Count > 1)
            {
                consulta.UnionAll(consultas[1]);
            }

            consulta.Add("?situacaoRetalho", Data.Model.SituacaoRetalhoProducao.Disponivel);

            var posicao = 0;

            // Carrega as entradas do estoque
            var entradas = consulta.Execute()
                .Select(f => new EntradaEstoqueChapa
                {
                    CodigoMaterial = f["CodMaterial"],
                    Posicao = posicao++,
                    Quantidade = f["Qtde"],
                    Largura = f["Largura"],
                    Altura = f["Altura"],
                    Retalho = f["Retalho"],
                    TipoCorte = TipoCorteTransversal.XY,
                    Prioridade = f.GetBoolean("Retalho") ? 999 : 0,
                }).ToList();

            foreach (var i in entradas)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Recupera os materiais.
        /// </summary>
        /// <param name="idsProd">Identificadores do produtos para recuperar os materiais.</param>
        /// <returns>Materiais associados com os produtos.</returns>
        private IEnumerable<IMaterial> ObterMateriais(IEnumerable<int> idsProd)
        {
            var idsProd2 = string.Join(",", idsProd);

            if (string.IsNullOrEmpty(idsProd2))
            {
                yield break;
            }

            var materiais = SourceContext.Instance.CreateQuery()
                .From<Data.Model.Produto>("p")
                .Where($"p.TipoMercadoria=?tipo AND p.IdProd IN({idsProd2})")
                .Add("?tipo", Data.Model.TipoMercadoria.MateriaPrima)
                .Select(@"p.CodOtimizacao, p.Descricao, p.Espessura,
                          p.RecorteX1, p.RecorteY1, p.RecorteX2, p.RecorteY2,
                          p.TransversalMaxX, p.TransversalMaxY,
                          p.DesperdicioMinX, p.DesperdicioMinY,
                          p.DistanciaMin, p.RecorteAutomaticoForma,
                          p.AnguloRecorteAutomatico")
                .Execute()
                .Select(f => new Material
                {
                    Codigo = f["CodOtimizacao"],
                    Descricao = f["Descricao"],
                    Tipo = TipoMaterial.Monolitico,
                    Espessura1 = f["Espessura"],
                    RecorteX1 = f["RecorteX1"],
                    RecorteY1 = f["RecorteY1"],
                    RecorteX2 = f["RecorteX2"],
                    RecorteY2 = f["RecorteY2"],
                    TransversalMaxX = f["TransversalMaxX"],
                    TransversalMaxY = f["TransversalMaxY"],
                    DesperdicioMinX = f["DesperdicioMinX"],
                    DesperdicioMinY = f["DesperdicioMinY"],
                    DistanciaMin = f["DistanciaMin"],
                    RecorteAutomaticoForma = f["RecorteAutomaticoForma"],
                    AnguloRecorteAutomatico = f["AnguloRecorteAutomatico"],
                }).ToList();

            foreach (var i in materiais)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Obtém o documento com os dados de etiqueta da solução de otimização.
        /// </summary>
        /// <param name="solucaoOtimizacao">Solução de otimização.</param>
        /// <returns>Documento de etiquetas da solução.</returns>
        private eCutter.DocumentoEtiquetas ObterDocumentoEtiquetas(Entidades.SolucaoOtimizacao solucaoOtimizacao)
        {
            foreach (var arquivo in this._repositorio.ObterArquivos(solucaoOtimizacao))
            {
                if (StringComparer.InvariantCultureIgnoreCase.Equals(
                    System.IO.Path.GetExtension(arquivo.Nome), ".optlbl"))
                {
                    using (var conteudo = arquivo.Abrir())
                    {
                        return eCutter.DocumentoEtiquetas.Open(conteudo);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Verifica se existe uma solução de otimização configurada para o arquivo de otimização.
        /// </summary>
        /// <param name="idArquivoOtimizacao">Identificador do arquiv de otimização.</param>
        /// <returns>Identifica se o arquivo possui uma solução de otimizaçao.</returns>
        public bool PossuiSolucaoOtimizacao(int idArquivoOtimizacao)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.SolucaoOtimizacao>()
                .Where("IdArquivoOtimizacao=?id")
                .Add("?id", idArquivoOtimizacao)
                .ExistsResult();
        }

        /// <summary>
        /// Obtém a solução de otimização pelo arquivo de otimização.
        /// </summary>
        /// <param name="idArquivoOtimizacao">Identificador do arquivo de otimização.</param>
        /// <returns>Solução de otimização associada com o arquivo.</returns>
        public Entidades.SolucaoOtimizacao ObterSolucaoOtimizacaoPelaArquivoOtimizacao(int idArquivoOtimizacao)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.SolucaoOtimizacao>()
                .Where("IdArquivoOtimizacao=?id")
                .Add("?id", idArquivoOtimizacao)
                .ProcessResult<Entidades.SolucaoOtimizacao>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Obtém a solução de otimização.
        /// </summary>
        /// <param name="idSolucaoOtimizacao">Identificador da solução.</param>
        /// <returns>Solução de otimização associada com o id.</returns>
        public Entidades.SolucaoOtimizacao ObterSolucaoOtimizacao(int idSolucaoOtimizacao)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.SolucaoOtimizacao>()
                .Where("IdSolucaoOtimizacao=?id")
                .Add("?id", idSolucaoOtimizacao)
                .ProcessResult<Entidades.SolucaoOtimizacao>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Recupera a sessão de otimização associado com o identificador do arquivo de otimização.
        /// </summary>
        /// <param name="idArquivoOtimizacao">Identificador do arquivo base para a sessão de otimização.</param>
        /// <returns>Sessão de otimização associada com o arquivo.</returns>
        public ISessaoOtimizacao ObterSessaoOtimizacao(int idArquivoOtimizacao)
        {
            // Recupera os identificadores dos produtos associado com o arquivo de otimização
            var idsProd = SourceContext.Instance.CreateQuery()
                .From<Data.Model.EtiquetaArquivoOtimizacao>("eao")
                .InnerJoin<Data.Model.ProdutosPedidoEspelho>("eao.IdPedido=ppe.IdPedido", "ppe")
                .InnerJoin<Data.Model.ProdutoPedidoProducao>("ppe.IdProdPed=ppp.IdProdPed AND ppp.NumEtiqueta=eao.NumEtiqueta", "ppp")
                .SelectDistinct("ppe.IdProd")
                .Where("IdArquivoOtimiz=?id")
                .Add("?id", idArquivoOtimizacao)
                .Execute()
                .Select(f => f.GetInt32(0))
                .ToList();

            IEnumerable<IEntradaEstoqueChapa> entradasEstoqueChapa;

            if (Configuracoes.OtimizacaoConfig.TipoEstoqueChapas == Data.Helper.DataSources.TipoEstoqueChapasOtimizacaoEnum.Interno)
            {
                entradasEstoqueChapa = this.ObterEntradasEstoqueChapas(idsProd);
            }
            else
            {
                entradasEstoqueChapa = new IEntradaEstoqueChapa[0];
            }

            var estoque = new EstoqueChapa(this.ObterMateriais(idsProd), entradasEstoqueChapa);

            return new SessaoOtimizacao(estoque, new IPecaPadrao[0]);
        }

        /// <summary>
        /// Realiza a importação do resultado de uma otimização.
        /// </summary>
        /// <param name="idArquivoOtimizacao">Identificador do arquivo de otimização.</param>
        /// <param name="arquivos">Arquivos da otimização.</param>
        /// <returns>Resultado da importação da otimização.</returns>
        public ImportacaoOtimizacao Importar(int idArquivoOtimizacao, IEnumerable<IArquivoSolucaoOtimizacao> arquivos)
        {
            var solucaoOtimizacao = SourceContext.Instance.CreateQuery()
                .From<Data.Model.SolucaoOtimizacao>()
                .Where("IdArquivoOtimizacao=?id")
                .Add("?id", idArquivoOtimizacao)
                .ProcessResult<Entidades.SolucaoOtimizacao>()
                .FirstOrDefault();

            var importacaoNova = solucaoOtimizacao == null;
            if (importacaoNova)
            {
                solucaoOtimizacao = new Entidades.SolucaoOtimizacao()
                {
                    IdArquivoOtimizacao = idArquivoOtimizacao,
                };
            }

            this._repositorio.SalvarArquivos(solucaoOtimizacao, arquivos);

            var documentoEtiquetas = this.ObterDocumentoEtiquetas(solucaoOtimizacao);
            var conversor = new ConversorSolucaoOtimizacao(solucaoOtimizacao);
            conversor.Executar(documentoEtiquetas);

            using (var session = SourceContext.Instance.CreateSession())
            {
                conversor.Salvar(session).ThrowInvalid();
                session.Execute(true);
            }

            return new ImportacaoOtimizacao(solucaoOtimizacao);
        }

        /// <summary>
        /// Obtém os itens da otimização base no identificador da solução de otimização.
        /// </summary>
        /// <param name="idSolucaoOtimizacao">Identificador da solução de otimização associada.</param>
        /// <returns>Itens de otimização associado com a solução.</returns>
        public IEnumerable<ItemOtimizacao> ObterItensPelaSolucao(int idSolucaoOtimizacao)
        {
            var etiquetasPeca = SourceContext.Instance.CreateQuery()
                .From<Data.Model.SolucaoOtimizacao>("so")
                .InnerJoin<Data.Model.PlanoOtimizacao>("so.IdSolucaoOtimizacao=po.IdSolucaoOtimizacao", "po")
                .InnerJoin<Data.Model.PlanoCorte>("po.IdPlanoOtimizacao=pc.IdPlanoOtimizacao", "pc")
                .InnerJoin<Data.Model.PecaPlanoCorte>("pc.IdPlanoCorte=ppc.IdPlanoCorte", "ppc")
                .InnerJoin<Data.Model.ProdutoPedidoProducao>("ppp.IdProdPedProducao=ppc.IdProdPedProducao", "ppp")
                .InnerJoin<Data.Model.ProdutosPedidoEspelho>("ppe.IdProdPed=ppp.IdProdPed", "ppe")
                .InnerJoin<Data.Model.Produto>("p.IdProd=ppe.IdProd", "p")
                .InnerJoin<Data.Model.CorVidro>("p.IdCorVidro=cv.IdCorVidro", "cv")
                .InnerJoin<Data.Model.EtiquetaProcesso>("ep.IdProcesso=ppe.IdProcesso", "ep")
                .InnerJoin<Data.Model.EtiquetaAplicacao>("ea.IdAplicacao=ppe.IdAplicacao", "ea")
                .Where("so.IdSolucaoOtimizacao=?id")
                .Add("?id", idSolucaoOtimizacao)
                .Select(@"ppe.IdProdPed, ppe.IdPedido, p.Descricao AS DescricaoProduto,
                         ppp.PecaReposta, ep.CodInterno AS CodProcesso, ea.CodInterno AS CodAplicacao,
                         ppe.Qtde, ppe.QtdImpresso, ppe.AlturaReal, ppe.Altura, ppe.LarguraReal, ppe.Largura,
                         ppe.Obs, ppe.TotM, ppe.TotM2Calc, ppp.NumEtiqueta,
                         po.IdPlanoOtimizacao, pc.IdPlanoCorte, po.Nome AS PlanoOtimizacao, pc.Posicao AS PosicaoPlanoCorte,
                         cv.Sigla AS Cor,
                         ppe.Espessura")
                .Execute()
                .Select(f => new
                {
                    IdProdPed = f.GetInt32("IdProdPed"),
                    IdPedido = f.GetInt32("IdPedido"),
                    DescricaoProduto = f.GetString("DescricaoProduto"),
                    PecaReposta = f.GetBoolean("PecaReposta"),
                    CodProcesso = f.GetString("CodProcesso"),
                    CodAplicacao = f.GetString("CodAplicacao"),
                    Qtde = f.GetInt32("Qtde"),
                    QtdImpresso = f.GetInt32("QtdImpresso"),
                    AlturaReal = f.GetFloat("AlturaReal"),
                    Altura = f.GetFloat("Altura"),
                    LarguraReal = f.GetInt32("LarguraReal"),
                    Largura = f.GetInt32("Largura"),
                    Obs = f.GetString("Obs"),
                    TotM = f.GetFloat("TotM"),
                    TotM2Calc = f.GetFloat("TotM2Calc"),
                    NumEtiqueta = f.GetString("NumEtiqueta"),
                    IdPlanoOtimizacao = f.GetInt32("IdPlanoOtimizacao"),
                    IdPlanoCorte = f.GetInt32("IdPlanoCorte"),
                    PlanoOtimizacao = f.GetString("PlanoOtimizacao"),
                    PosicaoPlanoCorte = f.GetInt32("PosicaoPlanoCorte"),
                    Cor = f.GetString("Cor"),
                }).ToList();

            var planosOtimizacao = new Dictionary<int, int>();

            // Obtém a relação da quantidade de planos de corte por planos de otimização
            foreach (var i in
               etiquetasPeca
                .GroupBy(f => f.IdPlanoOtimizacao)
                .Select(f => new
                {
                    IdPlanoOtimizacao = f.Key,
                    QuantidePlanosCorte = f.GroupBy(x => x.IdPlanoCorte).Count(),
                }))
            {
                planosOtimizacao.Add(i.IdPlanoOtimizacao, i.QuantidePlanosCorte);
            }

            var planosCorte = etiquetasPeca
                .GroupBy(f => $"{f.IdPlanoOtimizacao}|{f.IdPlanoCorte}")
                .Select(f => f.First())
                .Select(f => new
                {
                    IdPlanoCorte = f.IdPlanoCorte,
                    PlanoCorte = this._provedorPlanoCorte.ObterNumeroEtiqueta(f.PlanoOtimizacao, f.PosicaoPlanoCorte, planosOtimizacao[f.IdPlanoOtimizacao]),
                }).ToList();

            var pecas = etiquetasPeca
                .GroupBy(f => $"{f.IdProdPed}|{f.PlanoOtimizacao}|{f.PosicaoPlanoCorte}|{f.PecaReposta}")
                .Select(grupoProdPed =>
                {
                    var item = grupoProdPed.First();

                    // Calcula a quantidade a imprimir
                    var qtdAImprimir = !item.PecaReposta ? grupoProdPed.Count() : 0;

                    float totM2 = item.PecaReposta ? (item.TotM / item.Qtde) : (item.TotM / item.Qtde) * qtdAImprimir;
                    float totM2Calc = item.PecaReposta ? (item.TotM2Calc / item.Qtde) : (item.TotM2Calc / item.Qtde) * qtdAImprimir;

                    return new ItemOtimizacao
                    {
                        Tipo = TipoItemOtimizacao.Peca,
                        IdProdPed = item.IdProdPed,
                        IdPedido = item.IdPedido,
                        DescricaoProduto = item.DescricaoProduto,
                        PecaReposta = item.PecaReposta,
                        CodProcesso = item.CodProcesso,
                        CodAplicacao = item.CodAplicacao,
                        Qtde = item.Qtde,
                        QtdImpresso = item.QtdImpresso,
                        QtdAImprimir = qtdAImprimir,
                        AlturaProducao = item.AlturaReal > 0f ? item.AlturaReal : item.Altura,
                        LarguraProducao = item.LarguraReal > 0 ? item.LarguraReal : item.Largura,
                        Obs = item.Obs,
                        TotM2 = (float)Math.Round(totM2, 3),
                        TotM2Calc = (float)Math.Round(totM2Calc, 3),
                        PlanoCorteEtiqueta = planosCorte.First(f => f.IdPlanoCorte == item.IdPlanoCorte).PlanoCorte,
                        Etiquetas = grupoProdPed.Select(f => f.NumEtiqueta).ToList(),
                    };
                });

            var etiquetasRetalho = SourceContext.Instance.CreateQuery()
                .From<Data.Model.SolucaoOtimizacao>("so")
                .InnerJoin<Data.Model.PlanoOtimizacao>("so.IdSolucaoOtimizacao=po.IdSolucaoOtimizacao", "po")
                .InnerJoin<Data.Model.PlanoCorte>("po.IdPlanoOtimizacao=pc.IdPlanoOtimizacao", "pc")
                .InnerJoin<Data.Model.RetalhoPlanoCorte>("pc.IdPlanoCorte=rpc.IdPlanoCorte", "rpc")
                .LeftJoin<Data.Model.RetalhoProducao>("rpc.IdRetalhoProducao=rp.IdRetalhoProducao", "rp")
                .LeftJoin<Data.Model.ProdutoImpressao>("rpc.IdRetalhoProducao=pi.IdRetalhoProducao", "pi")
                .InnerJoin<Data.Model.Produto>("p.IdProd=po.IdProduto", "p")
                .Where("so.IdSolucaoOtimizacao=?id")
                .Add("?id", idSolucaoOtimizacao)
                .Select(@"p.Descricao AS DescricaoProduto,
                         rpc.Altura, rpc.Largura, pi.NumEtiqueta,
                         po.IdPlanoOtimizacao, pc.IdPlanoCorte, po.Nome AS PlanoOtimizacao, pc.Posicao AS PosicaoPlanoCorte")
                .Execute()
                .Select(f => new
                {
                    DescricaoProduto = f.GetString("DescricaoProduto"),
                    Altura = f.GetFloat("Altura"),
                    Largura = f.GetInt32("Largura"),
                    NumEtiqueta = f.GetString("NumEtiqueta"),
                    IdPlanoOtimizacao = f.GetInt32("IdPlanoOtimizacao"),
                    IdPlanoCorte = f.GetInt32("IdPlanoCorte"),
                    PlanoOtimizacao = f.GetString("PlanoOtimizacao"),
                    PosicaoPlanoCorte = f.GetInt32("PosicaoPlanoCorte"),
                });

            var retalhos = etiquetasRetalho
                .GroupBy(f => $"{f.PlanoOtimizacao}|{f.PosicaoPlanoCorte}|{f.Largura}X{f.Altura}")
                .Select(grupo =>
                {
                    var etiqueta = grupo.First();

                    return new ItemOtimizacao
                    {
                        Tipo = TipoItemOtimizacao.Retalho,
                        DescricaoProduto = etiqueta.DescricaoProduto,
                        Qtde = grupo.Count(),
                        QtdImpresso = 0,
                        QtdAImprimir = grupo.Count(),
                        AlturaProducao = etiqueta.Altura,
                        LarguraProducao = etiqueta.Largura,
                        PlanoCorteEtiqueta = planosCorte.First(f => f.IdPlanoCorte == etiqueta.IdPlanoCorte).PlanoCorte,
                        Etiquetas = grupo.Select(f => f.NumEtiqueta).ToList(),
                    };
                });

            return pecas.Concat(retalhos).ToList();
        }
    }
}
