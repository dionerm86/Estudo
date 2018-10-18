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
    public partial class ctrlProdComposicaoOrcamentoChild : UserControl
    {
        #region Propiedades

        public object IdProdOrcamento
        {
            get
            {
                return hdfChildIdProdOrcamento.Value.StrParaUint();
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
            var grdAmbiente = mainTable.FindControl("grdProdutosAmbienteOrcamento");

            ((DetailsView)dtvOrcamento).DataBind();
            ((GridView)grdProdutosOrcamentoComposicaoParent).DataBind();
            ((GridView)grdAmbiente).DataBind();
        }

        protected string NomeControleBenefComposicao()
        {
            return grdProdutosOrcamentoComposicao.EditIndex == -1 ? "ctrlChildBenefInserirComposicao" : "ctrlChildBenefEditarComposicao";
        }

        protected void ctrlBenef_Load(object sender, EventArgs e)
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

        protected void ctrlDescontoQtdeComposicao_Load(object sender, EventArgs e)
        {
            ctrlDescontoQtde desc = (ctrlDescontoQtde)sender;
            GridViewRow linha = desc.Parent.Parent as GridViewRow;
            Control dtvOrcamento = linha.Parent;

            while (dtvOrcamento.ID != "mainTable")
            {
                dtvOrcamento = dtvOrcamento.Parent;
            }

            dtvOrcamento = dtvOrcamento.FindControl("dtvOrcamento");

            desc.CampoQtde = linha.FindControl("txtQtdeIns");
            desc.CampoProdutoID = linha.FindControl("hdfIdProduto");
            desc.CampoClienteID = dtvOrcamento.FindControl("hdfIdCliente");
            desc.CampoTipoEntrega = dtvOrcamento.FindControl("hdfTipoEntrega");
            desc.CampoRevenda = dtvOrcamento.FindControl("hdfCliRevenda");
            desc.CampoValorUnit = linha.FindControl("txtValorIns");

            if (desc.CampoProdutoID == null)
            {
                desc.CampoProdutoID = hdfChildIdProdutoComposicao;
            }
        }

        protected void txtChildValorInsComposicao_Load(object sender, EventArgs e)
        {
            ((TextBox)sender).Enabled = PedidoConfig.DadosPedido.AlterarValorUnitarioProduto;
        }

        protected void txtChildEspessuraComposicao_DataBinding(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            GridViewRow linhaControle = txt.Parent.Parent as GridViewRow;

            var produtoOrcamento = linhaControle.DataItem as ProdutosOrcamento;
            txt.Enabled = produtoOrcamento.Espessura <= 0;
        }

        protected void lnkChildInsProdComposicao_Click(object sender, ImageClickEventArgs e)
        {
            if (grdProdutosOrcamentoComposicao.PageCount > 1)
            {
                grdProdutosOrcamentoComposicao.PageIndex = grdProdutosOrcamentoComposicao.PageCount - 1;
            }

            ctrlBenef benef = (ctrlBenef)grdProdutosOrcamentoComposicao.FooterRow.FindControl("ctrlChildBenefInserirComposicao");
            GridViewRow linhaControle = benef.Parent.Parent as GridViewRow;
            Control mainTable = linhaControle.Parent;

            while (mainTable.ID != "mainTable")
            {
                mainTable = mainTable.Parent;
            }

            var dtvOrcamento = (DetailsView)mainTable.FindControl("dtvOrcamento");
            var hdfIdAmbiente = (HiddenField)mainTable.FindControl("hdfIdProdAmbienteOrcamento");
            var idOrcamento = Request["idOrca"].StrParaInt();
            var idProd = (hdfChildIdProdutoComposicao?.Value?.StrParaInt()).GetValueOrDefault();
            var altura = (((TextBox)grdProdutosOrcamentoComposicao.FooterRow.FindControl("txtChildAlturaComposicaoIns"))?.Text?.StrParaFloat()).GetValueOrDefault();
            var largura = (((TextBox)grdProdutosOrcamentoComposicao.FooterRow.FindControl("txtChildLarguraComposicaoIns"))?.Text?.StrParaInt()).GetValueOrDefault();
            var espessura = (((TextBox)grdProdutosOrcamentoComposicao.FooterRow.FindControl("txtChildEspessuraComposicao"))?.Text?.StrParaFloat()).GetValueOrDefault();
            var redondo = (((CheckBox)benef.FindControl("Redondo_chkSelecao"))?.Checked).GetValueOrDefault();
            var aliquotaIcms = (((HiddenField)grdProdutosOrcamentoComposicao.FooterRow.FindControl("hdfChildAliquotaIcmsProdComposicao"))?.Value?.Replace('.', ',')?.StrParaFloat()).GetValueOrDefault();
            var valorIcms = (((HiddenField)grdProdutosOrcamentoComposicao.FooterRow.FindControl("hdfChildValorIcmsProdComposicao"))?.Value?.Replace('.', ',')?.StrParaDecimal()).GetValueOrDefault();
            var tipoEntrega = (((HiddenField)dtvOrcamento.FindControl("hdfTipoEntrega"))?.Value?.StrParaInt()).GetValueOrDefault();
            var idCliente = (((HiddenField)dtvOrcamento.FindControl("hdfIdCliente"))?.Value?.StrParaUint()).GetValueOrDefault();

            // Cria uma instância do ProdutosOrcamento
            var produtoOrcamento = new ProdutosOrcamento();
            produtoOrcamento.IdOrcamento = (uint)idOrcamento;
            produtoOrcamento.Qtde = ((TextBox)grdProdutosOrcamentoComposicao.FooterRow.FindControl("txtChildQtdeComposicaoIns"))?.Text?.Replace('.', ',')?.StrParaFloat();
            produtoOrcamento.ValorProd = ((TextBox)grdProdutosOrcamentoComposicao.FooterRow.FindControl("txtChildValorComposicaoIns"))?.Text?.StrParaDecimal();
            produtoOrcamento.PercDescontoQtde = ((ctrlDescontoQtde)grdProdutosOrcamentoComposicao.FooterRow.FindControl("ctrlDescontoQtdeComposicao")).PercDescontoQtde;
            produtoOrcamento.Altura = altura;
            produtoOrcamento.Largura = largura;
            produtoOrcamento.IdProd = (uint)idProd;
            produtoOrcamento.Espessura = espessura;
            produtoOrcamento.Redondo = redondo;
            produtoOrcamento.IdProdParent = hdfIdAmbiente.Value.StrParaUint();
            produtoOrcamento.IdAplicacao = ((HiddenField)grdProdutosOrcamentoComposicao.FooterRow.FindControl("hdfChildIdAplicacaoComposicao"))?.Value?.StrParaUint();
            produtoOrcamento.IdProcesso = ((HiddenField)grdProdutosOrcamentoComposicao.FooterRow.FindControl("hdfChildIdProcessoComposicao"))?.Value?.StrParaUint();
            produtoOrcamento.AliquotaIcms = aliquotaIcms;
            produtoOrcamento.ValorIcms = valorIcms;

            var idLoja = OrcamentoDAO.Instance.ObterIdLoja(null, idOrcamento);
            var lojaCalculaIpiPedido = LojaDAO.Instance.ObtemCalculaIpiPedido(null, (uint)idLoja);

            if (lojaCalculaIpiPedido && ClienteDAO.Instance.IsCobrarIpi(null, idCliente))
            {
                produtoOrcamento.AliquotaIpi = ProdutoDAO.Instance.ObtemAliqIpi(produtoOrcamento.IdProduto.Value);
            }

            produtoOrcamento.Beneficiamentos = benef.Beneficiamentos;
            produtoOrcamento.IdProdOrcamentoParent = (int?)IdProdOrcamento;

            try
            {
                // Insere o produto_orcamento
                produtoOrcamento.IdProd = ProdutosOrcamentoDAO.Instance.Insert(produtoOrcamento);

                if (PedidoConfig.TelaCadastro.ManterCodInternoCampoAoInserirProduto)
                {
                    var codInternoProduto = ProdutoDAO.Instance.GetCodInterno(null, (int)idProd);
                    Page.ClientScript.RegisterClientScriptBlock(typeof(string), "novoProd", $"ultimoCodProd = '{ codInternoProduto }';", true);
                }

                BindControls();
            }
            catch (Exception ex)
            {
                MensagemAlerta.ErrorMsg("Falha ao incluir produto no orçamento.", ex, Page);
                return;
            }
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

        protected void odsProdutosOrcamentoComposicao_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                MensagemAlerta.ErrorMsg(null, e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }

        #endregion

        #region gdrProdutosOrcamentoComposicao

        protected void grdProdutosOrcamentoComposicao_PreRender(object sender, EventArgs e)
        {
            ctrlBenef benef = (ctrlBenef)grdProdutosOrcamentoComposicao.FooterRow.FindControl("ctrlChildBenefInserirComposicao");
            GridViewRow linhaControle = benef.Parent.Parent as GridViewRow;
            Control mainTable = linhaControle.Parent;

            while (mainTable.ID != "mainTable")
            {
                mainTable = mainTable.Parent;
            }

            var hdfIdAmbiente = (HiddenField)mainTable.FindControl("hdfIdProdAmbienteOrcamento");
            var quantidadeProdutoComposicaoPecaPai = ProdutosOrcamentoDAO.Instance.ObterQuantidadeProdutoComposicao(Request["idOrca"].StrParaIntNullable(),
                hdfIdAmbiente.Value.StrParaIntNullable(), (int)IdProdOrcamento);

            if (quantidadeProdutoComposicaoPecaPai == 0)
            {
                grdProdutosOrcamentoComposicao.Rows[0].Visible = false;
            }

            if ((uint)IdProdOrcamento > 0 && ProdutosOrcamentoDAO.Instance.VerificarProdutoLaminado(null, (int)IdProdOrcamento))
            {
                grdProdutosOrcamentoComposicao.ShowFooter = false;
            }
        }

        protected void grdProdutosOrcamentoComposicao_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdProdutosOrcamentoComposicao.ShowFooter = e.CommandName != "Edit";
        }

        protected void grdProdutosOrcamentoComposicao_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
            BindControls();
        }

        protected void grdProdutosOrcamentoComposicao_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
            BindControls();
        }

        #endregion

        #endregion
    }
}