using System;
using System.Web.UI;
using Glass.Data.DAL;

namespace Glass.UI.Web.Utils
{
    public partial class SelAntecipFornec : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(Request["situacao"]))
            {
                drpSituacao.SelectedValue = Request["situacao"];
                drpSituacao.Enabled = false;
            }
    
            if (!String.IsNullOrEmpty(Request["idFornec"]))
            {
                txtNumFornec.Text = Request["idFornec"];
                txtNumFornec.Enabled = false;
                txtNomeFornec.Text = FornecedorDAO.Instance.GetNome(Glass.Conversoes.StrParaUint(Request["idFornec"]));
                txtNomeFornec.Enabled = false;
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdAntecip.PageIndex = 0;
        }
    }
}
