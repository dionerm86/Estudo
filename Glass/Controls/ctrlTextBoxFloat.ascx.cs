using System;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlTextBoxFloat : BaseUserControl
    {
        [Bindable(true)]
        public string Value
        {
            get { return txtNumber.Text; }
            set { txtNumber.Text = value; }
        }
    
        public short TabIndex
        {
            get { return txtNumber.TabIndex; }
            set { txtNumber.TabIndex = value; }
        }
    
        [Bindable(true)]
        public bool AcceptsEmptyValue
        {
            get { return !ctvTextBoxFloat.Visible; }
            set { ctvTextBoxFloat.Visible = !value; }
        }
    
        public Unit Width
        {
            get { return txtNumber.Width; }
            set { txtNumber.Width = value; }
        }
    
        protected void Page_Load(object sender, EventArgs e)
        {
            foreach (string a in Attributes.Keys)
                txtNumber.Attributes.Add(a, Attributes[a]);
        }
    }
}
