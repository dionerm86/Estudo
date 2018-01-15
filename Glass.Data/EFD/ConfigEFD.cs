using System;

namespace Glass.Data.EFD
{
    public static class ConfigEFD
    {
        #region Enumeradores

        public enum NaturezaPjEnum
        {
            PjEmGeral = 0,
            SociedadeCooperativa,

            /// <summary>
            /// Entidade sujeita ao PIS/Pasep exclusivamente com base na Folha de Salários
            /// </summary>
            SujeitaPisPasepExclusivamenteFolhaSalarios
        }

        public enum TipoImpostoEnum
        {
            ICMS = 1,
            ICMSST,
            IPI
        }

        public enum TipoAjusteEnum
        {
            AjusteDebito = 0,
            AjusteCredito
        }

        public enum IndicadorOrigemDocumentoEnum
        {
            ProcessoJudicial = 0,
            ProcessoAdministrativo,
            PER_DCOMP,
            Outros = 9
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Versão do leiaute do EFD Fiscal
        /// </summary>
        public static int VersaoLeiauteFiscal(DateTime dataInicio)
        {
            if (dataInicio >= DateTime.Parse("01/01/2014"))
                return 8;
            else if (dataInicio >= DateTime.Parse("01/01/2013"))
                return 7;
            else if (dataInicio >= DateTime.Parse("01/07/2012"))
                return 6;
            else if (dataInicio >= DateTime.Parse("01/01/2012"))
                return 5;
            else if (dataInicio >= DateTime.Parse("01/01/2011"))
                return 4;
            else if (dataInicio >= DateTime.Parse("01/01/2010"))
                return 3;
            else
                return 2;
        }

        /// <summary>
        /// Versão do leiaute do EFD Contribuições
        /// </summary>
        public static int VersaoLeiauteContribuicoes(DateTime dataInicio)
        {
            if (dataInicio >= DateTime.Parse("01/07/2012"))
                return 3;
            else
                return 2;
        }

        public static string VersaoLeiauteFCI()
        {
            return "1.0";
        }

        /// <summary>
        /// Natureza da Pessoa Jurídica.
        /// </summary>
        public static NaturezaPjEnum NaturezaPj
        {
            get { return NaturezaPjEnum.PjEmGeral; }
        }

        /// <summary>
        /// Define se a empresa usa a data de entrada/saída preferencialmente sobre a data de emissão
        /// da nota fiscal. Usado no registro E110 para o somatório dos registros C100, C500, D100 e D500.
        /// </summary>
        public static bool UsarDataEntradaSaidaRegE110
        {
            get { return true; }
        }

        public static string TipoUtilizacaoCreditoFiscal(string uf)
        {
            switch (uf)
            {
                default: return uf.ToUpper() + "01";
            }
        }

        #endregion
    }
}