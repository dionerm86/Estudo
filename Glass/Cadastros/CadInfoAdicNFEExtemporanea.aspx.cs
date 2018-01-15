using System;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadInfoAdicNFEExtemporanea : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["id"] != null)
            {
                dtvInfoAdicNFEExtemp.ChangeMode(DetailsViewMode.Edit);
            }
        }
    
        protected void dtvInfoAdicNFEExtemp_ItemUpdating(object sender, DetailsViewUpdateEventArgs e)
        {
            e.NewValues["ValorOutDeducao"] = Glass.Conversoes.StrParaDecimal(e.NewValues["ValorOutDeducao"].ToString());
            e.NewValues["ValorMulta"] = Glass.Conversoes.StrParaDecimal(e.NewValues["ValorMulta"].ToString());
            e.NewValues["ValorJuro"] = Glass.Conversoes.StrParaDecimal(e.NewValues["ValorJuro"].ToString());
        }
    
        protected void dtvInfoAdicNFEExtemp_ItemInserting(object sender, DetailsViewInsertEventArgs e)
        {
            e.Values["ValorOutDeducao"] = Glass.Conversoes.StrParaDecimal(e.Values["ValorOutDeducao"].ToString());
            e.Values["ValorMulta"] = Glass.Conversoes.StrParaDecimal(e.Values["ValorMulta"].ToString());
            e.Values["ValorJuro"] = Glass.Conversoes.StrParaDecimal(e.Values["ValorJuro"].ToString());
        }
    
        protected void dtvInfoAdicNFEExtemp_ItemCommand(object sender, DetailsViewCommandEventArgs e)
        {
            if (e.CommandName == "Cancel")
            {
                Response.Redirect("../Listas/LstInfoAdicNFEExtemporanea.aspx");
            }
        }
    
        protected void odsInfoAdicNFEExtemp_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar dados.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
            {
                Response.Redirect("../Listas/LstInfoAdicNFEExtemporanea.aspx");
            }
        }
        protected void odsInfoAdicNFEExtemp_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar dados.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
            {
                Response.Redirect("../Listas/LstInfoAdicNFEExtemporanea.aspx");
            }
        }
    
    }
}
