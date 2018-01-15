using System;
using System.Web.UI;

namespace Glass.UI.Web.Utils
{
    public partial class DisponibilizarRetalhos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterOnSubmitStatement(GetType(), "permitePostBack", "window.onunload = null;");
        }
    
        protected void odsRetalhosDisponiveis_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                e.ExceptionHandled = true;
                Glass.MensagemAlerta.ErrorMsg("Falha ao disponibilizar retalho.", e.Exception, Page);
            }
            else
            {
                grdRetalhosDisponiveis.DataBind();
                grdRetalhosEstoque.DataBind();
            }
        }
    
        protected void odsRetalhosEstoque_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                e.ExceptionHandled = true;
                Glass.MensagemAlerta.ErrorMsg("Falha ao colocar retalho em estoque.", e.Exception, Page);
            }
            else
            {
                grdRetalhosDisponiveis.DataBind();
                grdRetalhosEstoque.DataBind();
            }
        }

        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {

        }
    }
}
