using Glass.Data.DAL;
using Glass.Configuracoes;
using System;
using System.Linq;
using Glass.Comum.Cache;
using System.Collections.Generic;
using GDA;

namespace Glass.Data.Model.Calculos
{
    class DadosProduto : IDadosProduto
    {
        private readonly CacheMemoria<Produto, int> produtos;
        private readonly CacheMemoria<GrupoProd, int> grupos;
        private readonly CacheMemoria<SubgrupoProd, int> subgrupos;
        private readonly CacheMemoria<IEnumerable<ProdutoBaixaEstoque>, int> produtosBaixaEstoque;
        private readonly CacheMemoria<DescontoAcrescimoCliente, KeyValuePair<uint, uint?>> descontosAcrescimosCliente;
        
        private readonly IContainerCalculo container;

        internal DadosProduto(IContainerCalculo container)
        {
            this.container = container;

            produtos = new CacheMemoria<Produto, int>("produtos");
            grupos = new CacheMemoria<GrupoProd, int>("grupos");
            subgrupos = new CacheMemoria<SubgrupoProd, int>("subgrupos");
            produtosBaixaEstoque = new CacheMemoria<IEnumerable<ProdutoBaixaEstoque>, int>("produtosBaixaEstoque");
            descontosAcrescimosCliente = new CacheMemoria<DescontoAcrescimoCliente,
                KeyValuePair<uint, uint?>>("descontosAcrescimosCliente");
        }

        public bool CalcularAreaMinima(GDASession sessao, IProdutoCalculo produto, int numeroBeneficiamentos)
        {
            bool ativarAreaMinima = ObterProduto(sessao, produto).AtivarAreaMinima &&
                container.Cliente.CobrarAreaMinima;

            if (PedidoConfig.DadosPedido.CalcularAreaMinimaApenasVidroBeneficiado)
            {
                if (!ProdutoEVidro(sessao, produto) || !ativarAreaMinima)
                    return false;

                if (ObterSubgrupo(sessao, produto).IsVidroTemperado)
                    return true;

                return produto.Redondo || numeroBeneficiamentos > 0;
            }

            return ativarAreaMinima;
        }

        public float AreaMinima(GDASession sessao, IProdutoCalculo produto)
        {
            return ObterProduto(sessao, produto)
                .AreaMinima;
        }

        public int? AlturaProduto(GDASession sessao, IProdutoCalculo produto)
        {
            return ObterProduto(sessao, produto)
                .Altura;
        }

        public int? LarguraProduto(GDASession sessao, IProdutoCalculo produto)
        {
            return ObterProduto(sessao, produto)
                .Largura;
        }

        public int IdGrupoProd(GDASession sessao, IProdutoCalculo produto)
        {
            return ObterProduto(sessao, produto)
                .IdGrupoProd;
        }

        public string Descricao(GDASession sessao, IProdutoCalculo produto)
        {
            return ObterProduto(sessao, produto)
                .Descricao;
        }

        public bool ProdutoDeProducao(GDASession sessao, IProdutoCalculo produto)
        {
            var subgrupo = ObterSubgrupo(sessao, produto);
            return subgrupo.IdGrupoProd == (int)NomeGrupoProd.Vidro
                && subgrupo.ProdutosEstoque;
        }

        public bool ProdutoEVidro(GDASession sessao, IProdutoCalculo produto)
        {
            return IdGrupoProd(sessao, produto) == (int)NomeGrupoProd.Vidro;
        }

        public bool ProdutoEAluminio(GDASession sessao, IProdutoCalculo produto)
        {
            return IdGrupoProd(sessao, produto) == (int)NomeGrupoProd.Alumínio;
        }

        public string DescricaoSubgrupo(GDASession sessao, IProdutoCalculo produto)
        {
            return ObterSubgrupo(sessao, produto)
                .Descricao;
        }

        public TipoSubgrupoProd TipoSubgrupo(GDASession sessao, IProdutoCalculo produto)
        {
            return ObterSubgrupo(sessao, produto)
                .TipoSubgrupo;
        }

        public decimal CustoCompra(GDASession sessao, IProdutoCalculo produto)
        {
            return ObterProduto(sessao, produto)
                .CustoCompra;
        }

        public IEnumerable<float> QuantidadesProdutosBaixaEstoque(GDASession sessao, IProdutoCalculo produto)
        {
            return ObterProdutosBaixaEstoque(sessao, produto)
                .Select(produtoBaixaEstoque => produtoBaixaEstoque.Qtde);
        }

        public decimal ValorTabela(GDASession sessao, IProdutoCalculo produto, bool usarCliente = true)
        {
            var dadosProduto = ObterProduto(sessao, produto);
            var descontoAcrescimoCliente = usarCliente
                ? ObterDescontoAcrescimoCliente(sessao, produto)
                : new DescontoAcrescimoCliente();

            var percentualMultiplicar = descontoAcrescimoCliente.PercMultiplicar;

            if (container.Reposicao && !Liberacao.TelaLiberacao.CobrarPedidoReposicao)
            {
                return ValorReposicao(dadosProduto, descontoAcrescimoCliente);
            }

            if (PedidoConfig.UsarTabelaDescontoAcrescimoPedidoAVista)
            {
                var parcelaAVista = false;

                if (container.IdParcela > 0)
                {
                    parcelaAVista = ParcelasDAO.Instance.ObterParcelaAVista(null, (int)container.IdParcela.Value);
                }

                percentualMultiplicar = container.TipoVenda == (int)Pedido.TipoVendaPedido.AVista || parcelaAVista
                    ? descontoAcrescimoCliente.PercMultiplicarAVista
                    : descontoAcrescimoCliente.PercMultiplicar;
            }

            var revenda = container.Cliente != null
                && container.Cliente.Revenda;

            if (revenda)
            {
                return Math.Round(dadosProduto.ValorAtacado * percentualMultiplicar, 2);
            }

            var tipoEntrega = container.TipoEntrega ?? 0;
            if (tipoEntrega == 0)
            {
                tipoEntrega = 1;
            }

            switch (tipoEntrega)
            {
                case 1: // Balcão
                case 4: // Entrega
                    return Math.Round(dadosProduto.ValorBalcao * percentualMultiplicar, 2);
                default:
                    return Math.Round(dadosProduto.ValorObra * percentualMultiplicar, 2);
            }
        }
        
