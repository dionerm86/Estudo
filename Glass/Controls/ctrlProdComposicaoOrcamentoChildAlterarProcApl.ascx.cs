using Glass.Configuracoes;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Data.Model;
using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlProdComposicaoOrcamentoChildAlterarProcApl : UserControl
    {
        #region Propiedades

        public object IdProdOrcamento
        {
            get
            {
                return hdfChildIdProdOrcamento.Value.StrParaInt();
            }
            set
            {
                hdfChildIdProdOrcamento.Value = value.ToString();
            }
        }

        #endregion

        #region Metodos Protegidos

        protected void Page_Load(object sender, EventArgs e)
        {
            // Se a empresa não possuir acesso ao módulo PCP, esconde colunas Apl e Proc
            if (!Geral.ControlePCP)
            {
                grdProdutosOrcamentoComposicao.Columns[9].Visible = false;
                grdProdutosOrcamentoComposicao.Columns[10].Visible = false;
            }
        }

        private void BindControls()
        {
            var grdProdutosOrcamentoComposicaoParent = grdProdutosOrcamentoComposicao.Parent;

            while (grdProdutosOrcamentoComposicaoParent.ID != "grdProdutosOrcamento")
            {
                grdProdutosOrcamentoComposicaoParent = grdProdutosOrcamentoComposicaoParent.Parent;
            }

            var mainTable = grdProdutosOrcamentoComposicaoParent.Parent;

            while (mainTable.ID != "mainTable")
            {
                mainTable = mainTable.Parent;
            }

            var dtvOrcamento = mainTable.FindControl("dtvOrcamento");
            var grdProdutosAmbiente = mainTable.FindControl("grdProdutosAmbienteOrcamento");

            ((DetailsView)dtvOrcamento).DataBind();
            ((GridView)grdProdutosOrcamentoComposicaoParent).DataBind();
            ((GridView)grdProdutosAmbiente).DataBind();
        }

        protected string NomeControleBenefComposicao()
        {
            return grdProdutosOrcamentoComposicao.EditIndex == -1 ? "ctrlChildBenefInserirComposicao" : "ctrlChildBenefEditarComposicao";
        }

        protected void ctrlChildBenef_Load(object sender, EventArgs e)
        {
            ctrlBenef benef = (ctrlBenef)sender;
            GridViewRow linhaControle = benef.Parent.Parent as GridViewRow;
            Control dtvOrcamento = linhaControle.Parent;

            while (dtvOrcamento.ID != "mainTable")
            {
                dtvOrcamento = dtvOrcamento.Parent;
            }

            dtvOrcamento = dtvOrcamento.FindControl("dtvOrcamento");

            Control codProd = null;

            if (linhaControle.FindControl("lblChildCodProdComposicaoIns") != null)
            {
                codProd = linhaControle.FindControl("lblChildCodProdComposicaoIns");
            }
            else
            {
                codProd = linhaControle.FindControl("txtChildCodProdComposicaoIns");
            }

            TextBox txtAltura = (TextBox)linhaControle.FindControl("txtChildAlturaComposicaoIns");
            TextBox txtEspessura = (TextBox)linhaControle.FindControl("txtChildEspessuraComposicao");
            TextBox txtLargura = (TextBox)linhaControle.FindControl("txtChildLarguraComposicaoIns");
            HiddenField hdfPercComissao = (HiddenField)dtvOrcamento.FindControl("hdfPercComissao");
            TextBox txtQuantidade = (TextBox)linhaControle.FindControl("txtChildQtdeComposicaoIns");
            HiddenField hdfTipoEntrega = (HiddenField)dtvOrcamento.FindControl("hdfTipoEntrega");
            HiddenField hdfTotalM2 = null;

            if (!Beneficiamentos.UsarM2CalcBeneficiamentos)
            {
                if (linhaControle.FindControl("hdfChildTotMComposicao") != null)
                {
                    hdfTotalM2 = (HiddenField)linhaControle.FindControl("hdfChildTotMComposicao");
                }
                else if (linhaControle.FindControl("hdfChildTotM2ComposicaoIns") != null)
                {
                    hdfTotalM2 = (HiddenField)linhaControle.FindControl("hdfChildTotM2ComposicaoIns");
                }
            }
            else
            {
                if (linhaControle.FindControl("hdfChildTotM2CalcComposicao") != null)
                {
                    hdfTotalM2 = (HiddenField)linhaControle.FindControl("hdfChildTotM2CalcComposicao");
                }
                else if (linhaControle.FindControl("hdfChildTotM2CalcComposicaoIns") != null)
                {
                    hdfTotalM2 = (HiddenField)linhaControle.FindControl("hdfChildTotM2CalcComposicaoIns");
                }
            }

            TextBox txtValorIns = (TextBox)linhaControle.FindControl("txtChildValorComposicaoIns");
            HiddenField hdfCliRevenda = (HiddenField)dtvOrcamento.FindControl("hdfCliRevenda");
            HiddenField hdfIdCliente = (HiddenField)dtvOrcamento.FindControl("hdfIdCliente");
            HiddenField hdfCustoProd = (HiddenField)linhaControle.FindControl("hdfChildCustoProdComposicao");

            benef.CampoAltura = txtAltura;
            benef.CampoEspessura = txtEspessura;
            benef.CampoLargura = txtLargura;
            benef.CampoPercComissao = hdfPercComissao;
            benef.CampoQuantidade = txtQuantidade;
            benef.CampoQuantidadeAmbiente = null;
            benef.CampoTipoEntrega = hdfTipoEntrega;
            benef.CampoTotalM2 = hdfTotalM2;
            benef.CampoValorUnitario = txtValorIns;
            benef.CampoCusto = hdfCustoProd;
            benef.CampoProdutoID = codProd;
            benef.CampoRevenda = hdfCliRevenda;
            benef.CampoClienteID = hdfIdCliente;
            benef.CampoAplicacaoID = linhaControle.FindControl("hdfChildIdAplicacaoComposicao");
            benef.CampoProcessoID = linhaControle.FindControl("hdfChildIdProcessoComposicao");
            benef.CampoAplicacao = linhaControle.FindControl("txtChildAplComposicaoIns");
            benef.CampoProcesso = linhaControle.FindControl("txtChildProcComposicaoIns");
            benef.TipoBenef = TipoBenef.Venda;
        }

        protected void txtChildEspessuraComposicao_DataBinding(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            GridViewRow linhaControle = txt.Parent.Parent as GridViewRow;

            var produtoOrcamento = linhaControle.DataItem as ProdutosOrcamento;
            txt.Enabled = produtoOrcamento.Espessura <= 0;
        }

        protected bool UtilizarRoteiroProducao()
        {
            return PCPConfig.ControlarProducao && Data.Helper.Utils.GetSetores.Count(x => x.SetorPertenceARoteiro) > 0;
        }

        #region odsProdutosOrcamentoComposicao

        protected void odsProdutosOrcamentoComposicao_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                MensagemAlerta.ErrorMsg(null, e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }

        #endregion

        #region grdProdutosOrcamentoComposicao

        protected void grdProdutosOrcamentoComposicao_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdProdutosOrcamentoComposicao.ShowFooter = e.CommandName != "Edit";
        }

        protected void grdProdutosOrcamentoComposicao_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
            BindControls();
        }

        #endregion

        #endregion
    }
}