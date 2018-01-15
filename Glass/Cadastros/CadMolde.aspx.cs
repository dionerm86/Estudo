using System;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadMolde : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadMolde));
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            if (!IsPostBack)
            {
                if (Request["idMolde"] != null)
                    dtvMolde.ChangeMode(DetailsViewMode.Edit);
            }
        }
    
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Listas/LstMolde.aspx");
        }
    
        [Ajax.AjaxMethod]
        public string GetDadosPedido(string idPedidoStr)
        {
            return WebGlass.Business.Pedido.Fluxo.BuscarEValidar.Ajax.GetDadosPedido(idPedidoStr);
        }
    
        protected void odsMolde_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception == null)
                Response.Redirect("~/Cadastros/CadMolde.aspx?idMolde=" + e.ReturnValue.ToString() + "&relatorio=1");
            else
            {
                e.ExceptionHandled = true;
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir molde.", e.Exception, Page);
            }
        }
    
        protected void odsMolde_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception == null)
                Response.Redirect("~/Cadastros/CadMolde.aspx?idMolde=" + Request["idMolde"] + "&relatorio=1");
            else
            {
                e.ExceptionHandled = true;
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar molde.", e.Exception, Page);
            }
        }
    }
}
