using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadChequeReapresentado : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsFinanceiroPagto())
                Page.Title = "Controle de Cheques Próprios Reapresentados";
        }
    
        protected void grdCheque_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Reapresentado")
            {
                try
                {
                    uint idCheque = Glass.Conversoes.StrParaUint(e.CommandArgument.ToString());
                    Glass.UI.Web.Controls.ctrlData data = ((LinkButton)e.CommandSource).Parent.FindControl("ctrlDataReapresentado") as Glass.UI.Web.Controls.ctrlData;
    
                    ChequesDAO.Instance.ReapresentarCheque(idCheque, data != null ? data.Data : DateTime.Now);
    
                    grdCheque.DataBind();
    
                    Glass.MensagemAlerta.ShowMsg("Cheque marcado como Reapresentado com sucesso.", Page);
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao marcar Cheque como Reapresentado.", ex, Page);
                }
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdCheque.PageIndex = 0;
        }
    
        protected bool IsFinanceiroPagto()
        {
            return Request["pagto"] == "1";
        }
    }
}
