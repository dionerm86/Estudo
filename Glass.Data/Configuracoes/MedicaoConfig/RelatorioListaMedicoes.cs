using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    partial class MedicaoConfig
    {
        public class RelatorioListaMedicoes
        {
            /// <summary>
            /// Define que a listagem de medição será ordenada pelo Id Medicao na impressão e na listagem
            /// </summary>
            public static bool OrdenarPeloIdMedicao
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.OrdenarPeloIdMedicao); }
            }
        }
    }
}
