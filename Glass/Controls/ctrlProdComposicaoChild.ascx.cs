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
    public partial class ctrlProdComposicaoChild : System.Web.UI.UserControl
    {
        #region Propiedades

        public object IdProdPed
        {
            get
            {
                return hdfChild_IdProdPed.Value.StrParaUint();
            }
            set
            {
                hdfChild_IdProdPed.Value = value.ToString();
            }
        }

        #endregion

        #region Metodos Protegidos

        protected void Page_Load(object sender, EventArgs e)
        {
            // Se a empresa não possuir acesso ao módulo PCP, esconde colunas Apl e Proc
            if (!Geral.ControlePCP)
            {
                grdProdutosComposicao.Columns[9].Visible = false;
                grdProdutosComposicao.Columns[10].Visible = false;
            }
        }

        protected void odsProdXPed_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg(null, e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }

        protected void grdProdutos_RowUpdated(object sender, System.Web.UI.WebControls.GridViewUpdatedEventArgs e)
        {
            BindControls();
        }

        private void BindControls()
        {
            var grdProdutos = grdProdutosComposicao.Parent;
            while (grdProdutos.ID != "grdProdutos")
                grdProdutos = grdProdutos.Parent;

            var mainTable = grdProdutosComposicao.Parent;
            while (mainTable.ID != "mainTable")
                mainTable = mainTable.Parent;

            var dtvPedido = mainTable.FindControl("dtvPedido");
            var grdAmbiente = mainTable.FindControl("grdAmbiente");

            ((DetailsView)dtvPedido).DataBind();
            ((GridView)grdProdutos).DataBind();
            ((GridView)grdAmbiente).DataBind();
        }

        protected void grdProdutos_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            grdProdutosComposicao.ShowFooter = e.CommandName != "Edit";
        }

        protected void grdProdutos_PreRender(object sender, EventArgs e)
        {
            Glass.UI.Web.Controls.ctrlBenef benef = (Glass.UI.Web.Controls.ctrlBenef)grdProdutosComposicao.FooterRow.FindControl("ctrlChild_BenefInserirComposicao");

            GridViewRow linhaControle = benef.Parent.Parent as GridViewRow;

            Control mainTable = linhaControle.Parent;

            while (mainTable.ID != "mainTable")
                mainTable = mainTable.Parent;

            var hdfIdAmbiente = (HiddenField)mainTable.FindControl("hdfIdAmbiente");

            if (ProdutosPedidoDAO.Instance.CountInPedidoAmbiente(Request["idPedido"].StrParaUint(), hdfIdAmbiente.Value.StrParaUint(), true, (uint)IdProdPed) == 0)
                grdProdutosComposicao.Rows[0].Visible = false;

            if ((uint)IdProdPed > 0 && ProdutosPedidoDAO.Instance.IsProdLaminado((uint)IdProdPed))
                grdProdutosComposicao.ShowFooter = false;
        }

        protected void grdProdutos_RowDeleted(object sender, System.Web.UI.WebControls.GridViewDeletedEventArgs e)
        {
            BindControls();
        }

        protected string NomeControleBenefComposicao()
        {
            return grdProdutosComposicao.EditIndex == -1 ? "ctrlChild_BenefInserirComposicao" : "ctrlChild_BenefEditarComposicao";
        }

        protected void ctrl_Benef_Load(object sender, EventArgs e)
        {
            Glass.UI.Web.Controls.ctrlBenef benef = (Glass.UI.Web.Controls.ctrlBenef)sender;
            GridViewRow linhaControle = benef.Parent.Parent as GridViewRow;

            Control dtvPedido = linhaControle.Parent;

            while (dtvPedido.ID != "mainTable")
                dtvPedido = dtvPedido.Parent;

            dtvPedido = dtvPedido.FindControl("dtvPedido");

            var tipoPedido = PedidoDAO.Instance.GetTipoPedido(null, Conversoes.StrParaUint(Request["idPedido"]));

            Control codProd = null;
            if (linhaControle.FindControl("lblChild_CodProdComposicaoIns") != null)
                codProd = linhaControle.FindControl("lblChild_CodProdComposicaoIns");
            else
                codProd = linhaControle.FindControl("txtChild_CodProdComposicaoIns");

            TextBox txtAltura = (TextBox)linhaControle.FindControl("txtChild_AlturaComposicaoIns");
            TextBox txtEspessura = (TextBox)linhaControle.FindControl("txtChild_EspessuraComposicao");
            TextBox txtLargura = (TextBox)linhaControle.FindControl("txtChild_LarguraComposicaoIns");
            HiddenField hdfPercComissao = (HiddenField)dtvPedido.FindControl("hdfPercComissao");
            TextBox txtQuantidade = (TextBox)linhaControle.FindControl("txtChild_QtdeComposicaoIns");
            HiddenField hdfTipoEntrega = (HiddenField)dtvPedido.FindControl("hdfTipoEntrega");

            HiddenField hdfTotalM2 = null;
            if (!Beneficiamentos.UsarM2CalcBeneficiamentos)
            {
                if (linhaControle.FindControl("hdfChild_TotMComposicao") != null)
                    hdfTotalM2 = (HiddenField)linhaControle.FindControl("hdfChild_TotMComposicao");
                else if (linhaControle.FindControl("hdfChild_TotM2ComposicaoIns") != null)
                    hdfTotalM2 = (HiddenField)linhaControle.FindControl("hdfChild_TotM2ComposicaoIns");
            }
            else
            {
                if (linhaControle.FindControl("hdfChild_TotM2CalcComposicao") != null)
                    hdfTotalM2 = (HiddenField)linhaControle.FindControl("hdfChild_TotM2CalcComposicao");
                else if (linhaControle.FindControl("hdfChild_TotM2CalcComposicaoIns") != null)
                    hdfTotalM2 = (HiddenField)linhaControle.FindControl("hdfChild_TotM2CalcComposicaoIns");
            }

            TextBox txtValorIns = (TextBox)linhaControle.FindControl("txtChild_ValorComposicaoIns");
            HiddenField hdfCliRevenda = (HiddenField)dtvPedido.FindControl("hdfCliRevenda");
            HiddenField hdfIdCliente = (HiddenField)dtvPedido.FindControl("hdfIdCliente");
            HiddenField hdfCustoProd = (HiddenField)linhaControle.FindControl("hdfChild_CustoProdComposicao");

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
            benef.CampoAplicacaoID = linhaControle.FindControl("hdfChild_IdAplicacaoComposicao");
            benef.CampoProcessoID = linhaControle.FindControl("hdfChild_IdProcessoComposicao");
            benef.CampoAplicacao = linhaControle.FindControl("txtChild_AplComposicaoIns");
            benef.CampoProcesso = linhaControle.FindControl("txtChild_ProcComposicaoIns");
            benef.IdProdPed = IdProdPed;

            benef.TipoBenef = tipoPedido == Glass.Data.Model.Pedido.TipoPedidoEnum.MaoDeObraEspecial ?
                TipoBenef.MaoDeObraEspecial : TipoBenef.Venda;
        }

        protected void ctrl_DescontoQtdeComposicao_Load(object sender, EventArgs e)
        {
            Glass.UI.Web.Controls.ctrlDescontoQtde desc = (Glass.UI.Web.Controls.ctrlDescontoQtde)sender;
            GridViewRow linha = desc.Parent.Parent as GridViewRow;

            Control dtvPedido = linha.Parent;

            while (dtvPedido.ID != "mainTable")
                dtvPedido = dtvPedido.Parent;

            dtvPedido = dtvPedido.FindControl("dtvPedido");

            desc.CampoQtde = linha.FindControl("txtQtdeIns");
            desc.CampoProdutoID = linha.FindControl("hdfIdProd");
            desc.CampoClienteID = dtvPedido.FindControl("hdfIdCliente");
            desc.CampoTipoEntrega = dtvPedido.FindControl("hdfTipoEntrega");
            desc.CampoRevenda = dtvPedido.FindControl("hdfCliRevenda");
            desc.CampoReposicao = dtvPedido.FindControl("hdfIsReposicao");
            desc.CampoValorUnit = linha.FindControl("txtValorIns");

            if (desc.CampoProdutoID == null)
                desc.CampoProdutoID = hdfChild_IdProdComposicao;
        }

        protected void txt_ValorInsComposicao_Load(object sender, EventArgs e)
        {
            ((TextBox)sender).Enabled = PedidoConfig.DadosPedido.AlterarValorUnitarioProduto;
        }

        protected void txtChild_EspessuraComposicao_DataBinding(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            GridViewRow linhaControle = txt.Parent.Parent as GridViewRow;

            ProdutosPedido prodPed = linhaControle.DataItem as ProdutosPedido;
            txt.Enabled = prodPed.Espessura <= 0;
        }

        protected void lnk_InsProdComposicao_Click(object sender, ImageClickEventArgs e)
        {
            if (grdProdutosComposicao.PageCount > 1)
                grdProdutosComposicao.PageIndex = grdProdutosComposicao.PageCount - 1;

            Glass.UI.Web.Controls.ctrlBenef benef = (Glass.UI.Web.Controls.ctrlBenef)grdProdutosComposicao.FooterRow.FindControl("ctrlChild_BenefInserirComposicao");

            GridViewRow linhaControle = benef.Parent.Parent as GridViewRow;

            Control mainTable = linhaControle.Parent;

            while (mainTable.ID != "mainTable")
                mainTable = mainTable.Parent;

            var dtvPedido = (DetailsView)mainTable.FindControl("dtvPedido");
            var hdfIdAmbiente = (HiddenField)mainTable.FindControl("hdfIdAmbiente");


            uint idPedido = Request["IdPedido"].StrParaUint();
            int idProd = !string.IsNullOrEmpty(hdfChild_IdProdComposicao.Value) ? hdfChild_IdProdComposicao.Value.StrParaInt() : 0;
            string idAmbiente = hdfIdAmbiente.Value;
            string alturaString = ((TextBox)grdProdutosComposicao.FooterRow.FindControl("txtChild_AlturaComposicaoIns")).Text;
            string alturaRealString = ((HiddenField)grdProdutosComposicao.FooterRow.FindControl("hdfChild_AlturaRealComposicaoIns")).Value;
            string larguraString = ((TextBox)grdProdutosComposicao.FooterRow.FindControl("txtChild_LarguraComposicaoIns")).Text;
            Single altura = Glass.Conversoes.StrParaFloat(alturaString);
            Single alturaReal = Glass.Conversoes.StrParaFloat(alturaRealString);
            int largura = !String.IsNullOrEmpty(larguraString) ? Glass.Conversoes.StrParaInt(larguraString) : 0;
            string idProcessoStr = ((HiddenField)grdProdutosComposicao.FooterRow.FindControl("hdfChild_IdProcessoComposicao")).Value;
            string idAplicacaoStr = ((HiddenField)grdProdutosComposicao.FooterRow.FindControl("hdfChild_IdAplicacaoComposicao")).Value;
            string espessuraString = ((TextBox)grdProdutosComposicao.FooterRow.FindControl("txtChild_EspessuraComposicao")).Text;
            float espessura = !String.IsNullOrEmpty(espessuraString) ? Glass.Conversoes.StrParaFloat(espessuraString) : 0;
            bool redondo = ((CheckBox)benef.FindControl("Redondo_chkSelecao")) != null ? ((CheckBox)benef.FindControl("Redondo_chkSelecao")).Checked : false;
            float aliquotaIcms = Glass.Conversoes.StrParaFloat(((HiddenField)grdProdutosComposicao.FooterRow.FindControl("hdfChild_AliquotaIcmsProdComposicao")).Value.Replace('.', ','));
            decimal valorIcms = Glass.Conversoes.StrParaDecimal(((HiddenField)grdProdutosComposicao.FooterRow.FindControl("hdfChild_ValorIcmsProdComposicao")).Value.Replace('.', ','));

            float espBenef =  0;
            int? alturaBenef =  null;
            int? larguraBenef = null;

            int tipoEntrega = Glass.Conversoes.StrParaInt(((HiddenField)dtvPedido.FindControl("hdfTipoEntrega")).Value);
            uint idCliente = Glass.Conversoes.StrParaUint(((HiddenField)dtvPedido.FindControl("hdfIdCliente")).Value);
            bool reposicao = bool.Parse(((HiddenField)dtvPedido.FindControl("hdfIsReposicao")).Value);

            // Cria uma instância do ProdutosPedido
            ProdutosPedido prodPed = new ProdutosPedido();
            prodPed.IdPedido = idPedido;
            prodPed.Qtde = Glass.Conversoes.StrParaFloat(((TextBox)grdProdutosComposicao.FooterRow.FindControl("txtChild_QtdeComposicaoIns")).Text.Replace('.', ','));
            prodPed.ValorVendido = Glass.Conversoes.StrParaDecimal(((TextBox)grdProdutosComposicao.FooterRow.FindControl("txtChild_ValorComposicaoIns")).Text);
            prodPed.PercDescontoQtde = ((Glass.UI.Web.Controls.ctrlDescontoQtde)grdProdutosComposicao.FooterRow.FindControl("ctrl_DescontoQtdeComposicao")).PercDescontoQtde;
            prodPed.ValorTabelaPedido = ProdutoDAO.Instance.GetValorTabela(idProd, tipoEntrega, idCliente, false, reposicao, prodPed.PercDescontoQtde, (int?)idPedido, null, null);
            prodPed.Altura = altura;
            prodPed.AlturaReal = alturaReal;
            prodPed.Largura = largura;
            prodPed.IdProd = (uint)idProd;
            prodPed.Espessura = espessura;
            prodPed.Redondo = redondo;

            if (!String.IsNullOrEmpty(idAmbiente))
                prodPed.IdAmbientePedido = Glass.Conversoes.StrParaUint(idAmbiente);

            if (!String.IsNullOrEmpty(idAplicacaoStr))
                prodPed.IdAplicacao = Glass.Conversoes.StrParaUint(idAplicacaoStr);

            if (!String.IsNullOrEmpty(idProcessoStr))
                prodPed.IdProcesso = Glass.Conversoes.StrParaUint(idProcessoStr);

            prodPed.AliqIcms = aliquotaIcms;
            prodPed.ValorIcms = valorIcms;

            var idLoja = PedidoDAO.Instance.ObtemIdLoja(null, idPedido);
            var loja = LojaDAO.Instance.GetElement(idLoja);
            if (loja.CalcularIpiPedido && ClienteDAO.Instance.IsCobrarIpi(null, idCliente))
                prodPed.AliqIpi = ProdutoDAO.Instance.ObtemAliqIpi(prodPed.IdProd);

            prodPed.AlturaBenef = alturaBenef;
            prodPed.LarguraBenef = larguraBenef;
            prodPed.EspessuraBenef = espBenef;
            prodPed.Beneficiamentos = benef.Beneficiamentos;
            prodPed.PedCli = ((TextBox)grdProdutosComposicao.FooterRow.FindControl("txtChild_PedCliComposicao")).Text;
            prodPed.IdProdPedParent = (uint?)IdProdPed;

            uint idProdPed = 0;

            try
            {
                // Se o pedido estiver diferente de ativo-ativo/conferência não permite inserir produtos
                var situacao = PedidoDAO.Instance.ObtemSituacao(null, prodPed.IdPedido);
                if (situacao != Glass.Data.Model.Pedido.SituacaoPedido.Ativo && situacao != Glass.Data.Model.Pedido.SituacaoPedido.AtivoConferencia)
                {
                    MensagemAlerta.ShowMsg("Não é possível incluir produtos em pedidos que não estejam ativos.", Page);
                    return;
                }

                // Insere o produto_pedido
                idProdPed = ProdutosPedidoDAO.Instance.InsertEAtualizaDataEntrega(prodPed);

                ((HiddenField)grdProdutosComposicao.FooterRow.FindControl("hdfChild_AlturaRealComposicaoIns")).Value = "";

                if (Glass.Configuracoes.PedidoConfig.TelaCadastro.ManterCodInternoCampoAoInserirProduto)
                    Page.ClientScript.RegisterClientScriptBlock(typeof(string), "novoProd",
                        "ultimoCodProd = '" + ProdutoDAO.Instance.GetCodInterno((int)idProd) + "';", true);

                BindControls();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao incluir produto no Pedido.", ex, Page);
                return;
            }
        }

        protected void odsProdXPed_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg(null, e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }

        protected bool UtilizarRoteiroProducao()
        {
            return PCPConfig.ControlarProducao && Data.Helper.Utils.GetSetores.Count(x => x.SetorPertenceARoteiro) > 0;
        }

        protected string VerificaPedidoReposicao()
        {
            var idPedido = Request["idPedido"].StrParaUint();

            if (idPedido > 0 && PedidoDAO.Instance.ObtemTipoVenda(null, idPedido) == (int)Glass.Data.Model.Pedido.TipoVendaPedido.Reposição)
            {
                return "true";
            }

            return "false";
        }

        #endregion
    }
}
