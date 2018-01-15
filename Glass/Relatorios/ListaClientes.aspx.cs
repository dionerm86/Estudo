using System;
using System.Web.UI;
using Glass.Data.DAL;

namespace Glass.UI.Web.Relatorios
{
    public partial class ListaClientes : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            if (!IsPostBack)
            {
                lblApenasSemRota.Visible = imgPesqSemRota.Visible = chkApenasSemRota.Visible = RotaDAO.Instance.GetCount() > 0;
                lblApenasSemPrecoTabela.Visible = imgPesqSemTabela.Visible = chkApenasSemPrecoTabela.Visible = TabelaDescontoAcrescimoClienteDAO.Instance.GetCountReal() > 0;
    
                chkApenasSemRota.Attributes.Add("onclick", "disableRota()");
    
                if (chkApenasSemRota.Checked)
                    lblRota.Style["visibility"] = txtRota.Style["visibility"] = imgPesqRota.Style["visibility"] = "hidden";
                else
                    lblRota.Style["visibility"] = txtRota.Style["visibility"] = imgPesqRota.Style["visibility"] = "visible";
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdClientes.PageIndex = 0;
        }
    }
}