        public TipoCalculoGrupoProd TipoCalculo(GDASession sessao, IProdutoCalculo produto, bool fiscal = false)
        {
            var grupo = ObterGrupo(sessao, produto);
            var subgrupo = ObterSubgrupo(sessao, produto);

            TipoCalculoGrupoProd? tipoCalculoFiscal = subgrupo != null
                ? subgrupo.TipoCalculoNf ?? grupo.TipoCalculoNf
                : grupo.TipoCalculoNf;

            TipoCalculoGrupoProd? tipoCalculo = subgrupo != null
                ? subgrupo.TipoCalculo
                : grupo.TipoCalculo;

            var tipoCalc = fiscal
                ? tipoCalculoFiscal ?? tipoCalculo
                : tipoCalculo;

            return tipoCalc ?? TipoCalculoGrupoProd.Qtd;
        }

        private Produto ObterProduto(GDASession sessao, IProdutoCalculo produtoCalculo)
        {
            var idProduto = (int)produtoCalculo.IdProduto;
            var produto = produtos.RecuperarDoCache(idProduto);

            if (produto == null)
            {
                try
                {
                    produto = ProdutoDAO.Instance.GetElementByPrimaryKey(sessao, idProduto);
                }
                catch
                {
                    produto = new Produto();
                }

                produtos.AtualizarItemNoCache(produto, idProduto);
            }

            return produto;
        }

        private GrupoProd ObterGrupo(GDASession sessao, IProdutoCalculo produto)
        {
            var idGrupo = ObterProduto(sessao, produto).IdGrupoProd;            
            var grupo = grupos.RecuperarDoCache(idGrupo);

            if (grupo == null)
            {
                try
                {
                    grupo = GrupoProdDAO.Instance.GetElementByPrimaryKey(sessao, idGrupo);
                }
                catch
                {
                    grupo = new GrupoProd();
                }

                grupos.AtualizarItemNoCache(grupo, idGrupo);
            }

            return grupo;
        }

        private SubgrupoProd ObterSubgrupo(GDASession sessao, IProdutoCalculo produto)
        {
            var idSubgrupo = ObterProduto(sessao, produto)
                .IdSubgrupoProd;

            if (!idSubgrupo.HasValue)
                return new SubgrupoProd();

            var subgrupo = subgrupos.RecuperarDoCache(idSubgrupo.Value);

            if (subgrupo == null)
            {
                try
                {
                    subgrupo = SubgrupoProdDAO.Instance.GetElementByPrimaryKey(sessao, idSubgrupo.Value);
                }
                catch
                {
                    subgrupo = new SubgrupoProd();
                }

                subgrupos.AtualizarItemNoCache(subgrupo, idSubgrupo.Value);
            }

            return subgrupo;
        }

        private IEnumerable<ProdutoBaixaEstoque> ObterProdutosBaixaEstoque(GDASession sessao, IProdutoCalculo produto)
        {
            var idProduto = (int)produto.IdProduto;
            var produtosBaixa = produtosBaixaEstoque.RecuperarDoCache(idProduto);

            if (produtosBaixa == null)
            {
                try
                {
                    produtosBaixa = ProdutoBaixaEstoqueDAO.Instance.GetByProd(sessao, produto.IdProduto);
                }
                catch
                {
                    produtosBaixa = new ProdutoBaixaEstoque[0];
                }

                produtosBaixaEstoque.AtualizarItemNoCache(produtosBaixa, idProduto);
            }

            return produtosBaixa;
        }

        private DescontoAcrescimoCliente ObterDescontoAcrescimoCliente(GDASession sessao, IProdutoCalculo produto)
        {
            var idCliente = container.Cliente != null
                ? container.Cliente.Id
                : (uint?)null;

            var id = new KeyValuePair<uint, uint?>(produto.IdProduto, idCliente);
            var descontoAcrescimoCliente = descontosAcrescimosCliente.RecuperarDoCache(id);

            if (descontoAcrescimoCliente == null)
            {
                try
                {
                    descontoAcrescimoCliente = DescontoAcrescimoClienteDAO.Instance.GetDescontoAcrescimo(
                        sessao,
                        container,
                        ObterProduto(sessao, produto)
                    );
                }
                catch
                {
                    descontoAcrescimoCliente = new DescontoAcrescimoCliente();
                }

                descontosAcrescimosCliente.AtualizarItemNoCache(descontoAcrescimoCliente, id);
            }

            return descontoAcrescimoCliente;
        }

        private decimal ValorReposicao(Produto produto, DescontoAcrescimoCliente descontoAcrescimoCliente)
        {
            decimal valor = PedidoConfig.UsarValorReposicaoProduto
                ? produto.ValorReposicao
                : produto.CustoCompra;

            return Math.Round(valor * descontoAcrescimoCliente.PercMultiplicar, 2);
        }
    }
}
