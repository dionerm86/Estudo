using GDA;
using Glass.Data.DAL;
using System;
using System.ComponentModel;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(PagtoNotaFiscalDAO))]
    [PersistenceClass("pagto_nota_fiscal")]
    [Serializable]
    public class PagtoNotaFiscal
    {
        #region Enumeradores

        public enum FormaPagtoEnum
        {
            [Description("Dinheiro")]
            Dinheiro = 1,

            [Description("Cheque")]
            Cheque,

            [Description("Cartão de Crédito")]
            CartaoCredito,

            [Description("Cartão de Débito")]
            CartaoDebito,

            [Description("Crédito Loja")]
            CreditoLoja,

            [Description("Vale Alimentação")]
            ValeAlimentacao = 10,

            [Description("Vale Refeição")]
            ValeRefeicao,

            [Description("Vale Presente")]
            ValePresente,

            [Description("Vale Combustível")]
            ValeCombustivel,

            [Description("Outros")]
            Outros = 99
        }

        public enum BandeiraEnum
        {
            Visa = 1,
            Mastercard,
            AmericanExpress,
            Sorocred,
            Outros = 99
        }

        public enum TipoIntegracaoEnum
        {
            Integrado = 1,
            NaoIntegrado
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IdPagtoNf", PersistenceParameterType.IdentityKey)]
        public int IdPagtoNf { get; set; }

        [PersistenceProperty("IdNf")]
        public int IdNf { get; set; }

        [PersistenceProperty("FormaPagto")]
        public int FormaPagto { get; set; }

        [PersistenceProperty("Valor")]
        public decimal Valor { get; set; }

        [PersistenceProperty("CnpjCredenciadora")]
        public string CnpjCredenciadora { get; set; }

        [PersistenceProperty("Bandeira")]
        public int? Bandeira { get; set; }

        [PersistenceProperty("NumAut")]
        public string NumAut { get; set; }

        #endregion

        #region Propiedades de Suporte

        public string DescrFormaPagto
        {
            get
            {
                return Colosoft.Translator.Translate(((FormaPagtoEnum)FormaPagto)).Format();
            }
        }

        #endregion
    }
}
