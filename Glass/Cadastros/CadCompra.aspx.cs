using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Data.Model;
using Glass.Configuracoes;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadCompra : System.Web.UI.Page
    {
        private uint idFornec = 0;
    
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadCompra));
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            if (Request["idCompra"] != null && dtvCompra.CurrentMode == DetailsViewMode.Insert)
            {
                hdfIdCompra.Value = Request["idCompra"];
    
                if (ProdutosCompraDAO.Instance.CountInCompra(Glass.Conversoes.StrParaUint(Request["idCompra"])) == 0)
                    foreach (TableCell c in grdProdutos.Rows[0].Cells)
                        c.Text = String.Empty;
    
                dtvCompra.ChangeMode(DetailsViewMode.ReadOnly);
            }
    
            if (!IsPostBack && Request["idCompra"] != null && !CompraDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(Request["idCompra"])).EditVisible)
            {
                Response.Redirect("../Listas/LstCompras.aspx");
                return;
            }

            lnkProduto.Visible = grdProdutos.Visible = dtvCompra.CurrentMode == DetailsViewMode.ReadOnly;
        }
    
        protected void grdProdutos_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
            dtvCompra.DataBind();
        }
    
        protected void grdProdutos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdProdutos.ShowFooter = e.CommandName != "Edit";
        }
    
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            if (Request["pcp"] != "1")
                Response.Redirect("../Listas/LstCompras.aspx");
            else
                Response.Redirect("../Listas/LstCompraPcp.aspx");
        }
    
        protected void dtvCompra_DataBound(object sender, EventArgs e)
        {

        }
    
        protected void txtValorIns_Load(object sender, EventArgs e)
        {
            Button botao = dtvCompra.FindControl("btnEditar") as Button;
            if (botao != null)
                ((TextBox)sender).ReadOnly = !botao.Visible;
        }
    
        #region Eventos DataSource
    
        protected void odsCompra_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao cadastrar Compra.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
            {
                hdfIdCompra.Value = e.ReturnValue.ToString();
                Response.Redirect("CadCompra.aspx?IdCompra=" + hdfIdCompra.Value);
            }
        }
    
        protected void odsCompra_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar dados da Compra.", e.Exception, Page);
                e.ExceptionHandled = true;
                return;
            }
            else
                Response.Redirect("CadCompra.aspx?IdCompra=" + hdfIdCompra.Value);
        }
    
        protected void odsProdXCompra_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg(null, e.Exception, Page);
                e.ExceptionHandled = true;
            }
    
            dtvCompra.DataBind();
            grdProdutos.DataBind();
        }
    
        protected void odsProdXCompra_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg(null, e.Exception, Page);
                e.ExceptionHandled = true;
            }
    
            dtvCompra.DataBind();
        }
    
        #endregion
    
        #region Finalizar/Baixar estoque Compra
    
        protected void btnFinalizar_Click(object sender, EventArgs e)
        {
            try
            {
                string script;
                WebGlass.Business.Compra.Fluxo.FinalizarCompra.Instance.Finalizar(Glass.Conversoes.StrParaUint(Request["idCompra"]), 
                    Request["pcp"] == "1", true, out script);
    
                ClientScript.RegisterClientScriptBlock(typeof(string), "showRpt", script, true);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao finalizar compra.", ex, Page);
            }
        }
    
        protected void btnBaixarEstoque_Click(object sender, EventArgs e)
        {
            try
            {
                WebGlass.Business.Compra.Fluxo.AlterarEstoque.Instance.BaixarEstoque(Glass.Conversoes.StrParaUint(Request["idCompra"]));
                
                if (Request["pcp"] != "1")
                    Response.Redirect("../Listas/LstCompras.aspx");
                else
                    Response.Redirect("../Listas/LstCompraPcp.aspx");
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao baixar estoque da compra.", ex, Page);
            }
        }
    
        #endregion
    
        #region Insere Produto
    
        protected void lnkInsProd_Click(object sender, EventArgs e)
        {
            Glass.UI.Web.Controls.ctrlBenef benef = (Glass.UI.Web.Controls.ctrlBenef)grdProdutos.FooterRow.FindControl("ctrlBenefInserir");
            var produto = grdProdutos.FooterRow.FindControl("ctrlSelProd") as Glass.UI.Web.Controls.ctrlSelProduto;
            
            // Cria uma instância do ProdutosPedido
            Produto prod = ProdutoDAO.Instance.GetElement(produto.IdProd.GetValueOrDefault());
            ProdutosCompra prodCompra = new ProdutosCompra();
            prodCompra.IdCompra = Glass.Conversoes.StrParaUint(Request["IdCompra"]);
            prodCompra.Qtde = Glass.Conversoes.StrParaFloat(((TextBox)grdProdutos.FooterRow.FindControl("txtQtdeIns")).Text);
            prodCompra.Valor = Glass.Conversoes.StrParaDecimal(((TextBox)grdProdutos.FooterRow.FindControl("txtValorIns")).Text);
            prodCompra.IdProd = (uint)prod.IdProd;
            prodCompra.DescricaoItemGenerico = produto.DescricaoItemGenerico;
            prodCompra.Altura = Glass.Conversoes.StrParaFloat(((TextBox)grdProdutos.FooterRow.FindControl("txtAlturaIns")).Text);
            prodCompra.Largura = Glass.Conversoes.StrParaInt(((TextBox)grdProdutos.FooterRow.FindControl("txtLarguraIns")).Text);
            prodCompra.TotM = Glass.Conversoes.StrParaFloat(((TextBox)grdProdutos.FooterRow.FindControl("txtTotM2")).Text);
            prodCompra.Espessura = Glass.Conversoes.StrParaFloat(((TextBox)grdProdutos.FooterRow.FindControl("txtEspessura")).Text);
            prodCompra.Obs = ((TextBox)grdProdutos.FooterRow.FindControl("txtObsIns")).Text;
            prodCompra.Beneficiamentos = benef.Beneficiamentos;
            prodCompra.NaoCobrarVidro = ((CheckBox)grdProdutos.FooterRow.FindControl("chkNaoCobrarVidro")).Checked;
    
            try
            {
                ProdutosCompraDAO.Instance.Insert(prodCompra);
                grdProdutos.DataBind();
                dtvCompra.DataBind();
    
                grdProdutos.PageIndex = grdProdutos.PageCount - 1;
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao incluir produto na Compra.", ex, Page);
                return;
            }
        }
    
        #endregion
    
        #region Métodos Ajax
    
        /// <summary>
        /// Retorna o código que reprensenta a forma de pagamento "Cheque"
        /// </summary>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string GetChequeCod()
        {
            return ((int)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio).ToString();
        }
    
        /// <summary>
        /// Retorna o código que reprensenta a forma de pagamento "Dinheiro"
        /// </summary>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string GetDinheiroCod()
        {
            return ((int)Glass.Data.Model.Pagto.FormaPagto.Dinheiro).ToString();
        }
    
        [Ajax.AjaxMethod]
        public string IsFornecedorProprio(string idFornec)
        {
            return FornecedorDAO.Instance.IsFornecedorProprio(Glass.Conversoes.StrParaUint(idFornec)).ToString().ToLower();
        }
    
        /// <summary>
        /// Retorna o Código/Descrição do produto
        /// </summary>
        [Ajax.AjaxMethod()]
        public string GetProduto(string idLoja, string idFornec, string idProd)
        {
            return WebGlass.Business.Produto.Fluxo.BuscarEValidar.Ajax.GetProdutoCompra(idLoja, idFornec, idProd);
        }
    
        /// <summary>
        /// Calcula o Metro quadrado do produto em tempo real utilizando 3 casas decimais
        /// </summary>
        /// <param name="altura"></param>
        /// <param name="largura"></param>
        /// <param name="qtd"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string CalcM2(string idProd, string altura, string largura, string qtd, string tipoCalc)
        {
            return MetodosAjax.CalcM2Compra(idProd, tipoCalc, altura, largura, qtd);
        }
    
        [Ajax.AjaxMethod()]
        public string GetDescrAntecipFornec(string idAntecipFornec)
        {
            uint idAntecip = Glass.Conversoes.StrParaUint(idAntecipFornec);
    
            if (idAntecip == 0)
                return "";
    
            return AntecipacaoFornecedorDAO.Instance.GetDescricao(idAntecip);
        }
    
        #endregion
    
        #region Beneficiamentos
    
        protected void ctrlBenef_Load(object sender, EventArgs e)
        {
            Glass.UI.Web.Controls.ctrlBenef benef = (Glass.UI.Web.Controls.ctrlBenef)sender;
            GridViewRow linhaControle = benef.Parent.Parent as GridViewRow;
            ProdutosCompra pc = linhaControle.DataItem as ProdutosCompra;
    
            Control codProd = null;
            if (linhaControle.FindControl("lblCodProdIns") != null)
                codProd = linhaControle.FindControl("lblCodProdIns");
            else
                codProd = linhaControle.FindControl("hdfCodProd");
    
            TextBox txtAltura = (TextBox)linhaControle.FindControl("txtAlturaIns");
            TextBox txtEspessura = (TextBox)linhaControle.FindControl("txtEspessura");
            TextBox txtLargura = (TextBox)linhaControle.FindControl("txtLarguraIns");
            TextBox txtQuantidade = (TextBox)linhaControle.FindControl("txtQtdeIns");
            TextBox txtTotalM2 = (TextBox)linhaControle.FindControl("txtTotM2");
            TextBox txtValorIns = (TextBox)linhaControle.FindControl("txtValorIns");
            HiddenField hdfCustoProd = (HiddenField)linhaControle.FindControl("hdfCustoProd");
    
            benef.CampoAltura = txtAltura;
            benef.CampoEspessura = txtEspessura;
            benef.CampoLargura = txtLargura;
            benef.CampoQuantidade = txtQuantidade;
            benef.CampoTotalM2 = txtTotalM2;
            benef.CampoValorUnitario = txtValorIns;
            benef.CampoProdutoID = codProd;
            benef.CampoCusto = hdfCustoProd;
        }
    
        #endregion
    
        #region Parcelas
    
        protected void ctrlParcelas1_Load(object sender, EventArgs e)
        {
            Glass.UI.Web.Controls.ctrlParcelas ctrlParcelas = (Glass.UI.Web.Controls.ctrlParcelas)sender;
            HiddenField hdfCalcularParcelas = (HiddenField)dtvCompra.FindControl("hdfCalcularParcelas");
            HiddenField hdfExibirParcelas = (HiddenField)dtvCompra.FindControl("hdfExibirParcelas");
            TextBox txtNumParc = (TextBox)dtvCompra.FindControl("txtNumParc");
            TextBox txtTotal = (TextBox)dtvCompra.FindControl("txtTotal");
            TextBox txtDesconto = (TextBox)dtvCompra.FindControl("txtDesconto");
            HiddenField hdfDesconto = (HiddenField)dtvCompra.FindControl("hdfDesconto");
            HiddenField hdfAcrescimo = (HiddenField)dtvCompra.FindControl("hdfAcrescimo");
            HiddenField hdfAcrescimoAnterior = (HiddenField)dtvCompra.FindControl("hdfAcrescimoAnterior");
            TextBox txtEntrada = (TextBox)dtvCompra.FindControl("ctrValEntrada").FindControl("txtNumber");
            DropDownList drpFormaPagto = (DropDownList)dtvCompra.FindControl("drpFormaPagto");
            TextBox txtDataBaseVenc = ((TextBox)((Glass.UI.Web.Controls.ctrlData)dtvCompra.FindControl("ctrlDataBaseVenc")).FindControl("txtData"));
            HiddenField hdfPrazos = (HiddenField)dtvCompra.FindControl("hdfPrazos");
            
            /* Chamado 16565.
             * Evita que o valor da compra seja alterado caso o sinal tenha sido recebido,
             * e garante que o valor de entrada serÃ¡ desconsiderado ao calcular o valor das parcelas. */
            if (Conversoes.StrParaUint(Request["IdCompra"]) > 0 && CompraDAO.Instance.RecebeuSinal(Conversoes.StrParaUint(Request["IdCompra"])))
                ((TextBox)(dtvCompra.FindControl("ctrValEntrada").FindControl("txtNumber"))).ReadOnly = true;
    
            ctrlParcelas.CampoCalcularParcelas = hdfCalcularParcelas;
            ctrlParcelas.CampoExibirParcelas = hdfExibirParcelas;
            ctrlParcelas.CampoParcelasVisiveis = txtNumParc;
            //ctrlParcelas.CampoValorTotal = txtTotal;
            ctrlParcelas.CampoValorTotal = (TextBox)dtvCompra.FindControl("txtTotalFinal");
            ctrlParcelas.CampoValorDescontoAnterior = hdfDesconto;
            ctrlParcelas.CampoValorDescontoAtual = txtDesconto;
            ctrlParcelas.CampoValorAcrescimoAnterior = hdfAcrescimoAnterior;
            ctrlParcelas.CampoValorAcrescimoAtual = hdfAcrescimo;
            ctrlParcelas.CampoValorEntrada = txtEntrada;
            ctrlParcelas.NumParcelas = FinanceiroConfig.Compra.NumeroParcelasCompra;
            ctrlParcelas.CampoFormaPagto = drpFormaPagto;
            ctrlParcelas.CampoDataBase = txtDataBaseVenc;
            ctrlParcelas.CampoTextoParcelas = hdfPrazos;
        }
    
        #endregion
    
        protected uint GetIdFornec()
        {
            if (idFornec == 0 && !String.IsNullOrEmpty(Request["idCompra"]))
                idFornec = CompraDAO.Instance.ObtemIdFornec(Glass.Conversoes.StrParaUint(Request["idCompra"]));
    
            return idFornec;
        }
    
        protected void dtvCompra_ItemCommand(object sender, DetailsViewCommandEventArgs e)
        {
            grdProdutos.Visible = e.CommandName != "Edit";
        }
    
        protected void btnCancelar_Click1(object sender, EventArgs e)
        {
            Response.Redirect(Request.Url.ToString());
        }
    
        protected void drpLoja_DataBound(object sender, EventArgs e)
        {
            if (FinanceiroConfig.FinanceiroPagto.CompraLojaPadrao > 0)
                ((DropDownList)sender).SelectedValue = FinanceiroConfig.FinanceiroPagto.CompraLojaPadrao.ToString();
        }
    
        protected void chkContabil_Load(object sender, EventArgs e)
        {
            if (FinanceiroConfig.SepararValoresFiscaisEReaisContasPagar)
                ((CheckBox)sender).Style.Add("display", "none");
        }
    
        protected string NomeControleBenef()
        {
            return grdProdutos.EditIndex == -1 ? "ctrlBenefInserir" : "ctrlBenefEditar";
        }
    
        protected void drpTipoCompra_Load(object sender, EventArgs e)
        {
            if (FinanceiroConfig.UsarPgtoAntecipFornec && FornecedorConfig.TipoUsoAntecipacaoFornecedor == DataSources.TipoUsoAntecipacaoFornecedor.CompraOuNotaFiscal)
            {
                var drpTipoCompra = dtvCompra.FindControl("drpTipoCompra");
    
                if (drpTipoCompra != null)
                    ((DropDownList)drpTipoCompra).Items.Add(new ListItem("Antecip. Fornecedor", ((int)Compra.TipoCompraEnum.AntecipFornec).ToString()));
            }
        }

        protected void txtValorTributado_Load(object sender, EventArgs e)
        {
            // Esconde este campo, posteriormente será apagado
            ((TextBox)sender).Style.Add("display", "none");
        }

        protected void lblValorTributado_Load(object sender, EventArgs e)
        {
            // Esconde este campo, posteriormente será apagado
            ((Label)sender).Style.Add("display", "none");
        }
    }
}
