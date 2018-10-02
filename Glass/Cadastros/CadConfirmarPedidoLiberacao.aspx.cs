using Glass.Configuracoes;
using Glass.Data.DAL;
using Glass.Data.Exceptions;
using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadConfirmarPedidoLiberacao : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(CadConfirmarPedidoLiberacao));

            if (!IsPostBack)
            {
                //Caso o sistema seja Lite a Opção "Conferência dos pedidos já realizada" é oculta 
                if (Geral.SistemaLite || !Geral.ControlePCP)
                {
                    chkGerarEspelho.Visible = false;
                    chkGerarEspelho.Checked = false;
                }
                else
                    chkGerarEspelho.Checked = PedidoConfig.TelaConfirmaPedidoLiberacao.GerarPedidoMarcado;

                ((TextBox)ctrlDataIni.FindControl("txtData")).Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(-2).ToString("dd/MM/yyyy");
                ((TextBox)ctrlDataFim.FindControl("txtData")).Text = DateTime.Now.ToString("dd/MM/yyyy");
            }
        }

        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            try
            {
                if (chkAlterarDataEntrega.Checked && string.IsNullOrWhiteSpace(((TextBox)ctrlDataEntrega.FindControl("txtData")).Text))
                {
                    throw new Exception("Informe a data de entrega dos pedidos.");
                }

                var idsPedido = new List<int>();

                for (var i = 0; i < grdPedido.Rows.Count; i++)
                {
                    var chkMarcar = grdPedido.Rows[i].Cells[0].FindControl("chkMarcar") as CheckBox;
                    var hdfIdPedido = grdPedido.Rows[i].Cells[0].FindControl("hdfIdPedido") as HiddenField;

                    if (chkMarcar != null && chkMarcar.Checked)
                    {
                        if (hdfIdPedido != null)
                        {
                            idsPedido.Add(hdfIdPedido.Value.StrParaInt());
                        }
                    }
                }

                if (idsPedido.Count == 0)
                {
                    throw new Exception("Informe os pedidos que serão confirmados.");
                }

                var data = ((TextBox)ctrlDataEntrega.FindControl("txtData")).Text;

                string script;

                if (chkGerarEspelho.Checked)
                {
                    // Verifica se o usuário possui permissão para gerar espelho de pedido
                    if (Data.Helper.UserInfo.GetUserInfo.IdCliente == null || Data.Helper.UserInfo.GetUserInfo.IdCliente == 0)
                    {
                        foreach (var idPedido in idsPedido)
                        {
                            var isMaoDeObra = PedidoDAO.Instance.IsMaoDeObra(null, (uint)idPedido);

                            if (!Data.Helper.Config.PossuiPermissao(Data.Helper.Config.FuncaoMenuPCP.GerarConferenciaPedido))
                            {
                                if (Data.Helper.Config.PossuiPermissao(Data.Helper.Config.FuncaoMenuPCP.ImprimirEtiquetasMaoDeObra) && !isMaoDeObra)
                                {
                                    throw new Exception("Você pode gerar conferência apenas de pedidos mão de obra.");
                                }
                                else
                                {
                                    throw new Exception("Você não possui permissão para gerar Conferências de Pedidos.");
                                }
                            }
                        }

                    }
                }

                WebGlass.Business.Pedido.Fluxo.ConfirmarPedido.Instance.ConfirmarPedidoLiberacao(idsPedido, chkAlterarDataEntrega.Checked,
                    !string.IsNullOrEmpty(data) ? (DateTime?)DateTime.Parse(data) : null, chkGerarEspelho.Checked, out script);

                grdPedido.DataBind();

                if (!string.IsNullOrEmpty(script))
                {
                    ClientScript.RegisterStartupScript(typeof(string), "ok", script, true);
                }
            }
            catch (ValidacaoPedidoFinanceiroException f)
            {
                var mensagem = MensagemAlerta.FormatErrorMsg(string.Empty, f);

                var script = $@"var resposta = CadConfirmarPedidoLiberacao.EnviarConfirmarFinanceiro('{ string.Join(",", f.IdsPedido) }', '{ mensagem }').value.split('|');
                    
                    if (resposta[0] == 'Ok')
                        redirectUrl(window.location.href);
                    else
                        alert(resposta[1]);";

                if (FinanceiroConfig.PerguntarVendedorConfirmacaoFinanceiro)
                {
                    var incluirS = f.IdsPedido.Count > 1 ? "s" : string.Empty;

                    script = $"if (confirm('Não foi possível confirmar o{ incluirS } pedido{ incluirS } { string.Join(", ", f.IdsPedido) }. Erro: { mensagem.TrimEnd(' ', '.') }." +
                        $"\\nDeseja enviar esse pedido para confirmar pelo Financeiro?')) { "{" }{ script }{ "}" }";
                }
                else
                {
                    var incluirS = f.IdsPedido.Count > 1 ? "s" : string.Empty;

                    script = $"alert('Houve um erro ao confirmar o{ incluirS } pedido{ incluirS } { string.Join(", ", f.IdsPedido) }. " +
                        $"Eles foram disponibilizados para confirmação pelo Financeiro.'); { "{" }{ script }{ "}" }";
                }

                Page.ClientScript.RegisterStartupScript(GetType(), "btnFinalizar", script, true);
            }
            catch (Exception ex)
            {
                MensagemAlerta.ErrorMsg("Falha ao confirmar pedido.", ex, Page);
            }
        }

        [Ajax.AjaxMethod]
        public string EnviarConfirmarFinanceiro(string idsPedido, string mensagem)
        {
            try
            {
                PedidoDAO.Instance.DisponibilizaConfirmacaoFinanceiro(null, idsPedido?.Split(',')?.Select(f => f.StrParaInt())?.ToList(), mensagem);
                return "Ok";
            }
            catch (Exception ex)
            {
                return "Erro|" + ex.Message;
            }
        }

        protected void grdPedido_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;

            var item = e.Row.DataItem as Glass.Data.Model.Pedido;
            if (item == null)
                return;

            var corLinha = Color.Black;

            if (!string.IsNullOrEmpty(PedidoConfig.TelaConfirmaPedidoLiberacao.SubgruposDestacar))
                foreach (var prodPed in ProdutosPedidoDAO.Instance.GetByPedido(item.IdPedido))
                {
                    if (prodPed.IdProd == 0)
                        continue;

                    var idSubgrupoProd = ProdutoDAO.Instance.ObtemIdSubgrupoProd((int)prodPed.IdProd);

                    if (idSubgrupoProd.GetValueOrDefault() == 0)
                        continue;

                    if (idSubgrupoProd != null)
                    {
                        var descricaoSubgrupoProd = SubgrupoProdDAO.Instance.ObtemDescricao((int)idSubgrupoProd);

                        /* Chamado 16287 e 16631. */
                        if (PedidoConfig.TelaConfirmaPedidoLiberacao.SubgruposDestacar.Contains(descricaoSubgrupoProd))
                            corLinha = Color.Red;
                    }
                }

            if (corLinha == Color.Black)
                corLinha = item.CorLinhaLista;

            if (corLinha != Color.Black)
                foreach (TableCell c in e.Row.Cells)
                {
                    if (e.Row.Cells.GetCellIndex(c) == 0)
                        continue;

                    c.ForeColor = corLinha;

                    foreach (Control c1 in c.Controls)
                    {
                        var control = c1 as WebControl;
                        if (control != null)
                            control.ForeColor = c.ForeColor;
                    }
                }
        }

        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdPedido.PageIndex = 0;
        }
    }
}
