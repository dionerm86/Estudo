using Glass.Data.DAL;
using Glass.Configuracoes;
using System;
using System.Linq;
using Glass.Comum.Cache;
using System.Collections.Generic;
using GDA;

namespace Glass.Data.Model.Calculos
{
    class DadosProdutoDTO : BaseCalculoDTO, IDadosProduto
    {
        private static readonly CacheMemoria<Produto, int> cacheProdutos;
        private static readonly CacheMemoria<DescontoAcrescimoCliente, KeyValuePair<uint, uint?>> cacheDescontosAcrescimosCliente;

        private readonly IProdutoCalculo produtoCalculo;
        private readonly IDadosGrupoSubgrupo dadosGrupoSubgrupo;
        private readonly IDadosChapaVidro dadosChapaVidro;
        private readonly IDadosBaixaEstoque dadosBaixaEstoque;

        private readonly Lazy<Produto> produto;
        private readonly Lazy<DescontoAcrescimoCliente> descontoAcrescimoCliente;
        private readonly bool parcelaContainerAVista;

        static DadosProdutoDTO()
        {
            cacheProdutos = new CacheMemoria<Produto, int>("produtos");
            cacheDescontosAcrescimosCliente = new CacheMemoria<DescontoAcrescimoCliente,
                KeyValuePair<uint, uint?>>("descontosAcrescimosCliente");
        }

        internal DadosProdutoDTO(GDASession sessao, IProdutoCalculo produtoCalculo)
        {
            this.produtoCalculo = produtoCalculo;

            produto = ObterProduto(sessao, (int)produtoCalculo.IdProduto);
            descontoAcrescimoCliente = ObterDescontoAcrescimoCliente(sessao, produtoCalculo);

            parcelaContainerAVista = produtoCalculo.Container?.IdParcela > 0
                && ParcelasDAO.Instance.ObterParcelaAVista(null, (int)produtoCalculo.Container.IdParcela.Value);

            dadosGrupoSubgrupo = new DadosGrupoSubgrupoDTO(sessao, produto);
            dadosChapaVidro = new DadosChapaVidroDTO(sessao, produtoCalculo);
            dadosBaixaEstoque = new DadosBaixaEstoqueDTO(sessao, produto);
        }

        public IDadosGrupoSubgrupo DadosGrupoSubgrupo
        {
            get { return dadosGrupoSubgrupo; }
        }

        public IDadosChapaVidro DadosChapaVidro
        {
            get { return dadosChapaVidro; }
        }

        public IDadosBaixaEstoque DadosBaixaEstoque
        {
            get { return dadosBaixaEstoque; }
        }

        public bool CalcularAreaMinima(int numeroBeneficiamentos)
        {
            bool ativarAreaMinima = produto.Value.AtivarAreaMinima &&
                (produtoCalculo.Container?.Cliente?.CobrarAreaMinima ?? true);

            if (PedidoConfig.DadosPedido.CalcularAreaMinimaApenasVidroBeneficiado)
            {
                if (!dadosGrupoSubgrupo.ProdutoEVidro() || !ativarAreaMinima)
                {
                    return false;
                }

                if (dadosGrupoSubgrupo.IsVidroTemperado())
                {
                    return true;
                }

                return produto.Value.Redondo || numeroBeneficiamentos > 0;
            }

            return ativarAreaMinima;
        }

        public float AreaMinima()
        {
            return produto.Value.AreaMinima;
        }

        public int? AlturaProduto()
        {
            return produto.Value.Altura;
        }

        public int? LarguraProduto()
        {
            return produto.Value.Largura;
        }

        public string Descricao()
        {
            return produto.Value.Descricao;
        }

        public decimal CustoCompra()
        {
            return produto.Value.CustoCompra;
        }

        public decimal ValorTabela(bool usarCliente = true)
        {
            var percentualMultiplicar = descontoAcrescimoCliente.Value.PercMultiplicar;

            if ((produtoCalculo.Container?.Reposicao ?? false) && !Liberacao.TelaLiberacao.CobrarPedidoReposicao)
            {
                return ValorReposicao();
            }

            var pedidoAVista = produtoCalculo.Container?.TipoVenda == (int)Pedido.TipoVendaPedido.AVista
                || parcelaContainerAVista;

            if (PedidoConfig.UsarTabelaDescontoAcrescimoPedidoAVista && pedidoAVista)
            {
                percentualMultiplicar = descontoAcrescimoCliente.Value.PercMultiplicarAVista;
            }

            var revenda = produtoCalculo.Container?.Cliente != null
                && produtoCalculo.Container.Cliente.Revenda;

            decimal baseCalculo;

            if (revenda)
            {
                baseCalculo = produto.Value.ValorAtacado;
            }
            else
            {
                var tipoEntrega = produtoCalculo.Container?.TipoEntrega
                    ?? (int)Pedido.TipoEntregaPedido.Balcao;

                var tiposEntregaValorBalcao = new[]
                {
                    (int)Pedido.TipoEntregaPedido.Balcao,
                    (int)Pedido.TipoEntregaPedido.Entrega
                };

                baseCalculo = tiposEntregaValorBalcao.Contains(tipoEntrega)
                    ? produto.Value.ValorBalcao
                    : produto.Value.ValorObra;
            }

            var adicionalAlturaChapaVidro = 1 + (decimal)DadosChapaVidro.PercentualAcrescimoAltura();

            return Math.Round((baseCalculo * percentualMultiplicar) * adicionalAlturaChapaVidro, 2);
        }

        private decimal ValorReposicao()
        {
            decimal valor = PedidoConfig.UsarValorReposicaoProduto
                ? produto.Value.ValorReposicao
                : produto.Value.CustoCompra;

            return Math.Round(valor * descontoAcrescimoCliente.Value.PercMultiplicar, 2);
        }

        private Lazy<Produto> ObterProduto(GDASession sessao, int idProduto)
        {
            return ObterUsandoCache(
                cacheProdutos,
                idProduto,
                () => ProdutoDAO.Instance.GetElementByPrimaryKey(sessao, idProduto)
            );
        }

        private Lazy<DescontoAcrescimoCliente> ObterDescontoAcrescimoCliente(GDASession sessao, IProdutoCalculo produtoCalculo)
        {
            var idCliente = produtoCalculo.Container != null && produtoCalculo.Container.Cliente != null
                ? produtoCalculo.Container.Cliente.Id
                : (uint?)null;

            var id = new KeyValuePair<uint, uint?>(produtoCalculo.IdProduto, idCliente);

            Func<DescontoAcrescimoCliente> recuperarBanco = () =>
                DescontoAcrescimoClienteDAO.Instance.GetDescontoAcrescimo(
                    sessao,
                    produtoCalculo.Container,
                    produto.Value
                );

            return ObterUsandoCache(
                cacheDescontosAcrescimosCliente,
                id,
                recuperarBanco
            );
        }
    }
}
