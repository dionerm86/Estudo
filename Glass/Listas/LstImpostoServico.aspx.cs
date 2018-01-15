using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Model;

namespace Glass.UI.Web.Listas
{
    public partial class LstImpostoServico : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            if (!IsPostBack && Request["idImpostoServ"] != null)
                txtNumImpostoServ.Text = Request["idImpostoServ"];
        }
    
        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Cadastros/CadImpostoServico.aspx");
        }
    
        protected void grdImpostoServ_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Cancelar")
            {
                try
                {
                    ImpostoServDAO.Instance.Cancelar(Glass.Conversoes.StrParaUint(e.CommandArgument.ToString()));
                    Glass.MensagemAlerta.ShowMsg("Imposto/serviço cancelado.", Page);
                    grdImpostoServ.DataBind();
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao cancelar imposto/serviço.", ex, Page);
                }
            }
            else if (e.CommandName == "Finalizar")
            {
                uint idImpostoServ = 0;

                try
                {
                    idImpostoServ = Glass.Conversoes.StrParaUint(e.CommandArgument.ToString());
                    ImpostoServDAO.Instance.Finalizar(idImpostoServ);
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao finalizar imposto/serviço.", ex, Page);
                    return;
                }

                if (ImpostoServDAO.Instance.ObtemTipoPagto(idImpostoServ) == (int)ImpostoServ.TipoPagtoEnum.AVista)
                    Response.Redirect("../Cadastros/CadContaPagar.aspx?idImpostoServ=" + idImpostoServ, false);
                else
                {
                    Glass.MensagemAlerta.ShowMsg("Imposto/serviço finalizado.", Page);
                    grdImpostoServ.DataBind();
                }
            }
            else if (e.CommandName == "Reabrir")
            {
                try
                {
                    ImpostoServDAO.Instance.Reabrir(Glass.Conversoes.StrParaUint(e.CommandArgument.ToString()));
                    grdImpostoServ.DataBind();
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao reabrir imposto/serviço.", ex, Page);
                }
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdImpostoServ.PageIndex = 0;
        }
    }
}
