using System;
using System.Web.UI;
using WebGlass.Business.Pedido.Fluxo;

namespace Glass.UI.Web.Utils
{
    public partial class SetFinalizarFinanceiro : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Page.Title = Request["tipo"] + " Pedido pelo Financeiro";
                btnExecutar.Text = Request["tipo"];
            }
    
            if (!FinalizarFinanceiro.Instance.PodeExecutarAcao(Glass.Conversoes.StrParaUint(Request["id"]), Request["tipo"]))
            {
                FecharPagina(null);
                return;
            }
        }
    
        protected void btnExecutar_Click(object sender, EventArgs e)
        {
            uint idPedido = Glass.Conversoes.StrParaUint(Request["id"]);
            string tipo = Request["tipo"] != null ? Request["tipo"].ToLower() : "";
            bool aprovado = bool.Parse(drpAprovado.SelectedValue);
    
            try
            {
                if (!Data.Helper.Config.PossuiPermissao(Data.Helper.Config.FuncaoMenuFinanceiro.FinalizarConfirmarPedidoPeloFinanceiro))
                    throw new Exception("Você não tem permissão para finalizar/confirmar pedido pelo financeiro.");
    
                FinalizarFinanceiro.Instance.ExecutarAcao(idPedido, tipo, txtObs.Text, !aprovado);
                FecharPagina(aprovado ? "Pedido " + tipo.TrimEnd('r') + "do com sucesso!" : 
                    "Pedido não foi " + tipo.TrimEnd('r') + "do, voltando à situação anterior.");
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao " + tipo + " pedido. ", ex, Page);
            }
        }
    
        private void FecharPagina(string mensagem)
        {
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "fechar", (String.IsNullOrEmpty(mensagem) ? "" :
                "alert('" + mensagem + "'); ") + "window.opener.redirectUrl(window.opener.location.href); closeWindow();", true);
        }
    }
}
