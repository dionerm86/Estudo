using System;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadChapaVidro : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadChapaVidro));
        }
    
        #region Métodos Ajax
    
        [Ajax.AjaxMethod]
        public string GetProduto(string codInterno)
        {
            return WebGlass.Business.Produto.Fluxo.BuscarEValidar.Ajax.GetProdutoChapaVidro(codInterno);
        }
    
        [Ajax.AjaxMethod]
        public string CalcM2(string idProd, string altura, string largura)
        {
            return MetodosAjax.CalcM2(((int)Glass.Data.Model.TipoCalculoGrupoProd.M2).ToString(), altura, largura, "1", idProd, "false", "0", "false");
        }
    
        #endregion
    
        protected void dtvChapaVidro_ItemInserted(object sender, DetailsViewInsertedEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir chapa de vidro.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
                Response.Redirect("~/Listas/LstChapaVidro.aspx");
        }
    
        protected void dtvChapaVidro_ItemCommand(object sender, DetailsViewCommandEventArgs e)
        {
            if (e.CommandName == "Cancel")
                Response.Redirect("~/Listas/LstChapaVidro.aspx");
        }
    }
}
