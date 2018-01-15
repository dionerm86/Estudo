using System;
using System.Web.UI.WebControls;
using Glass.Data.DAL;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadCartaCorrecao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Listas/LstNotaFiscal.aspx");
        }
    
        protected void dtvNf_ItemInserting(object sender, DetailsViewInsertEventArgs e)
        {
            e.Values["idNf"] = Request["idNf"];
        }
    
        protected void dtvNf_ItemInserted(object sender, DetailsViewInsertedEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir carta de correção.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
            {
                grdCarta.DataBind();
            }
        }
    
        protected void grdCarta_SelectedIndexChanged(object sender, EventArgs e)
        {
            dtvNf.ChangeMode(DetailsViewMode.Edit);
        }
    
        protected void dtvNf_ItemUpdated(object sender, DetailsViewUpdatedEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar carta de correção.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
            {
                grdCarta.DataBind();
            }
        }
    
        protected void grdCarta_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EnviarCarta")
            {
                try
                {
                    Glass.MensagemAlerta.ShowMsg(CartaCorrecaoDAO.Instance.EmitirCce(Convert.ToUInt32(e.CommandArgument), false), this);
                    grdCarta.DataBind();
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg(null, ex, this);
                }
            }
        }
        protected void textoValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = args.Value.Length >= 15 && args.Value.Length <= 1000;
        }
    }
}
