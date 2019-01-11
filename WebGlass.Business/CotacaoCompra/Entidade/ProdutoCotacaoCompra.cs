using Glass.Data.DAL;

namespace WebGlass.Business.CotacaoCompra.Entidade
{
    public class ProdutoCotacaoCompra
    {
        internal Glass.Data.Model.ProdutoCotacaoCompra _produto;
        private string _codInterno, _descricao;

        #region Construtores

        public ProdutoCotacaoCompra()
            : this(new Glass.Data.Model.ProdutoCotacaoCompra())
        {
        }

        internal ProdutoCotacaoCompra(Glass.Data.Model.ProdutoCotacaoCompra model)
        {
            _produto = model;
        }

        #endregion

        #region Propriedades

        public uint Codigo
        {
            get { return _produto.IdProdCotacaoCompra; }
            set { _produto.IdProdCotacaoCompra = value; }
        }

        public uint CodigoCotacaoCompra
        {
            get { return _produto.IdCotacaoCompra; }
            set { _produto.IdCotacaoCompra = value; }
        }

        public uint CodigoProduto
        {
            get { return _produto.IdProd; }
            set
            { 
                _produto.IdProd = value;
                _codInterno = null;
                _descricao = null;
            }
        }

        public string CodigoInternoProduto
        {
            get
            {
                if (_codInterno == null)
                    _codInterno = ProdutoDAO.Instance.GetCodInterno((int)CodigoProduto);

                return _codInterno;
            }
        }

        public string DescricaoProduto
        {
            get
            {
                if (_descricao == null)
                    _descricao = ProdutoDAO.Instance.ObtemDescricao((int)CodigoProduto);

                return _descricao;
            }
        }

        public string CodigoDescricaoProduto
        {
            get { return CodigoInternoProduto + " - " + DescricaoProduto; }
        }

        public float Altura
        {
            get { return _produto.Altura; }
            set { _produto.Altura = value; }
        }

        public string AlturaString
        {
            get { return Altura.ToString(); }
            set { Altura = Glass.Conversoes.StrParaFloat(value); }
        }

        public int Largura
        {
            get { return _produto.Largura; }
            set { _produto.Largura = value; }
        }

        public float Quantidade
        {
            get { return _produto.Qtde; }
            set { _produto.Qtde = value; }
        }

        public string QuantidadeString
        {
            get { return Quantidade.ToString(); }
            set { Quantidade = Glass.Conversoes.StrParaFloat(value); }
        }

        public float TotalM2
        {
            get { return _produto.TotM; }
        }

        public int TipoCalc
        {
            get { return Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo((int)CodigoProduto); }
        }

        #endregion
    }
}
