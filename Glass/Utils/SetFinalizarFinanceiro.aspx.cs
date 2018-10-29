using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using WebGlass.Business.Pedido.Fluxo;

namespace Glass.UI.Web.Utils
{
    public partial class SetFinalizarFinanceiro : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                if (this.Request["ids"] == null)
                {
                    this.Page.Title = this.Request["tipo"] + " Pedido pelo Financeiro";
                    this.btnExecutar.Text = this.Request["tipo"];
                }
                else
                {
                    this.Page.Title = "Finalizar/Confirmar Pedidos pelo Financeiro";
                    this.btnExecutar.Text = "Finalizar/Confirmar";
                }
            }

            if (this.Request["ids"] == null && !FinalizarFinanceiro.Instance.PodeExecutarAcao(Glass.Conversoes.StrParaUint(this.Request["id"]), this.Request["tipo"]))
            {
                this.FecharPagina(null);
                return;
            }
        }

        protected void btnExecutar_Click(object sender, EventArgs e)
        {
            if (!Data.Helper.Config.PossuiPermissao(Data.Helper.Config.FuncaoMenuFinanceiro.FinalizarConfirmarPedidoPeloFinanceiro))
            {
                throw new InvalidOperationException("Você não tem permissão para finalizar/confirmar pedido pelo financeiro.");
            }

            bool aprovado = bool.Parse(this.drpAprovado.SelectedValue);

            if (this.Request["ids"] != null)
            {
                var idsPedidoTipo = this.Request["ids"].Split(';');
                var mensagens = new StringBuilder();

                foreach (var item in idsPedidoTipo)
                {
                    var idTipo = item.Split(',').Select(f => f.StrParaInt()).ToArray();
                    var tipo = idTipo[1] == 1 ? "finalizar" : "confirmar";

                    try
                    {
                        FinalizarFinanceiro.Instance.ExecutarAcao((uint)idTipo[0], tipo, this.txtObs.Text, !aprovado);
                    }
                    catch (Exception ex)
                    {
                        mensagens.Append($"Falha ao {tipo} o pedido {idTipo[0]}. " + ex.Message.ToString());
                    }
                }

                mensagens.Insert(0, aprovado ? "Pedidos Finalizados/Confirmados com sucesso!" : "Pedidos não foram Finalizados/Confirmados, voltando à situação anterior.");

                this.FecharPagina(mensagens.ToString());
            }
            else
            {
                uint idPedido = Glass.Conversoes.StrParaUint(this.Request["id"]);
                string tipo = this.Request["tipo"] != null ? this.Request["tipo"].ToLower() : string.Empty;

                try
                {
                    FinalizarFinanceiro.Instance.ExecutarAcao(idPedido, tipo, this.txtObs.Text, !aprovado);
                    this.FecharPagina(aprovado ? "Pedido " + tipo.TrimEnd('r') + "do com sucesso!" :
                        "Pedido não foi " + tipo.TrimEnd('r') + "do, voltando à situação anterior.");
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao " + tipo + " pedido. ", ex, this.Page);
                }
            }
        }

        private void FecharPagina(string mensagem)
        {
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "fechar", (String.IsNullOrEmpty(mensagem) ? "" :
                "alert('" + mensagem + "'); ") + "window.opener.redirectUrl(window.opener.location.href); closeWindow();", true);
        }
    }
}
