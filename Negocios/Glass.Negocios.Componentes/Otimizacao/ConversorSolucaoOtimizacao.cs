using Colosoft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Otimizacao.Negocios.Componentes
{
    /// <summary>
    /// Classe responsável pela conversão dos dados para a solução da otimização.
    /// </summary>
    internal class ConversorSolucaoOtimizacao
    {
        private readonly List<Global.Negocios.Entidades.Produto> _produtosRetalhos = new List<Global.Negocios.Entidades.Produto>();
        private readonly List<Data.Model.RetalhoProducao> _retalhosProducao = new List<Data.Model.RetalhoProducao>();
        private readonly Entidades.SolucaoOtimizacao _solucao;

        /// <summary>
        /// Obtém a solução associada.
        /// </summary>
        public Entidades.SolucaoOtimizacao Solucao => _solucao;

        /// <summary>
        /// Obtém ou define um valor que indica se é para criar um retalho de produção
        /// para cada retalho importado.
        /// </summary>
        public bool CriarRetalhosProducao { get; set; } = false;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConversorSolucaoOtimizacao"/>.
        /// </summary>
        /// <param name="solucao">Instância da solução que será convertida.</param>
        public ConversorSolucaoOtimizacao(Entidades.SolucaoOtimizacao solucao)
        {
            if (solucao == null)
            {
                throw new ArgumentNullException(nameof(solucao));
            }

            this._solucao = solucao;
        }

        /// <summary>
        /// Preenche os dados da solução de otimização com as etiquetas do documento informado.
        /// </summary>
        /// <param name="documentoEtiquetas">Documento contendo as etiquetas.</param>
        public void Executar(eCutter.DocumentoEtiquetas documentoEtiquetas)
        {
            if (!documentoEtiquetas.PlanosOtimizacao.Any())
            {
                return;
            }

            var processados = new List<Entidades.PlanoOtimizacao>();

            foreach (var etiqueta in documentoEtiquetas.PlanosOtimizacao)
            {
                int idProd = 0;

                var etiquetaPeca = etiqueta.PlanosCorte
                    .Select(f => f.Etiquetas.OfType<eCutter.EtiquetaPeca>())
                    .FirstOrDefault(f => f.Any(x => !string.IsNullOrWhiteSpace(x.Notas)))
                    .FirstOrDefault(f => !string.IsNullOrWhiteSpace(f.Notas));

                if (etiquetaPeca != null)
                {
                    var prod = SourceContext.Instance.CreateQuery()
                        .From<Data.Model.Produto>("p")
                        .InnerJoin<Data.Model.ProdutosPedidoEspelho>("p.IdProd = pp.IdProd", "pp")
                        .InnerJoin<Data.Model.ProdutoPedidoProducao>("pp.IdProdPed = ppp.IdProdPed", "ppp")
                        .Where("ppp.NumEtiqueta = ?etq").Add("?etq", etiquetaPeca.Notas)
                        .ProcessLazyResult<Global.Negocios.Entidades.Produto>()
                        .FirstOrDefault();

                    if (prod != null)
                    {
                        idProd = prod.BaixasEstoque.Any() ? prod.BaixasEstoque.FirstOrDefault().IdProdBaixa : prod.IdProd;
                    }
                }

                if (idProd == 0)
                {
                    continue;
                }

                var planoOtimizacao = this.Solucao.PlanosOtimizacao.FirstOrDefault(f => f.Nome == etiqueta.Nome);
                if (planoOtimizacao == null)
                {
                    planoOtimizacao = SourceContext.Instance.Create<Entidades.PlanoOtimizacao>();
                    planoOtimizacao.Nome = etiqueta.Nome;
                    this.Solucao.PlanosOtimizacao.Add(planoOtimizacao);
                }

                planoOtimizacao.IdProduto = idProd;

                this.PreencherPlanosCorte(planoOtimizacao, etiqueta);

                processados.Add(planoOtimizacao);
            }

            var apagados = this.Solucao.PlanosOtimizacao.Where(f => !processados.Contains(f)).ToArray();
            foreach (var planoOtimizacao in apagados)
            {
                this.Solucao.PlanosOtimizacao.Remove(planoOtimizacao);
            }
        }

        /// <summary>
        /// Salva todos os dados gerados na conversão.
        /// </summary>
        /// <param name="session">Sessão de persistencia que será usada para salva os dados.</param>
        /// <returns>Resultado da operação.</returns>
        public Colosoft.Business.SaveResult Salvar(Colosoft.Data.IPersistenceSession session)
        {
            Colosoft.Business.SaveResult resultado;

            foreach (var produto in this._produtosRetalhos)
            {
                resultado = produto.Save(session);
                if (!resultado)
                {
                    return resultado;
                }
            }

            foreach (var retalhoProducao in this._retalhosProducao)
            {
                session.Insert(retalhoProducao);
            }

            return this.Solucao.Save(session);
        }

        /// <summary>
        /// Obtém o código interno do produto associado com o retalho.
        /// </summary>
        /// <param name="retalho">Retalho base que será usado para recuperar o código interno do produto.</param>
        /// <returns>Código interno do produto associado com o retalho do plano de corte.</returns>
        private string ObterCodInternoProduto(Entidades.RetalhoPlanoCorte retalho)
        {
            var planoOtimizacao = retalho.GetOwner<Entidades.PlanoOtimizacao>();
            return $"{planoOtimizacao.Produto.CodInterno}-{retalho.Altura}X{retalho.Largura}-R";
        }

        /// <summary>
        /// Obtém o produto associado com o retalho do plano de corte.
        /// </summary>
        /// <param name="retalho">Retalho de onde será recupera o produto.</param>
        /// <returns>Produto associado com o retalho.</returns>
        private Global.Negocios.Entidades.Produto ObterProduto(Entidades.RetalhoPlanoCorte retalho)
        {
            var codInterno = this.ObterCodInternoProduto(retalho);
            var produto = this._produtosRetalhos.FirstOrDefault(f => StringComparer.InvariantCultureIgnoreCase.Equals(f.CodInterno, codInterno));

            var produtoJaCarregado = produto != null;

            if (produto == null)
            {
                produto = SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Produto>()
                    .Where("CodInterno=?codInterno AND IdGrupoProd=?idGrupoProd AND IdSubgrupoProd=?idSubgrupoProd")
                    .Add("?codInterno", codInterno)
                    .Add("?idGrupoProd", Data.Model.NomeGrupoProd.Vidro)
                    .Add("?idSubgrupoProd", Data.Helper.Utils.SubgrupoProduto.RetalhosProducao)
                    .ProcessResult<Global.Negocios.Entidades.Produto>()
                    .FirstOrDefault();
            }

            var planoOtimizacao = retalho.GetOwner<Entidades.PlanoOtimizacao>();

            if (produto == null)
            {
                produto = (Global.Negocios.Entidades.Produto)planoOtimizacao.Produto.Clone();
                produto.ProdutoBeneficiamentos.Clear();
                produto.BaixasEstoque.Clear();
                produto.BaixasEstoqueFiscal.Clear();
                produto.Mva.Clear();
                produto.AliquotasIcms.Clear();
                produto.NCMs.Clear();
                produto.FlagArqMesaProduto.Clear();

                produto.ResetAllUids();
                produto.Altura = (int)retalho.Altura;
                produto.Largura = (int)retalho.Largura;
                produto.IdGrupoProd = (int)Data.Model.NomeGrupoProd.Vidro;
                produto.IdSubgrupoProd = (int)Data.Helper.Utils.SubgrupoProduto.RetalhosProducao;
                produto.CodInterno = codInterno;
                produto.IdProdOrig = planoOtimizacao.Produto.IdProd;
                produto.Situacao = Situacao.Ativo;
                produto.Obs = string.Empty;
                produto.DataCadastro = DateTime.Now;

                if (Data.Helper.UserInfo.GetUserInfo != null)
                {
                    produto.IdUsuarioCadastro = (int)Data.Helper.UserInfo.GetUserInfo.CodUser;
                }
            }
            else
            {
                produto.Descricao = planoOtimizacao.Produto.Descricao;
            }

            var m2 = (produto.Altura.GetValueOrDefault(0) * produto.Largura.GetValueOrDefault(0)) / 1000000m;
            produto.ValorAtacado = m2 * produto.ValorAtacado;
            produto.ValorBalcao = m2 * produto.ValorBalcao;
            produto.ValorObra = m2 * produto.ValorObra;

            if (!produtoJaCarregado)
            {
                this._produtosRetalhos.Add(produto);
            }

            return produto;
        }

        /// <summary>
        /// Preenche os dados planos de corte do plano de otimização com base nas etiquetas.
        /// </summary>
        /// <param name="planoOtimizacao">Plano de otimização.</param>
        /// <param name="etiquetaPlanoOtimizacao">Eetiqueta do plano de otimização.</param>
        private void PreencherPlanosCorte(Entidades.PlanoOtimizacao planoOtimizacao, eCutter.EtiquetaPlanoOtimizacao etiquetaPlanoOtimizacao)
        {
            var processados = new List<Entidades.PlanoCorte>();

            foreach (var etiqueta in etiquetaPlanoOtimizacao.PlanosCorte)
            {
                var planoCorte = planoOtimizacao.PlanosCorte.FirstOrDefault(f => f.Posicao == etiqueta.Posicao);
                if (planoCorte == null)
                {
                    planoCorte = SourceContext.Instance.Create<Entidades.PlanoCorte>();
                    planoCorte.Posicao = etiqueta.Posicao;
                    planoOtimizacao.PlanosCorte.Add(planoCorte);
                }

                planoCorte.IdChapa = etiqueta.IdChapa;
                planoCorte.Largura = etiqueta.Largura;
                planoCorte.Altura = etiqueta.Altura;

                this.PreencherPecasPlanoCorte(planoCorte, etiqueta);
                this.PreencherRetalhosPlanoCorte(planoCorte, etiqueta);

                processados.Add(planoCorte);
            }

            var apagados = planoOtimizacao.PlanosCorte.Where(f => !processados.Contains(f)).ToArray();
            foreach (var planoCorte in apagados)
            {
                planoOtimizacao.PlanosCorte.Remove(planoCorte);
            }
        }

        /// <summary>
        /// Preenche os dados da peças do plano de corte.
        /// </summary>
        /// <param name="planoCorte">Plano de corte que será preenchido.</param>
        /// <param name="etiquetaPlanoCorte">Etiqueta o plano de corte que será usado como referência.</param>
        private void PreencherPecasPlanoCorte(Entidades.PlanoCorte planoCorte, eCutter.EtiquetaPlanoCorte etiquetaPlanoCorte)
        {
            var processadas = new List<Entidades.PecaPlanoCorte>();

            // os números das etiquetas do produto pedido pe
            var numerosEtiqueta = etiquetaPlanoCorte.Etiquetas.OfType<eCutter.EtiquetaPeca>()
                .Select(f => f.Notas?.Trim())
                .Where(f => !string.IsNullOrWhiteSpace(f) && f.Contains(".") && f.Contains("/") && f.Contains("-"))
                .Distinct();

            var index = 0;
            var parametros = numerosEtiqueta
                .Select(f => new Colosoft.Query.QueryParameter($"?p{index++}", f))
                .ToList();

            var etiquetasProdutoPedidoProducao = SourceContext.Instance.CreateQuery()
                .From<Data.Model.ProdutoPedidoProducao>()
                .Where($"NumEtiqueta IN ({string.Join(",", parametros.Select(f => f.Name))})")
                .Add(parametros)
                .Select("NumEtiqueta, IdProdPedProducao")
                .Execute()
                .Select(f => new
                {
                    Etiqueta = f.GetString(0),
                    IdProdPedProducao = f.GetInt32(1),
                })
                .ToList();

            foreach (var etiqueta in etiquetaPlanoCorte.Etiquetas.OfType<eCutter.EtiquetaPeca>())
            {
                var produtoPedidoProducao = etiquetasProdutoPedidoProducao.FirstOrDefault(f => f.Etiqueta == etiqueta.Notas?.Trim());
                if (produtoPedidoProducao == null)
                {
                    continue;
                }

                var peca = planoCorte.Pecas.FirstOrDefault(f => f.IdProdPedProducao == produtoPedidoProducao.IdProdPedProducao);
                if (peca == null)
                {
                    peca = SourceContext.Instance.Create<Entidades.PecaPlanoCorte>();
                    peca.IdProdPedProducao = produtoPedidoProducao.IdProdPedProducao;
                    planoCorte.Pecas.Add(peca);
                }

                peca.Rotacionada = etiqueta.Rotacionada;
                peca.PosicaoGeral = etiqueta.PosicaoPeca;
                peca.Posicao = etiqueta.Posicao;
                peca.Forma = etiqueta.Forma;

                processadas.Add(peca);
            }

            var apagadas = planoCorte.Pecas.Where(f => !processadas.Contains(f)).ToArray();
            foreach (var peca in apagadas)
            {
                planoCorte.Pecas.Remove(peca);
            }
        }

        /// <summary>
        /// Preenche os dados dos retalhos do plano de corte.
        /// </summary>
        /// <param name="planoCorte">Plano de corte que será preenchido.</param>
        /// <param name="etiquetaPlanoCorte">Etiqueta do plano de corte.</param>
        private void PreencherRetalhosPlanoCorte(Entidades.PlanoCorte planoCorte, eCutter.EtiquetaPlanoCorte etiquetaPlanoCorte)
        {
            var processados = new List<Entidades.RetalhoPlanoCorte>();

            foreach (var etiqueta in etiquetaPlanoCorte.Etiquetas.OfType<eCutter.EtiquetaRetalho>().Where(f => f.Reaproveitavel))
            {
                var retalho = planoCorte.Retalhos.FirstOrDefault(f => f.Posicao == etiqueta.Posicao);
                if (retalho == null)
                {
                    retalho = SourceContext.Instance.Create<Entidades.RetalhoPlanoCorte>();
                    retalho.Posicao = etiqueta.Posicao;
                    planoCorte.Retalhos.Add(retalho);
                }

                retalho.Posicao = etiqueta.Posicao;
                retalho.Largura = etiqueta.Largura;
                retalho.Altura = etiqueta.Altura;
                retalho.Reaproveitavel = etiqueta.Reaproveitavel;

                if (this.CriarRetalhosProducao)
                {
                    var produto = this.ObterProduto(retalho);

                    var retalhoProducao = new Data.Model.RetalhoProducao
                    {
                        IdRetalhoProducao = retalho.TypeManager.GenerateInstanceUid(typeof(Data.Model.RetalhoProducao)),
                        DataCad = DateTime.Now,
                        IdProd = produto.IdProd,
                        Situacao = Data.Model.SituacaoRetalhoProducao.Indisponivel,
                        UsuCad = Data.Helper.UserInfo.GetUserInfo?.CodUser ?? 0,
                    };

                    this._retalhosProducao.Add(retalhoProducao);
                    retalho.IdRetalhoProducao = retalhoProducao.IdRetalhoProducao;
                }

                processados.Add(retalho);
            }

            var apagados = planoCorte.Retalhos.Where(f => !processados.Contains(f)).ToArray();
            foreach (var retalho in apagados)
            {
                planoCorte.Retalhos.Remove(retalho);
            }
        }
    }
}
