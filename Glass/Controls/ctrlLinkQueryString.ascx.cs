using System;
using System.ComponentModel;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlLinkQueryString : BaseUserControl
    {
        private string m_NameQueryString = "";
    
        public string NameQueryString
        {
            get { return m_NameQueryString; }
            set { m_NameQueryString = value; }
        }
    
        [Bindable(true)]
        public string Text
        {
            get { return hdfLink.Value; }
            set { hdfLink.Value = value; }
        }
    
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            string value = Request[m_NameQueryString];
    
            if (value != null)
                hdfLink.Value = value;
        }
    
    }
}
