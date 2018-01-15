using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public static partial class EtiquetaConfig
    {
        /// <summary>
        /// Indica o tipo de exportação/importação de etiquetas da empresa.
        /// </summary>
        public static DataSources.TipoExportacaoEtiquetaEnum TipoExportacaoEtiqueta
        {
            get { return Config.GetConfigItem<DataSources.TipoExportacaoEtiquetaEnum>(Config.ConfigEnum.TipoExportacaoEtiqueta); }
        }

        /// <summary>
        /// Indica a data que será exibida na etiqueta.
        /// </summary>
        public static DataSources.TipoDataEtiquetaEnum TipoDataEtiqueta
        {
            get { return Config.GetConfigItem<DataSources.TipoDataEtiquetaEnum>(Config.ConfigEnum.TipoDataEtiqueta); }
        }

        /// <summary>
        /// Define que o código de barras será girado em 90 graus
        /// </summary>
        public static bool Girar90GrausCodigoDeBarras
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.Girar90GrausCodigoDeBarras); }
        }
    }
}
