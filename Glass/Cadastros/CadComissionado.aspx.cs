using System;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Microsoft.Practices.ServiceLocation;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadComissionado : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            odsComissionado.Register();
            dtvComissionado.Register("~/Listas/LstComissionado.aspx");

            if (Request["idComissionado"] != null)
                dtvComissionado.ChangeMode(DetailsViewMode.Edit);

            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadComissionado));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarComissionado))
            {
                Response.Redirect("~/WebGlass/Main.aspx");
                return;
            }
        }
    
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Listas/LstComissionado.aspx");
        }
    
        [Ajax.AjaxMethod()]
        public string CheckIfExists(string cpfCnpj)
        {
            var fluxo = ServiceLocator.Current.GetInstance<Glass.Global.Negocios.IComissionadoFluxo>();
            return fluxo.VerificarCpfCnpj(cpfCnpj).ToString().ToLower();
        }
    }
}
