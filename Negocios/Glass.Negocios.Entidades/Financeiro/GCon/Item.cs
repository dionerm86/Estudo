using System;

namespace Glass.Financeiro.Negocios.Entidades.GCon
{
    public class Item
    {
        #region Enums

        public enum TipoArquivoEnum
        {
            Receber = 1,
            Pagar
        }

        public enum TipoContabilEnum
        {
            ChequesDinheiro = 6,
            JurosPagos = 5101,
            JurosRecebidos = 5313,
            DescontosObtidos = 5314,
            DescontosConcedidos = 5102,
            BancoBrasil = 21,
            Banrisul = 22,
            Sicredi = 28,
            Bradesco = 23,
            AdiantadosPagamento = 171,
            AdiantadosRecebimento = 103
        }

        public enum TipoContaEnum
        {
            Credito = 1,
            Debito
        }

        public enum TipoRegistroEnum
        {
            TotalRecebimento = 1,
            TotalPagamento,
            Juros,
            Desconto,
            Recebimento,
            Pagamento
        }

        #endregion

        #region Variaveis locais

        private string _inscEstadual { get; set; }

        #endregion

        #region Propiedades

        public int IdConta { get; set; }

        public TipoArquivoEnum TipoArquivo { get; set; }

        public DateTime DataLiquidacao { get; set; }

        public string Identificador
        {
            get
            {
                return DataLiquidacao.ToShortDateString().Replace("/", "") + (TipoArquivo == TipoArquivoEnum.Receber ? "2" : "1") + NumRegistro.ToString().FormataNumero("", 9, false);
            }
        }

        public string TipoConta
        {
            get
            {
                if (TipoArquivo == TipoArquivoEnum.Receber)
                {
                    if ((TipoRegistro == TipoRegistroEnum.Juros || TipoRegistro == TipoRegistroEnum.Recebimento))
                        return "\"C \";";
                    else
                        return "\"D \";";
                }
                else
                {
                    if (TipoRegistro == TipoRegistroEnum.Pagamento || TipoRegistro == TipoRegistroEnum.Juros)
                        return "\"D \";";
                    else
                        return "\"C \";";
                }
            }
        }

        public string TipoContabil { get; set; }

        public decimal Valor { get; set; }

        public int IdCliente { get; set; }

        public string RazaoSocial { get; set; }

        public string CpfCnpj { get; set; }

        public string TipoPessoa { get; set; }

        public string InscEstadual
        {
            get
            {
                if (!string.IsNullOrEmpty(TipoPessoa) &&
                    TipoPessoa.ToLower() == "f")
                    return "ISENTO";

                return _inscEstadual;

            }
            set
            {
                _inscEstadual = value;
            }
        }

        public TipoRegistroEnum TipoRegistro { get; set; }

        public int NumeroNFe { get; set; }

        public int Parcela { get; set; }

        public string DescricaoTipoRegistro
        {
            get
            {
                switch (TipoRegistro)
                {
                    case TipoRegistroEnum.TotalRecebimento:
                        return "RECEBIMENTO DE DUPLICATAS NESTA DATA";
                    case TipoRegistroEnum.TotalPagamento:
                        return "PAGAMENTO DE DUPLICATAS NESTA DATA";
                    case TipoRegistroEnum.Juros:
                        return TipoArquivo == TipoArquivoEnum.Receber ? "JUROS RECEBIDOS da conta" : "JUROS PAGOS da conta";
                    case TipoRegistroEnum.Desconto:
                        return "Desconto da conta";
                    case TipoRegistroEnum.Recebimento:
                        return "Recebimento da conta";
                    case TipoRegistroEnum.Pagamento:
                        return "Pagamento da conta";
                    default:
                        return "";
                }
            }
        }

        public int NumRegistro { get; set; }

        public decimal Desconto { get; set; }

        public decimal Acrescimo { get; set; }

        public decimal Juros { get; set; }

        public int? IdAcerto { get; set; }

        public int? IdSinal { get; set; }

        public bool Totalizador
        {
            get
            {
                return TipoRegistro == TipoRegistroEnum.TotalRecebimento || TipoRegistro == TipoRegistroEnum.TotalPagamento;
            }
        }

        #region Valores do Tipo Contabil

        public decimal ValorBancoBrasil { get; set; }

        public decimal ValorJurosBancoBrasil { get; set; }

        public decimal ValorBanrisul { get; set; }

        public decimal ValorJurosBanrisul { get; set; }

        public decimal ValorSicredi { get; set; }

        public decimal ValorJurosSicredi { get; set; }

        public decimal ValorBradesco { get; set; }

        public decimal ValorJurosBradesco { get; set; }

        public decimal ValorDinheiroCheque { get; set; }

        public decimal ValorJurosDinheiroCheque { get; set; }

        public decimal ValorAdiantadosRecebimento { get; set; }

        public decimal ValorJurosAdiantadosRecebimento { get; set; }

        public decimal SomaValoresContabil
        {
            get
            {
                return ValorBancoBrasil +
                    ValorJurosBancoBrasil +
                    ValorBanrisul +
                    ValorJurosBanrisul +
                    ValorSicredi +
                    ValorJurosSicredi +
                    ValorDinheiroCheque +
                    ValorJurosDinheiroCheque +
                    ValorAdiantadosRecebimento +
                    ValorJurosAdiantadosRecebimento;
            }
        }

        public decimal diff { get { return Math.Round(SomaValoresContabil - (Valor + Juros), 3); } }

        #endregion

        #endregion

        #region Metodos Publicos

        /// <summary>
        /// Serializa o registro do arquivo Gcon
        /// </summary>
        /// <param name="writer"></param>
        public void Serializar(System.IO.TextWriter writer, int numRegistro)
        {
            NumRegistro = numRegistro;

            var retorno = "00;00000;00000;";
            retorno += DataLiquidacao.ToShortDateString().Replace("/", "") + ";";
            retorno += Identificador + ";";
            retorno += TipoConta;
            retorno += TipoContabil.FormataNumero("", 6, false) + ";";
            retorno += "00000;";
            retorno += (Math.Round(Valor + Desconto, 2)).ToString().Replace(",", ".").FormataNumero("", 16, false) + ";";
            retorno += "0;2;0000;";
            if (Totalizador)
                retorno += "\"" + DescricaoTipoRegistro.FormataTexto("", 200, false) + "\";";
            else
                retorno += "\"" + (DescricaoTipoRegistro + " " + NumeroNFe + "-" + Parcela + "-" + IdCliente + "-" + RazaoSocial).FormataTexto("", 200, false) + "\";";
            retorno += "000000000000000000;";
            retorno += "\"" + Glass.Formatacoes.RetiraCaracteresEspeciais(CpfCnpj).FormataTexto("", 14, false) + "\";";
            retorno += "\"" + Glass.Formatacoes.RetiraCaracteresEspeciais(InscEstadual).FormataTexto("", 14, false) + "\";";
            retorno += TipoArquivo == TipoArquivoEnum.Receber ? "\"1\";" : "\"2\";";


            writer.WriteLine(retorno);
        }

        #endregion
    }
}
