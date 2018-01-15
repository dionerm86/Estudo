using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Data.Helper;
using Glass.Configuracoes;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadCompraPcp : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadCompraPcp));
            Page.ClientScript.RegisterOnSubmitStatement(GetType(), "this_submit", "setDadosProdutos();\n");
        }
    
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                WebGlass.Business.PedidoEspelho.Fluxo.BuscarEValidar.Instance.BuscarCompraPcp(Glass.Conversoes.StrParaUint(txtIdPedido.Text));
    
                cadastro.Visible = true;
            }
            catch (Exception ex)
            {
                cadastro.Visible = false;
                Glass.MensagemAlerta.ShowMsg(Glass.MensagemAlerta.FormatErrorMsg("Erro ao buscar pedido", ex), Page);
            }
        }
    
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Listas/LstCompraPcp.aspx");
        }
    
        protected void hdfIdPedidoEspelho_Load(object sender, EventArgs e)
        {
            ((HiddenField)sender).Value = txtIdPedido.Text;
        }
    
        protected void odsCompra_Inserting(object sender, Colosoft.WebControls.VirtualObjectDataSourceMethodEventArgs e)
        {
            if (hdfDadosProdutos.Value.Length == 0)
            {
                e.Cancel = true;
                return;
            }
    
            var compra = (Glass.Data.Model.Compra)e.InputParameters[0];
            compra.IdLoja = FinanceiroConfig.FinanceiroPagto.CompraLojaPadrao > 0 ? 
                FinanceiroConfig.FinanceiroPagto.CompraLojaPadrao.Value : UserInfo.GetUserInfo.IdLoja;
            compra.IdFormaPagto = (uint)Glass.Data.Model.Pagto.FormaPagto.Boleto;
            compra.TipoCompra = (int)Glass.Data.Model.Compra.TipoCompraEnum.APrazo;
            compra.DataFabrica = PedidoEspelhoDAO.Instance.ObtemDataFabrica(null, Conversoes.StrParaUint(txtIdPedido.Text));

            compra.Nf = string.IsNullOrEmpty(txtIdPedido.Text) ? string.Empty : txtIdPedido.Text;
            compra.IdPedidoEspelho = string.IsNullOrEmpty(txtIdPedido.Text) ? null : txtIdPedido.Text.StrParaUintNullable();
            compra.TipoCompra = (int)Compra.TipoCompraEnum.AVista;
        }
    
        protected void odsCompra_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception == null)
            {
                try
                {
                    WebGlass.Business.Compra.Fluxo.Compra.Instance.CompraPcpInserted(Glass.Conversoes.StrParaUint(e.ReturnValue.ToString()), hdfDadosProdutos.Value.Trim(' ', '|').Split('|'));
    
                    if (Glass.Configuracoes.CompraConfig.TelaCadastroPcp.PerguntarContinuarTelaAoCadastrar)
                    {
                        Page.ClientScript.RegisterStartupScript(GetType(), "pergunta", @"
                        if (!confirm('Compra gerada com sucesso! Número da compra: " + e.ReturnValue + @"\nDeseja continuar nesta tela?'))
                            redirectUrl('" + this.ResolveClientUrl("~/Listas/LstCompraPcp.aspx") + "');\n", true);
                    }
                    else 
                        Response.Redirect("~/Cadastros/CadCompra.aspx?idCompra=" + e.ReturnValue + "&pcp=1");
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar compra.", ex, Page);
                }
            }
            else
            {
                Glass.MensagemAlerta.ErrorMsg("Erro ao inserir conta.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
    
        [Ajax.AjaxMethod]
        public string GetPlanoConta(string idFornecedor)
        {
            uint id = !String.IsNullOrEmpty(idFornecedor) ? Glass.Conversoes.StrParaUint(idFornecedor) : 0;
            if (id == 0)
                return "";
    
            uint? idConta = FornecedorDAO.Instance.ObtemIdConta(null, id);
            return idConta != null ? idConta.ToString() : String.Empty;
        }
    
        protected void grdBenef_PreRender(object sender, EventArgs e)
        {
            ((GridView)sender).DataBind();
        }
    
        protected void chkBenef_PreRender(object sender, EventArgs e)
        {
            CheckBox chkBenef = (CheckBox)sender;
            HiddenField hdfIdBenef = chkBenef.Parent.FindControl("hdfIdBenef") as HiddenField;
            HiddenField hdfIdProdPed = chkBenef.Parent.Parent.Parent.Parent.Parent.FindControl("hdfIdProdPed") as HiddenField;
            HiddenField hdfQtde = chkBenef.Parent.Parent.Parent.Parent.Parent.FindControl("hdfQtde") as HiddenField;
            HiddenField hdfQtdeBenef = chkBenef.Parent.FindControl("hdfQtdeBenef") as HiddenField;
    
            uint? idBenef = hdfIdBenef != null ? Glass.Conversoes.StrParaUintNullable(hdfIdBenef.Value) : null;
            uint? idProdPed = hdfIdProdPed != null ? Glass.Conversoes.StrParaUintNullable(hdfIdProdPed.Value) : null;
            int qtde = Glass.Conversoes.StrParaInt(hdfQtde.Value);
    
            int qtdeFazer = WebGlass.Business.Compra.Fluxo.Compra.Instance.QuantidadeBenefCompraPcp(idBenef, idProdPed, qtde);
    
            if (hdfQtdeBenef != null)
                hdfQtdeBenef.Value = qtdeFazer.ToString();
    
            chkBenef.Visible = qtdeFazer > 0;
        }
    
        protected void chkNaoCobrarVidro_DataBinding(object sender, EventArgs e)
        {
            CheckBox chkNaoCobrarVidro = (CheckBox)sender;
            GridViewRow linha = chkNaoCobrarVidro.Parent.Parent as GridViewRow;
    
            if (linha == null)
                return;
    
            ProdutosPedidoEspelho produto = linha.DataItem as ProdutosPedidoEspelho;
    
            if (produto == null)
                return;
    
            int qtdeJaFeita = ProdutosCompraBenefDAO.Instance.GetCountByProdPedBenef(produto.IdProdPed, 0);
            if (produto.Beneficiamentos.Count == 0 || qtdeJaFeita == produto.Beneficiamentos.NumeroBeneficiamentos)
            {
                chkNaoCobrarVidro.Enabled = false;
                chkNaoCobrarVidro.Checked = false;
            }
            else if (produto.QtdeComprar == 0)
            {
                chkNaoCobrarVidro.Enabled = false;
                chkNaoCobrarVidro.Checked = true;
            }
        }
    
        [Ajax.AjaxMethod]
        public string GetAmbientes(string idPedidoStr)
        {
            return WebGlass.Business.AmbientePedidoEspelho.Fluxo.BuscarEValidar.Ajax.GetAmbientesCompraPcp(idPedidoStr);
        }
    
        protected void grdProdutosPedido_Load(object sender, EventArgs e)
        {
            ((GridView)sender).Visible = !PedidoConfig.DadosPedido.AmbientePedido;
        }
    }
}
