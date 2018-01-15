using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Configuracoes;

namespace Glass.UI.Web.Listas
{
    public partial class LstInstalacao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Listas.LstInstalacao));
    
            if (Request["idPedido"] != null)
                txtNumPedido.Text = Request["idPedido"];
    
            if (Request["situacao"] != null)
                cbdSituacao.SelectedValue = Request["situacao"];
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdColocacoes.PageIndex = 0;
        }
    
        protected void drpLoja_DataBound(object sender, EventArgs e)
        {
            if (!IsPostBack && InstalacaoConfig.TelaListagem.FiltrarPorLojaComoPadrao)
            {
                drpLoja.SelectedValue = UserInfo.GetUserInfo.IdLoja.ToString();
                grdColocacoes.DataBind();
            }
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
    
        /// <summary>
        /// Função para cancelar a instalação
        /// </summary>
        /// <param name="idInstalacao"></param>
        /// <param name="obs"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string Cancelar(string idInstalacao, string obs)
        {
            try
            {
                InstalacaoDAO.Instance.CancelarFinalizada(Glass.Conversoes.StrParaUint(idInstalacao), obs);
                
                return "Instalação Cancelada";
            }
            catch (Exception ex)
            {
                return "Erro|Falha ao cancelar Instalação. " + ex.Message;
            }
        }
    
        #endregion
    
        protected void grdColocacoes_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }
    }
}
