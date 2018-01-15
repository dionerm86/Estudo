using System;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadProdutoOrcamento : System.Web.UI.Page
    {
        private string _produtos = null;
    
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                ((TextBox)dtvLoja.FindControl("txtDescr")).Attributes.Add("onchange", 
                    "mudarDescr = " + AtualizarDescricaoPaiAoInserirOuRemoverProduto().ToString().ToLower() + " ? this.value.length == 0 : true");
        }
    
        protected void odsProdOrc_Inserting(object sender, Colosoft.WebControls.VirtualObjectDataSourceMethodEventArgs e)
        {
            _produtos = ((ProdutosOrcamento)e.InputParameters[0]).DadosProdutos;
        }
    
        protected void odsProdOrc_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao incluir Produto no Orçamento.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
            {
                if (!String.IsNullOrEmpty(_produtos))
                {
                    _produtos = _produtos.Replace("\r\n", "\\n' +\r\n'").Replace("\t", "\\t");
                    ClientScript.RegisterStartupScript(this.GetType(), "cadastrarProdutos", "cadastrarProdutos('" + e.ReturnValue.ToString() + "', '" + _produtos + "', null)", true);
                }
                else
                    ClientScript.RegisterClientScriptBlock(this.GetType(), "ok", "window.opener.redirectUrl(window.opener.location.href + '&atualizar=1'); closeWindow();", true);
            }
        }
    
        protected void lkbInserir_Load(object sender, EventArgs e)
        {
            ((LinkButton)sender).Visible = OrcamentoConfig.ItensProdutos.ItensProdutosOrcamento;
        }
    
        protected void btnInserir_Load(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(Request["editar"]))
            {
                ((Button)sender).Text = "Atualizar";
                ((Button)sender).CommandName = "Atualizar";
            }
        }
    
        protected void dtvLoja_DataBound(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(Request["idProd"]))
            {
                ProdutosOrcamento prod = ProdutosOrcamentoDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(Request["idProd"]));
                ((TextBox)dtvLoja.FindControl("txtAmbiente")).Text = prod.Ambiente;
    
                if (!String.IsNullOrEmpty(Request["editar"]))
                {
                    if (prod.Qtde == null)
                        prod.Qtde = 1;
    
                    ((TextBox)dtvLoja.FindControl("txtDescr")).Text = prod.Descricao;
                    ((TextBox)dtvLoja.FindControl("txtQtd")).Text = prod.Qtde.Value.ToString();
                    ((Glass.UI.Web.Controls.ctrlTextBoxFloat)dtvLoja.FindControl("ctrlTextBoxFloat1")).Value = prod.ValorProd.Value.ToString("0.#0");
                }
                
                if (!String.IsNullOrEmpty(Request["idProd"]))
                    ((HiddenField)dtvLoja.FindControl("hdfNumSeq")).Value = ProdutosOrcamentoDAO.Instance.GetNumSeq(Glass.Conversoes.StrParaUint(Request["idProd"])).ToString();
            }
        }
    
        protected void dtvLoja_ItemCommand(object sender, DetailsViewCommandEventArgs e)
        {
            if (e.CommandName == "Atualizar")
            {
                ProdutosOrcamento prod = ProdutosOrcamentoDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(Request["idProd"]));
                prod.Ambiente = ((TextBox)dtvLoja.FindControl("txtAmbiente")).Text;
                prod.Descricao = ((TextBox)dtvLoja.FindControl("txtDescr")).Text;
                prod.Qtde = Glass.Conversoes.StrParaInt(((TextBox)dtvLoja.FindControl("txtQtd")).Text);
                prod.ValorProd = decimal.Parse(((Glass.UI.Web.Controls.ctrlTextBoxFloat)dtvLoja.FindControl("ctrlTextBoxFloat1")).Value.Replace('.', ','));
                prod.NumSeq = Glass.Conversoes.StrParaUint(((HiddenField)dtvLoja.FindControl("hdfNumSeq")).Value);
    
                ProdutosOrcamentoDAO.Instance.UpdateComTransacao(prod);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "ok", "window.opener.redirectUrl(window.opener.location.href + '&atualizar=1'); closeWindow();", true);
            }
        }
    
        protected string GetPercComissao()
        {
            uint idOrca = Glass.Conversoes.StrParaUint(Request["idOrca"]);
            return PedidoConfig.Comissao.ComissaoAlteraValor ? OrcamentoDAO.Instance.RecuperaPercComissao(idOrca).ToString() : "0";
        }
    
        protected string GetRevenda()
        {
            uint? idCliente = OrcamentoDAO.Instance.ObtemIdCliente(Glass.Conversoes.StrParaUint(Request["idOrca"]));
    
            return idCliente > 0 && ClienteDAO.Instance.IsRevenda(idCliente.Value) ? "1" : "0";
        }
    
        protected void ddlAmbiente_Load(object sender, EventArgs e)
        {
            ((DropDownList)sender).Visible = !String.IsNullOrEmpty(Request["idAmbiente"]);
    
            if (((DropDownList)sender).Visible)
            {
                uint idAmbienteOrca = Glass.Conversoes.StrParaUint(Request["idAmbiente"]);
                ((DropDownList)sender).Items.Add(new ListItem(AmbienteOrcamentoDAO.Instance.ObtemAmbiente(idAmbienteOrca), idAmbienteOrca.ToString()));
            }
        }
    
        protected void txtAmbiente_Load(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(Request["idAmbiente"]))
            {
                ((TextBox)sender).Style.Add("display", "none");
                ((TextBox)sender).Text = AmbienteOrcamentoDAO.Instance.ObtemAmbiente(Glass.Conversoes.StrParaUint(Request["idAmbiente"]));
            }
        }
    
        protected bool AtualizarDescricaoPaiAoInserirOuRemoverProduto()
        {
            return OrcamentoConfig.TelaCadastroProdutoOrcamento.AtualizarDescricaoPaiAoInserirOuRemoverProduto;
        }
    }
}
