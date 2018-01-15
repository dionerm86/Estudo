using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.Data.ItemTemplates
{
    public class DataItemTemplate : ITemplate
    {
        private string _eval;
        private string _format;

        public DataItemTemplate(string eval)
            : this(eval, "dd/MM/yyyy HH:mm:ss")
        {
        }

        public DataItemTemplate(string eval, string format)
        {
            _eval = eval;
            _format = format;
        }

        public void InstantiateIn(Control container)
        {
            Label lblData = new Label();
            lblData.ID = "lblData" + _eval;
            lblData.DataBinding += new EventHandler(delegate(object sender, EventArgs e)
            {
                object data = DataBinder.Eval(((GridViewRow)lblData.NamingContainer).DataItem, _eval);
                lblData.Text = data != null && data.ToString() != String.Empty ? DateTime.Parse(data.ToString()).ToString(_format) : "";

            });

            container.Controls.Add(lblData);
        }
    }
}
