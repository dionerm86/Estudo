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
            hdfCustom.Value = Request["custom"]; // Identifica que ao selecionar cliente, ser� usado uma fun��o customizada na tela
            hdfControleFormaPagto.Value = Request["controleFormaPagto"];
    
            if (Request["rota"] == "true")
                lblAviso.Text = "N�o ser�o buscados clientes que j� possuam rota associada.";
    
            if (!IsPostBack)
                txtNome.Focus();

            if(hdfNfe.Value == "1")
            {
                hdfSituacaoBusca.Value = ((int)Data.Model.SituacaoCliente.Ativo).ToString();
            }

        }
        
        #region M�todos Ajax
    
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
