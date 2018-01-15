using Glass.Data.Helper;
using Glass.Data.DAL;

namespace Glass.Configuracoes
{
    public partial class InstalacaoConfig
    {
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
    }
}
