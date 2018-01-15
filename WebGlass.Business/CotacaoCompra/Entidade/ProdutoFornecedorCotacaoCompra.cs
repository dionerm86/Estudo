using System;
using System.Collections.Generic;
using Glass.Data.DAL;
using Glass;

namespace WebGlass.Business.CotacaoCompra.Entidade
{
    public class ProdutoFornecedorCotacaoCompra
    {
        internal Glass.Data.Model.ProdutoFornecedorCotacaoCompra _produtoFornecedor;
        private string _codigoInternoProduto, _descricaoProduto, _nomeFornecedor, _descricaoParcela;

        #region Construtores

        public ProdutoFornecedorCotacaoCompra()
            : this(new Glass.Data.Model.ProdutoFornecedorCotacaoCompra())
        {
        }

        internal ProdutoFornecedorCotacaoCompra(Glass.Data.Model.ProdutoFornecedorCotacaoCompra model)
        {
            _produtoFornecedor = model;
        }

        #endregion

        #region Propriedades

        public uint CodigoCotacaoCompra
        {
            get { return _produtoFornecedor.IdCotacaoCompra; }
            set { _produtoFornecedor.IdCotacaoCompra = value; }
        }

        public uint CodigoProduto
        {
            get { return _produtoFornecedor.IdProd; }
            set
            {
                _produtoFornecedor.IdProd = value;
                _codigoInternoProduto = null;
                _descricaoProduto = null;
            }
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

        public string CodigoDescricaoProduto
        {
            get { return CodigoInternoProduto + " - " + DescricaoProduto; }
        }

        public uint CodigoFornecedor
        {
            get { return _produtoFornecedor.IdFornec; }
            set
            {
                _produtoFornecedor.IdFornec = value;
                _nomeFornecedor = null;
            }
        }

        public string NomeFornecedor
        {
            get
            {
                if (_nomeFornecedor == null)
                    _nomeFornecedor = FornecedorDAO.Instance.GetNome(CodigoFornecedor);

                return _nomeFornecedor;
            }
        }

        public string CodigoNomeFornecedor
        {
            get { return CodigoFornecedor + " - " + NomeFornecedor; }
        }

        public decimal CustoUnitario
        {
            get { return _produtoFornecedor.CustoUnit; }
            set { _produtoFornecedor.CustoUnit = value; }
        }

        public decimal CustoTotal
        {
            get { return _produtoFornecedor.CustoTotal; }
            set { _produtoFornecedor.CustoTotal = value; }
        }

        public long PrazoEntregaDias
        {
            get { return _produtoFornecedor.PrazoEntregaDias; }
            set { _produtoFornecedor.PrazoEntregaDias = value; }
        }

        public int CodigoParcela
        {
            get
            {
                return _produtoFornecedor.IdParcela > 0 ? (int)_produtoFornecedor.IdParcela.Value : 
                    _produtoFornecedor.Cadastrado ? -1 : 0;
            }
            set
            {
                _produtoFornecedor.IdParcela = value > 0 ? (ulong?)value : null;
                _descricaoParcela = null;
            }
        }

        public string DescricaoParcela
        {
            get
            {
                if (_descricaoParcela == null)
                {
                    if (CodigoParcela > 0)
                        _descricaoParcela = ParcelasDAO.Instance.ObtemDescricao((uint)CodigoParcela);

                    else if (CodigoParcela == -1)
                    {
                        _descricaoParcela = NumeroParcelasConfiguradas + " parcelas: ";

                        foreach (DateTime data in DatasParcelasConfiguradas)
                            _descricaoParcela += data.ToString("dd/MM/yyyy") + ", ";

                        _descricaoParcela = _descricaoParcela.TrimEnd(' ', ',');
                    }
                }

                return _descricaoParcela;
            }
        }

        public int NumeroParcelasConfiguradas
        {
            get { return DatasParcelasConfiguradas.Length; }
        }

        public DateTime[] DatasParcelasConfiguradas
        {
            get
            {
                List<DateTime> retorno = new List<DateTime>();
                string[] datas = (_produtoFornecedor.DatasPagamentos ?? "").Split(',');

                Array.ForEach(datas, x =>
                {
                    if (!String.IsNullOrEmpty(x))
                        retorno.Add(Conversoes.ConverteDataNotNull(x));
                });

                retorno.Sort();
                return retorno.ToArray();
            }
            set
            {
                if (value != null && CodigoParcela == -1)
                {
                    string datas = "";
                    foreach (DateTime data in value)
                        datas += data.ToString("dd/MM/yyyy") + ",";

                    _produtoFornecedor.DatasPagamentos = datas.TrimEnd(',');
                }
                else
                    _produtoFornecedor.DatasPagamentos = null;
            }
        }

        public bool Cadastrado
        {
            get { return _produtoFornecedor.Cadastrado; }
            internal set { _produtoFornecedor.Cadastrado = value; }
        }

        #endregion
    }
}
