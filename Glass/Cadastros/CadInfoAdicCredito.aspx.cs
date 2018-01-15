using System;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadInfoAdicCredito : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["id"] != null)
            {
                dtvInfoAdicCredito.ChangeMode(DetailsViewMode.Edit);
            }
        }
        protected void dtvInfoAdicCredito_ItemUpdating(object sender, DetailsViewUpdateEventArgs e)
        {
            e.NewValues["ValorCredPerResAnt"] = Glass.Conversoes.StrParaDecimal(e.NewValues["ValorCredPerResAnt"].ToString());
            e.NewValues["ValorCredDeclCompAnt"] = Glass.Conversoes.StrParaDecimal(e.NewValues["ValorCredDeclCompAnt"].ToString());
            e.NewValues["ValorCredDescAnt"] = Glass.Conversoes.StrParaDecimal(e.NewValues["ValorCredDescAnt"].ToString());
            e.NewValues["ValorCredPerRes"] = Glass.Conversoes.StrParaDecimal(e.NewValues["ValorCredPerRes"].ToString());
            e.NewValues["ValorCredDeclComp"] = Glass.Conversoes.StrParaDecimal(e.NewValues["ValorCredDeclComp"].ToString());
            e.NewValues["ValorCredTransf"] = Glass.Conversoes.StrParaDecimal(e.NewValues["ValorCredTransf"].ToString());
            e.NewValues["ValorCredOutro"] = Glass.Conversoes.StrParaDecimal(e.NewValues["ValorCredOutro"].ToString());
        }
        protected void dtvInfoAdicCredito_ItemInserting(object sender, DetailsViewInsertEventArgs e)
        {
            e.Values["ValorCredPerResAnt"] = Glass.Conversoes.StrParaDecimal(e.Values["ValorCredPerResAnt"].ToString());
            e.Values["ValorCredDeclCompAnt"] = Glass.Conversoes.StrParaDecimal(e.Values["ValorCredDeclCompAnt"].ToString());
            e.Values["ValorCredDescAnt"] = Glass.Conversoes.StrParaDecimal(e.Values["ValorCredDescAnt"].ToString());
            e.Values["ValorCredPerRes"] = Glass.Conversoes.StrParaDecimal(e.Values["ValorCredPerRes"].ToString());
            e.Values["ValorCredDeclComp"] = Glass.Conversoes.StrParaDecimal(e.Values["ValorCredDeclComp"].ToString());
            e.Values["ValorCredTransf"] = Glass.Conversoes.StrParaDecimal(e.Values["ValorCredTransf"].ToString());
            e.Values["ValorCredOutro"] = Glass.Conversoes.StrParaDecimal(e.Values["ValorCredOutro"].ToString());
    
        }
        protected void dtvInfoAdicCredito_ItemCommand(object sender, DetailsViewCommandEventArgs e)
        {
            if (e.CommandName == "Cancel")
            {
                Response.Redirect("../Listas/LstInfoAdicCredito.aspx");
            }
        }
        protected void odsInfoAdicCredito_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar dados.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
            {
                Response.Redirect("../Listas/LstInfoAdicCredito.aspx");
            }
        }
        protected void odsInfoAdicCredito_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar dados.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
            {
                Response.Redirect("../Listas/LstInfoAdicCredito.aspx");
            }
        }
    }
}
