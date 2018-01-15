using System;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadValorRetidoFonte : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["idValorRetidoFonte"] != null)
            {
                dtvValorRetidoFonte.ChangeMode(DetailsViewMode.Edit);
            }
        }
        protected void dtvValorRetidoFonte_ItemUpdating(object sender, DetailsViewUpdateEventArgs e)
        {
            e.NewValues["ValorPisRetido"] = Glass.Conversoes.StrParaDecimal(e.NewValues["ValorPisRetido"].ToString());
            e.NewValues["ValorCofinsRetido"] = Glass.Conversoes.StrParaDecimal(e.NewValues["ValorCofinsRetido"].ToString());
            e.NewValues["ValorRetido"] = Glass.Conversoes.StrParaDecimal(e.NewValues["ValorRetido"].ToString());
            e.NewValues["BcRetencao"] = Glass.Conversoes.StrParaDecimal(e.NewValues["BcRetencao"].ToString());
    
        }
        protected void dtvValorRetidoFonte_ItemInserting(object sender, DetailsViewInsertEventArgs e)
        {
            e.Values["ValorPisRetido"] = Glass.Conversoes.StrParaDecimal(e.Values["ValorPisRetido"].ToString());
            e.Values["ValorCofinsRetido"] = Glass.Conversoes.StrParaDecimal(e.Values["ValorCofinsRetido"].ToString());
            e.Values["ValorRetido"] = Glass.Conversoes.StrParaDecimal(e.Values["ValorRetido"].ToString());
            e.Values["BcRetencao"] = Glass.Conversoes.StrParaDecimal(e.Values["BcRetencao"].ToString());
        }
        protected void dtvValorRetidoFonte_ItemCommand(object sender, DetailsViewCommandEventArgs e)
        {
            if (e.CommandName == "Cancel")
            {
                Response.Redirect("../Listas/LstValorRetidoFonte.aspx");
            }
    
        }
        protected void odsValorRetidoFonte_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar dados da receita retida na fonte.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
            {
                Response.Redirect("../Listas/LstValorRetidoFonte.aspx");
            }
        }
        protected void odsValorRetidoFonte_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar dados da receita retida na fonte.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
            {
                Response.Redirect("../Listas/LstValorRetidoFonte.aspx");
            }
        }
    }
}
