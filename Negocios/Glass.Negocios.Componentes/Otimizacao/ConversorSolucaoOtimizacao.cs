using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Colosoft;

namespace Glass.Otimizacao.Negocios.Componentes
{
    /// <summary>
    /// Classe responsável pela conversão dos dados para a solução da otimização.
    /// </summary>
    class ConversorSolucaoOtimizacao
    {
        #region Variáveis Locais

        private readonly List<Global.Negocios.Entidades.Produto> _produtosRetalhos = 
            new List<Global.Negocios.Entidades.Produto>();

        private readonly List<Data.Model.RetalhoProducao> _retalhosProducao = 
            new List<Data.Model.RetalhoProducao>();

        #endregion

        #region Properties

        /// <summary>
        /// Obtém a solução associada.
        /// </summary>
        public Entidades.SolucaoOtimizacao Solucao { get; }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="solucao"></param>
        public ConversorSolucaoOtimizacao(Entidades.SolucaoOtimizacao solucao)
        {
            if (solucao == null)
                throw new ArgumentNullException(nameof(solucao));

            Solucao = solucao;
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Preenche os dados da solução de otimização com as etiquetas do documento informado.
        /// </summary>
        /// <param name="documentoEtiquetas"></param>
        public void Executar(eCutter.DocumentoEtiquetas documentoEtiquetas)
        {
            if (!documentoEtiquetas.PlanosOtimizacao.Any()) return;

            var codigosMateriais = documentoEtiquetas.PlanosOtimizacao.Select(f => f.CodigoMaterial).Distinct();

            var index = 0;
            var parametros = codigosMateriais
                .Select(f => new Colosoft.Query.QueryParameter($"?p{index++}", f))
                .ToList();

            var produtos = SourceContext.Instance.CreateQuery()
                .From<Data.Model.Produto>()
                .Where($"CodOtimizacao IN ({string.Join(",", parametros.Select(f => f.Name))})")
                .Add(parametros)
                .Select("CodOtimizacao, IdProd")
                .Execute()
                .Select(f => new
                {
                    CodOtimizacao = f.GetString(0),
                    IdProd = f.GetInt32(1)
                }).ToList();

            var processados = new List<Entidades.PlanoOtimizacao>();

            foreach (var etiqueta in documentoEtiquetas.PlanosOtimizacao)
            {
                var produto = produtos.FirstOrDefault(f => f.CodOtimizacao == etiqueta.CodigoMaterial);
                if (produto == null)
                    continue;

                var planoOtimizacao = Solucao.PlanosOtimizacao.FirstOrDefault(f => f.Nome == etiqueta.Nome);
                if (planoOtimizacao == null)
                {
                    planoOtimizacao = SourceContext.Instance.Create<Entidades.PlanoOtimizacao>();
                    planoOtimizacao.Nome = etiqueta.Nome;
                    Solucao.PlanosOtimizacao.Add(planoOtimizacao);
                }

                planoOtimizacao.IdProduto = produto.IdProd;

                PreencherPlanosCorte(planoOtimizacao, etiqueta);

                processados.Add(planoOtimizacao);
            }

            var apagados = Solucao.PlanosOtimizacao.Where(f => !processados.Contains(f)).ToArray();
            foreach (var planoOtimizacao in apagados)
                Solucao.PlanosOtimizacao.Remove(planoOtimizacao);
        }

        /// <summary>
        /// Salva todos os dados gerados na conversão.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult Salvar(Colosoft.Data.IPersistenceSession session)
        {
            Colosoft.Business.SaveResult resultado;

            foreach (var produto in _produtosRetalhos)
            {
                resultado = produto.Save(session);
                if (!resultado)
                    return resultado;
            }

            foreach (var retalhoProducao in _retalhosProducao)
                session.Insert(retalhoProducao);

            return Solucao.Save(session);
        }

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Obtém o código interno do produto associado com o retalho.
        /// </summary>
        /// <param name="retalho"></param>
        /// <returns></returns>
        private string ObterCodInternoProduto(Entidades.RetalhoPlanoCorte retalho)
        {
            var planoOtimizacao = retalho.GetOwner<Entidades.PlanoOtimizacao>();
            return $"{planoOtimizacao.Produto.CodInterno}-{retalho.Altura}x{retalho.Largura}-R";
        }

        /// <summary>
        /// Obtém o produto associado com o retalho do plano de corte.
        /// </summary>
        /// <param name="retalho"></param>
        /// <returns></returns>
        private Global.Negocios.Entidades.Produto ObterProduto(Entidades.RetalhoPlanoCorte retalho)
        {
            var codInterno = ObterCodInternoProduto(retalho);
            var produto =
                _produtosRetalhos.FirstOrDefault(f => f.CodInterno == codInterno);

            var produtoJaCarregado = produto != null;
             
            produto = produto ?? SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Produto>()
                    .Where("CodInterno=?codInterno AND IdGrupoProd=?idGrupoProd AND IdSubgrupoProd=?idSubgrupoProd")
                    .Add("?codInterno", codInterno)
                    .Add("?idGrupoProd", Data.Model.NomeGrupoProd.Vidro)
                    .Add("?idSubgrupoProd", Data.Helper.Utils.SubgrupoProduto.RetalhosProducao)
                    .ProcessResult<Global.Negocios.Entidades.Produto>()
                    .FirstOrDefault();

            var planoOtimizacao = retalho.GetOwner<Entidades.PlanoOtimizacao>();

            if (produto == null)
            {
                produto = (Global.Negocios.Entidades.Produto)planoOtimizacao.Produto.Clone();
                produto.ResetAllUids();
                produto.Altura = (int)retalho.Altura;
                produto.Largura = (int)retalho.Largura;
                produto.IdGrupoProd = (int)Data.Model.NomeGrupoProd.Vidro;
                produto.IdSubgrupoProd = (int)Data.Helper.Utils.SubgrupoProduto.RetalhosProducao;
                produto.CodInterno = codInterno;
                produto.IdProdOrig = planoOtimizacao.Produto.IdProd;
                produto.Situacao = Situacao.Ativo;
                produto.Obs = "";

                if (Data.Helper.UserInfo.GetUserInfo != null)
                    produto.IdUsuarioCadastro = (int)Data.Helper.UserInfo.GetUserInfo.CodUser;
            }
            else
                produto.Descricao = planoOtimizacao.Produto.Descricao;

            var m2 = (produto.Altura.GetValueOrDefault(0) * produto.Largura.GetValueOrDefault(0)) / 1000000m;
            produto.ValorAtacado = m2 * produto.ValorAtacado;
            produto.ValorBalcao = m2 * produto.ValorBalcao;
            produto.ValorObra = m2 * produto.ValorObra;

            if (!produtoJaCarregado)
                _produtosRetalhos.Add(produto);

            return produto;
        }

        /// <summary>
        /// Preenche os dados planos de corte do plano de otimização com base nas etiquetas.
        /// </summary>
        /// <param name="planoOtimizacao"></param>
        /// <param name="etiquetaPlanoOtimizacao"></param>
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

                PreencherPecasPlanoCorte(planoCorte, etiqueta);
                PreencherRetalhosPlanoCorte(planoCorte, etiqueta);

                processados.Add(planoCorte);
            }

