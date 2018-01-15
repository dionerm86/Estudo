using System;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadTipoCliente : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            odsTipoCliente.Register();
            grdTipoCliente.Register(true, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }  
    
        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            var tipoCliente = new Glass.Global.Negocios.Entidades.TipoCliente();
            tipoCliente.Descricao = ((TextBox)grdTipoCliente.FooterRow.FindControl("txtDescricao")).Text;
            tipoCliente.CobrarAreaMinima = ((CheckBox)grdTipoCliente.FooterRow.FindControl("chkCobrarAreaMinima")).Checked;

            var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator.Current
                .GetInstance<Glass.Global.Negocios.IClienteFluxo>();

            // Salva os dados do tipo de funcionario
            var resultado = fluxo.SalvarTipoCliente(tipoCliente);

            if (!resultado)
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir tipo de cliente.", resultado);
            else
                grdTipoCliente.DataBind();
        }

        protected void grdTipoCliente_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdTipoCliente.ShowFooter = e.CommandName != "Edit";
        }  
    }
}
