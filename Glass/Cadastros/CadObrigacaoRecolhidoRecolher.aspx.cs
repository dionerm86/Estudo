using System;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadObrigacaoRecolhidoRecolher : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["id"] != null)
                dtvObrigacao.ChangeMode(DetailsViewMode.Edit);
            else
                ((DropDownList)dtvObrigacao.FindControl("drpIndicadorOrigem")).SelectedValue = ((int)Data.Model.AjusteApuracaoInfoAdicional.IndProcEnum.Outros).ToString();
        }
    
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Listas/LstObrigacaoRecolhidoRecolher.aspx");
        }
    
        protected void odsObrigacao_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao tentar cadastrar.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
            {
                Response.Redirect("../Listas/LstObrigacaoRecolhidoRecolher.aspx");
    
            }
        }
        protected void odsObrigacao_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao tentar atualizar.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
            {
                Response.Redirect("../Listas/LstObrigacaoRecolhidoRecolher.aspx");
    
            }
        }
        protected void dtvObrigacao_ItemUpdating(object sender, DetailsViewUpdateEventArgs e)
        {
            e.NewValues["Valor"] = Convert.ToDecimal(e.NewValues["Valor"]);
        }
    
        protected void dtvObrigacao_ItemInserting(object sender, DetailsViewInsertEventArgs e)
        {
            e.Values["Valor"] = Convert.ToDecimal(e.Values["Valor"]);
        }
    }
}
