﻿using GDA;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Estoque.Negocios.Entidades;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

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
            if (txtNumPedido.Text == string.Empty || grdProdutos.Rows.Count <= 0)
            {
                return;
            }

            try
            {
                if (UserInfo.GetUserInfo.TipoUsuario != (uint)Data.Helper.Utils.TipoFuncionario.AuxAlmoxarifado &&
                    !Config.PossuiPermissao(Config.FuncaoMenuEstoque.ControleEstoque))
                {
                    MensagemAlerta.ShowMsg("Você não tem permissão para marcar saída de produtos.", Page);
                    return;
                }

                var idPedido = txtNumPedido.Text.StrParaUint();
                Data.Model.Pedido.SituacaoPedido situacao = PedidoDAO.Instance.ObtemSituacao(null, idPedido);

                if (situacao != Data.Model.Pedido.SituacaoPedido.Confirmado)
                {
                    MensagemAlerta.ShowMsg(string.Format("Este pedido ainda não foi {0}.",
                        Configuracoes.PedidoConfig.LiberarPedido ? "liberado" : "confirmado"), Page);

                    tbSaida.Visible = false;
                    return;
                }

                var lstProdPed = new List<KeyValuePair<int, float>>();

                foreach (GridViewRow r in grdProdutos.Rows)
                {
                    var qtdSaidaInformada = ((TextBox)r.FindControl("txtQtdSaida")).Text.StrParaFloat();

                    if (qtdSaidaInformada == 0)
                    {
                        continue;
                    }

                    var dadosProduto = new KeyValuePair<int, float>(
                        ((HiddenField)r.FindControl("hdfIdProdPed")).Value.StrParaInt(),
                        qtdSaidaInformada);

                    lstProdPed.Add(dadosProduto);
                }

                using (var transacao = new GDATransaction())
                {
                    try
                    {
                        MovEstoqueDAO.Instance.BaixaEstoqueManual(transacao, drpLoja.SelectedValue.StrParaInt(), (int)idPedido, lstProdPed, txtObservacao.Text);

                        transacao.Commit();
                        transacao.Close();
                    }
                    catch
                    {
                        transacao.Rollback();
                        transacao.Close();
                        throw;
                    }
                }

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

            using (var transacao = new GDATransaction())
            {
                try
                {
                    transacao.BeginTransaction();

                    foreach (GridViewRow r in grdProdutosProd.Rows)
                    {
                        var idProd = Glass.Conversoes.StrParaInt(((HiddenField)r.FindControl("hdfIdProd")).Value);
                        float qtde = Glass.Conversoes.StrParaFloat(((TextBox)r.FindControl("txtQtdSaida")).Text);

                        if (idProd == 0 || qtde == 0)
                            continue;

                        MovEstoqueDAO.Instance.BaixaEstoqueManual(transacao, (uint)idProd, idLoja, (decimal)qtde, null, DateTime.Now, txtObservacao.Text);
                    }

                    transacao.Commit();
                }
                catch
                {
                    transacao.Rollback();
                    throw;
                }
            }

            txtObservacao.Text = null;
            grdProdutosProd.DataBind();
        }
    }
}
