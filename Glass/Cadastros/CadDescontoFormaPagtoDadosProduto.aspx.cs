using Glass.Data.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadDescontoFormaPagtoDadosProduto : System.Web.UI.Page
    {
        private uint? idDescontoFormaPagamentoDadosProduto;

        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadDescontoFormaPagtoDadosProduto));

            idDescontoFormaPagamentoDadosProduto = Conversoes.StrParaUintNullable(Request["idDescontoFormaPagamentoDadosProduto"]);

            if (!IsPostBack)
            {
                if (idDescontoFormaPagamentoDadosProduto != null)
                {
                    dtvDescontoFormaPagtoDadosProduto.ChangeMode(DetailsViewMode.Edit);
                    var tipoVenda = ((DropDownList)dtvDescontoFormaPagtoDadosProduto.FindControl("drpTipoVenda")).SelectedValue.StrParaInt();

                    // Caso o tipo de venda seja à vista e o controle de desconto por forma de pagamento e dados do produto esteja habilitado, esconde somente as parcelas.
                    if (Configuracoes.FinanceiroConfig.UsarControleDescontoFormaPagamentoDadosProduto && tipoVenda == 1)
                    {
                        ((DropDownList)dtvDescontoFormaPagtoDadosProduto.FindControl("drpParcelas")).Style.Add("display", "none");
                        ((DropDownList)dtvDescontoFormaPagtoDadosProduto.FindControl("drpFormaPagto")).Style.Add("display", string.Empty);
                        ((DropDownList)dtvDescontoFormaPagtoDadosProduto.FindControl("drpTipoCartao")).Style.Add("display", string.Empty);
                    }
                    // Caso o tipo de venda seja obra ou funcionário, esconde as parcelas, forma de pagamento e tipo de cartão.
                    else if (tipoVenda == 1 || tipoVenda == 5 || tipoVenda == 6)
                    {
                        ((DropDownList)dtvDescontoFormaPagtoDadosProduto.FindControl("drpParcelas")).Style.Add("display", "none");
                        ((DropDownList)dtvDescontoFormaPagtoDadosProduto.FindControl("drpFormaPagto")).Style.Add("display", "none");
                        ((DropDownList)dtvDescontoFormaPagtoDadosProduto.FindControl("drpTipoCartao")).Style.Add("display", "none");
                    }
                    else
                    {
                        ((DropDownList)dtvDescontoFormaPagtoDadosProduto.FindControl("drpParcelas")).Style.Add("display", string.Empty);
                        ((DropDownList)dtvDescontoFormaPagtoDadosProduto.FindControl("drpFormaPagto")).Style.Add("display", string.Empty);
                        ((DropDownList)dtvDescontoFormaPagtoDadosProduto.FindControl("drpTipoCartao")).Style.Add("display", string.Empty);
                    }
                }
            }
        }

        protected void drpGrupoProd_DataBound(object sender, EventArgs e)
        {
            DropDownList drpGrupo = (DropDownList)sender;
            DropDownList drpSubgrupo = dtvDescontoFormaPagtoDadosProduto.FindControl("drpSubgrupo") as DropDownList;

            var grupoProdutoFluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<Glass.Global.Negocios.IGrupoProdutoFluxo>();

            foreach (var s in grupoProdutoFluxo.ObtemSubgruposProduto(drpGrupo.SelectedValue.StrParaInt()))
                drpSubgrupo.Items.Add(new ListItem(s.Name, s.Id.ToString()));

            if (drpSubgrupo.Items.Count > 0 && drpSubgrupo.Items[0].Value != "")
                drpSubgrupo.Items.Insert(0, new ListItem());

            if (idDescontoFormaPagamentoDadosProduto != null)
            {
                var idGrupoProd = DescontoFormaPagamentoDadosProdutoDAO.Instance.ObtemIdGrupoProd((uint)idDescontoFormaPagamentoDadosProduto);
                ((DropDownList)dtvDescontoFormaPagtoDadosProduto.Rows[dtvDescontoFormaPagtoDadosProduto.PageIndex].FindControl("drpGrupoProd")).SelectedValue = idGrupoProd != null ? idGrupoProd.ToString() : "";

                var idSubgrupoProd = DescontoFormaPagamentoDadosProdutoDAO.Instance.ObtemIdSubgrupoProd((uint)idDescontoFormaPagamentoDadosProduto);
                ((DropDownList)dtvDescontoFormaPagtoDadosProduto.Rows[dtvDescontoFormaPagtoDadosProduto.PageIndex].FindControl("drpSubgrupo")).SelectedValue = idSubgrupoProd != null ? idSubgrupoProd.ToString() : "";
            }
        }

        #region Métodos Ajax

        [Ajax.AjaxMethod]
        public string GetSubgrupos(string idGrupo)
        {
            return WebGlass.Business.SubgrupoProd.Fluxo.BuscarEValidar.Ajax.GetSubgrupos(idGrupo);
        }

        /// <summary>
        /// Retorna o código que reprensenta a forma de pagamento "Cartao"
        /// </summary>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string GetCartaoCod()
        {
            return ((int)Glass.Data.Model.Pagto.FormaPagto.Cartao).ToString();
        }

        [Ajax.AjaxMethod]
        public string LoadAjax(string tipo, string tipoVendaStr)
        {
            return WebGlass.Business.Pedido.Fluxo.DadosPedido.Ajax.LoadAjax(tipo, null, tipoVendaStr);
        }

        #endregion

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Listas/LstDescontoFormaPagtoDadosProduto.aspx");
        }

        protected void dtvDescontoFormaPagtoDadosProduto_ItemInserted(object sender, DetailsViewInsertedEventArgs e)
        {
            if (e.Exception == null)
                btnCancelar_Click(sender, e);
            else
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao cadastrar Desconto por Forma de Pagamento e Dados do Produto.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }

        protected void dtvDescontoFormaPagtoDadosProduto_ItemUpdated(object sender, DetailsViewUpdatedEventArgs e)
        {
            if (e.Exception == null)
                btnCancelar_Click(sender, e);
            else
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao cadastrar Desconto por Forma de Pagamento e Dados do Produto.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
    }
}