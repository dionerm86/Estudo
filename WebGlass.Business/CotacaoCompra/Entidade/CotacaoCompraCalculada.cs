using System;
using Glass.Configuracoes;

namespace WebGlass.Business.CotacaoCompra.Entidade
{
    public class CotacaoCompraCalculada
    {
        private CotacaoCompra _cotacao;
        private ProdutoCotacaoCompra _produto;
        private ProdutoFornecedorCotacaoCompra _fornecedor;

        #region Construtores

        internal CotacaoCompraCalculada(Glass.Data.Model.CotacaoCompra cotacao,
            Glass.Data.Model.ProdutoCotacaoCompra produto, Glass.Data.Model.ProdutoFornecedorCotacaoCompra fornecedor)
        {
            _cotacao = new CotacaoCompra(cotacao);
            _produto = new ProdutoCotacaoCompra(produto);
            _fornecedor = new ProdutoFornecedorCotacaoCompra(fornecedor);
        }

        #endregion

        #region Propriedades

        public uint CodigoCotacaoCompra
        {
            get { return _cotacao.Codigo; }
        }

        public DateTime DataCadastroCotacaoCompra
        {
            get { return _cotacao.DataCadastro; }
        }

        public string NomeFuncCadastroCotacaoCompra
        {
            get { return _cotacao.NomeFuncCadastro; }
        }

        public string ObservacaoCotacaoCompra
        {
            get { return _cotacao.Observacao; }
        }

        public uint CodigoProduto
        {
            get { return _produto.CodigoProduto; }
        }

        public string CodigoInternoProduto
        {
            get { return _produto.CodigoInternoProduto; }
        }

        public string DescricaoProduto
        {
            get { return _produto.DescricaoProduto; }
        }

        public float AlturaProduto
        {
            get { return _produto.Altura; }
        }

        public int LarguraProduto
        {
            get { return _produto.Largura; }
        }

        public string TituloAlturaLarguraProduto
        {
            get { return !PedidoConfig.EmpresaTrabalhaAlturaLargura ? "Largura x Altura" : "Altura x Largura"; }
        }

        public string DescricaoAlturaLarguraProduto
        {
            get 
            {
                return !PedidoConfig.EmpresaTrabalhaAlturaLargura ? LarguraProduto + " x " + AlturaProduto :
                    AlturaProduto + " x " + LarguraProduto;
            }
        }

        public float QuantidadeProduto
        {
            get { return _produto.Quantidade; }
        }

        public float TotalM2Produto
        {
            get { return _produto.TotalM2; }
        }

        public uint CodigoFornecedor
        {
            get { return _fornecedor.CodigoFornecedor; }
        }

        public string NomeFornecedor
        {
            get { return _fornecedor.NomeFornecedor; }
        }

        public decimal CustoUnitarioProdutoFornecedor
        {
            get { return _fornecedor.CustoUnitario; }
        }

        public decimal CustoTotalProdutoFornecedor
        {
            get { return _fornecedor.CustoTotal; }
        }

        public long PrazoEntregaDiasFornecedor
        {
            get { return _fornecedor.PrazoEntregaDias; }
        }

        public int CodigoParcela
        {
            get { return _fornecedor.CodigoParcela; }
        }

        public string DescricaoParcela
        {
            get { return _fornecedor.DescricaoParcela; }
        }

        internal DateTime[] DatasParcelasConfiguradas
        {
            get { return _fornecedor.DatasParcelasConfiguradas; }
        }

        #endregion
    }
}
