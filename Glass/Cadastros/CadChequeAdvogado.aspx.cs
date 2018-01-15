using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadChequeAdvogado : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsFinanceiroPagto())
            {
                Page.Title = "Controle de Cheques Próprios em Advogado";
                
                if (!IsPostBack)
                    drpTipo.SelectedValue = "1";
            }
        }
    
        protected void grdCheque_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "MarcarAdvogado")
            {
                try
                {
                    uint idCheque = Glass.Conversoes.StrParaUint(e.CommandArgument.ToString());
    
                    ChequesDAO.Instance.MarcarAdvogado(idCheque);
    
                    grdCheque.DataBind();
    
                    Glass.MensagemAlerta.ShowMsg("Cheque marcado como Advogado com sucesso.", Page);
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao marcar Cheque como Advogado.", ex, Page);
                }
            }
            else if (e.CommandName == "DesmarcarAdvogado")
            {
                try
                {
                    uint idCheque = Glass.Conversoes.StrParaUint(e.CommandArgument.ToString());
    
                    ChequesDAO.Instance.DesmarcarAdvogado(idCheque);
    
                    grdCheque.DataBind();
    
                    Glass.MensagemAlerta.ShowMsg("Cheque desmarcado como Advogado com sucesso.", Page);
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao desmarcar cheque como Advogado.", ex, Page);
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
