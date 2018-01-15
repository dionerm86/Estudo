using System;
using System.Web.UI;
using Glass.Data.Model;
using System.Web.Security;
using Glass.Data.DAL;
using Glass.Data.Helper;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadPedidoCorte : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (Request["sit"])
            {
                case "1":
                    lblPedidos.Text = "Confirmados"; break;
                case "2":
                    lblPedidos.Text = "Em Produção"; break;
                case "3":
                    lblPedidos.Text = "Prontos"; break;
                case "4":
                    lblPedidos.Text = "Entregues"; break;
            }
    
            UserInfo.SetActivity();
    
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdCorte.PageIndex = 0;
        }
    
        protected void lnkConsProducao_Click(object sender, EventArgs e)
        {
            Response.Redirect("CadPedidoCorte.aspx?sit=" + (int)PedidoCorte.SituacaoEnum.Producao);
        }
    
        protected void lnkConsProntos_Click(object sender, EventArgs e)
        {
            Response.Redirect("CadPedidoCorte.aspx?sit=" + (int)PedidoCorte.SituacaoEnum.Pronto);
        }
    
        protected void lnkConsEntregue_Click(object sender, EventArgs e)
        {
            Response.Redirect("CadPedidoCorte.aspx?sit=" + (int)PedidoCorte.SituacaoEnum.Entregue);
        }
    
        protected void btnProducao_Click(object sender, EventArgs e)
        {
            AlteraSituacaoPedido("Produção", 2);
        }
    
        protected void btnPronto_Click(object sender, EventArgs e)
        {
            AlteraSituacaoPedido("Pronto", 3);
        }
    
        protected void btnEntregue_Click(object sender, EventArgs e)
        {
            AlteraSituacaoPedido("Entregue", 4);
        }
    
        private void AlteraSituacaoPedido(string descrSituacao, int situacao)
        {
            // Coloca confirmação via javascript da alteração da situação do pedido
            btnConfirmar.OnClientClick = "return confirm('Confirma alterar situação do pedido para \"" + descrSituacao + "\"?');";
    
            // Muda o título da tabela de alteração da situação do pedido
            lblTitulo.Text = "Alterar situação para " + descrSituacao;
    
            // Guarda no hiddenField, para qual situação o pedido será modificado
            hdfSitConf.Value = situacao.ToString();
    
            // Mostra a parte que confirma alteração da situação do pedido e esconde os botões
            tbConfirmar.Visible = true;
            tbSituacoes.Visible = false;
        }
    
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            uint idPedido = Glass.Conversoes.StrParaUint(hdfIdPedido.Value);
            int sitConf = Glass.Conversoes.StrParaInt(hdfSitConf.Value);
    
            try
            {
                // Altera a situação do pedido para a que foi escolhida
                PedidoCorteDAO.Instance.AlteraSituacao(UserInfo.GetUserInfo.CodUser, idPedido, sitConf);
    
                tbConfirmar.Visible = false;
                tbSituacoes.Visible = true;
    
                // Limpa o hiddenField que guardou o idPedido e para qual situação o pedido foi alterado
                hdfSitConf.Value = String.Empty;
                hdfIdPedido.Value = String.Empty;
    
                btnConfirmar.Visible = false;
    
                grdCorte.DataBind();
                
                //string novaSituacao = sitConf == 2 ? "Produção" : sitConf == 3 ? "Pronto" : sitConf == 4 ? "Entregue" : String.Empty;
                //Glass.MensagemAlerta.ShowMsg("Pedido " + idPedido + " alterado para " + novaSituacao + " com sucesso.", Page);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao alterar situação do Pedido.", ex, Page);
            }
        }
    
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            // Mostra botões que alteram a situação do pedido e esconde a parte de confirmar alteração
            tbConfirmar.Visible = false;
            tbSituacoes.Visible = true;
            btnConfirmar.Visible = false;
    
            // Limpa o hiddenField que guardou o idPedido e para qual situação o pedido foi alterado
            hdfSitConf.Value = String.Empty;
            hdfIdPedido.Value = String.Empty;
        }
    
        protected void lnkPesqPed_Click(object sender, EventArgs e)
        {
            // Salva o id do Pedido buscado
            hdfIdPedido.Value = txtNumPedidoConf.Text;
    
            tbConfirmar.Visible = true;
            tbSituacoes.Visible = false;
        }
    
        #region Trata possíveis erros que podem ocorrer ao buscar pedido
        
        protected void odsPedido_Selected(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg(null, e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else if (!String.IsNullOrEmpty(hdfIdPedido.Value))
                btnConfirmar.Visible = true;
        }
    
        protected void odsProdXPed_Selected(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg(null, e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
    
        #endregion
    
        protected void lnkRemFiltro_Click(object sender, EventArgs e)
        {
            txtDataIni.Text = String.Empty;
            txtDataFim.Text = String.Empty;
            txtNumPedido.Text = String.Empty;
        }
    
        protected void lnkLgout_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            FormsAuthentication.SignOut();
            FormsAuthentication.RedirectToLoginPage();
        }
    }
}
