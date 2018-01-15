using System;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadReceitaDiversa : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["idReceita"] != null)
            {
                dtvReceitaDiversa.ChangeMode(DetailsViewMode.Edit);
            }
        }
        protected void dtvReceitaDiversa_ItemUpdating(object sender, DetailsViewUpdateEventArgs e)
        {
            e.NewValues["ValorReceita"] = Glass.Conversoes.StrParaDecimal(e.NewValues["ValorReceita"].ToString());
            e.NewValues["BcPis"] = Glass.Conversoes.StrParaDecimal(e.NewValues["BcPis"].ToString());
            e.NewValues["BcCofins"] = Glass.Conversoes.StrParaDecimal(e.NewValues["BcCofins"].ToString());
            e.NewValues["AliquotaPis"] = Glass.Conversoes.StrParaFloat(e.NewValues["AliquotaPis"].ToString());
            e.NewValues["AliquotaCofins"] = Glass.Conversoes.StrParaFloat(e.NewValues["AliquotaCofins"].ToString());
    
        }
        protected void dtvReceitaDiversa_ItemInserting(object sender, DetailsViewInsertEventArgs e)
        {
            e.Values["ValorReceita"] = Glass.Conversoes.StrParaDecimal(e.Values["ValorReceita"].ToString());
            e.Values["BcPis"] = Glass.Conversoes.StrParaDecimal(e.Values["BcPis"].ToString());
            e.Values["BcCofins"] = Glass.Conversoes.StrParaDecimal(e.Values["BcCofins"].ToString());
            e.Values["AliquotaPis"] = Glass.Conversoes.StrParaFloat(e.Values["AliquotaPis"].ToString());
            e.Values["AliquotaCofins"] = Glass.Conversoes.StrParaFloat(e.Values["AliquotaCofins"].ToString());
        }
        protected void dtvReceitaDiversa_ItemCommand(object sender, DetailsViewCommandEventArgs e)
        {
            if (e.CommandName == "Cancel")
            {
                Response.Redirect("../Listas/LstReceitaDiversa.aspx");
            }
    
        }
        protected void odsReceitaDiversa_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar dados da receita diversa.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
            {
                Response.Redirect("../Listas/LstReceitaDiversa.aspx");
            }
        }
        protected void odsReceitaDiversa_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar dados da receita diversa.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
            {
                Response.Redirect("../Listas/LstReceitaDiversa.aspx");
            }
        }
    }
}
