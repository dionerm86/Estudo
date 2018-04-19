using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadPedidoInterno : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadPedidoInterno));
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));

            if (!IsPostBack)
            {
                if (Request["idPedidoInterno"] != null)
                    dtvPedidoInterno.ChangeMode(DetailsViewMode.ReadOnly);
                else
                {
                    ((Label) dtvPedidoInterno.FindControl("lblData")).Text = DateTime.Now.ToString("dd/MM/yyyy");
                    ((Label) dtvPedidoInterno.FindControl("lblSituacao")).Text = "Aberto";

                    if ((Label) dtvPedidoInterno.FindControl("lblFuncionarioCad") != null)
                        ((Label) dtvPedidoInterno.FindControl("lblFuncionarioCad")).Text = UserInfo.GetUserInfo.Nome;

                    ((HiddenField) dtvPedidoInterno.FindControl("hdfSituacao")).Value =
                        ((int) SituacaoPedidoInt.Aberto).ToString();

                    ((DropDownList) dtvPedidoInterno.FindControl("drpFuncionario")).Enabled = EstoqueConfig.PermitirAlterarFuncionarioPedidoInterno;

                    ((DropDownList) dtvPedidoInterno.FindControl("drpFuncionario")).SelectedValue = UserInfo.GetUserInfo.CodUser.ToString();
                }
            }

            if (!Configuracoes.FiscalConfig.UsarControleCentroCusto)
            {
                if (((Label)dtvPedidoInterno.FindControl("lblCentroCusto")) != null)
                    ((Label)dtvPedidoInterno.FindControl("lblCentroCusto")).Visible = false;

                if (((Label)dtvPedidoInterno.FindControl("lblDescricaoCentroCusto")) != null)
                    ((Label)dtvPedidoInterno.FindControl("lblDescricaoCentroCusto")).Visible = false;

                if (((DropDownList)dtvPedidoInterno.FindControl("ddlCentroCusto")) != null)
                    ((DropDownList)dtvPedidoInterno.FindControl("ddlCentroCusto")).Visible = false;
            }

            grdProdutos.Visible = dtvPedidoInterno.CurrentMode == DetailsViewMode.ReadOnly;
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            if (dtvPedidoInterno.CurrentMode == DetailsViewMode.Edit)
                Response.Redirect(Request.Url.ToString());
            else
                Response.Redirect("../Listas/LstPedidoInterno.aspx" + (Request["producao"] == "1" ? "?producao=1&popup=true" : ""));
        }
    
        protected void dtvPedidoInterno_ItemCommand(object sender, DetailsViewCommandEventArgs e)
        {
            if (e.CommandName == "Finalizar")
            {
                try
                {
                    uint idPedidoInterno = Glass.Conversoes.StrParaUint(e.CommandArgument.ToString());
                    PedidoInternoDAO.Instance.Finalizar(idPedidoInterno);
                    Response.Redirect("../Listas/LstPedidoInterno.aspx" + (Request["producao"] == "1" ? "?producao=1&popup=true" : ""));
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao finalizar o pedido.", ex, Page);
                }
            }

            grdProdutos.Visible = e.CommandName != "Edit";
        }

        protected void odsPedidoInterno_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception == null)
            {
                var url = Request.Url.ToString() + (Request.Url.ToString().Contains("producao") ? "&" : "?") + "idPedidoInterno=" + e.ReturnValue.ToString();

                Response.Redirect(url);
            }
            else
            {
                e.ExceptionHandled = true;
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir o pedido interno.", e.Exception, Page);
            }
        }

        protected void odsPedidoInterno_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception == null)
                Response.Redirect(Request.Url.ToString());
            else
            {
                e.ExceptionHandled = true;
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar o pedido interno.", e.Exception, Page);
            }
        }

        protected void grdProdutos_DataBound(object sender, EventArgs e)
        {
            if (grdProdutos.Rows.Count == 1)
            {
                if (ProdutoPedidoInternoDAO.Instance.GetCountReal(Glass.Conversoes.StrParaUint(Request["idPedidoInterno"])) == 0)
                    grdProdutos.Rows[0].Visible = false;
            }
        }

        #region Inserir produto

        protected void imgAdd_Click(object sender, ImageClickEventArgs e)
        {
            string idProd = ((HiddenField)grdProdutos.FooterRow.FindControl("hdfIdProd")).Value;
            string altura = ((TextBox)grdProdutos.FooterRow.FindControl("txtAltura")).Text;
            string largura = ((TextBox)grdProdutos.FooterRow.FindControl("txtLargura")).Text;
            string qtde = ((TextBox)grdProdutos.FooterRow.FindControl("txtQtde")).Text;
            string obs = ((TextBox)grdProdutos.FooterRow.FindControl("txtObs")).Text;

            ProdutoPedidoInterno novo = new ProdutoPedidoInterno();
            novo.IdPedidoInterno = Glass.Conversoes.StrParaUint(Request["idPedidoInterno"]);
            novo.IdProd = Glass.Conversoes.StrParaUint(idProd);
            novo.Altura = Glass.Conversoes.StrParaFloat(altura);
            novo.Largura = !String.IsNullOrEmpty(largura) ? Glass.Conversoes.StrParaInt(largura) : 0;
            novo.Qtde = Glass.Conversoes.StrParaFloat(qtde);
            novo.Observacao = obs;

            ProdutoPedidoInternoDAO.Instance.Insert(novo);
            grdProdutos.DataBind();
        }

        #endregion

        #region Métodos Ajax

        [Ajax.AjaxMethod]
        public string GetProduto(string codInterno)
        {
            return WebGlass.Business.Produto.Fluxo.BuscarEValidar.Ajax.GetProdutoInterno(codInterno);
        }

        #endregion
    }
}
