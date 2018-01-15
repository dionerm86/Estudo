using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebGlass.Business.CotacaoCompra.Entidade;
using WebGlass.Business.CotacaoCompra.Fluxo;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadCotacaoCompra : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadCotacaoCompra));
    
            if (String.IsNullOrEmpty(Request["id"]))
            {
                dtvCotacaoCompra.ChangeMode(DetailsViewMode.Insert);
                ExibirItens(false);
            }
            else
            {
                if (!WebGlass.Business.CotacaoCompra.Fluxo.BuscarEValidar.Instance.PodeEditar(Glass.Conversoes.StrParaUint(Request["id"])))
                {
                    Response.Redirect("~/Listas/LstCotacaoCompra.aspx", true);
                    return;
                }
    
                if (!IsPostBack)
                    ExibirItens(true);
            }
    
            if (!IsPostBack && PedidoConfig.EmpresaTrabalhaAlturaLargura)
            {
                var coluna = grdProdutoCotacaoCompra.Columns[4];
                grdProdutoCotacaoCompra.Columns.RemoveAt(4);
                grdProdutoCotacaoCompra.Columns.Insert(3, coluna);
            }
        }
    
        protected void dtvCotacaoCompra_ItemCommand(object sender, DetailsViewCommandEventArgs e)
        {
            if (e.CommandName == "Finalizar")
            {
                try
                {
                    int tipo = Glass.Conversoes.StrParaInt((dtvCotacaoCompra.FooterRow.FindControl("drpTipoCalculo") as DropDownList).SelectedValue);
                    FinalizarCotacaoCompra.Instance.Finalizar(Glass.Conversoes.StrParaUint(e.CommandArgument as string), 
                        (Glass.Data.Model.CotacaoCompra.TipoCalculoCotacao)tipo);
    
                    btnVoltar_Click(sender, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao finalizar cotação.", ex, Page);
                }
            }
            else
                ExibirItens(e.CommandName != "Edit");
        }
    
        private void ExibirItens(bool exibir)
        {
            separador1.Visible = exibir;
            separador2.Visible = exibir;
            separadorProdutos.Visible = exibir;
            tituloProdutos.Visible = exibir;
            produtos.Visible = exibir;
            fornecedorProduto.Visible = exibir;
            grdProdutoCotacaoCompra.Visible = exibir;
        }
    
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Listas/LstCotacaoCompra.aspx");
        }
    
        protected void odsCotacaoCompra_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir cotação de compra.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
                Response.Redirect("CadCotacaoCompra.aspx?id=" + e.ReturnValue);
        }
    
        protected void odsCotacaoCompra_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar cotação de compra.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
    
        protected void grdProdutoCotacaoCompra_DataBound(object sender, EventArgs e)
        {
            grdProdutoCotacaoCompra.Rows[0].Visible = grdProdutoCotacaoCompra.Rows.Count > 1 ||
                CRUD.Instance.ObtemNumeroRealProdutosCotacaoCompra(Glass.Conversoes.StrParaUint(Request["id"])) > 0;
        }
    
        protected void imgAdd_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                ProdutoCotacaoCompra p = new ProdutoCotacaoCompra()
                {
                    CodigoCotacaoCompra = Glass.Conversoes.StrParaUint(Request["id"]),
                    CodigoProduto = (grdProdutoCotacaoCompra.FooterRow.FindControl("ctrlSelProduto1") as Glass.UI.Web.Controls.ctrlSelProduto).IdProd.Value,
                    Altura = Glass.Conversoes.StrParaFloat((grdProdutoCotacaoCompra.FooterRow.FindControl("txtAltura") as TextBox).Text),
                    Largura = Glass.Conversoes.StrParaInt((grdProdutoCotacaoCompra.FooterRow.FindControl("txtLargura") as TextBox).Text),
                    Quantidade = Glass.Conversoes.StrParaFloat((grdProdutoCotacaoCompra.FooterRow.FindControl("txtQtde") as TextBox).Text)
                };
    
                CRUD.Instance.InserirProdutoCotacaoCompra(p);
                
                grdProdutoCotacaoCompra.DataBind();
                grdProdutoFornecedorCotacaoCompra.DataBind();
    
                drpProduto.DataBind();
                drpFornecedor.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir produto.", ex, Page);
            }
        }
    
        protected void grdProdutoCotacaoCompra_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdProdutoCotacaoCompra.ShowFooter = e.CommandName != "Edit";
        }
    
        protected void grdProdutoFornecedorCotacaoCompra_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow || e.Row.DataItem == null)
                return;
    
            var pfcc = e.Row.DataItem as ProdutoFornecedorCotacaoCompra;
            if (!pfcc.Cadastrado)
                foreach (TableCell c in e.Row.Cells)
                {
                    c.ForeColor = System.Drawing.Color.Silver;
                    foreach (Control c1 in c.Controls)
                        if (c1 is WebControl)
                            (c1 as WebControl).Enabled = false;
                }
    
            CheckBox chkHabilitar = e.Row.FindControl("chkHabilitar") as CheckBox;
            chkHabilitar.Enabled = true;
            chkHabilitar.Attributes.Add("onclick", String.Format("habilitar({0}, {1}, {2}, this)", 
                pfcc.CodigoCotacaoCompra, pfcc.CodigoProduto, pfcc.CodigoFornecedor));
        }
    
        protected void grdProdutoCotacaoCompra_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
            grdProdutoFornecedorCotacaoCompra.DataBind();
            drpProduto.DataBind();
            drpFornecedor.DataBind();
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
    
        }
    
        protected void ctrlParcelas_Load(object sender, EventArgs e)
        {
            var ctrl = sender as Glass.UI.Web.Controls.ctrlParcelasSelecionar;
            ctrl.CampoFornecedorID = ctrl.Parent.Parent.FindControl("hdfCodigoFornecedor");
            ctrl.ControleParcelas = ctrl.Parent.Parent.FindControl("ctrlParcelasCustom") as Glass.UI.Web.Controls.ctrlParcelas;
            ctrl.NumeroParcelasMaximo = FinanceiroConfig.Compra.NumeroParcelasCompra;
        }
    
        protected void ctrlParcelasCustom_Load(object sender, EventArgs e)
        {
            var ctrl = sender as Glass.UI.Web.Controls.ctrlParcelas;
            ctrl.CampoCalcularParcelas = ctrl.Parent.Parent.FindControl("hdfCalcularParcelas");
            ctrl.CampoDataBase = ctrl.Parent.Parent.FindControl("hdfDataBase");
    
            (ctrl.CampoDataBase as HiddenField).Value = DateTime.Now.ToString("dd/MM/yyyy");
        }
    
        protected void hdfNumParcFornec_Load(object sender, EventArgs e)
        {
            HiddenField fornec = (sender as HiddenField).Parent.Parent.FindControl("hdfCodigoFornecedor") as HiddenField;
            if (!String.IsNullOrEmpty(fornec.Value))
                (sender as HiddenField).Value = ParcelasDAO.Instance.GetNumParcByFornecedor(Glass.Conversoes.StrParaUint(fornec.Value)).ToString();
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
    
        [Ajax.AjaxMethod]
        public string Habilitar(string idCotacaoCompra, string idProd, string idFornec, string custo, 
            string prazo, string idParcela, string parcelasConfiguradas, string habilitar)
        {
            return WebGlass.Business.CotacaoCompra.Fluxo.CRUD.Ajax.Habilitar(idCotacaoCompra, idProd, idFornec,
                custo, prazo, idParcela, parcelasConfiguradas, habilitar);
        }
    }
}
