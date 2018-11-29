using System;
using System.Web.UI;

namespace Glass.UI.Web.Utils
{
    public partial class SelCliente : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(Utils.SelCliente));
    
            hdfNfe.Value = Request["Nfe"];
            hdfDadosCliente.Value = Request["dadosCliente"];
            hdfMedicao.Value = Request["medicao"];
            hdfChequeDev.Value = Request["chequeDev"];
            hdfCustom.Value = Request["custom"]; // Identifica que ao selecionar cliente, será usado uma função customizada na tela
            hdfControleFormaPagto.Value = Request["controleFormaPagto"];
    
            if (Request["rota"] == "true")
                lblAviso.Text = "Não serão buscados clientes que já possuam rota associada.";
    
            if (!IsPostBack)
                txtNome.Focus();

            if(hdfNfe.Value == "1")
            {
                hdfSituacaoBusca.Value = ((int)Data.Model.SituacaoCliente.Ativo).ToString();
            }

        }
        
        #region Métodos Ajax
    
        [Ajax.AjaxMethod]
        public string GetCliMedicao(string idClienteStr)
        {
            return WebGlass.Business.Medicao.Fluxo.BuscarEValidar.Ajax.GetCli(idClienteStr);
        }
    
        #endregion
        
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdCliente.PageIndex = 0;
        }
    
        protected void imgExcluirFiltro_Click(object sender, ImageClickEventArgs e)
        {
            txtNome.Text = "";
            txtCodigo.Text = "";
            txtBairro.Text = "";
    
            grdCliente.PageIndex = 0;
        }
    
        protected void LnkCliente_Click(object sender, EventArgs e)
        {
            Response.Redirect("../cadastros/cadcliente.aspx?popup=1");
        }
    }
}
