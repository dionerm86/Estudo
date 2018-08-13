using Glass.Otimizacao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Colosoft;

namespace Glass.Otimizacao.Negocios.Componentes
{
    /// <summary>
    /// Fluxo de negócio da otimização.
    /// </summary>
    public class OtimizacaoFluxo : IOtimizacaoFluxo
    {
        #region Propriedades

        /// <summary>
        /// Obtém o repositório da solução.
        /// </summary>
        protected IRepositorioSolucaoOtimizacao Repositorio { get; }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="repositorioSolucaoOtimizacao"></param>
        public OtimizacaoFluxo(IRepositorioSolucaoOtimizacao repositorioSolucaoOtimizacao)
        {
            Repositorio = repositorioSolucaoOtimizacao;
        }

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Recupera as entradas do estoque de chapas.
        /// </summary>
        /// <param name="idsProd"></param>
        /// <returns></returns>
        private IEnumerable<IEntradaEstoqueChapa> ObterEntradasEstoqueChapas(IEnumerable<int> idsProd)
        {
            var idsProd2 = string.Join(",", idsProd);

            if (string.IsNullOrEmpty(idsProd2))
                yield break;

            // Recupera os identificadores dos produtos de baixa
            var idsProdBaixa = SourceContext.Instance.CreateQuery()
                .From<Data.Model.ProdutoBaixaEstoque>()
                .Where($"IdProd IN ({idsProd2})")
                .Select("IdProdBaixa")
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
                    .LeftJoin<Data.Model.ProdutoPedidoProducao>("r.IdProdPedProducaoOrig=ppp.IdProdPedProducao", "ppp")
                    .LeftJoin(SourceContext.Instance.CreateQuery()
                        .From<Data.Model.UsoRetalhoProducao>("ur1")
                        .GroupBy("ur1.IdRetalhoProducao")
                        .Select("MIN(ur1.IdUsoRetalhoProducao) AS IdUsoRetalhoProducao, ur1.IdRetalhoProducao"),
                        "r.IdRetalhoProducao=ur.IdRetalhoProducao", "ur")
                    .Where($"r.Situacao=?situacaoRetalho AND (p.IdProdOrig IN ({idsProdBaixa2}) OR p.IdProdBase IN ({idsProdBaixa2}))")
                    .GroupBy("r.IdProd")
                    .Select("MIN(p.CodOtimizacao) AS CodMaterial, MIN(p.Altura) AS Altura, MIN(p.Largura) AS Largura, COUNT(r.IdProd) AS Qtde, 1 AS Retalho"));

            }

            idsProd2 = string.Join(",", idsProd.Concat(idsProdBaixa));

            consultas.Add(SourceContext.Instance.CreateQuery()
                .From<Data.Model.Produto>("p")
                .InnerJoin<Data.Model.SubgrupoProd>("p.IdSubgrupoProd=sg.IdSubgrupoProd AND (sg.TipoSubgrupo=1 OR sg.TipoSubgrupo=3)", "sg")
                .LeftJoin<Data.Model.RetalhoProducao>("p.IdProd=rp.IdProd AND rp.Situacao=?situacaoRetalho", "rp")
                .InnerJoin<Data.Model.ProdutoLoja>("p.IdProd=pl.IdProd", "pl")
                .LeftJoin<Data.Model.Produto>("p.IdProdBase=pbase.IdProd", "pbase")
                .LeftJoin<Data.Model.Produto>("p.IdProdOrig=pbase.IdProd", "porig")
                .Where($"rp.IdRetalhoProducao IS NULL AND (p.IdProd IN({idsProd2}) OR p.IdProdOrig IN ({idsProd2}) OR p.IdProdBase IN ({idsProd2}))")
                .GroupBy("ISNULL(p.CodOtimizacao, ISNULL(pbase.CodOtimizacao, porig.CodOtimizacao)), Altura, Largura")
                .Having("SUM(pl.QtdEstoque) > 0")
                .OrderBy("CodMaterial")
                .Select(@"ISNULL(MIN(p.CodOtimizacao), ISNULL(MIN(pbase.CodOtimizacao), MIN(porig.CodOtimizacao))) AS CodMaterial, 
                         p.Altura, p.Largura, SUM(pl.QtdEstoque) AS Qtde, 0 AS Retalho"));

            var consulta = consultas.First();

            if (consultas.Count > 0)
                consulta.UnionAll(consultas[1]);

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
                    Prioridade = f.GetBoolean("Retalho") ? 999 : 0
                }).ToList();

