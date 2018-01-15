using System;
using System.Configuration;

namespace Glass.UI.Web.Utils
{
    public partial class Rastrear : System.Web.UI.Page
    {
        protected string keyGoogleMaps;
    
        protected void Page_Load(object sender, EventArgs e)
        {
            keyGoogleMaps = ConfigurationManager.AppSettings["googleMaps"];
        }
    }
}
