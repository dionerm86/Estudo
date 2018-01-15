using System;
using System.Web.UI;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlNovaLinhaGrid : BaseUserControl
    {
        #region Propriedades
    
        [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        public string InnerHTML { get; set; }
    
        public int PaddingTop { get; set; }
    
        public ITemplate Controles { get; set; }
    
        #endregion
    
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected string GetHtmlExibir()
        {
            if (String.IsNullOrEmpty(InnerHTML))
                return String.Empty;
    
            return InnerHTML.Replace("~/", this.ResolveClientUrl("~/"));
        }
    }
}
