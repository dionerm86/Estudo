using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;

namespace Glass.UI.Web.Listas
{
    public partial class LstObrigacaoRecolhidoRecolher : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdObrigacao.PageIndex = 0;
        }
        protected void grdObrigacao_DataBound(object sender, EventArgs e)
        {
            if (grdObrigacao.Rows.Count == 1)
            {
                int tipo = !string.IsNullOrEmpty(drpTipoImposto0.SelectedValue) ? Glass.Conversoes.StrParaInt(drpTipoImposto0.SelectedValue) : 0;
    
                grdObrigacao.Rows[0].Visible = ObrigacaoRecolhidoRecolherDAO.Instance.GetCount((Glass.Data.EFD.ConfigEFD.TipoImpostoEnum)tipo, txtDataInicio.DataString, txtDataFim.DataString) > 0;
            }
    
        }
        protected void grdObrigacao_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdObrigacao.ShowFooter = e.CommandName != "Edit";
        }
        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Cadastros/CadObrigacaoRecolhidoRecolher.aspx");
        }
    }
}
