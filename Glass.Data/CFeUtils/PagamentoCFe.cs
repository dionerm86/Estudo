namespace Glass.Data.CFeUtils
{
    public class PagamentoCFe
    {
        #region Enumeradores

        public enum FormaPagamentoCFe
        {
            Dinheiro = 1,
            Cheque = 2,
            CartaoCredito = 3,
            CartaoDebito = 4,
            ValeRefeicao = 5,
            ValeAlimentacao = 6,
            ValePresente = 7,
            CreditoPorFinanceira = 8,
            DebitoFolhaPagamentoFuncionarios = 9,
            PagamentoBancario = 10,
            CreditoDevoluçãoMercadoria = 11,
            CreditoEmpresaConveniada = 12,
            PagamentoAntecipado = 13,
            OutrosValesOuMeiosPagamento = 99
        }

        public enum OperadorasCartaoCreditoCFe
        {
            Nenhum = 0,
            AdministradoraCartoesSicrediLtda = 1,
            AdministradoraCartoesSicrediLtdaFilialRS = 2,
            BancoAmericanExpress = 3,
            BancoGECapital = 4,
            BancoSafra = 5,
            BancoTopazio = 6,
            BancoTriangulo = 7,
            BigCardAdmConveniosServ = 8,
            BourbonAdmCartoesCredito = 9,
            CabalBrasil = 10,
            CetelemBrasil = 11,
            Cielo = 12,
            Credi21ParticipacoesLtda = 13,
            EcxCardAdmProcessadoraCartoes = 14,
            Embratec = 15,
            EmporioCardLtda = 16,
            FreeddomTecnologiaServicos = 17,
            FuncionalCardLtda = 18,
            HipercardBancoMultiplo = 19,
            MapaAdminConvCartoesLtda = 20,
            NovoPagAdmProcMeiosEletronicosPagto = 21,
            PernambucanasFinanciadoraCredito = 22,
            PolicardSystemsServicos = 23,
            ProvarNegociosVarejo = 24,
            RedeCard = 25,
            RennerAdmCartoesCreditoLtda = 26,
            RPAdministraçaoConveniosLtda = 27,
            SantinvestCreditoFinanciamentoInvestimentos = 28,
            SodexHoPassServicosComercio = 29,
            SorocredMeiosPagamentos = 30,
            TecnologiaBancariaSA_TECBAN = 31,
            TicketServiços = 32,
            TrivaleAdministracao = 33,
            UnicardBancoMultiploTricard = 34,
            Outros = 999
        }

        #endregion

        #region Propriedades

        public FormaPagamentoCFe FormaPagamento { get; set; }

        public float ValorPagamento { get; set; }

        public OperadorasCartaoCreditoCFe OperadoraCartao { get; set; }

        #endregion
    }
}