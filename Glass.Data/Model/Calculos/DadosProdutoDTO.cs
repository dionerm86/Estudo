using Glass.Data.DAL;
using Glass.Configuracoes;
using System;
using System.Linq;
using Glass.Comum.Cache;
using System.Collections.Generic;
using GDA;

namespace Glass.Data.Model.Calculos
{
    class DadosProdutoDTO : IDadosProduto
    {
        private static readonly CacheMemoria<Produto, int> produtos;
        private static readonly CacheMemoria<DescontoAcrescimoCliente, KeyValuePair<uint, uint?>> descontosAcrescimosCliente;
        
        private readonly IProdutoCalculo produtoCalculo;
        private readonly IDadosGrupoSubgrupo dadosGrupoSubgrupo;
        private readonly IDadosChapaVidro dadosChapaVidro;
        private readonly IDadosBaixaEstoque dadosBaixaEstoque;

        private readonly Produto produto;
        private readonly DescontoAcrescimoCliente descontoAcrescimoCliente;
        private readonly bool parcelaContainerAVista;

        static DadosProdutoDTO()
        {
            produtos = new CacheMemoria<Produto, int>("produtos");
            descontosAcrescimosCliente = new CacheMemoria<DescontoAcrescimoCliente,
                KeyValuePair<uint, uint?>>("descontosAcrescimosCliente");
        }

        internal DadosProdutoDTO(GDASession sessao, IProdutoCalculo produtoCalculo)
        {
            this.produtoCalculo = produtoCalculo;

            produto = ObterProduto(sessao, produtoCalculo);
            descontoAcrescimoCliente = ObterDescontoAcrescimoCliente(sessao, produtoCalculo);

            parcelaContainerAVista = produtoCalculo.Container?.IdParcela > 0
                && ParcelasDAO.Instance.ObterParcelaAVista(null, (int)produtoCalculo.Container.IdParcela.Value);

            dadosGrupoSubgrupo = new DadosGrupoSubgrupoDTO(sessao, produto.IdGrupoProd, produto.IdSubgrupoProd);
            dadosChapaVidro = new DadosChapaVidroDTO(sessao, produtoCalculo);
            dadosBaixaEstoque = new DadosBaixaEstoqueDTO(sessao, produto.IdProd);
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
            bool ativarAreaMinima = produto.AtivarAreaMinima &&
                (produtoCalculo.Container?.Cliente?.CobrarAreaMinima ?? false);

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

                return produto.Redondo || numeroBeneficiamentos > 0;
            }

            return ativarAreaMinima;
        }

        public float AreaMinima()
        {
            return produto.AreaMinima;
        }

        public int? AlturaProduto()
        {
            return produto.Altura;
        }

        public int? LarguraProduto()
        {
            return produto.Largura;
        }

        public string Descricao()
        {
            return produto.Descricao;
        }

        public decimal CustoCompra()
        {
            return produto.CustoCompra;
        }

        public decimal ValorTabela(bool usarCliente = true)
        {
            var percentualMultiplicar = descontoAcrescimoCliente.PercMultiplicar;

            if ((produtoCalculo.Container?.Reposicao ?? false) && !Liberacao.TelaLiberacao.CobrarPedidoReposicao)
            {
                return ValorReposicao();
            }

            var pedidoAVista = produtoCalculo.Container?.TipoVenda == (int)Pedido.TipoVendaPedido.AVista
                || parcelaContainerAVista;

            if (PedidoConfig.UsarTabelaDescontoAcrescimoPedidoAVista && pedidoAVista)
            {
                percentualMultiplicar = descontoAcrescimoCliente.PercMultiplicarAVista;
            }

            var revenda = produtoCalculo.Container?.Cliente != null
                && produtoCalculo.Container.Cliente.Revenda;

            decimal baseCalculo;

            if (revenda)
            {
                baseCalculo = produto.ValorAtacado;
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
                    ? produto.ValorBalcao
                    : produto.ValorObra;
            }

            return Math.Round(baseCalculo * percentualMultiplicar, 2);
        }

        private decimal ValorReposicao()
        {
            decimal valor = PedidoConfig.UsarValorReposicaoProduto
                ? produto.ValorReposicao
                : produto.CustoCompra;

            return Math.Round(valor * descontoAcrescimoCliente.PercMultiplicar, 2);
        }

        private Produto ObterProduto(GDASession sessao, IProdutoCalculo produtoCalculo)
        {
            var idProduto = (int)produtoCalculo.IdProduto;
            var produtoCache = produtos.RecuperarDoCache(idProduto);

            if (produtoCache == null)
            {
                try
                {
                    produtoCache = ProdutoDAO.Instance.GetElementByPrimaryKey(sessao, idProduto)
                        ?? new Produto();
                }
                catch
                {
                    produtoCache = new Produto();
                }

                produtos.AtualizarItemNoCache(produtoCache, idProduto);
            }

            return produtoCache;
        }

        private DescontoAcrescimoCliente ObterDescontoAcrescimoCliente(GDASession sessao, IProdutoCalculo produtoCalculo)
        {
            var idCliente = produtoCalculo.Container != null && produtoCalculo.Container.Cliente != null
                ? produtoCalculo.Container.Cliente.Id
                : (uint?)null;

            var id = new KeyValuePair<uint, uint?>(produtoCalculo.IdProduto, idCliente);
            var descontoAcrescimoClienteCache = descontosAcrescimosCliente.RecuperarDoCache(id);

            if (descontoAcrescimoClienteCache == null)
            {
                try
                {
                    descontoAcrescimoClienteCache = DescontoAcrescimoClienteDAO.Instance.GetDescontoAcrescimo(
                        sessao,
                        produtoCalculo.Container,
                        produto
                    ) ?? new DescontoAcrescimoCliente();
                }
                catch
                {
                    descontoAcrescimoClienteCache = new DescontoAcrescimoCliente();
                }

                descontosAcrescimosCliente.AtualizarItemNoCache(descontoAcrescimoClienteCache, id);
            }

            return descontoAcrescimoClienteCache;
        }
    }
}
