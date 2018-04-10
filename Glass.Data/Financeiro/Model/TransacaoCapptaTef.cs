using GDA;
using Glass.Data.DAL;
using Glass.Log;
using System;
using System.Linq;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(TransacaoCapptaTefDAO))]
    [PersistenceClass("transacao_cappta_tef")]
    public class TransacaoCapptaTef : ModelBaseCadastro
    {
        #region Propiedades

        [PersistenceProperty("IdTransacaoCappta", PersistenceParameterType.IdentityKey)]
        public int IdTransacaoCappta { get; set; }

        [Log("Tipo de Pagto.")]
        [PersistenceProperty("TipoRecebimento")]
        public Helper.UtilsFinanceiro.TipoReceb TipoRecebimento { get; set; }

        [Log("Id. Referência")]
        [PersistenceProperty("IdReferencia")]
        public int IdReferencia { get; set; }

        [Log("Cód do Cliente")]
        [PersistenceProperty("IdCliente")]
        public int IdCliente { get; set; }

        [Log("Checkout GUID")]
        [PersistenceProperty("CheckoutGuid")]
        public string CheckoutGuid { get; set; }

        [Log("Cód de Controle")]
        [PersistenceProperty("CodigoControle")]
        public string CodigoControle { get; set; }

        [Log("Valor")]
        [PersistenceProperty("Valor")]
        public decimal Valor { get; set; }

        private string _comprovanteLoja;

        [Log("Comprovante da loja")]
        [PersistenceProperty("ComprovanteLoja")]
        public string ComprovanteLoja
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_comprovanteLoja))
                    return "";

                var arr = _comprovanteLoja.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList();

                arr = arr.Select(f => f.Trim()).ToList();

                return string.Join(Environment.NewLine, arr);
            }
            set
            {
                _comprovanteLoja = value;
            }
        }

        private string _comprovanteCliente;

        [Log("Comprovante do cliente")]
        [PersistenceProperty("ComprovanteCliente")]
        public string ComprovanteCliente
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_comprovanteCliente))
                    return "";

                var arr = _comprovanteCliente.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList();

                arr = arr.Select(f => f.Trim()).ToList();

                return string.Join(Environment.NewLine, arr);
            }
            set
            {
                _comprovanteCliente = value;
            }
        }

        #endregion

        #region Propiedades Estendidas

        [PersistenceProperty("NomeCliente", DirectionParameter.InputOptional)]
        public string NomeCliente { get; set; }

        #endregion

        #region Propiedades de Suporte

        public string Referencia
        {
            get
            {
                switch (TipoRecebimento)
                {
                    case Helper.UtilsFinanceiro.TipoReceb.LiberacaoAVista:
                        return "Liberação: " + IdReferencia;
                    case Helper.UtilsFinanceiro.TipoReceb.Acerto:
                        return "Acerto: " + IdReferencia;
                    case Helper.UtilsFinanceiro.TipoReceb.ChequeDevolvido:
                        return "Acerto de Cheque: " + IdReferencia;
                    case Helper.UtilsFinanceiro.TipoReceb.ContaReceber:
                        return "Conta recebida: " + IdReferencia;
                    case Helper.UtilsFinanceiro.TipoReceb.SinalPedido:
                        return "Sinal/Pagto. Antecipado: " + IdReferencia;
                    case Helper.UtilsFinanceiro.TipoReceb.Obra:
                        return "Obra/Créd. Gerado: " + IdReferencia;
                    default:
                        return IdReferencia.ToString();
                }
            }
        }

        public bool ExcluirVisible =>
            DateTime.Now.Date == AuthorizationDateTime.Date && !PaymentProductName.Contains("Estorno");

        public bool ExibirFinalizarTransacao =>
            TransacaoCapptaTefDAO.Instance.ExibirFinalizarTransacao(this);

        #region Consulta Pagamento

        public string AcquirerAffiliationKey { get; set; }

        public string AcquirerAuthorizationCode { get; set; }

        public DateTime AuthorizationDateTime { get; set; }

        public string AcquirerName { get; set; }

        public string AcquirerUniqueSequentialNumber { get; set; }

        public string AdministrativeCode { get; set; }

        public string CardBrandName { get; set; }

        public string CustomerCardBin { get; set; }

        public string CustomerCardLastFourDigits { get; set; }

        private string _customerReceipt;

        public string CustomerReceipt
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_customerReceipt))
                    return "";

                var arr = _customerReceipt.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList();

                arr = arr.Select(f => f.Replace("\"", "")).ToList();
                arr = arr.Select(f => f.Trim()).ToList();

                return string.Join(Environment.NewLine, arr);
            }
            set
            {
                _customerReceipt = value;
            }
        }

        private string _merchantReceipt;

        public string MerchantReceipt
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_merchantReceipt))
                    return "";

                var arr = _merchantReceipt.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList();

                arr = arr.Select(f => f.Replace("\"", "")).ToList();
                arr = arr.Select(f => f.Trim()).ToList();

                return string.Join(Environment.NewLine, arr);
            }
            set
            {
                _merchantReceipt = value;
            }
        }

        public string ReducedReceipt { get; set; }

        public string PaymentProductName { get; set; }

        public decimal PaymentTransactionAmount { get; set; }

        public int PaymentTransactionInstallments { get; set; }

        public string UniqueSequentialNumber { get; set; }

        #endregion

        #endregion

        #region Métodos Publicos

        public void CopiarPropiedades(TransacaoCapptaTef transacao)
        {
            if (transacao == null)
                return;

            AcquirerAffiliationKey = transacao.AcquirerAffiliationKey;
            AcquirerAuthorizationCode = transacao.AcquirerAuthorizationCode;
            AuthorizationDateTime = transacao.AuthorizationDateTime;
            AcquirerName = transacao.AcquirerName;
            AcquirerUniqueSequentialNumber = transacao.AcquirerUniqueSequentialNumber;
            AdministrativeCode = transacao.AdministrativeCode;
            CardBrandName = transacao.CardBrandName;
            CustomerCardBin = transacao.CustomerCardBin;
            CustomerCardLastFourDigits = transacao.CustomerCardLastFourDigits;
            CustomerReceipt = transacao.CustomerReceipt;
            MerchantReceipt = transacao.MerchantReceipt;
            ReducedReceipt = transacao.ReducedReceipt;
            PaymentProductName = transacao.PaymentProductName;
            PaymentTransactionAmount = transacao.PaymentTransactionAmount;
            PaymentTransactionInstallments = transacao.PaymentTransactionInstallments;
            UniqueSequentialNumber = transacao.UniqueSequentialNumber;
        }

        #endregion
    }
}
