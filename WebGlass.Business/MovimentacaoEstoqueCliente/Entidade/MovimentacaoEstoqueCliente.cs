using System;
using Glass.Data.DAL;

namespace WebGlass.Business.MovimentacaoEstoqueCliente.Entidade
{
    public class MovimentacaoEstoqueCliente
    {
        internal Glass.Data.Model.MovEstoqueCliente _movEstoque;
        private string _nomeCliente, _nomeLoja, _codigoInternoProduto, _descricaoProduto, _unidadeMedidaProduto, _nomeFuncionario;

        #region Construtores

        public MovimentacaoEstoqueCliente()
            : this(new Glass.Data.Model.MovEstoqueCliente())
        {
        }

        internal MovimentacaoEstoqueCliente(Glass.Data.Model.MovEstoqueCliente movEstoque)
        {
            _movEstoque = movEstoque;
        }

        #endregion

        #region Propriedades

        public uint Codigo
        {
            get { return _movEstoque.IdMovEstoqueCliente; }
            set { _movEstoque.IdMovEstoqueCliente = value; }
        }

        public string Referencia
        {
            get { return _movEstoque.Referencia; }
        }

        public uint CodigoCliente
        {
            get { return _movEstoque.IdCliente; }
            set
            {
                _movEstoque.IdCliente = value;
                _nomeCliente = null;
            }
        }

        public string NomeCliente
        {
            get
            {
                if (_nomeCliente == null)
                    _nomeCliente = ClienteDAO.Instance.GetNome(CodigoCliente);

                return _nomeCliente;
            }
        }

        public uint CodigoLoja
        {
            get { return _movEstoque.IdLoja; }
            set
            {
                _movEstoque.IdLoja = value;
                _nomeLoja = null;
            }
        }

        public string NomeLoja
        {
            get
            {
                if (_nomeLoja == null)
                    _nomeLoja = LojaDAO.Instance.GetNome(CodigoLoja);

                return _nomeLoja;
            }
        }

        public uint CodigoProduto
        {
            get { return _movEstoque.IdProd; }
            set
            {
                _movEstoque.IdProd = value;
                _codigoInternoProduto = null;
                _descricaoProduto = null;
                _unidadeMedidaProduto = null;
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
                    _descricaoProduto = ProdutoDAO.Instance.GetDescrProduto((int)CodigoProduto);

                return _descricaoProduto;
            }
        }

        public string UnidadeMedidaProduto
        {
            get
            {
                if (_unidadeMedidaProduto == null)
                    _unidadeMedidaProduto = ProdutoDAO.Instance.ObtemUnidadeMedida((int)CodigoProduto);

                return _unidadeMedidaProduto;
            }
        }

        public uint CodigoFuncionario
        {
            get { return _movEstoque.IdFunc; }
            set
            {
                _movEstoque.IdFunc = value;
                _nomeFuncionario = null;
            }
        }

        public string NomeFuncionario
        {
            get
            {
                if (_nomeFuncionario == null)
                    _nomeFuncionario = FuncionarioDAO.Instance.GetNome(CodigoFuncionario);

                return _nomeFuncionario;
            }
        }

        public DateTime DataMovimentacao
        {
            get { return _movEstoque.DataMov; }
            set { _movEstoque.DataMov = value; }
        }

        public DateTime? DataCadastro
        {
            get { return _movEstoque.DataCad; }
            set { _movEstoque.DataCad = value; }
        }

        public decimal QuantidadeMovimentacao
        {
            get { return _movEstoque.QtdeMov; }
            set { _movEstoque.QtdeMov = value; }
        }

        public decimal SaldoQuantidade
        {
            get { return _movEstoque.SaldoQtdeMov; }
            set { _movEstoque.SaldoQtdeMov = value; }
        }

        public int TipoMovimentacao
        {
            get { return _movEstoque.TipoMov; }
            set { _movEstoque.TipoMov = value; }
        }

        public string DescricaoTipoMovimentacao
        {
            get { return _movEstoque.DescrTipoMov; }
        }

        public decimal ValorMovimentacao
        {
            get { return _movEstoque.ValorMov; }
            set { _movEstoque.ValorMov = value; }
        }

        public decimal SaldoValor
        {
            get { return _movEstoque.SaldoValorMov; }
            set { _movEstoque.SaldoValorMov = value; }
        }

        public string Observacao
        {
            get { return _movEstoque.Observacao; }
            set { _movEstoque.Observacao = value; }
        }

        public bool PodeExcluir
        {
            get { return _movEstoque.LancManual; }
        }

        #endregion
    }
}
