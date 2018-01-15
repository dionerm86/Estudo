using System;
using Glass.Data.DAL;

namespace WebGlass.Business.MovimentacaoBancaria.Entidade
{
    public class MovimentacaoBancaria
    {
        public enum TipoMovimentacaoEnum
        {
            Entrada = 1,
            Saída
        }

        private Glass.Data.Model.MovBanco _movBanco;
        private string _nomeCliente, _nomeFornecedor, _descricaoContaBancaria, _nomePlanoContas;

        #region Construtores

        public MovimentacaoBancaria()
            : this(new Glass.Data.Model.MovBanco())
        {
        }

        internal MovimentacaoBancaria(Glass.Data.Model.MovBanco model)
        {
            _movBanco = model;
        }

        #endregion

        public uint CodigoMovimentacao
        {
            get { return _movBanco.IdMovBanco; }
        }

        public uint? CodigoCliente
        {
            get { return _movBanco.IdCliente; }
        }

        public string NomeCliente
        {
            get
            {
                if (CodigoCliente > 0 && _nomeCliente == null)
                    _nomeCliente = ClienteDAO.Instance.GetNome(CodigoCliente.Value);

                return _nomeCliente;
            }
        }

        public uint? CodigoFornecedor
        {
            get { return _movBanco.IdFornecedor; }
        }

        public string NomeFornecedor
        {
            get
            {
                if (CodigoFornecedor > 0 && _nomeFornecedor == null)
                    _nomeFornecedor = FornecedorDAO.Instance.GetNome(CodigoFornecedor.Value);

                return _nomeFornecedor;
            }
        }

        public uint CodigoContaBancaria
        {
            get { return _movBanco.IdContaBanco; }
        }

        public string DescricaoContaBancaria
        {
            get
            {
                if (_descricaoContaBancaria == null)
                    _descricaoContaBancaria = ContaBancoDAO.Instance.GetDescricao(CodigoContaBancaria);

                return _descricaoContaBancaria;
            }
        }

        public uint CodigoPlanoContas
        {
            get { return _movBanco.IdConta; }
        }

        public string NomePlanoContas
        {
            get
            {
                if (_nomePlanoContas == null && CodigoPlanoContas > 0)
                    _nomePlanoContas = PlanoContasDAO.Instance.GetDescricao(CodigoPlanoContas, false);

                return _nomePlanoContas;
            }
        }

        public string ReferenciaMovimentacao
        {
            get { return _movBanco.Referencia; }
        }

        public decimal ValorMovimentacao
        {
            get { return _movBanco.ValorMov; }
        }

        public decimal ValorJuros
        {
            get { return _movBanco.Juros; }
        }

        public DateTime DataMovimentacao
        {
            get { return _movBanco.DataMov; }
        }

        public TipoMovimentacaoEnum TipoMovimentacao
        {
            get { return (TipoMovimentacaoEnum)_movBanco.TipoMov; }
        }

        public decimal SaldoAposMovimentacao
        {
            get { return _movBanco.Saldo; }
        }

        public string Observacao
        {
            get { return _movBanco.Obs; }
        }

        public bool LancamentoManual
        {
            get { return _movBanco.LancManual; }
        }
    }
}
