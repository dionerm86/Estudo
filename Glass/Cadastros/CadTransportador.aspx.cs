using System;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Data.DAL;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadTransportador : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            dtvTransportador.Register("~/Listas/LstTransportador.aspx");
            odsTransportador.Register();

            odsTransportador.Inserted += odsTransp_Inserted;

            if (Request["idTransp"] != null)
                dtvTransportador.ChangeMode(DetailsViewMode.Edit);

            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadTransportador));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarTransportadora))
            {
                Response.Redirect("~/WebGlass/Main.aspx");
                return;
            }
        }
    
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Listas/LstTransportador.aspx");
        }
    
        protected void odsTransp_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (Request["popup"] != null)
            {
                uint idTransp = Glass.Conversoes.StrParaUint(e.ReturnValue.ToString());
                ClientScript.RegisterClientScriptBlock(this.GetType(), "busca", "window.opener.setFornec(" + idTransp + ",'" + 
                    TransportadorDAO.Instance.GetNome(idTransp).Replace("'", "") + "'); closeWindow();", true);
            }
            else
                Response.Redirect("../Listas/LstTransportador.aspx");
        }
    
        [Ajax.AjaxMethod()]
        public string CheckIfExists(string cnpj)
        {
            var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<Glass.Global.Negocios.ITransportadorFluxo>();

            return fluxo.VerificarCpfCnpj(cnpj).ToString().ToLower();
        }
    }
}
