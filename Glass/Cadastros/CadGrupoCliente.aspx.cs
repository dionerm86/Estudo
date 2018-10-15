using System;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadGrupoCliente : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            odsGrupoCliente.Register();
            grdGrupoCliente.Register(true, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            var grupoCliente = new Glass.Global.Negocios.Entidades.GrupoCliente();
            grupoCliente.Descricao = ((TextBox)grdGrupoCliente.FooterRow.FindControl("txtDescricao")).Text;

            if (string.IsNullOrEmpty(grupoCliente.Descricao))
            {
                Glass.MensagemAlerta.ShowMsg("A descrição não pode ser vazia.", Page);
                return;
            }

            var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator.Current
                .GetInstance<Glass.Global.Negocios.IClienteFluxo>();

            // Salva os dados do tipo de funcionario
            var resultado = fluxo.SalvarGrupoCliente(grupoCliente);

            if (!resultado)
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir grupo de cliente.", resultado);
            else
                grdGrupoCliente.DataBind();
        }

        protected void grdGrupoCliente_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdGrupoCliente.ShowFooter = e.CommandName != "Edit";
        }
    }
}
