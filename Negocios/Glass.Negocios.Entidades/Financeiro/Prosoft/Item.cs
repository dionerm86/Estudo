using System;
using System.ComponentModel;

namespace Glass.Financeiro.Negocios.Entidades.Prosoft
{
    public class Item : ICloneable
    {
        #region Enums

        public enum TipoArquivoEnum
        {
            Receber = 1,
            Pagar
        }

        public enum TipoContabilEnum
        {
            [Description("Depósito - Boleto")]
            Caixa2875 = 11103,

            [Description("Depósito - Boleto")]
            Santander002999 = 11106,

            [Description("Depósito - Boleto")]
            BancoNordeste29053 = 11113,

            [Description("Depósito - Boleto")]
            BancoNordeste30869 = 22535,

            [Description("Depósito - Boleto")]
            BancoNordeste31244 = 24900,

            [Description("Depósito - Boleto")]
            BancoNordeste37942 = 27657,

            [Description("Dinheiro")]
            Dinheiro = 11111,

            [Description("Cheque")]
            Cheque = 11112,

            [Description("Crédito")]
            Credito = 11110,

            [Description("Cartão")]
            Cartao = 11114,

            [Description("Outro")]
            Outro = 11115,
        }

        #endregion

        #region Propiedades

        public DateTime DataLiquidacao { get; set; }

        public int IdConta { get; set; }

        public int NumeroNFe { get; set; }

        public string CpfCnpj { get; set; }

        public decimal Valor { get; set; }

        public string Obs { get; set; }

        public TipoArquivoEnum TipoArquivo { get; set; }

        public int Identificador
        {
            get
            {
                return NumeroNFe > 0 ? NumeroNFe : IdConta;
            }
        }

        public TipoContabilEnum TipoContabil { get; set; }

        public int? IdAcerto { get; set; }

        public int? IdSinal { get; set; }

        #endregion

        #region Metodos Publicos

        /// <summary>
        /// Serializa o registro do arquivo Gcon
        /// </summary>
        /// <param name="writer"></param>
        public void Serializar(System.IO.TextWriter writer, int numRegistro)
        {
            var retorno = "LC1";
            retorno += numRegistro.ToString().FormataNumero("Sequencial", 5, true);
            retorno += "".FormataTexto("Filler", 3, true);
            retorno += 1.ToString().FormataNumero("Modo Lançamento", 1, true);
            retorno += DataLiquidacao.ToShortDateString().Replace("/", "");
            retorno += Identificador.ToString().FormataNumero("Numero do documento", 10, true);
            retorno += "".FormataTexto("Numero do lote", 5, true);
            retorno += "".FormataTexto("Origem do Lançamento", 30, true);
            retorno += "".FormataTexto("Qtde Contas", 3, true);

            var codAcesso = (int)TipoContabil;
            var codInterno = "";

            if (TipoContabil == TipoContabilEnum.BancoNordeste29053 || TipoContabil == TipoContabilEnum.BancoNordeste30869 ||
                TipoContabil == TipoContabilEnum.BancoNordeste31244 || TipoContabil == TipoContabilEnum.BancoNordeste37942)
            {
                codAcesso = 11113;
                codInterno = ((int)TipoContabil).ToString();
            }

            if (TipoArquivo == TipoArquivoEnum.Pagar)
            {
                retorno += "21201".FormataNumero("Cód Acesso", 5, true);
                retorno += Glass.Formatacoes.RetiraCaracteresEspeciais(CpfCnpj).FormataTexto("CPF/CNPJ", 14, true);
                retorno += "".FormataTexto("C/Custo", 5, true);

                retorno += codAcesso.ToString().FormataNumero("Cód Acesso", 5, true);
                retorno += !string.IsNullOrEmpty(codInterno) ? codInterno.FormataNumero("Cód Interno", 6, true) + "".FormataTexto("", 8, false) : codInterno.FormataTexto("Cód Interno", 14, true);
                retorno += "".FormataTexto("C/Custo", 5, true);
            }
            else
            {
                retorno += codAcesso.ToString().FormataNumero("Cód Acesso", 5, true);
                retorno += !string.IsNullOrEmpty(codInterno) ? codInterno.FormataNumero("Cód Interno", 6, true) + "".FormataTexto("", 8, false) : codInterno.FormataTexto("Cód Interno", 14, true);
                retorno += "".FormataTexto("C/Custo", 5, true);

                retorno += "11202".FormataNumero("Cód Acesso", 5, true);
                retorno += Glass.Formatacoes.RetiraCaracteresEspeciais(CpfCnpj).FormataTexto("CPF/CNPJ", 14, true);
                retorno += "".FormataTexto("C/Custo", 5, true);
            }

            retorno += Math.Round(Valor, 2).ToString().Replace(",", ".").FormataNumero("Valor Lançamento", 16, true);
            retorno += (!string.IsNullOrEmpty(Obs) ? Obs : "Forma Pagto.: " + Colosoft.Translator.Translate(TipoContabil).Format()).FormataTexto("Historico", 240, false);
            retorno += "".FormataTexto("Ind. Conciliação - Deb", 1, false);
            retorno += "".FormataTexto("Ind. Conciliação - Cred", 1, false);
            retorno += "".FormataTexto("Filler", 74, false);

            writer.WriteLine(retorno);
        }

        public object Clone()
        {
            return (Item)this.MemberwiseClone();
        }

        #endregion
    }
}
