using Glass.Data.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Configuracoes
{
    public partial class FiscalConfig
    {
        public static class ManifestoEletronico
        {
            /// <summary>
            /// Indica se a empresa usará a contingência de MDF-e.
            /// </summary>
            public static DataSources.TipoContingenciaMDFe ContingenciaMDFe
            {
                get { return Config.GetConfigItem<DataSources.TipoContingenciaMDFe>(Config.ConfigEnum.ContingenciaMDFe); }
            }

            /// <summary>
            /// Retorna a justificativa da contingência do MDF-e.
            /// </summary>
            public static string JustificativaContingenciaMDFe
            {
                get { return Config.GetConfigItem<string>(Config.ConfigEnum.JustificativaContingenciaMDFe); }
            }
        }
    }
}
