using GDA;
using Glass.Data.DAL;
using System;
using System.ComponentModel;

namespace Glass.Data.Model
{
    #region Enumeradores

    public enum FormaPagtoNotaFiscalEnum
    {
        [Description("Dinheiro")]
        Dinheiro = 1,

        [Description("Cheque")]
        Cheque = 2,

        [Description("Cartão de Crédito")]
        CartaoCredito = 3,

        [Description("Cartão de Débito")]
        CartaoDebito = 4,

        [Description("Crédito Loja")]
        CreditoLoja = 5,

        [Description("Vale Alimentação")]
        ValeAlimentacao = 10,

        [Description("Vale Refeição")]
        ValeRefeicao = 11,

        [Description("Vale Presente")]
        ValePresente = 12,

        [Description("Vale Combustível")]
        ValeCombustivel = 13,

        [Description("Boleto Bancário")]
        BoletoBancario = 15,

        [Description("Sem pagamento")]
        SemPagamento = 90,

        [Description("Outros")]
        Outros = 99
    }

    public enum BandeiraEnum
    {
        [Description("Visa")]
        Visa = 1,

        [Description("Mastercard")]
        Mastercard = 2,

        [Description("American Express")]
        AmericanExpress = 3,

        [Description("Sorocred")]
        Sorocred = 4,

        [Description("DinersClub")]
        DinersClub = 5,

        [Description("Elo")]
        Elo = 6,

        [Description("Hipercard")]
        Hipercard = 7,

        [Description("Aura")]
        Aura = 8,

        [Description("Cabal")]
        Cabal = 9,

        [Description("Outros")]
        Outros = 99
    }

    public enum TipoIntegracaoEnum
    {
        Integrado = 1,
        NaoIntegrado
    }

    #endregion

    [PersistenceBaseDAO(typeof(PagtoNotaFiscalDAO))]
    [PersistenceClass("pagto_nota_fiscal")]
    [Serializable]
    public class PagtoNotaFiscal
    {
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
                return Colosoft.Translator.Translate(((FormaPagtoNotaFiscalEnum)FormaPagto)).Format();
            }
        }

        /// <summary>
        /// Retorna o IdFormaPagto correspondente à forma de pagamento da Nota Fiscal
        /// </summary>
        public Pagto.FormaPagto IdFormaPagtoCorrespondente
        {
            get
            {
                switch (FormaPagto)
                {
                    case (int)FormaPagtoNotaFiscalEnum.Dinheiro:
                        return Pagto.FormaPagto.Dinheiro;
                    case (int)FormaPagtoNotaFiscalEnum.Cheque:
                        return Pagto.FormaPagto.ChequeTerceiro;
                    case (int)FormaPagtoNotaFiscalEnum.CartaoCredito:
                    case (int)FormaPagtoNotaFiscalEnum.CartaoDebito:
                        return Pagto.FormaPagto.Cartao;
                    case (int)FormaPagtoNotaFiscalEnum.CreditoLoja:
                        return Pagto.FormaPagto.Credito;
                    case (int)FormaPagtoNotaFiscalEnum.ValeAlimentacao:
                    case (int)FormaPagtoNotaFiscalEnum.ValeCombustivel:
                    case (int)FormaPagtoNotaFiscalEnum.ValePresente:
                    case (int)FormaPagtoNotaFiscalEnum.ValeRefeicao:
                        return Pagto.FormaPagto.Prazo;
                    case (int)FormaPagtoNotaFiscalEnum.BoletoBancario:
                        return Pagto.FormaPagto.Boleto;
                    default:
                        return Pagto.FormaPagto.Prazo;
                }
            }
        }

        #endregion
    }
}
