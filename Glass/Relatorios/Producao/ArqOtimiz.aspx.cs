using System;
using System.Web.UI;

namespace Glass.UI.Web.Relatorios.Producao
{
    public partial class ArqOtimiz : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdArqOtimiz.PageIndex = 0;
        }

        protected string MontarEnderecoECutter()
        {
            var address = Request.Url.AbsoluteUri;

            address = address.Substring(0, address.IndexOf("/relatorios", StringComparison.InvariantCultureIgnoreCase));
            var token = Request.Cookies[System.Web.Security.FormsAuthentication.FormsCookieName]?.Value;

            var uri = new Uri($"{address}/handlers/ecutteroptimizationservice.ashx?launcher=OptyWay&token={token}&id=");

            return $"ecutter-opt{uri.AbsoluteUri.Substring(uri.Scheme.Length)}";
        }
    }
}
