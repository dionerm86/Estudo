using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Listas
{
    public partial class LstCheque : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Listas.LstCheque));
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            if (!PedidoConfig.LiberarPedido)
            {
                lblLiberacao.Visible = false;
                imgPesqLib.Visible = false;
                txtNumLiberarPedido.Visible = false;
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdCheque.PageIndex = 0;
        }
    
        #region M�todos AJAX
    
        /// <summary>
        /// Busca o cliente em tempo real
        /// </summary>
        /// <param name="idCli"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string GetCli(string idCli)
        {
            if (!ClienteDAO.Instance.Exists(Glass.Conversoes.StrParaUint(idCli)))
                return "Erro;Cliente n�o encontrado.";
            else
                return "Ok;" + ClienteDAO.Instance.GetNome(Glass.Conversoes.StrParaUint(idCli));
        }
    
        #endregion
    
        protected void grdCheque_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "CancelarDevolucao")
            {
                try
                {
                    uint idCheque = Glass.Conversoes.StrParaUint(e.CommandArgument.ToString());
                    DepositoChequeDAO.Instance.CancelarDevolucao(idCheque);
    
                    grdCheque.DataBind();
                    Glass.MensagemAlerta.ShowMsg("Devolu��o do cheque cancelada.", Page);
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao cancelar devolu��o do cheque.", ex, Page);
                }
            }
            if(e.CommandName == "DesmarcarProtestado")
            {
                uint idCheque = Glass.Conversoes.StrParaUint(e.CommandArgument.ToString());
                DepositoChequeDAO.Instance.CancelarProtesto(idCheque);

                grdCheque.DataBind();
                Glass.MensagemAlerta.ShowMsg("Protesto do cheque cancelado.", Page);
            }
        }
    }
}
