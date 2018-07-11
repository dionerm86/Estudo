using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Model;
using System.Drawing;
using Glass.Data.Exceptions;
using Glass.Configuracoes;

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
                if (chkAlterarDataEntrega.Checked && ((TextBox)ctrlDataEntrega.FindControl("txtData")).Text == "")
                    throw new Exception("Informe a data de entrega dos pedidos.");
    
                var idsPedidos = string.Empty;
                for (var i = 0; i < grdPedido.Rows.Count; i++)
                {
                    var chkMarcar = grdPedido.Rows[i].Cells[0].FindControl("chkMarcar") as CheckBox;
                    var hdfIdPedido = grdPedido.Rows[i].Cells[0].FindControl("hdfIdPedido") as HiddenField;
    
                    if (chkMarcar != null && chkMarcar.Checked)
                        if (hdfIdPedido != null) idsPedidos += "," + hdfIdPedido.Value;
                }
    
                if (idsPedidos.Length == 0)
                    throw new Exception("Informe os pedidos que serão confirmados.");
                
                // Motido da retirada: Com o substring a vírgula era desconsiderada normalmente, porém no método ConfirmarPedidoLiberacao havia outro
                // substring que fazia o número do pedido ficar incorreto, de qualquer forma utilizar o TrimStart é mais seguro.
                // idsPedidos = idsPedidos.Substring(1);
                idsPedidos = idsPedidos.TrimStart(',');
    
                var data = ((TextBox)ctrlDataEntrega.FindControl("txtData")).Text;
    
                string script;

                if(chkGerarEspelho.Checked)
                {
                    // Verifica se o usuário possui permissão para gerar espelho de pedido
                    if (Data.Helper.UserInfo.GetUserInfo.IdCliente == null || Glass.Data.Helper.UserInfo.GetUserInfo.IdCliente == 0)
                    {
                        var ids = idsPedidos.Split(',');
                        foreach (var s in ids)
                        {
                            var idPedido = Conversoes.StrParaUint(s);

                            bool isMaoDeObra = PedidoDAO.Instance.IsMaoDeObra(null, idPedido);
                            if (!Data.Helper.Config.PossuiPermissao(Data.Helper.Config.FuncaoMenuPCP.GerarConferenciaPedido))
                            {
                                if (Data.Helper.Config.PossuiPermissao(Data.Helper.Config.FuncaoMenuPCP.ImprimirEtiquetasMaoDeObra) && !isMaoDeObra)
                                    throw new Exception("Você pode gerar conferência apenas de pedidos mão de obra.");
                                else
                                    throw new Exception("Você não possui permissão para gerar Conferências de Pedidos.");
                            }
                        }
                       
                    }
                }

                WebGlass.Business.Pedido.Fluxo.ConfirmarPedido.Instance.ConfirmarPedidoLiberacao(idsPedidos,
                    chkAlterarDataEntrega.Checked, !string.IsNullOrEmpty(data) ? (DateTime?)DateTime.Parse(data) : null,
                    chkGerarEspelho.Checked, out script);
    
                grdPedido.DataBind();
    
                if (!string.IsNullOrEmpty(script))
                    ClientScript.RegisterStartupScript(typeof(string), "ok", script, true);
            }
            catch (ValidacaoPedidoFinanceiroException f)
            {
                var mensagem = MensagemAlerta.FormatErrorMsg("", f);
    
                var script = @"
                    var resposta = CadConfirmarPedidoLiberacao.EnviarConfirmarFinanceiro('" + f.IdsPedidos + "', '" + mensagem + @"').value.split('|');
                    
                    if (resposta[0] == 'Ok')
                        redirectUrl(window.location.href);
                    else
                        alert(resposta[1]);
                ";
    
                if (FinanceiroConfig.PerguntarVendedorConfirmacaoFinanceiro)
                {
                    script = string.Format("if (confirm('Não foi possível confirmar o{0} pedido{0} {1}. Erro: ",
                        f.IdsPedidos.Contains(",") ? "s" : "", f.IdsPedidos) + mensagem.TrimEnd(' ', '.') +
                        ".\\nDeseja enviar esse pedido para confirmar pelo Financeiro?')) {" + script + "}";
                }
                else
                {
                    script = string.Format("alert('Houve um erro ao confirmar o{0} pedido{0} {1}. " +
                        "Eles foram disponibilizados para confirmação pelo Financeiro.');", 
                        f.IdsPedidos.Contains(",") ? "s" : "", f.IdsPedidos) + script;
                }
    
                Page.ClientScript.RegisterStartupScript(GetType(), "btnFinalizar", script, true);
            }
            catch (Exception ex)
            {
                MensagemAlerta.ErrorMsg("Falha ao confirmar pedido.", ex, Page);
            }
        }
    
        [Ajax.AjaxMethod]
        public string EnviarConfirmarFinanceiro(string idsPedidos, string mensagem)
        {
            try
            {
                PedidoDAO.Instance.DisponibilizaConfirmacaoFinanceiro(null, idsPedidos, mensagem);
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
