using System;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadDeducaoDiversa : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["idDeducao"] != null)
            {
                dtvDeducaoDiversa.ChangeMode(DetailsViewMode.Edit);
            }
        }
        protected void dtvDeducaoDiversa_ItemUpdating(object sender, DetailsViewUpdateEventArgs e)
        {
            e.NewValues["ValorPisDeduzir"] = Glass.Conversoes.StrParaDecimal(e.NewValues["ValorPisDeduzir"].ToString());
            e.NewValues["ValorCofinsDeduzir"] = Glass.Conversoes.StrParaDecimal(e.NewValues["ValorCofinsDeduzir"].ToString());
            e.NewValues["BcDeducao"] = Glass.Conversoes.StrParaDecimal(e.NewValues["BcDeducao"].ToString());
    
        }
        protected void dtvDeducaoDiversa_ItemInserting(object sender, DetailsViewInsertEventArgs e)
        {
            e.Values["ValorPisDeduzir"] = Glass.Conversoes.StrParaDecimal(e.Values["ValorPisDeduzir"].ToString());
            e.Values["ValorCofinsDeduzir"] = Glass.Conversoes.StrParaDecimal(e.Values["ValorCofinsDeduzir"].ToString());
            e.Values["BcDeducao"] = Glass.Conversoes.StrParaDecimal(e.Values["BcDeducao"].ToString());
        }
        protected void dtvDeducaoDiversa_ItemCommand(object sender, DetailsViewCommandEventArgs e)
        {
            if (e.CommandName == "Cancel")
            {
                Response.Redirect("../Listas/LstDeducaoDiversa.aspx");
            }
    
        }
        protected void odsDeducaoDiversa_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar dados da dedução diversa.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
            {
                Response.Redirect("../Listas/LstDeducaoDiversa.aspx");
            }
        }
        protected void odsDeducaoDiversa_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar dados da dedução diversa.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
            {
                Response.Redirect("../Listas/LstDeducaoDiversa.aspx");
            }
        }
    }
}
