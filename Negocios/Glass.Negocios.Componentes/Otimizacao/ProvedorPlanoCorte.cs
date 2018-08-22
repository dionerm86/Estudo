using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glass.Otimizacao.Negocios.Entidades;
using Colosoft;

namespace Glass.Otimizacao.Negocios.Componentes
{
    /// <summary>
    /// Representa o provedor dos planos de corte.
    /// </summary>
    public class ProvedorPlanoCorte : IProvedorPlanoCorte
    {
        #region Properties

        /// <summary>
        /// Obtém o gerenciador de tipos de entidades.
        /// </summary>
        protected Colosoft.Business.IEntityTypeManager TypeManager { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="typeManager"></param>
        public ProvedorPlanoCorte(Colosoft.Business.IEntityTypeManager typeManager)
        {
            TypeManager = typeManager;
        }

        #endregion

        /// <summary>
        /// Obtém os produtos de pedido de produto associados com o plano de corte.
        /// </summary>
        /// <param name="planoCorte"></param>
        /// <returns></returns>
        private IEnumerable<ProdutoPedidoProducaoEtiqueta> ObterProdutosPedidoProducao(Entidades.PlanoCorte planoCorte)
        {
            var idsProdPedProducao = planoCorte.Pecas.Select(f => f.IdProdPedProducao).Distinct();

            var ids = string.Join(",", idsProdPedProducao);

            if (!string.IsNullOrEmpty(ids))
                return SourceContext.Instance.CreateQuery()
                    .From<Data.Model.ProdutoPedidoProducao>("ppp")
                    .LeftJoin<Data.Model.ProdutosPedido>("ppp.IdProdPed=pp.IdProdPedEsp", "pp")
                    .Where($"ppp.IdProdPedProducao IN ({ids})")
                    .Select("ppp.IdProdPedProducao, ppp.NumEtiqueta, pp.IdProdPed")
                    .Execute()
                    .Select(f => new ProdutoPedidoProducaoEtiqueta(f["IdProdPedProducao"], f["NumEtiqueta"], f["IdProdPed"]))
                    .ToArray();

            return new ProdutoPedidoProducaoEtiqueta[0];
        }

        /// <summary>
        /// Obtém os produtos de impressão associadas com as etiquetas informadas.
        /// </summary>
        /// <param name="etiquetas"></param>
        /// <returns></returns>
        private void CarregarProdutosImpressao(IEnumerable<ProdutoPedidoProducaoEtiqueta> etiquetas)
        {
            var consultas = SourceContext.Instance.CreateMultiQuery();

            foreach (var i in etiquetas)
            {
                var etiqueta = i;

                consultas
                    .Add<Data.Model.ProdutoImpressao>(SourceContext.Instance.CreateQuery()
                        .From<Data.Model.ProdutoImpressao>("pi")
                        .LeftJoin<Data.Model.ImpressaoEtiqueta>("pi.IdImpressao=ie.IdImpressao", "ie")
                        .Where(@"(ie.IdImpressao IS NULL OR ie.Situacao=?situacao) AND pi.Cancelado=0 AND 
                                  pi.IdPedido=?idPedido AND pi.PosicaoProd=?posicao AND pi.ItemEtiqueta=?item AND
                                  pi.QtdeProd=?qtde")
                        .Add("?situacao", Data.Model.ImpressaoEtiqueta.SituacaoImpressaoEtiqueta.Ativa)
                        .Add("?idPedido", etiqueta.IdPedido)
                        .Add("?posicao", etiqueta.Posicao)
                        .Add("?item", etiqueta.Item)
                        .Add("?qtde", etiqueta.Quantidade),
                    (sender, query, result) => etiqueta.ProdutoImpressao = result.FirstOrDefault());
            }

            consultas.Execute();
        }

        /// <summary>
        /// Obtém o produto de impressão para a peça do plano de corte.
        /// </summary>
        /// <param name="peca"></param>
        /// <param name="produtosPedidoProducao"></param>
        /// <param name="sequencia"></param>
        private Data.Model.ProdutoImpressao ObterProdutoImpressao(
            PecaPlanoCorte peca,
            IEnumerable<ProdutoPedidoProducaoEtiqueta> produtosPedidoProducao, 
            int sequencia)
        {
            var etiqueta = produtosPedidoProducao.First(f => f.IdProdPedProducao == peca.IdProdPedProducao.Value);

            if (etiqueta.ProdutoImpressao == null)
                etiqueta.ProdutoImpressao = new Data.Model.ProdutoImpressao
                {
                    IdProdImpressao = TypeManager.GenerateInstanceUid(typeof(Data.Model.ProdutoImpressao)),
                    IdPedido = (uint)etiqueta.IdPedido,
                    IdProdPed = (uint)etiqueta.IdProdPed,
                    PosicaoProd = etiqueta.Posicao,
                    ItemEtiqueta = etiqueta.Item,
                    QtdeProd = etiqueta.Quantidade,
                    NumEtiqueta = etiqueta.NumEtiqueta
                };

            var planoCorte = (PlanoCorte)peca.Owner;
            etiqueta.ProdutoImpressao.Forma = peca.Forma;
            etiqueta.ProdutoImpressao.PlanoCorte = planoCorte.NumeroEtiqueta;
            etiqueta.ProdutoImpressao.PosicaoArqOtimiz = peca.Posicao;
            etiqueta.ProdutoImpressao.NumSeq = sequencia;

            return etiqueta.ProdutoImpressao;
        }

        /// <summary>
        /// Obtém o número de etiqueta para o plano de corte.
        /// </summary>
        /// <param name="planoOtimizacao">Nome do plano de otimização pai.</param>
        /// <param name="posicaoPlanoCorte">Posição do plano de corte.</param>
        /// <param name="quantidadePlanosCorte">Quantidade de planos de corte no plano de otimização.</param>
        /// <returns></returns>
        public string ObterNumeroEtiqueta(string planoOtimizacao, int posicaoPlanoCorte, int quantidadePlanosCorte)
        {
            return string.Format("{0}-{1:#00}/{2:#00}", planoOtimizacao, posicaoPlanoCorte, quantidadePlanosCorte);
        }

        /// <summary>
        /// Obtém os produtos de impressão associados do o plano de corte.
        /// </summary>
        /// <param name="planoCorte"></param>
        /// <returns></returns>
        public IEnumerable<Data.Model.ProdutoImpressao> ObterProdutosImpressao(Entidades.PlanoCorte planoCorte)
        {
            var produtosPedidoProducao = ObterProdutosPedidoProducao(planoCorte);
            CarregarProdutosImpressao(produtosPedidoProducao);

            var sequencia = -1;

            foreach (var item in planoCorte.Itens)
            {
                sequencia++;

                var peca = item as PecaPlanoCorte;

                if (peca != null && peca.IdProdPedProducao.HasValue)
                {
                    yield return ObterProdutoImpressao(peca, produtosPedidoProducao, sequencia);
                    continue;
                }

                var retalho = item as RetalhoPlanoCorte;
                if (retalho != null &&  retalho.IdRetalhoProducao.HasValue)
                {
                    string etiqueta = "R" + retalho.IdRetalhoProducao + "-1/1";

                    yield return new Data.Model.ProdutoImpressao
                    {
                        IdProdImpressao = TypeManager.GenerateInstanceUid(typeof(Data.Model.ProdutoImpressao)),
                        IdRetalhoProducao = retalho.IdRetalhoProducao,
                        PosicaoProd = 1,
                        ItemEtiqueta = 1,
                        QtdeProd = 1,
                        PosicaoArqOtimiz = retalho.Posicao,
                        PlanoCorte = planoCorte.NumeroEtiqueta,
                        NumEtiqueta = etiqueta
                    };
                }
            }
        }

        #region Tipos Aninhados

        /// <summary>
        /// Representa a relação do produto pedido produção com a etiqueta.
        /// </summary>
        class ProdutoPedidoProducaoEtiqueta
        {
            /// <summary>
            /// Obtém identificador do produto do pedido de produção.
            /// </summary>
            public int IdProdPedProducao { get; }

            /// <summary>
            /// Obtém o número da etiqueta.
            /// </summary>
            public string NumEtiqueta { get; }

            /// <summary>
            /// Obtém do pedido.
            /// </summary>
            public int IdPedido { get; }

            /// <summary>
            /// Obtém o identificador do produto do pedido.
            /// </summary>
            public int IdProdPed { get; }

            /// <summary>
            /// Obtém a posição.
            /// </summary>
            public int Posicao { get; }

            /// <summary>
            /// Obtém o número do item.
            /// </summary>
            public int Item { get; }

            /// <summary>
            /// Obtém a quantidade.
            /// </summary>
            public int Quantidade { get; }

            /// <summary>
            /// Obtém ou define o produto de impressão associado.
            /// </summary>
            public Data.Model.ProdutoImpressao ProdutoImpressao { get; set; }

            /// <summary>
            /// Construtor padrão.
            /// </summary>
            /// <param name="idProdPedProducao"></param>
            /// <param name="numEtiqueta"></param>
            /// <param name="idProdPed"></param>
            public ProdutoPedidoProducaoEtiqueta(int idProdPedProducao, string numEtiqueta, int idProdPed)
            {
                IdProdPedProducao = idProdPedProducao;
                NumEtiqueta = numEtiqueta;
                IdProdPed = idProdPed;

                var dadosEtiqueta = numEtiqueta.Split('-', '.', '/');

                // Verifica se os dados da etiqueta são válidos
                if (dadosEtiqueta.Length >= 4)
                {
                    IdPedido = int.Parse(dadosEtiqueta[0].ToUpper().TrimStart('N', 'R'));
                    Posicao = int.Parse(dadosEtiqueta[1]);
                    Item = int.Parse(dadosEtiqueta[2]);
                    Quantidade = int.Parse(dadosEtiqueta[3]);
                }
            }
        }

        #endregion
    }
}
