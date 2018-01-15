using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Glass.Data.DAL;

namespace Glass.UI.Web.Utils
{
    public partial class Rota : System.Web.UI.Page
    {
        protected string keyGoogleMaps;
    
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Utils.Mapa));
    
            keyGoogleMaps = System.Configuration.ConfigurationSettings.AppSettings["googleMaps"];
        }
    
        /// <summary>
        /// Verifica se existe algum ponto dados os filtros passados
        /// </summary>
        /// <param name="idEquipe"></param>
        /// <param name="dtInicio"></param>
        /// <param name="dtFim"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string ExistePontos(string idEquipe, string dtInicio, string dtFim)
        {
            return PontosRotaDAO.Instance.CheckPontosByEquipe(Glass.Conversoes.StrParaUint(idEquipe), dtInicio, dtFim).ToString();
        }
    }
}
