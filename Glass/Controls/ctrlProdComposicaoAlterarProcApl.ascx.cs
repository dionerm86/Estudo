using Glass.Configuracoes;
using Glass.Data.DAL;
using Glass.Data.Model;
using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlProdComposicaoAlterarProcApl : System.Web.UI.UserControl
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

            var tipoPedido = PedidoDAO.Instance.GetTipoPedido(null, Conversoes.StrParaUint(Request["idPedido"]));

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

        protected void txt_EspessuraComposicao_DataBinding(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            GridViewRow linhaControle = txt.Parent.Parent as GridViewRow;

            ProdutosPedido prodPed = linhaControle.DataItem as ProdutosPedido;
            txt.Enabled = prodPed.Espessura <= 0;
        }

        protected bool UtilizarRoteiroProducao()
        {
            return PCPConfig.ControlarProducao && Data.Helper.Utils.GetSetores.Count(x => x.SetorPertenceARoteiro) > 0;
        }

        #endregion
    }
}