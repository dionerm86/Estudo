using System;
using System.Web.UI;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlTextoTooltip : BaseUserControl
    {
        #region Propriedades
    
        public string Texto
        {
            get { return lblTooltip.Text; }
            set { lblTooltip.Text = value; }
        }
    
        #endregion
    
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterClientScriptInclude("Tooltip", ResolveClientUrl("~/Scripts/wz_tooltip.js"));
            Page.PreRender += new EventHandler(Page_PreRender);
        }
    
        private void Page_PreRender(object sender, EventArgs e)
        {
            try
            {
                imgIcone.Attributes.Add("onmouseover", "TagToTip('" + divTooltip.ClientID + "', FADEIN, 200, COPYCONTENT, false);");
                imgIcone.Attributes.Add("onmouseout", "UnTip()");
            }
            catch
            {
                imgIcone.Visible = false;
                divTooltip.Visible = false;
            }
        }
    }
}
