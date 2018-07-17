using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Estoque.Negocios.Entidades;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadSaidaEstoque : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            txtNumPedido.Focus();
        }
    
        protected void imbPesq_Click(object sender, ImageClickEventArgs e)
        {
            uint idPedido = Glass.Conversoes.StrParaUint(txtNumPedido.Text);
    
            if (!PedidoDAO.Instance.PedidoExists(idPedido))
            {
                Glass.MensagemAlerta.ShowMsg("Não existe nenhum pedido com o número passado.", Page);
                tbSaida.Visible = false;
            }
            else
            {
                Glass.Data.Model.Pedido.SituacaoPedido situacao = PedidoDAO.Instance.ObtemSituacao(null, idPedido);
                var tipoEntrega = PedidoDAO.Instance.ObtemTipoEntrega(idPedido);
    
                if (PedidoDAO.Instance.IsProducao(null, idPedido))
                {
                    Glass.MensagemAlerta.ShowMsg("Este pedido é um pedido para produção. Não é possível selecionar um pedido desse tipo.", Page);
                    tbSaida.Visible = false;
                }
                else if (situacao == Glass.Data.Model.Pedido.SituacaoPedido.Cancelado)
                {
                    Glass.MensagemAlerta.ShowMsg("Este pedido foi cancelado.", Page);
                    tbSaida.Visible = false;
                }
                else if (situacao != Glass.Data.Model.Pedido.SituacaoPedido.Confirmado)
                {
                    Glass.MensagemAlerta.ShowMsg(string.Format("Este pedido ainda não foi {0}.",
                        Configuracoes.PedidoConfig.LiberarPedido ? "liberado" : "confirmado"), Page);
                    tbSaida.Visible = false;
                }
                else
                    tbSaida.Visible = true;
            }
    
            drpLoja.AutoPostBack = false;
            tbSaidaProd.Visible = false;
        }
    
        protected void lnkTodos_Click(object sender, EventArgs e)
        {
            // Marca todos os campos de qtd de saída com o total de saída que falta ser lançado
            foreach (GridViewRow row in grdProdutos.Rows)
            {
                ((TextBox)row.FindControl("txtQtdSaida")).Text = (decimal.Parse(((HiddenField)row.FindControl("hdfQtde")).Value) -
                    decimal.Parse(((HiddenField)row.FindControl("hdfQtdSaida")).Value)).ToString();
            }
        }
    
        protected void btnMarcarSaida_Click(object sender, EventArgs e)
        {
            if (txtNumPedido.Text == String.Empty || grdProdutos.Rows.Count <= 0)
                return;
    
            try
            {
                if (UserInfo.GetUserInfo.TipoUsuario != (uint)Data.Helper.Utils.TipoFuncionario.AuxAlmoxarifado &&
                    !Config.PossuiPermissao(Config.FuncaoMenuEstoque.ControleEstoque))
                {
                    Glass.MensagemAlerta.ShowMsg("Você não tem permissão para marcar saída de produtos.", Page);
                    return;
                }

                // Chamado 69935
                uint idPedido = Glass.Conversoes.StrParaUint(txtNumPedido.Text);
                Glass.Data.Model.Pedido.SituacaoPedido situacao = PedidoDAO.Instance.ObtemSituacao(null, idPedido);
                if (situacao != Glass.Data.Model.Pedido.SituacaoPedido.Confirmado)
                {
                    Glass.MensagemAlerta.ShowMsg(string.Format("Este pedido ainda não foi {0}.",
                        Configuracoes.PedidoConfig.LiberarPedido ? "liberado" : "confirmado"), Page);
                    tbSaida.Visible = false;
                    return;
                }

                var lstProdPed = new List<DetalhesBaixaEstoque>();
    
                foreach (GridViewRow r in grdProdutos.Rows)
                {
                    string qtdSaidaInformadaString = ((TextBox)r.FindControl("txtQtdSaida")).Text;

                    var dadosProduto = new DetalhesBaixaEstoque()
                    {
                        IdProdPed = Glass.Conversoes.StrParaInt(((HiddenField)r.FindControl("hdfIdProdPed")).Value),
                        Qtde = String.IsNullOrEmpty(qtdSaidaInformadaString) ? 0 : float.Parse(qtdSaidaInformadaString),
                        DescricaoBaixa = ((HiddenField)r.FindControl("hdfDescr")).Value.Replace("'", "").Replace("\"", "")
                    };
    
                    // Se a quantidade de saída for 0, continua no próximo item
                    if (dadosProduto.Qtde == 0)
                        continue;
    
                    lstProdPed.Add(dadosProduto);
                }
    
                WebGlass.Business.Pedido.Fluxo.AlterarEstoque.Instance.BaixarEstoqueComTransacao
                    (drpLoja.SelectedValue.StrParaUint(), lstProdPed, null, null, true, txtObservacao.Text);

                txtObservacao.Text = null;
                Glass.MensagemAlerta.ShowMsg("Saída de produtos efetuada com sucesso.", Page);
    
                grdProdutos.DataBind();
                txtNumPedido.Focus();
    
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao marcar saída de estoque.", ex, Page);
            }
        }
    
        protected void imgPesq1_Click(object sender, ImageClickEventArgs e)
        {
            tbSaida.Visible = false;
            grdProdutosProd.DataBind();
    
            tbSaidaProd.Visible = grdProdutosProd.Rows.Count > 0;
            drpLoja.AutoPostBack = tbSaidaProd.Visible;
        }
    
        protected void btnMarcarProd_Click(object sender, EventArgs e)
        {
            uint idLoja = Glass.Conversoes.StrParaUint(drpLoja.SelectedValue);
    
            foreach (GridViewRow r in grdProdutosProd.Rows)
            {
                var idProd = Glass.Conversoes.StrParaInt(((HiddenField)r.FindControl("hdfIdProd")).Value);
                float qtde = Glass.Conversoes.StrParaFloat(((TextBox)r.FindControl("txtQtdSaida")).Text);
    
                if (idProd == 0 || qtde == 0)
                    continue;
    
                int tipoCalc = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(idProd);
                bool m2 = tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 || tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto;
    
                MovEstoqueDAO.Instance.BaixaEstoqueManual((uint)idProd, idLoja, (decimal)qtde, null, DateTime.Now, txtObservacao.Text);
            }

            txtObservacao.Text = null;
            grdProdutosProd.DataBind();
        }
    }
}
