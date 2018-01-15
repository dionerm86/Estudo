using Glass.Data.DAL;
using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class ProjetoConfig
    {
        public static class Caixilho
        {
            /// <summary>
            /// Retorna o ID padrão do processo para o caixilho.
            /// </summary>
            public static uint? ProcessoCaixilho
            {
                get { return EtiquetaProcessoDAO.Instance.ObtemIdProcesso(Config.GetConfigItem<string>(Config.ConfigEnum.ProcessoCaixilho)); }
            }

            /// <summary>
            /// Retorna o ID padrão da aplicação para o caixilho.
            /// </summary>
            public static uint? AplicacaoCaixilho
            {
                get { return EtiquetaAplicacaoDAO.Instance.ObtemIdAplicacao(Config.GetConfigItem<string>(Config.ConfigEnum.AplicacaoCaixilho)); }
            }
        }
    }
}
