using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.Data.ItemTemplates
{
    public class NumeroItemTemplate : ITemplate
    {
        private string _eval;
        private string _format;

        public NumeroItemTemplate(string eval)
            : this(eval, "N2")
        {
        }

        public NumeroItemTemplate(string eval, string format)
        {
            _eval = eval;
            _format = format;
        }

        public void InstantiateIn(Control container)
        {
            Label lblNumero = new Label();
            lblNumero.ID = "lblNumero" + _eval;
            lblNumero.DataBinding += new EventHandler(delegate(object sender, EventArgs e)
            {
                object numero = DataBinder.Eval(((GridViewRow)lblNumero.NamingContainer).DataItem, _eval);
                lblNumero.Text = numero != null && numero.ToString() != String.Empty ? float.Parse(numero.ToString()).ToString(_format) : "";

            });

            container.Controls.Add(lblNumero);
        }
    }
}
