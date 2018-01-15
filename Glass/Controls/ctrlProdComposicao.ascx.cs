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
    public partial class ctrlProdComposicao : System.Web.UI.UserControl
    {
        #region Propiedades

        public object IdProdPed
        {
            get
            {
                return hdf_IdProdPed.Value.StrParaUint();
            }
            set
            {
                hdf_IdProdPed.Value = value.ToString();
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
            var grdProdutos = grdProdutosComposicao.Parent;
            while (grdProdutos.ID != "grdProdutos")
                grdProdutos = grdProdutos.Parent;


            var dtvPedido = grdProdutosComposicao.Parent;
            while (dtvPedido.ID != "mainTable")
                dtvPedido = dtvPedido.Parent;
            dtvPedido = dtvPedido.FindControl("dtvPedido");

            ((DetailsView)dtvPedido).DataBind();
            ((GridView)grdProdutos).DataBind();
        }

        protected void grdProdutos_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            grdProdutosComposicao.ShowFooter = e.CommandName != "Edit";
        }

        protected void grdProdutos_PreRender(object sender, EventArgs e)
        {
            Glass.UI.Web.Controls.ctrlBenef benef = (Glass.UI.Web.Controls.ctrlBenef)grdProdutosComposicao.FooterRow.FindControl("ctrl_BenefInserirComposicao");

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
            var grdProdutos = grdProdutosComposicao.Parent;
            while (grdProdutos.ID != "grdProdutos")
                grdProdutos = grdProdutos.Parent;


            var dtvPedido = grdProdutosComposicao.Parent;
            while (dtvPedido.ID != "mainTable")
                dtvPedido = dtvPedido.Parent;
            dtvPedido = dtvPedido.FindControl("dtvPedido");

            ((DetailsView)dtvPedido).DataBind();
            ((GridView)grdProdutos).DataBind();
        }

        protected string NomeControleBenefComposicao()
        {
            return grdProdutosComposicao.EditIndex == -1 ? "ctrl_BenefInserirComposicao" : "ctrl_BenefEditarComposicao";
        }

        protected void ctrl_Benef_Load(object sender, EventArgs e)
        {
            Glass.UI.Web.Controls.ctrlBenef benef = (Glass.UI.Web.Controls.ctrlBenef)sender;
            GridViewRow linhaControle = benef.Parent.Parent as GridViewRow;

            Control dtvPedido = linhaControle.Parent;

            while (dtvPedido.ID != "mainTable")
                dtvPedido = dtvPedido.Parent;

            dtvPedido = dtvPedido.FindControl("dtvPedido");

            var tipoPedido = PedidoDAO.Instance.GetTipoPedido(Conversoes.StrParaUint(Request["idPedido"]));

            Control codProd = null;
            if (linhaControle.FindControl("lbl_CodProdComposicaoIns") != null)
                codProd = linhaControle.FindControl("lbl_CodProdComposicaoIns");
            else
                codProd = linhaControle.FindControl("txt_CodProdComposicaoIns");

            TextBox txtAltura = (TextBox)linhaControle.FindControl("txt_AlturaComposicaoIns");
            TextBox txtEspessura = (TextBox)linhaControle.FindControl("txt_EspessuraComposicao");
            TextBox txtLargura = (TextBox)linhaControle.FindControl("txt_LarguraComposicaoIns");
            HiddenField hdfPercComissao = (HiddenField)dtvPedido.FindControl("hdfPercComissao");
            TextBox txtQuantidade = (TextBox)linhaControle.FindControl("txt_QtdeComposicaoIns");
            HiddenField hdfTipoEntrega = (HiddenField)dtvPedido.FindControl("hdfTipoEntrega");

            HiddenField hdfTotalM2 = null;
            if (!Beneficiamentos.UsarM2CalcBeneficiamentos)
            {
                if (linhaControle.FindControl("hdf_TotMComposicao") != null)
                    hdfTotalM2 = (HiddenField)linhaControle.FindControl("hdf_TotMComposicao");
                else if (linhaControle.FindControl("hdf_TotM2ComposicaoIns") != null)
                    hdfTotalM2 = (HiddenField)linhaControle.FindControl("hdf_TotM2ComposicaoIns");
            }
            else
            {
                if (linhaControle.FindControl("hdf_TotM2CalcComposicao") != null)
                    hdfTotalM2 = (HiddenField)linhaControle.FindControl("hdf_TotM2CalcComposicao");
                else if (linhaControle.FindControl("hdf_TotM2CalcComposicaoIns") != null)
                    hdfTotalM2 = (HiddenField)linhaControle.FindControl("hdf_TotM2CalcComposicaoIns");
            }

            TextBox txtValorIns = (TextBox)linhaControle.FindControl("txt_ValorComposicaoIns");
            HiddenField hdfCliRevenda = (HiddenField)dtvPedido.FindControl("hdfCliRevenda");
            HiddenField hdfIdCliente = (HiddenField)dtvPedido.FindControl("hdfIdCliente");
            HiddenField hdfCustoProd = (HiddenField)linhaControle.FindControl("hdf_CustoProdComposicao");

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
            benef.CampoAplicacaoID = linhaControle.FindControl("hdf_IdAplicacaoComposicao");
            benef.CampoProcessoID = linhaControle.FindControl("hdf_IdProcessoComposicao");
            benef.CampoAplicacao = linhaControle.FindControl("txt_AplComposicaoIns");
            benef.CampoProcesso = linhaControle.FindControl("txt_ProcComposicaoIns");
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
                desc.CampoProdutoID = hdf_IdProdComposicao;
        }

        protected void txt_ValorInsComposicao_Load(object sender, EventArgs e)
        {
            ((TextBox)sender).Enabled = PedidoConfig.DadosPedido.AlterarValorUnitarioProduto;
        }

        protected void txt_EspessuraComposicao_DataBinding(object sender, EventArgs e)
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

            Glass.UI.Web.Controls.ctrlBenef benef = (Glass.UI.Web.Controls.ctrlBenef)grdProdutosComposicao.FooterRow.FindControl("ctrl_BenefInserirComposicao");

            GridViewRow linhaControle = benef.Parent.Parent as GridViewRow;

            Control mainTable = linhaControle.Parent;

            while (mainTable.ID != "mainTable")
                mainTable = mainTable.Parent;

            var dtvPedido = (DetailsView)mainTable.FindControl("dtvPedido");
            var hdfIdAmbiente = (HiddenField)mainTable.FindControl("hdfIdAmbiente") != null ? ((HiddenField)mainTable.FindControl("hdfIdAmbiente")).Value : string.Empty;

            uint idPedido = Request["IdPedido"].StrParaUint();
            int idProd = !string.IsNullOrEmpty(hdf_IdProdComposicao.Value) ? hdf_IdProdComposicao.Value.StrParaInt() : 0;
            string alturaString = ((TextBox)grdProdutosComposicao.FooterRow.FindControl("txt_AlturaComposicaoIns")).Text;
            string alturaRealString = ((HiddenField)grdProdutosComposicao.FooterRow.FindControl("hdf_AlturaRealComposicaoIns")).Value;
            string larguraString = ((TextBox)grdProdutosComposicao.FooterRow.FindControl("txt_LarguraComposicaoIns")).Text;
            Single altura = Glass.Conversoes.StrParaFloat(alturaString);
            Single alturaReal = Glass.Conversoes.StrParaFloat(alturaRealString);
            int largura = !String.IsNullOrEmpty(larguraString) ? Glass.Conversoes.StrParaInt(larguraString) : 0;
            string idProcessoStr = ((HiddenField)grdProdutosComposicao.FooterRow.FindControl("hdf_IdProcessoComposicao")).Value;
            string idAplicacaoStr = ((HiddenField)grdProdutosComposicao.FooterRow.FindControl("hdf_IdAplicacaoComposicao")).Value;
            string espessuraString = ((TextBox)grdProdutosComposicao.FooterRow.FindControl("txt_EspessuraComposicao")).Text;
            float espessura = !String.IsNullOrEmpty(espessuraString) ? Glass.Conversoes.StrParaFloat(espessuraString) : 0;
            bool redondo = ((CheckBox)benef.FindControl("Redondo_chkSelecao")) != null ? ((CheckBox)benef.FindControl("Redondo_chkSelecao")).Checked : false;
            float aliquotaIcms = Glass.Conversoes.StrParaFloat(((HiddenField)grdProdutosComposicao.FooterRow.FindControl("hdf_AliquotaIcmsProdComposicao")).Value.Replace('.', ','));
            decimal valorIcms = Glass.Conversoes.StrParaDecimal(((HiddenField)grdProdutosComposicao.FooterRow.FindControl("hdf_ValorIcmsProdComposicao")).Value.Replace('.', ','));

            float espBenef =  0;
            int? alturaBenef =  null;
            int? larguraBenef = null;

            int tipoEntrega = Glass.Conversoes.StrParaInt(((HiddenField)dtvPedido.FindControl("hdfTipoEntrega")).Value);
            uint idCliente = Glass.Conversoes.StrParaUint(((HiddenField)dtvPedido.FindControl("hdfIdCliente")).Value);
            bool reposicao = bool.Parse(((HiddenField)dtvPedido.FindControl("hdfIsReposicao")).Value);

            // Cria uma instância do ProdutosPedido
            ProdutosPedido prodPed = new ProdutosPedido();
            prodPed.IdPedido = idPedido;
            prodPed.Qtde = Glass.Conversoes.StrParaFloat(((TextBox)grdProdutosComposicao.FooterRow.FindControl("txt_QtdeComposicaoIns")).Text.Replace('.', ','));
            prodPed.ValorVendido = Glass.Conversoes.StrParaDecimal(((TextBox)grdProdutosComposicao.FooterRow.FindControl("txt_ValorComposicaoIns")).Text);
            prodPed.PercDescontoQtde = ((Glass.UI.Web.Controls.ctrlDescontoQtde)grdProdutosComposicao.FooterRow.FindControl("ctrl_DescontoQtdeComposicao")).PercDescontoQtde;
            prodPed.ValorTabelaPedido = ProdutoDAO.Instance.GetValorTabela(idProd, tipoEntrega, idCliente, false, reposicao, prodPed.PercDescontoQtde);
            prodPed.Altura = altura;
            prodPed.AlturaReal = alturaReal;
            prodPed.Largura = largura;
            prodPed.IdProd = (uint)idProd;
            prodPed.Espessura = espessura;
            prodPed.Redondo = redondo;

            if (!string.IsNullOrEmpty(hdfIdAmbiente))
                prodPed.IdAmbientePedido = Glass.Conversoes.StrParaUint(hdfIdAmbiente);

            if (!String.IsNullOrEmpty(idAplicacaoStr))
                prodPed.IdAplicacao = Glass.Conversoes.StrParaUint(idAplicacaoStr);

            if (!String.IsNullOrEmpty(idProcessoStr))
                prodPed.IdProcesso = Glass.Conversoes.StrParaUint(idProcessoStr);

            prodPed.AliqIcms = aliquotaIcms;
            prodPed.ValorIcms = valorIcms;

            if (PedidoConfig.Impostos.CalcularIpiPedido && ClienteDAO.Instance.IsCobrarIpi(null, idCliente))
                prodPed.AliqIpi = ProdutoDAO.Instance.ObtemAliqIpi(prodPed.IdProd);

            prodPed.AlturaBenef = alturaBenef;
            prodPed.LarguraBenef = larguraBenef;
            prodPed.EspessuraBenef = espBenef;
            prodPed.Beneficiamentos = benef.Beneficiamentos;
            prodPed.PedCli = ((TextBox)grdProdutosComposicao.FooterRow.FindControl("txt_PedCliComposicao")).Text;
            prodPed.IdProdPedParent = (uint?)IdProdPed;

            uint idProdPed = 0;

            try
            {
                // Se o pedido estiver diferente de ativo-ativo/conferência não permite inserir produtos
                var situacao = PedidoDAO.Instance.ObtemSituacao(prodPed.IdPedido);
                if (situacao != Glass.Data.Model.Pedido.SituacaoPedido.Ativo && situacao != Glass.Data.Model.Pedido.SituacaoPedido.AtivoConferencia)
                {
                    MensagemAlerta.ShowMsg("Não é possível incluir produtos em pedidos que não estejam ativos.", Page);
                    return;
                }

                // Insere o produto_pedido
                idProdPed = ProdutosPedidoDAO.Instance.Insert(prodPed);

                ((HiddenField)grdProdutosComposicao.FooterRow.FindControl("hdf_AlturaRealComposicaoIns")).Value = "";

                if (Glass.Configuracoes.PedidoConfig.TelaCadastro.ManterCodInternoCampoAoInserirProduto)
                    Page.ClientScript.RegisterClientScriptBlock(typeof(string), "novoProd",
                        "ultimoCodProd = '" + ProdutoDAO.Instance.GetCodInterno((int)idProd) + "';", true);

                grdProdutosComposicao.DataBind();

                var grdProdutos = grdProdutosComposicao.Parent;
                while (grdProdutos.ID != "grdProdutos")
                    grdProdutos = grdProdutos.Parent;

                ((DetailsView)dtvPedido).DataBind();
                ((GridView)grdProdutos).DataBind();

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

        #endregion
    }
}