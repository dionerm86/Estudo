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
    public partial class ctrlProdComposicaoOrcamento : UserControl
    {
        #region Propiedades

        public object IdProdOrcamento
        {
            get
            {
                return hdfIdProdOrcamento.Value.StrParaInt();
            }
            set
            {
                hdfIdProdOrcamento.Value = value.ToString();
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

        protected string NomeControleBenefComposicao()
        {
            return grdProdutosOrcamentoComposicao.EditIndex == -1 ? "ctrlBenefInserirComposicao" : "ctrlBenefEditarComposicao";
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

            if (linhaControle.FindControl("lblCodProdComposicaoIns") != null)
            {
                codProd = linhaControle.FindControl("lblCodProdComposicaoIns");
            }
            else
            {
                codProd = linhaControle.FindControl("txtCodProdComposicaoIns");
            }

            TextBox txtAltura = (TextBox)linhaControle.FindControl("txtAlturaComposicaoIns");
            TextBox txtEspessura = (TextBox)linhaControle.FindControl("txtEspessuraComposicao");
            TextBox txtLargura = (TextBox)linhaControle.FindControl("txtLarguraComposicaoIns");
            HiddenField hdfPercComissao = (HiddenField)dtvOrcamento.FindControl("hdfPercComissao");
            TextBox txtQuantidade = (TextBox)linhaControle.FindControl("txtQtdeComposicaoIns");
            HiddenField hdfTipoEntrega = (HiddenField)dtvOrcamento.FindControl("hdfTipoEntrega");
            HiddenField hdfTotalM2 = null;

            if (!Beneficiamentos.UsarM2CalcBeneficiamentos)
            {
                if (linhaControle.FindControl("hdfTotMComposicao") != null)
                {
                    hdfTotalM2 = (HiddenField)linhaControle.FindControl("hdfTotMComposicao");
                }
                else if (linhaControle.FindControl("hdfTotM2ComposicaoIns") != null)
                {
                    hdfTotalM2 = (HiddenField)linhaControle.FindControl("hdfTotM2ComposicaoIns");
                }
            }
            else
            {
                if (linhaControle.FindControl("hdfTotM2CalcComposicao") != null)
                {
                    hdfTotalM2 = (HiddenField)linhaControle.FindControl("hdfTotM2CalcComposicao");
                }
                else if (linhaControle.FindControl("hdfTotM2CalcComposicaoIns") != null)
                {
                    hdfTotalM2 = (HiddenField)linhaControle.FindControl("hdfTotM2CalcComposicaoIns");
                }
            }

            TextBox txtValorIns = (TextBox)linhaControle.FindControl("txtValorComposicaoIns");
            HiddenField hdfCliRevenda = (HiddenField)dtvOrcamento.FindControl("hdfCliRevenda");
            HiddenField hdfIdCliente = (HiddenField)dtvOrcamento.FindControl("hdfIdCliente");
            HiddenField hdfCustoProd = (HiddenField)linhaControle.FindControl("hdfCustoProdComposicao");

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
            benef.CampoAplicacaoID = linhaControle.FindControl("hdfIdAplicacaoComposicao");
            benef.CampoProcessoID = linhaControle.FindControl("hdfIdProcessoComposicao");
            benef.CampoAplicacao = linhaControle.FindControl("txtAplComposicaoIns");
            benef.CampoProcesso = linhaControle.FindControl("txtProcComposicaoIns");
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
            desc.CampoProdutoID = linha.FindControl("hdfIdProd");
            desc.CampoClienteID = dtvOrcamento.FindControl("hdfIdCliente");
            desc.CampoTipoEntrega = dtvOrcamento.FindControl("hdfTipoEntrega");
            desc.CampoRevenda = dtvOrcamento.FindControl("hdfCliRevenda");
            desc.CampoReposicao = dtvOrcamento.FindControl("hdfIsReposicao");
            desc.CampoValorUnit = linha.FindControl("txtValorIns");

            if (desc.CampoProdutoID == null)
            {
                desc.CampoProdutoID = hdfIdProdutoComposicao;
            }
        }

        protected void txtValorInsComposicao_Load(object sender, EventArgs e)
        {
            ((TextBox)sender).Enabled = PedidoConfig.DadosPedido.AlterarValorUnitarioProduto;
        }

        protected void txtEspessuraComposicao_DataBinding(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            GridViewRow linhaControle = txt.Parent.Parent as GridViewRow;

            var produtoOrcamento = linhaControle.DataItem as ProdutosOrcamento;
            txt.Enabled = produtoOrcamento.Espessura <= 0;
        }

        protected void lnkInsProdComposicao_Click(object sender, ImageClickEventArgs e)
        {
            if (grdProdutosOrcamentoComposicao.PageCount > 1)
            {
                grdProdutosOrcamentoComposicao.PageIndex = grdProdutosOrcamentoComposicao.PageCount - 1;
            }

            ctrlBenef benef = (ctrlBenef)grdProdutosOrcamentoComposicao.FooterRow.FindControl("ctrlBenefInserirComposicao");
            GridViewRow linhaControle = benef.Parent.Parent as GridViewRow;
            Control mainTable = linhaControle.Parent;

            while (mainTable.ID != "mainTable")
            {
                mainTable = mainTable.Parent;
            }

            var dtvOrcamento = (DetailsView)mainTable.FindControl("dtvOrcamento");
            var hdfIdAmbiente = ((HiddenField)mainTable.FindControl("hdfIdProdAmbienteOrcamento"))?.Value ?? string.Empty;
            var idOrcamento = Request["IdOrca"].StrParaInt();
            var idProd = (hdfIdProdutoComposicao?.Value?.StrParaInt()).GetValueOrDefault();
            var altura = (((TextBox)grdProdutosOrcamentoComposicao.FooterRow.FindControl("txtAlturaComposicaoIns"))?.Text?.StrParaFloat()).GetValueOrDefault();
            var alturaReal = (((HiddenField)grdProdutosOrcamentoComposicao.FooterRow.FindControl("hdfAlturaRealComposicaoIns"))?.Value?.StrParaFloat()).GetValueOrDefault();
            var largura = (((TextBox)grdProdutosOrcamentoComposicao.FooterRow.FindControl("txtLarguraComposicaoIns"))?.Text?.StrParaInt()).GetValueOrDefault();
            var espessura = (((TextBox)grdProdutosOrcamentoComposicao.FooterRow.FindControl("txtEspessuraComposicao"))?.Text?.StrParaFloat()).GetValueOrDefault();
            var redondo = (((CheckBox)benef.FindControl("Redondo_chkSelecao"))?.Checked).GetValueOrDefault();
            var aliquotaIcms = (((HiddenField)grdProdutosOrcamentoComposicao.FooterRow.FindControl("hdfAliquotaIcmsProdComposicao"))?.Value?.Replace('.', ',')?.StrParaFloat()).GetValueOrDefault();
            var valorIcms = (((HiddenField)grdProdutosOrcamentoComposicao.FooterRow.FindControl("hdfValorIcmsProdComposicao"))?.Value?.Replace('.', ',')?.StrParaDecimal()).GetValueOrDefault();
            var tipoEntrega = (((HiddenField)dtvOrcamento.FindControl("hdfTipoEntrega"))?.Value?.StrParaInt()).GetValueOrDefault();
            var idCliente = (((HiddenField)dtvOrcamento.FindControl("hdfIdCliente"))?.Value?.StrParaInt()).GetValueOrDefault();

            // Cria uma instância do ProdutosOrcamento
            var produtoOrcamento = new ProdutosOrcamento();
            produtoOrcamento.IdOrcamento = (uint)idOrcamento;
            produtoOrcamento.Qtde = (((TextBox)grdProdutosOrcamentoComposicao.FooterRow.FindControl("txtQtdeComposicaoIns"))?.Text?.Replace('.', ',')?.StrParaFloat()).GetValueOrDefault();
            produtoOrcamento.ValorProd = (((TextBox)grdProdutosOrcamentoComposicao.FooterRow.FindControl("txtValorComposicaoIns"))?.Text?.StrParaDecimal()).GetValueOrDefault();
            produtoOrcamento.PercDescontoQtde = (((ctrlDescontoQtde)grdProdutosOrcamentoComposicao.FooterRow.FindControl("ctrlDescontoQtdeComposicao"))?.PercDescontoQtde).GetValueOrDefault();
            produtoOrcamento.Altura = altura;
            produtoOrcamento.AlturaCalc = alturaReal;
            produtoOrcamento.Largura = largura;
            produtoOrcamento.IdProd = (uint)idProd;
            produtoOrcamento.Espessura = espessura;
            produtoOrcamento.Redondo = redondo;
            produtoOrcamento.IdProdParent = hdfIdAmbiente?.StrParaUint();
            produtoOrcamento.IdAplicacao = (((HiddenField)grdProdutosOrcamentoComposicao.FooterRow.FindControl("hdfIdAplicacaoComposicao"))?.Value?.StrParaUint()).GetValueOrDefault();
            produtoOrcamento.IdProcesso = ((HiddenField)grdProdutosOrcamentoComposicao.FooterRow.FindControl("hdfIdProcessoComposicao"))?.Value?.StrParaUint();
            produtoOrcamento.AliquotaIcms = aliquotaIcms;
            produtoOrcamento.ValorIcms = valorIcms;
            produtoOrcamento.Beneficiamentos = benef.Beneficiamentos;
            produtoOrcamento.IdProdOrcamentoParent = IdProdOrcamento.ToString().StrParaInt();

            var idLoja = OrcamentoDAO.Instance.ObterIdLoja(null, idOrcamento);
            var lojaCalculaIpiPedido = LojaDAO.Instance.ObtemCalculaIpiPedido(null, (uint)idLoja);

            if (lojaCalculaIpiPedido && ClienteDAO.Instance.IsCobrarIpi(null, (uint)idCliente))
            {
                produtoOrcamento.AliquotaIpi = ProdutoDAO.Instance.ObtemAliqIpi(produtoOrcamento.IdProduto.Value);
            }

            try
            {
                // Insere o produto_orçamento
                produtoOrcamento.IdProd = ProdutosOrcamentoDAO.Instance.Insert(produtoOrcamento);

                ((HiddenField)grdProdutosOrcamentoComposicao.FooterRow.FindControl("hdfAlturaRealComposicaoIns")).Value = string.Empty;

                if (PedidoConfig.TelaCadastro.ManterCodInternoCampoAoInserirProduto)
                {
                    Page.ClientScript.RegisterClientScriptBlock(typeof(string), "novoProd", $"ultimoCodProd = '{ ProdutoDAO.Instance.GetCodInterno((int)idProd) }';", true);
                }

                grdProdutosOrcamentoComposicao.DataBind();

                var grdProdutosOrcamentoComposicaoParent = grdProdutosOrcamentoComposicao.Parent;

                while (grdProdutosOrcamentoComposicaoParent.ID != "grdProdutosOrcamento")
                {
                    grdProdutosOrcamentoComposicaoParent = grdProdutosOrcamentoComposicaoParent.Parent;
                }

                dtvOrcamento.DataBind();
                ((GridView)grdProdutosOrcamentoComposicaoParent).DataBind();

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

        #region grdProdutosOrcamentoComposicao

        protected void grdProdutosOrcamentoComposicao_PreRender(object sender, EventArgs e)
        {
            ctrlBenef benef = (ctrlBenef)grdProdutosOrcamentoComposicao.FooterRow.FindControl("ctrlBenefInserirComposicao");
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

            if (IdProdOrcamento?.ToString()?.StrParaInt() > 0 && ProdutosOrcamentoDAO.Instance.VerificarProdutoLaminado(null, IdProdOrcamento.ToString().StrParaInt()))
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
            var grdProdutosOrcamentoComposicaoParent = grdProdutosOrcamentoComposicao.Parent;

            while (grdProdutosOrcamentoComposicaoParent.ID != "grdProdutosOrcamento")
            {
                grdProdutosOrcamentoComposicaoParent = grdProdutosOrcamentoComposicaoParent.Parent;
            }

            var dtvOrcamento = grdProdutosOrcamentoComposicao.Parent;

            while (dtvOrcamento.ID != "mainTable")
            {
                dtvOrcamento = dtvOrcamento.Parent;
            }

            dtvOrcamento = dtvOrcamento.FindControl("dtvOrcamento");

            ((DetailsView)dtvOrcamento).DataBind();
            ((GridView)grdProdutosOrcamentoComposicaoParent).DataBind();
        }

        protected void grdProdutosOrcamentoComposicao_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
            var grdProdutosOrcamentoComposicaoParent = grdProdutosOrcamentoComposicao.Parent;

            while (grdProdutosOrcamentoComposicaoParent.ID != "grdProdutosOrcamento")
            {
                grdProdutosOrcamentoComposicaoParent = grdProdutosOrcamentoComposicaoParent.Parent;
            }

            var dtvOrcamento = grdProdutosOrcamentoComposicao.Parent;

            while (dtvOrcamento.ID != "mainTable")
            {
                dtvOrcamento = dtvOrcamento.Parent;
            }

            dtvOrcamento = dtvOrcamento.FindControl("dtvOrcamento");

            ((DetailsView)dtvOrcamento).DataBind();
            ((GridView)grdProdutosOrcamentoComposicaoParent).DataBind();
        }

        #endregion

        #endregion
    }
}