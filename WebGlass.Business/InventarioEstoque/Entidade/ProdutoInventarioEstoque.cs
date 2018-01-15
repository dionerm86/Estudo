using System;
using Glass.Data.DAL;

namespace WebGlass.Business.InventarioEstoque.Entidade
{
    [Serializable]
    public class ProdutoInventarioEstoque
    {
        internal Glass.Data.Model.ProdutoInventarioEstoque _produto;
        private string _codigoInternoProduto, _descricaoProduto, _unidadeMedida;

        #region Construtores

        public ProdutoInventarioEstoque()
            : this(new Glass.Data.Model.ProdutoInventarioEstoque())
        {
        }

        internal ProdutoInventarioEstoque(Glass.Data.Model.ProdutoInventarioEstoque model)
        {
            _produto = model;
        }

        #endregion

        #region Propriedades

        public uint CodigoInventarioEstoque
        {
            get { return _produto.IdInventarioEstoque; }
            set { _produto.IdInventarioEstoque = value; }
        }

        public uint CodigoProduto
        {
            get { return _produto.IdProd; }
            set
            {
                _produto.IdProd = value;
                _codigoInternoProduto = null;
                _descricaoProduto = null;
                _unidadeMedida = null;
            }
        }

        public string CodigoInternoProduto
        {
            get
            {
                if (_codigoInternoProduto == null)
                    using (var dao = ProdutoDAO.Instance)
                        _codigoInternoProduto = dao.GetCodInterno((int)CodigoProduto);

                return _codigoInternoProduto;
            }
        }

        public string DescricaoProduto
        {
            get
            {
                if (_descricaoProduto == null)
                    using (var dao = ProdutoDAO.Instance)
                        _descricaoProduto = dao.ObtemDescricao((int)CodigoProduto);

                return _descricaoProduto;
            }
        }

        public string UnidadeMedida
        {
            get
            {
                if (_unidadeMedida == null)
                    using (var dao = ProdutoDAO.Instance)
                        _unidadeMedida = dao.ObtemUnidadeMedida((int)CodigoProduto);

                return _unidadeMedida;
            }
        }

        public float QtdeInicial
        {
            get { return _produto.QtdeIni; }
            set { _produto.QtdeIni = value; }
        }

        public float M2Inicial
        {
            get { return _produto.M2Ini; }
            set { _produto.M2Ini = value; }
        }

        public float? QtdeFinalizacao
        {
            get { return _produto.QtdeFim; }
            set { _produto.QtdeFim = value; }
        }

        public float? M2Finalizacao
        {
            get { return _produto.M2Fim; }
            set { _produto.M2Fim = value; }
        }

        //private int? _tipoCalculo;

        public bool UtilizarCampoQtde
        {
            get
            {
                //if (_tipoCalculo == null)
                //    _tipoCalculo = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(CodigoProduto);

                //var lista = new List<int> {
                //    (int)Glass.Data.Model.TipoCalculoGrupoProd.M2,
                //    (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto
                //};
                
                //return lista.Contains(_tipoCalculo.Value);

                return true;
            }
        }

        #endregion
    }
}
