using Glass.Configuracoes;
using System;
using System.Web.UI;

namespace Glass.UI.Web.Relatorios
{
    public partial class ListaPendenciaCarregamento : System.Web.UI.Page
    {
        #region Variaveis locais
    
        private bool corAlternada = true;
    
        #endregion
    
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            if (!IsPostBack)
            {
                if (!Glass.Configuracoes.PedidoConfig.ExibirOpcaoDeveTransferir)
                {
                    chkIgnorarPedidoVendaTransferencia.Checked = false;
                    chkIgnorarPedidoVendaTransferencia.Style.Add("display", "none");
                }

                if (!OrdemCargaConfig.ControlarPedidosImportados)
                    tbClienteExterno.Style.Add("Display", "none");
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            gvrPendencia.DataBind();
        }
    
        protected string GetAlternateClass()
        {
            corAlternada = !corAlternada;
            return corAlternada ? "alt" : "";
        }
    }
}
