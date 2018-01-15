using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.Data.ItemTemplates
{
    public class StringItemTemplate : ITemplate
    {
        private string _eval;

        public StringItemTemplate(string eval)
        {
            _eval = eval;
        }

        public void InstantiateIn(Control container)
        {
            Label lblString = new Label();
            lblString.ID = "lblString" + _eval;

            lblString.DataBinding += new EventHandler(delegate(object sender, EventArgs e)
            {
                object data = DataBinder.Eval(((GridViewRow)lblString.NamingContainer).DataItem, _eval);
                lblString.Text = data != null && data.ToString() != String.Empty ? data.ToString() : "";

            });

            container.Controls.Add(lblString);
        }
    }
}
