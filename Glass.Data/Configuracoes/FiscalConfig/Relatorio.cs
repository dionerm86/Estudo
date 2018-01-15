using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    partial class FiscalConfig
    {
        public class Relatorio
        {
            /// <summary>
            /// Define se a empresa exibe o total de produtos de CST 60 no relatório.
            /// </summary>
            public static bool RecuperarTotalCst60
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.RecuperarTotalCst60); }
            }

            /// <summary>
            /// Define que será usada a data de entrada/saída ou datacad da nota ao invés da data entrada/saída ou data emissão no livro de registro
            /// </summary>
            public static bool UsarDataCadNotaLivroRegistro
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarDataCadNotaLivroRegistro); }
            }
        }
    }
}
