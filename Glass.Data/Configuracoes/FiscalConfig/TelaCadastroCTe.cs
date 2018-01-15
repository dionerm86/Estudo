using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class FiscalConfig
    {
        public class TelaCadastroCTe
        {
            #region CTe Saída

            /// <summary>
            /// Valor padrão do campo Reponsavel do controle SeguroCte.
            /// </summary>
            public static string ReponsavelSeguroCtePadraoCteSaida
            {
                get { return Config.GetConfigItem<string>(Config.ConfigEnum.ReponsavelSeguroCtePadraoCteSaida); }
            }

            /// <summary>
            /// Valor padrão do campo TipoPeriodoData do controle EntregaCte.
            /// </summary>
            public static string TipoPeriodoDataEntregaCtePadraoCteSaida
            {
                get { return Config.GetConfigItem<string>(Config.ConfigEnum.TipoPeriodoDataEntregaCtePadraoCteSaida); }
            }

            /// <summary>
            /// Valor padrão do campo TipoPeriodoHora do controle EntregaCte.
            /// </summary>
            public static string TipoPeriodoHoraEntregaCtePadraoCteSaida
            {
                get { return Config.GetConfigItem<string>(Config.ConfigEnum.TipoPeriodoHoraEntregaCtePadraoCteSaida); }
            }

            /// <summary>
            /// Valor padrão do campo ProdutoPredominante do controle InfoCte.
            /// </summary>
            public static string ProdutoPredominanteInfoCtePadraoCteSaida
            {
                get { return Config.GetConfigItem<string>(Config.ConfigEnum.ProdutoPredominanteInfoCtePadraoCteSaida); }
            }

            #region Info carga CTe

            /// <summary>
            /// Valor padrão do campo TipoUnidade do controle InfoCargaCte.
            /// </summary>
            public static string TipoUnidadeInfoCargaCtePadraoCteSaida
            {
                get { return Config.GetConfigItem<string>(Config.ConfigEnum.TipoUnidadeInfoCargaCtePadraoCteSaida); }
            }

            /// <summary>
            /// Valor padrão do campo Quantidade do controle InfoCargaCte.
            /// </summary>
            public static string QuantidadeInfoCargaCtePadraoCteSaida
            {
                get { return Config.GetConfigItem<string>(Config.ConfigEnum.QuantidadeInfoCargaCtePadraoCteSaida); }
            }

            /// <summary>
            /// Valor padrão do campo TipoMedida do controle InfoCargaCte.
            /// </summary>
            public static string TipoMedidaInfoCargaCtePadraoCteSaida
            {
                get { return Config.GetConfigItem<string>(Config.ConfigEnum.TipoMedidaInfoCargaCtePadraoCteSaida); }
            }

            #endregion

            #region Imposto CTe

            #region ICMS

            /// <summary>
            /// Valor padrão do campo CSTICMS do controle ImpostoCte.
            /// </summary>
            public static string CSTICMSImpostoCtePadraoCteSaida
            {
                get { return Config.GetConfigItem<string>(Config.ConfigEnum.CSTICMSImpostoCtePadraoCteSaida); }
            }

            #endregion

            #region PIS

            /// <summary>
            /// Valor padrão do campo CSTPIS do controle ImpostoCte.
            /// </summary>
            public static string CSTPISImpostoCtePadraoCteSaida
            {
                get { return Config.GetConfigItem<string>(Config.ConfigEnum.CSTPISImpostoCtePadraoCteSaida); }
            }

            /// <summary>
            /// Valor padrão do campo BCSTRetidoPIS do controle ImpostoCte.
            /// </summary>
            public static string BCSTRetidoPISImpostoCtePadraoCteSaida
            {
                get { return Config.GetConfigItem<string>(Config.ConfigEnum.BCSTRetidoPISImpostoCtePadraoCteSaida); }
            }

            /// <summary>
            /// Valor padrão do campo AliquotaPIS do controle ImpostoCte.
            /// </summary>
            public static string AliquotaPISImpostoCtePadraoCteSaida
            {
                get { return Config.GetConfigItem<string>(Config.ConfigEnum.AliquotaPISImpostoCtePadraoCteSaida); }
            }

            /// <summary>
            /// Valor padrão do campo ValorSTRetidoPIS do controle ImpostoCte.
            /// </summary>
            public static string ValorSTRetidoPISImpostoCtePadraoCteSaida
            {
                get { return Config.GetConfigItem<string>(Config.ConfigEnum.ValorSTRetidoPISImpostoCtePadraoCteSaida); }
            }

            /// <summary>
            /// Informa se o imposto PIS é obrigatório no CTe de saída
            /// </summary>
            public static bool PISObrigatorioCTeSaida
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.PISObrigatorioCTeSaida); }
            }

            #endregion

            #region COFINS

            /// <summary>
            /// Valor padrão do campo CSTCOFINS do controle ImpostoCte.
            /// </summary>
            public static string CSTCOFINSImpostoCtePadraoCteSaida
            {
                get { return Config.GetConfigItem<string>(Config.ConfigEnum.CSTCOFINSImpostoCtePadraoCteSaida); }
            }

            /// <summary>
            /// Valor padrão do campo BCSTRetidoCOFINS do controle ImpostoCte.
            /// </summary>
            public static string BCSTRetidoCOFINSImpostoCtePadraoCteSaida
            {
                get { return Config.GetConfigItem<string>(Config.ConfigEnum.BCSTRetidoCOFINSImpostoCtePadraoCteSaida); }
            }

            /// <summary>
            /// Valor padrão do campo AliquotaCOFINS do controle ImpostoCte.
            /// </summary>
            public static string AliquotaCOFINSImpostoCtePadraoCteSaida
            {
                get { return Config.GetConfigItem<string>(Config.ConfigEnum.AliquotaCOFINSImpostoCtePadraoCteSaida); }
            }

            /// <summary>
            /// Valor padrão do campo ValorSTRetidoCOFINS do controle ImpostoCte.
            /// </summary>
            public static string ValorSTRetidoCOFINSImpostoCtePadraoCteSaida
            {
                get { return Config.GetConfigItem<string>(Config.ConfigEnum.ValorSTRetidoCOFINSImpostoCtePadraoCteSaida); }
            }

            /// <summary>
            /// Informa se o imposto COFINS é obrigatório no CTe de saída
            /// </summary>
            public static bool COFINSObrigatorioCTeSaida
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.COFINSObrigatorioCTeSaida); }
            }

            #endregion

            #endregion

            /// <summary>
            /// Valor padrão do campo Transportador do controle OrdemColetaCteRod.
            /// </summary>
            public static string TransportadorOrdemColetaCteRodPadraoCteSaida
            {
                get { return Config.GetConfigItem<string>(Config.ConfigEnum.TransportadorOrdemColetaCteRodPadraoCteSaida); }
            }

            /// <summary>
            /// Valor padrão do campo TipoCte.
            /// </summary>
            public static string TipoCtePadraoCteSaida
            {
                get { return Config.GetConfigItem<string>(Config.ConfigEnum.TipoCtePadraoCteSaida); }
            }

            /// <summary>
            /// Valor padrão do campo TipoServico.
            /// </summary>
            public static string TipoServicoPadraoCteSaida
            {
                get { return Config.GetConfigItem<string>(Config.ConfigEnum.TipoServicoPadraoCteSaida); }
            }

            /// <summary>
            /// Valor padrão do campo CidadeEnvio.
            /// </summary>
            public static string CidadeEnvioPadraoCteSaida
            {
                get { return Config.GetConfigItem<string>(Config.ConfigEnum.CidadeEnvioPadraoCteSaida); }
            }

            /// <summary>
            /// Valor padrão do campo CidadeInicio.
            /// </summary>
            public static string CidadeInicioPadraoCteSaida
            {
                get { return Config.GetConfigItem<string>(Config.ConfigEnum.CidadeInicioPadraoCteSaida); }
            }

            /// <summary>
            /// Valor padrão do campo Serie.
            /// </summary>
            public static string SeriePadraoCteSaida
            {
                get { return Config.GetConfigItem<string>(Config.ConfigEnum.SeriePadraoCteSaida); }
            }

            /// <summary>
            /// Valor padrão do campo Lotacao do controle ConhecimentoTransRod.
            /// </summary>
            public static bool LotacaoConhecimentoTransRodPadraoCteSaida
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.LotacaoConhecimentoTransRodPadraoCteSaida); }
            }

            #endregion
        }
    }
}
