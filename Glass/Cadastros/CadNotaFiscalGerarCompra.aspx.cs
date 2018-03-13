using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Data.Model;
using System.Collections;
using System.Drawing;
using System.Linq;
using Glass.Configuracoes;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadNotaFiscalGerarCompra : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadNotaFiscalGerarCompra));
    
            txtIdFornec.Text = "";
            txtNomeFornec.Text = "";
    
            if (!IsPostBack)
            {
                if (!String.IsNullOrEmpty(Request["idCompra"]))
                    ClientScript.RegisterStartupScript(GetType(), "adcomp", "addCompra(" + Request["idCompra"] + ");", true);
            }
            else if (!string.IsNullOrEmpty(hdfBuscarIdsCompras.Value.Trim(',')) && drpPlanoContas.Items.Count == 1)
            {
                var idCompra = Glass.Conversoes.StrParaUint(hdfBuscarIdsCompras.Value.TrimEnd(','));
                var compra = CompraDAO.Instance.GetElementByPrimaryKey(idCompra);
    
                drpTipoCompra.SelectedValue = compra.TipoCompra == (int)Compra.TipoCompraEnum.AVista ||
                    compra.TipoCompra == (int)Compra.TipoCompraEnum.AntecipFornec ? "1" : "2";
    
                drpPlanoContas.DataBind();

                if (compra.IdConta > 0)
                {
                    var situacao = PlanoContasDAO.Instance.ObterSituacao(null, (int)compra.IdConta.Value);

                    if (situacao != PlanoContas.SituacaoEnum.Ativo)
                        MensagemAlerta.ShowMsg("O plano de contas associado à compra está inativo. Ative-o ou selecione outro plano de contas para a nota fiscal.", Page);
                    else
                        drpPlanoContas.SelectedValue = compra.IdConta.ToString();
                }
            }
        }
    
        protected void imgPesq_Click(object sender, EventArgs e)
        {
            grdCompras.DataBind();
        }
    
        protected void odsLoja_Selected(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (!(e.ReturnValue is IEnumerable))
                return;
    
            drpLoja.SelectedIndex = drpLoja.Items.IndexOf(drpLoja.Items.FindByValue(UserInfo.GetUserInfo.IdLoja.ToString()));
        }
    
        protected void btnBuscarCompras_Click(object sender, EventArgs e)
        {
            grdCompras.DataBind();
        }
    
        protected void odsCompras_Selected(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.ReturnValue is IEnumerable)
            {
                Compra[] compras = (Compra[])e.ReturnValue;
                lblMensagem.Text = String.Empty;
                btnGerarNf.Enabled = false;
                gerar.Visible = compras.Length > 0;
    
                var lojas = compras.Select(x => x.IdLoja).Distinct().ToList();
                if (lojas.Count == 1)
                {
                    if (drpLoja.Items.Count == 0) drpLoja.DataBind();
                    drpLoja.SelectedValue = lojas[0].ToString();
                }
    
                if (compras.Length == 0)
                    lblMensagem.Text = "Selecione pelo menos uma compra para gerar a nota.";
                else
                {
                    var fornecedores = new List<uint>();
                    foreach (Compra c in compras)
                        if (!fornecedores.Contains(c.IdFornec.GetValueOrDefault()))
                            fornecedores.Add(c.IdFornec.GetValueOrDefault());
    
                    if (fornecedores.Count > 1)
                    {
                        lblMensagem.Text = "Há compras selecionadas de mais de um fornecedor.";
                        return;
                    }
    
                    btnGerarNf.Enabled = true;
                }
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            hdfBuscarIdsCompras.Value = "";
        }
    
        protected void grdCompras_DataBound(object sender, EventArgs e)
        {
            float total = 0;
    
            for (int i = 0; i < grdCompras.Rows.Count; i++)
            {
                uint idCompra = Glass.Conversoes.StrParaUint(((HiddenField)grdCompras.Rows[i].Cells[0].FindControl("hdfIdCompra")).Value);
                string notasGeradas = CompraNotaFiscalDAO.Instance.ObtemNumerosNFe(idCompra);
    
                hdfIdFornec.Value = CompraDAO.Instance.ObtemIdFornec(idCompra).ToString();
    
                HiddenField hdfNotasGeradas = (HiddenField)grdCompras.Rows[i].Cells[0].FindControl("hdfNotasGeradas");
                hdfNotasGeradas.Value = notasGeradas;
                Color corLinha = !String.IsNullOrEmpty(notasGeradas) ? Color.Red : Color.Black;
    
                for (int j = 0; j < grdCompras.Rows[i].Cells.Count; j++)
                    grdCompras.Rows[i].Cells[j].ForeColor = corLinha;
    
                Label totalCompra = (Label)grdCompras.Rows[i].FindControl("lblTotalCompra");
    
                string totalStr = totalCompra.Visible ? totalCompra.Text : "0";
    
                float totalLinha = Glass.Conversoes.StrParaFloat(totalStr.Replace("R$", "").Replace(" ", "").Replace(".", ""));
                total += totalLinha;
            }
    
            lblTotal.Text = total.ToString("C");
        }
    
        #region Métodos AJAX
    
        [Ajax.AjaxMethod]
        public string GerarNf(string idsCompras, string idNaturezaOperacao, string idLoja, string dadosNaturezasOperacao,
            string idFornecedor, string tipoCompra, string idConta, string numeroNFe)
        {
            return WebGlass.Business.NotaFiscal.Fluxo.Gerar.Ajax.GerarNf(idsCompras, idNaturezaOperacao, idLoja, 
                dadosNaturezasOperacao, idFornecedor, tipoCompra, idConta, numeroNFe);
        }
    
        [Ajax.AjaxMethod]
        public string GetAgruparProdutoNf()
        {
            return FiscalConfig.NotaFiscalConfig.AgruparProdutosGerarNFeTerceiros.ToString();
        }
    
        [Ajax.AjaxMethod]
        public string GetComprasByFornecedor(string idFornec, string nomeFornec)
        {
            return WebGlass.Business.Compra.Fluxo.BuscarEValidar.Ajax.GetComprasByFornecedor(idFornec, nomeFornec);
        }
    
        [Ajax.AjaxMethod]
        public string ValidaCompra(string idCompra)
        {
            return WebGlass.Business.Compra.Fluxo.BuscarEValidar.Ajax.ValidaCompra(idCompra);
        }
    
        #endregion
    }
}
