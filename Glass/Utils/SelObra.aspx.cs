using System;
using System.Web.UI;
using Glass.Data.DAL;

namespace Glass.UI.Web.Utils
{
    public partial class SelObra : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(Request["situacao"]))
            {
                drpSituacao.SelectedValue = Request["situacao"];
                drpSituacao.Enabled = false;
            }
    
            if (!String.IsNullOrEmpty(Request["tipo"]))
            {
                drpTipoObra.SelectedIndex = Glass.Conversoes.StrParaInt(Request["tipo"]);
                drpTipoObra.Enabled = false;
            }
    
            if (!String.IsNullOrEmpty(Request["idCliente"]))
            {
                txtNumCli.Text = Request["idCliente"];
                txtNumCli.Enabled = false;
                txtNomeCliente.Text = ClienteDAO.Instance.GetNome(Glass.Conversoes.StrParaUint(Request["idCliente"]));
                txtNomeCliente.Enabled = false;
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdObra.PageIndex = 0;
        }
    }
}
