using Glass.Data.DAL;

namespace WebGlass.Business.MvaProdutoPorUf.Entidade
{
    public class MvaProdutoPorUf
    {
        internal Glass.Data.Model.MvaProdutoUf _mvaProduto;
        private string _codigoInternoProduto, _descricaoProduto;

        #region Construtores

        public MvaProdutoPorUf()
            : this(new Glass.Data.Model.MvaProdutoUf())
        {
        }

        internal MvaProdutoPorUf(Glass.Data.Model.MvaProdutoUf model)
        {
            _mvaProduto = model;
        }

        #endregion

        public int CodigoProduto
        {
            get { return _mvaProduto.IdProd; }
            set { _mvaProduto.IdProd = value; }
        }

        public string CodigoInternoProduto
        {
            get
            {
                if (_codigoInternoProduto == null)
                    _codigoInternoProduto = ProdutoDAO.Instance.GetCodInterno((int)CodigoProduto);

                return _codigoInternoProduto;
            }
        }

        public string DescricaoProduto
        {
            get
            {
                if (_descricaoProduto == null)
                    _descricaoProduto = ProdutoDAO.Instance.ObtemDescricao((int)CodigoProduto);

                return _descricaoProduto;
            }
        }

        public string UfOrigem
        {
            get { return _mvaProduto.UfOrigem; }
            set { _mvaProduto.UfOrigem = value; }
        }

        public string UfDestino
        {
            get { return _mvaProduto.UfDestino; }
            set { _mvaProduto.UfDestino = value; }
        }

        public float MvaOriginal
        {
            get { return _mvaProduto.MvaOriginal; }
            set { _mvaProduto.MvaOriginal = value; }
        }

        public float MvaClientesOptantesPeloSimples
        {
            get { return _mvaProduto.MvaSimples; }
            set { _mvaProduto.MvaSimples = value; }
        }
    }
}
