using System;
using System.Web.UI;

namespace Glass.UI.Web.Utils
{
    public partial class SelFornec : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            hdfNfe.Value = Request["Nfe"];
    
            if (!IsPostBack)
                txtNomeFantasia.Focus();
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdFornecedor.PageIndex = 0;
        }
    
        protected void imgExcluirFiltro_Click(object sender, ImageClickEventArgs e)
        {
            txtNomeFantasia.Text = "";
            txtCodigo.Text = "";
            grdFornecedor.PageIndex = 0;
        }
    
        protected void lnkNovoFornec_Click(object sender, EventArgs e)
        {
            Response.Redirect("../cadastros/cadFornecedor.aspx?popup=1");
        }
    }
}
