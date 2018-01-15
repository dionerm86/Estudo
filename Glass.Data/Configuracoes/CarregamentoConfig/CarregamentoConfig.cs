using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glass.Data.Helper;
using Glass.Data.Model;

namespace Glass.Configuracoes
{
    public partial class CarregamentoConfig
    {
        /// <summary>
        /// Define se os carregamentos serão mostrados na tela (Ordem de Carga > Carregamentos) ao ser aberta.
        /// </summary>
        public static bool CarregarRegistrosAoAbrirTela
        {
            get
            {
                switch (ControleSistema.GetSite())
                {
                    case ControleSistema.ClienteSistema.Mirandex:
                    case ControleSistema.ClienteSistema.MirandexAcre:
                        return false;

                    default:
                        return true;
                }
            }
        }
    }
}