using System;
using System.Web.UI.WebControls;
using Glass.Data.Model.Cte;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadProprietarioVeiculo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["idPropVeiculo"] != null)
                dtvCadProprietario.ChangeMode(DetailsViewMode.Edit);
    
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
        }
    
        protected void odsProprietarioVeiculo_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao cadastrar Proprietário.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
                Response.Redirect("../Listas/LstProprietarioVeiculo.aspx");
        }
    
        protected void odsProprietarioVeiculo_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar dados do proprietário.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
                Response.Redirect("../Listas/LstProprietarioVeiculo.aspx");
        }
    
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Listas/LstProprietarioVeiculo.aspx");
        }
        protected void odsPropVeiculo_Inserting(object sender, Colosoft.WebControls.VirtualObjectDataSourceMethodEventArgs e)
        {
            var objInsert = (ProprietarioVeiculo)e.InputParameters[0];
    
            objInsert.Cnpj = !String.IsNullOrEmpty(objInsert.Cnpj) ? objInsert.Cnpj.Replace(".", "").Replace("/", "") : objInsert.Cnpj;
            objInsert.Cpf = !String.IsNullOrEmpty(objInsert.Cpf) ? objInsert.Cpf.Replace(".", "").Replace("-", "") : objInsert.Cpf;
        }
    }
}
