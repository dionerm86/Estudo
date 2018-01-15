using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Relatorios.Administrativos
{
    public partial class DescontoContasRec : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Relatorios.Administrativos.DescontoContasRec));
    
            if (!PedidoConfig.LiberarPedido)
            {
                grdDescParc.Columns[1].Visible = false;
                lblLiberarPedido.Visible = txtIdLiberarPedido.Visible = imgPesqLib.Visible = false;
            }
            else
            {
                grdDescParc.Columns[0].Visible = false;
                grdDescParc.Columns[3].Visible = false;
                grdDescParc.Columns[5].Visible = false;
                lblPedido.Visible = txtIdPedido.Visible = imgPesqPed.Visible = false;
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdDescParc.PageIndex = 0;
        }
    
        #region Métodos AJAX
    
        /// <summary>
        /// Busca o cliente em tempo real
        /// </summary>
        /// <param name="idCli"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string GetCli(string idCli)
        {
            if (!ClienteDAO.Instance.Exists(Glass.Conversoes.StrParaUint(idCli)))
                return "Erro;Cliente não encontrado.";
            else
                return "Ok;" + ClienteDAO.Instance.GetNome(Glass.Conversoes.StrParaUint(idCli));
        }
    
        #endregion
    
        protected void ddlOrigem_Load(object sender, EventArgs e)
        {
            ((WebControl)sender).Visible = OrigemTrocaDescontoDAO.Instance.GetList().Length > 0;
        }
    }
}
