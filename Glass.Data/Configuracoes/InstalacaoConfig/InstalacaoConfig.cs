using Glass.Data.Helper;
using Glass.Data.DAL;

namespace Glass.Configuracoes
{
    public partial class InstalacaoConfig
    {
        public static bool UsarControleEntregaInstalacao
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarControleEntregaInstalacao); }
        }

        /// <summary>
        /// Retora o ID padrão do processo para a instalação.
        /// </summary>
        public static uint? ProcessoInstalacao
        {
            get { return EtiquetaProcessoDAO.Instance.ObtemIdProcesso(Config.GetConfigItem<string>(Config.ConfigEnum.ProcessoInstalacao)); }
        }

        /// <summary>
        /// Retorna o ID padrão da aplicação para a instalação.
        /// </summary>
        public static uint? AplicacaoInstalacao
        {
            get { return EtiquetaAplicacaoDAO.Instance.ObtemIdAplicacao(Config.GetConfigItem<string>(Config.ConfigEnum.AplicacaoInstalacao)); }
        }

        /// <summary>
        /// Define se a situação inicial de uma instalação será Depto Técnico
        /// </summary>
        public static bool SituacaoInicialDeptoTecnico
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.SituacaoInicialDeptoTecnico); }
        }

        /// <summary>
        /// Define se serão usadas as seguintes situações adicionais nas instalações: Depto. Técnico, Produção, A Agendar, Colagem
        /// </summary>
        public static bool UsarSituacoesExtrasInstalacao
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarSituacoesExtrasInstalacao); }
        }
    }
}