            foreach (var i in entradas)
                yield return i;
        }

        /// <summary>
        /// Recupera os materiais.
        /// </summary>
        /// <param name="idsProd"></param>
        /// <returns></returns>
        private IEnumerable<IMaterial> ObterMateriais(IEnumerable<int> idsProd)
        {
            var idsProd2 = string.Join(",", idsProd);

            if (string.IsNullOrEmpty(idsProd2))
                yield break;

            var materiais = SourceContext.Instance.CreateQuery()
                .From<Data.Model.Produto>("p")
                .Where($"p.TipoMercadoria=?tipo AND p.IdProd IN({idsProd2})")
                .Add("?tipo", Data.Model.TipoMercadoria.MateriaPrima)
                .Select("p.CodOtimizacao, p.Descricao, p.Espessura")
                .Execute()
                .Select(f => new Material
                {
                    Codigo = f["CodOtimizacao"],
                    Descricao = f["Descricao"],
                    Tipo = TipoMaterial.Monolitico,
                    Espessura1 = f["Espessura"]
                }).ToList();

            foreach (var i in materiais)
                yield return i;
        }

        /// <summary>
        /// Realiza a importação do arquivo de otimização do optyway
        /// </summary>
        /// <param name="arquivo"></param>
        /// <returns></returns>
        private ImportacaoOtimizacao ImportarArquivoOptyWay(IArquivoSolucaoOtimizacao arquivo)
        {
            var proditosPedidoEspelho = new List<Data.Model.ProdutosPedidoEspelho>();
            var etiquetasJaImpressas = string.Empty;
            var qtdPecasImpressas = 0;

            List<string> lstEtiquetas = new List<string>();
            var pedidosAlteradosAposExportacao = new List<int>();

          
            // Lê o arquivo de otimização enviado
            var xmlDoc = new System.Xml.XmlDocument();
            using (var reader = new System.IO.StreamReader(arquivo.Abrir()))
                /* Chamado 50941. */
                xmlDoc.LoadXml(reader.ReadToEnd());

            Data.DAL.ImpressaoEtiquetaDAO.Instance.ImportarArquivoOtimizacaoOptyWay(xmlDoc, ref lstEtiquetas, ref etiquetasJaImpressas, ref pedidosAlteradosAposExportacao, ref proditosPedidoEspelho, ref qtdPecasImpressas);

            if (lstEtiquetas == null || lstEtiquetas.Count == 0)
                throw new InvalidOperationException("Não há etiquetas para importar.");

            string extensao = System.IO.Path.GetExtension(arquivo.Nome);

            var a = Data.DAL.ArquivoOtimizacaoDAO.Instance.InserirArquivoOtimizacao(Data.Model.ArquivoOtimizacao.DirecaoEnum.Importar,
                extensao, lstEtiquetas, null, null);

            // Salva arquivo otimizado
            xmlDoc.Save(System.IO.Path.Combine(Data.Helper.Utils.GetArquivoOtimizacaoPath, a.NomeArquivo));

            return new ImportacaoOtimizacao
            {
                IdArquivoOtimizacao = (int)a.IdArquivoOtimizacao
            };
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Verifica se existe uma solução de otimização configurada para o arquivo de otimização.
        /// </summary>
        /// <param name="idArquivoOtimizacao">Identificador do arquiv de otimização.</param>
        /// <returns></returns>
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
        /// <returns></returns>
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
        /// <returns></returns>
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
        /// <param name="idArquivoOtimizacao"></param>
        /// <returns></returns>
        public ISessaoOtimizacao ObterSessaoOtimizacao(int idArquivoOtimizacao)
        {
            // Recupera os identificadores dos produtos associado com o arquivo de otimização
            var idsProd = SourceContext.Instance.CreateQuery()
                .From<Data.Model.EtiquetaArquivoOtimizacao>("eao")
                .InnerJoin<Data.Model.ProdutosPedidoEspelho>("eao.IdPedido=ppe.IdPedido", "ppe")
                .InnerJoin<Data.Model.ProdutoPedidoProducao>("ppe.IdProdPed=ppp.IdProdPed AND ppp.NumEtiqueta=eao.NumEtiqueta", "ppp")
                .SelectDistinct("ppe.IdProd")
                .Execute()
                .Select(f => f.GetInt32(0))
                .ToList();

            var estoque = new EstoqueChapa(ObterMateriais(idsProd), ObterEntradasEstoqueChapas(idsProd));

            return new SessaoOtimizacao(estoque, new IPecaPadrao[0]);
        }

        /// <summary>
        /// Realiza a importação do resultado de uma otimização.
        /// </summary>
        /// <param name="idArquivoOtimizacao">Identificador do arquivo de otimização.</param>
        /// <param name="arquivos">Arquivos da otimização.</param>
        /// <returns></returns>
        public ImportacaoOtimizacao Importar(int idArquivoOtimizacao, IEnumerable<IArquivoSolucaoOtimizacao> arquivos)
        {
            var solucaoOtimizacao = SourceContext.Instance.CreateQuery()
                .From<Data.Model.SolucaoOtimizacao>()
                .Where("IdArquivoOtimizacao=?id")
                .Add("?id", idArquivoOtimizacao)
                .ProcessResult<Entidades.SolucaoOtimizacao>()
                .FirstOrDefault();

            if (solucaoOtimizacao == null)
            {
                solucaoOtimizacao = new Entidades.SolucaoOtimizacao()
                {
                    IdArquivoOtimizacao = idArquivoOtimizacao
                };

                SourceContext.Instance.ExecuteSave(solucaoOtimizacao);
            }

            Repositorio.SalvarArquivos(solucaoOtimizacao, arquivos);

            foreach(var arquivo in Repositorio.ObterArquivos(solucaoOtimizacao))
            {
                if (StringComparer.InvariantCultureIgnoreCase.Equals(
                    System.IO.Path.GetExtension(arquivo.Nome), ".optlbl"))
                {
                    using (var conteudo = arquivo.Abrir())
                    {
                        var documento = eCutter.DocumentoEtiquetas.Open(conteudo);
                    }
                }
            }

            return new ImportacaoOtimizacao
            {
                IdArquivoOtimizacao = 0
            };

            //var tipoExportacaoEtiqueta = Configuracoes.EtiquetaConfig.TipoExportacaoEtiqueta;

            //foreach(var arquivo in arquivos)
            //{
            //    if ((tipoExportacaoEtiqueta == Data.Helper.DataSources.TipoExportacaoEtiquetaEnum.OptyWay ||
            //         tipoExportacaoEtiqueta == Data.Helper.DataSources.TipoExportacaoEtiquetaEnum.eCutter) &&
            //         StringComparer.InvariantCultureIgnoreCase.Equals(System.IO.Path.GetExtension(arquivo.Nome), ".xml"))
            //        return ImportarArquivoOptyWay(arquivo);
            //}

            //throw new InvalidOperationException("Não foi encontrado nenhum arquivo compatível para a importação.");
        }

        /// <summary>
        /// Recupera os itens da otimização.
        /// </summary>
        /// <param name="idArquivoOtimizacao">Identificador do arquivo da otimização no qual os itens estão associados.</param>
        /// <returns></returns>
        public IEnumerable<ItemOtimizacao> ObterItens(int idArquivoOtimizacao)
        {
            var etiquetas = SourceContext.Instance.CreateQuery()
                .From<Data.Model.ArquivoOtimizacao>("ao")
                .InnerJoin<Data.Model.EtiquetaArquivoOtimizacao>("ao.IdArquivoOtimizacao=eao.IdArquivoOtimiz", "eao")
                .InnerJoin<Data.Model.ProdutoPedidoProducao>("ppp.NumEtiqueta=eao.NumEtiqueta", "ppp")
                .InnerJoin<Data.Model.ProdutosPedidoEspelho>("ppe.IdPedido=eao.IdPedido AND ppe.IdProdPed=ppp.IdProdPed", "ppe")
                .InnerJoin<Data.Model.Produto>("p.IdProd=ppe.IdProd", "p")
                .InnerJoin<Data.Model.CorVidro>("p.IdCorVidro=cv.IdCorVidro", "cv")
                .InnerJoin<Data.Model.EtiquetaProcesso>("ep.IdProcesso=ppe.IdProcesso", "ep")
                .InnerJoin<Data.Model.EtiquetaAplicacao>("ea.IdAplicacao=ppe.IdAplicacao", "ea")
                .LeftJoin<Data.Model.ProdutoImpressao>("pi.IdProdPed=ppe.IdProdPed AND  pi.NumEtiqueta=eao.NumEtiqueta AND (pi.Cancelado IS NULL OR pi.Cancelado=0)", "pi")
                .Where("ao.IdArquivoOtimizacao=?id")
                .Add("?id", idArquivoOtimizacao)
                .Select(@"ppe.IdProdPed, ppe.IdPedido, p.Descricao AS DescricaoProduto, 
                         ppp.PecaReposta, ep.CodInterno AS CodProcesso, ea.CodInterno AS CodAplicacao,
                         ppe.Qtde, ppe.QtdImpresso, ppe.AlturaReal, ppe.Altura, ppe.LarguraReal, ppe.Largura,
                         ppe.Obs, ppe.TotM, ppe.TotM2Calc, eao.NumEtiqueta, pi.PlanoCorte, cv.Sigla AS Cor,
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
                    PlanoCorte = f.GetString("PlanoCorte"),
                    Cor = f.GetString("Cor"),
                    Espessura = f.GetFloat("Espessura")
                }).ToList();


            // Recupera a relação de etiquetas impressas
            var etiquetasImpressas = SourceContext.Instance.CreateQuery()
                .From<Data.Model.EtiquetaArquivoOtimizacao>("eao")
                .InnerJoin<Data.Model.ProdutoImpressao>("eao.NumEtiqueta=pi.NumEtiqueta", "pi")
                .Select("eao.NumEtiqueta")
                .Where("pi.IdImpressao IS NOT NULL AND (pi.Cancelado IS NULL OR pi.Cancelado=0) AND eao.IdArquivoOtimiz=?id")
                .Add("?id", idArquivoOtimizacao)
                .Execute()
                .Select(f => f.GetString("NumEtiqueta"))
                .ToList();

            var itens = etiquetas
                //.OrderBy(f => $"{f.Cor}|{f.Espessura}")
                .GroupBy(f => $"{f.IdProdPed}|{f.PlanoCorte}|{f.PecaReposta}")
                .Select(grupoProdPed =>
                {
                    var item = grupoProdPed.First();

                    // Calcula a quantidade a imprimir
                    var qtdAImprimir = !item.PecaReposta ? grupoProdPed.Count() : 0;

                    float totM2 = item.PecaReposta ? (item.TotM / item.Qtde) : (item.TotM / item.Qtde) * qtdAImprimir;
                    float totM2Calc = item.PecaReposta ? (item.TotM2Calc / item.Qtde) : (item.TotM2Calc / item.Qtde) * qtdAImprimir;

                    return new ItemOtimizacao
                    {
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
                        PlanoCorteEtiqueta = item.PlanoCorte,
                        Etiquetas = grupoProdPed.Select(f => f.NumEtiqueta)
                    };
                }).ToList();

            return itens;
        }

        #endregion
    }
}
