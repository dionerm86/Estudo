using System;
using GDA;
using System.ComponentModel;
using Glass.Data.DAL;
using Glass.Configuracoes;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(CreditoFornecedorDAO))]
    [PersistenceClass("credito_fornecedor")]
    public class CreditoFornecedor
    {
        #region Enumeradores
        
        public enum SituacaoCredito
        {
            [Description("Ativo")]
            Ativo = 1,
            [Description("Cancelado")]
            Cancelado,
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IdCreditoFornecedor", PersistenceParameterType.IdentityKey)]
        public uint IdCreditoFornecedor { get; set; }

        [PersistenceProperty("Descricao")]
        public string Descricao { get; set; }

        [PersistenceProperty("IdFornec")]
        public uint IdFornecedor { get; set; }

        [PersistenceProperty("DataCad")]
        public DateTime DataCad { get; set; }

        [PersistenceProperty("UsuCad")]
        public uint? UsuCad { get; set; }

        [PersistenceProperty("SITUACAO")]
        public uint? Situacao { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NomeFornecedor", DirectionParameter.InputOptional)]
        public string NomeFornecedor { get; set; }

        [PersistenceProperty("NomeUsuarioCad", DirectionParameter.InputOptional)]
        public string NomeUsuarioCad { get; set; }

        #endregion

        #region Propriedades de Suporte

        private decimal[] _valoresPagto = null;

        public decimal[] ValoresPagto
        {
            get
            {
                if (_valoresPagto == null)
                    _valoresPagto = new decimal[PedidoConfig.FormaPagamento.NumeroFormasPagto];

                return _valoresPagto;
            }
            set { _valoresPagto = value; }
        }

        private DateTime?[] _datasPagto = null;

        public DateTime?[] DatasPagto
        {
            get
            {
                if (_datasPagto == null)
                    _datasPagto = new DateTime?[PedidoConfig.FormaPagamento.NumeroFormasPagto];

                return _datasPagto;
            }
            set { _datasPagto = value; }
        }

        private uint[] _formasPagto = null;

        public uint[] FormasPagto
        {
            get
            {
                if (_formasPagto == null)
                    _formasPagto = new uint[PedidoConfig.FormaPagamento.NumeroFormasPagto];

                return _formasPagto;
            }
            set { _formasPagto = value; }
        }

        private uint[] _tiposCartaoPagto = null;

        public uint[] TiposCartaoPagto
        {
            get
            {
                if (_tiposCartaoPagto == null)
                    _tiposCartaoPagto = new uint[PedidoConfig.FormaPagamento.NumeroFormasPagto];

                return _tiposCartaoPagto;
            }
            set { _tiposCartaoPagto = value; }
        }

        private uint[] _parcelasCartaoPagto = null;

        public uint[] ParcelasCartaoPagto
        {
            get
            {
                if (_parcelasCartaoPagto == null)
                    _parcelasCartaoPagto = new uint[PedidoConfig.FormaPagamento.NumeroFormasPagto];

                return _parcelasCartaoPagto;
            }
            set { _parcelasCartaoPagto = value; }
        }

        private uint[] _contasBancoPagto = null;

        public uint[] ContasBancoPagto
        {
            get
            {
                if (_contasBancoPagto == null)
                    _contasBancoPagto = new uint[PedidoConfig.FormaPagamento.NumeroFormasPagto];

                return _contasBancoPagto;
            }
            set { _contasBancoPagto = value; }
        }

        public DateTime? DataRecebimento { get; set; }

        public string NumAutConstrucard { get; set; }

        [Log("Situação")]
        public string SituacaoDescricao
        {
            get { return RepositorioEnumedores.GetEnumDescricao((SituacaoCredito)Situacao); }
        }

        public string DataCadString
        {
            get { return DataCad.ToString("dd/MM/yyyy"); }
        }

        public bool EditVisible
        {
            get { return Situacao == (uint)SituacaoCredito.Ativo; }
        }

        public bool CancelVisible
        {
            get { return Situacao != (uint)SituacaoCredito.Cancelado; }
        }

        public PagtoCreditoFornecedor[] Pagamentos { get; set; }


        public string ChequesPagto { get; set; }

        #endregion
    }
}