            var apagados = planoOtimizacao.PlanosCorte.Where(f => !processados.Contains(f)).ToArray();
            foreach (var planoCorte in apagados)
                planoOtimizacao.PlanosCorte.Remove(planoCorte);
        }

        /// <summary>
        /// Preenche os dados da peças do plano de corte.
        /// </summary>
        /// <param name="planoCorte"></param>
        /// <param name="etiquetaPlanoCorte"></param>
        private void PreencherPecasPlanoCorte(Entidades.PlanoCorte planoCorte, eCutter.EtiquetaPlanoCorte etiquetaPlanoCorte)
        {
            var processadas = new List<Entidades.PecaPlanoCorte>();

            // os números das etiquetas do produto pedido pe
            var numerosEtiqueta = etiquetaPlanoCorte.Etiquetas.OfType<eCutter.EtiquetaPeca>()
                .Select(f => f.Notas?.Trim())
                // Verifica se a etiqueta é aceita no webglass
                .Where(f => f.Contains(".") && f.Contains("/") && f.Contains("-"))
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
                    IdProdPedProducao = f.GetInt32(1)
                })
                .ToList();

            foreach (var etiqueta in etiquetaPlanoCorte.Etiquetas.OfType<eCutter.EtiquetaPeca>())
            {
                var produtoPedidoProducao = etiquetasProdutoPedidoProducao.FirstOrDefault(f => f.Etiqueta == etiqueta.Notas?.Trim());
                if (produtoPedidoProducao == null) continue;

                var peca = planoCorte.Pecas.FirstOrDefault(f => f.IdProdPedProducao == produtoPedidoProducao.IdProdPedProducao);
                if (peca == null)
                {
                    peca = SourceContext.Instance.Create<Entidades.PecaPlanoCorte>();
                    peca.IdProdPedProducao = produtoPedidoProducao.IdProdPedProducao;
                    planoCorte.Pecas.Add(peca);
                }

                peca.Rotacionada = etiqueta.Rotacionada;
                peca.Posicao = etiqueta.Posicao;
                peca.Forma = etiqueta.Forma;

                processadas.Add(peca);
            }

            var apagadas = planoCorte.Pecas.Where(f => !processadas.Contains(f)).ToArray();
            foreach (var peca in apagadas)
                planoCorte.Pecas.Remove(peca);
        }

        /// <summary>
        /// Preenche os dados dos retalhos do plano de corte.
        /// </summary>
        /// <param name="planoCorte"></param>
        /// <param name="etiquetaPlanoCorte"></param>
        private void PreencherRetalhosPlanoCorte(Entidades.PlanoCorte planoCorte, eCutter.EtiquetaPlanoCorte etiquetaPlanoCorte)
        {
            var processados = new List<Entidades.RetalhoPlanoCorte>();

            foreach (var etiqueta in etiquetaPlanoCorte.Etiquetas.OfType<eCutter.EtiquetaRetalho>())
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

                var produto = ObterProduto(retalho);

                var retalhoProducao = new Data.Model.RetalhoProducao
                {
                    IdRetalhoProducao = retalho.TypeManager.GenerateInstanceUid(typeof(Data.Model.RetalhoProducao)),
                    DataCad = DateTime.Now,
                    IdProd = (uint)produto.IdProd,
                    Situacao = Data.Model.SituacaoRetalhoProducao.Indisponivel,
                    UsuCad = Data.Helper.UserInfo.GetUserInfo?.CodUser ?? 0
                };

                _retalhosProducao.Add(retalhoProducao);
                retalho.IdRetalhoProducao = retalhoProducao.IdRetalhoProducao;
                processados.Add(retalho);
            }

            var apagados = planoCorte.Retalhos.Where(f => !processados.Contains(f)).ToArray();
            foreach (var retalho in apagados)
                planoCorte.Retalhos.Remove(retalho);
        }

        #endregion
    }
}
