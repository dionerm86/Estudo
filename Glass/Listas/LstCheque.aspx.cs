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
    
        protected void grdCheque_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "CancelarDevolucao")
            {
                try
                {
                    uint idCheque = Glass.Conversoes.StrParaUint(e.CommandArgument.ToString());
                    DepositoChequeDAO.Instance.CancelarDevolucao(idCheque);
    
                    grdCheque.DataBind();
                    Glass.MensagemAlerta.ShowMsg("Devolução do cheque cancelada.", Page);
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao cancelar devolução do cheque.", ex, Page);
                }
            }
            if(e.CommandName == "DesmarcarProtestado")
            {
                uint idCheque = Glass.Conversoes.StrParaUint(e.CommandArgument.ToString());
                DepositoChequeDAO.Instance.CancelarProtesto(idCheque);

                grdCheque.DataBind();
                Glass.MensagemAlerta.ShowMsg("Protesto do cheque cancelado.", Page);
            }

            if (e.CommandName == "CancelarReapresentado")
            {
                try
                {
                    uint idCheque = Glass.Conversoes.StrParaUint(e.CommandArgument.ToString());
                    Glass.UI.Web.Controls.ctrlData data = ((LinkButton)e.CommandSource).Parent.FindControl("ctrlDataReapresentado") as Glass.UI.Web.Controls.ctrlData;

                    ChequesDAO.Instance.CancelarReapresentacaoDeCheque(idCheque);

                    grdCheque.DataBind();

                    Glass.MensagemAlerta.ShowMsg("Cancelamento da reapresentação efetuado com sucesso.", Page);
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao cancelar reapresentação.", ex, Page);
                }
            }
        }

        /// <summary>
        /// Evento pós atualização do cheque
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void odsCheques_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            ///Verifica o retorno e caso tenha exibe o mesmo na tela.
            if (e.ReturnValue != null)
            {
                Glass.MensagemAlerta.ShowMsg(e.ReturnValue.ToString(), Page);
            }
        }
    }
}
