using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Data.Helper;
using Glass.Data.NFeUtils;
using System.Collections.Generic;
using System.Drawing;
using Glass.Data.Exceptions;
using System.Linq;
using Glass.Configuracoes;
using Glass.UI.Web.Cadastros;

namespace Glass.UI.Web.Utils
{
    public partial class AlterarProcessoAplicacao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(CadPedido));
            Ajax.Utility.RegisterTypeForAjax(typeof(AlterarProcessoAplicacao));
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));

            dtvPedido.ChangeMode(DetailsViewMode.ReadOnly);
            bool isMaoDeObra = IsPedidoMaoDeObra();
            bool isProducao = IsPedidoProducao();            

            if (isMaoDeObra)
            {
                grdAmbiente.Columns[1].HeaderText = "Peça de vidro";
                grdAmbiente.Columns[2].Visible = false;
                grdAmbiente.Columns[3].Visible = true;
                grdAmbiente.Columns[4].Visible = true;
                grdAmbiente.Columns[5].Visible = true;
                grdAmbiente.Columns[6].Visible = true;
                grdAmbiente.Columns[7].Visible = true;
                grdAmbiente.Columns[8].Visible = true;

                grdProdutos.Columns[9].Visible = false;
                grdProdutos.Columns[10].Visible = false;                
            }

            if (!OrcamentoConfig.Desconto.DescontoAcrescimoItensOrcamento)
            {
                grdAmbiente.Columns[9].Visible = isMaoDeObra;
                grdAmbiente.Columns[10].Visible = false;
                grdAmbiente.Columns[11].Visible = false;
            }

            

            // Se a empresa não possuir acesso ao módulo PCP, esconde colunas Apl e Proc
            if (!Geral.ControlePCP)
            {
                grdProdutos.Columns[9].Visible = false;
                grdProdutos.Columns[10].Visible = false;
            }              
            
            divProduto.Visible = dtvPedido.CurrentMode == DetailsViewMode.ReadOnly;
            grdProdutos.Visible = divProduto.Visible;

            if (Geral.NaoVendeVidro())
            {
                grdProdutos.Columns[grdProdutos.Columns.Count - 2].Visible = false;
                grdProdutos.Columns[grdProdutos.Columns.Count - 3].Visible = false;
            }

            if (!IsPostBack)
                hdfNaoVendeVidro.Value = Glass.Configuracoes.Geral.NaoVendeVidro().ToString().ToLower();

            // Se a empresa trabalha com ambiente de pedido e não houver nenhum ambiente cadastrado, esconde grid de produtos
            bool exibirAmbiente = PedidoConfig.DadosPedido.AmbientePedido || isMaoDeObra;
            grdProdutos.Visible = (exibirAmbiente && !String.IsNullOrEmpty(hdfIdAmbiente.Value) && hdfIdAmbiente.Value != "0") || !exibirAmbiente;            

            // Se a empresa não trabalha com Ambiente no pedido, esconde grdAmbiente
            grdAmbiente.Visible = exibirAmbiente;

            if (!exibirAmbiente && !String.IsNullOrEmpty(Request["idPedido"]) && PedidoDAO.Instance.PossuiCalculoProjeto(Glass.Conversoes.StrParaUint(Request["idPedido"])))
            {
                grdAmbiente.Visible = true;
                
            }            

            if (Glass.Configuracoes.Geral.SistemaLite)
            {
                grdAmbiente.Columns[6].Visible = false;
                grdAmbiente.Columns[7].Visible = false;
            }
        }        

        protected void grdProdutos_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
            var idPedido = Request["idPedido"].StrParaIntNullable();
            
            dtvPedido.DataBind();
            grdAmbiente.DataBind();
        }

        protected void grdProdutos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            
        }

        protected void grdProdutos_PreRender(object sender, EventArgs e)
        {
            string ambiente = hdfIdAmbiente.Value;

            // Se não houver nenhum produto cadastrado no pedido (e no ambiente passado)
            if (ProdutosPedidoDAO.Instance.CountInPedidoAmbiente(Glass.Conversoes.StrParaUint(Request["idPedido"]), !String.IsNullOrEmpty(ambiente) ? Glass.Conversoes.StrParaUint(ambiente) : 0) == 0)
                grdProdutos.Rows[0].Visible = false;
        }

        #region Eventos DataSource
        
        protected void odsProdXPed_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg(null, e.Exception, Page);
                e.ExceptionHandled = true;
            }

            dtvPedido.DataBind();
        }

        #endregion

        #region Métodos Ajax

        [Ajax.AjaxMethod()]
        public bool SubgrupoProdComposto(int idProd)
        {
            return SubgrupoProdDAO.Instance.ObtemTipoSubgrupo(idProd) == TipoSubgrupoProd.VidroDuplo;
        }

        [Ajax.AjaxMethod]
        public string GetTamanhoMaximoProduto(string idPedido, string codInterno, string totM2Produto, string idProdPed)
        {
            return WebGlass.Business.Obra.Fluxo.DadosObra.Ajax.GetTamanhoMaximoProduto(idPedido, codInterno,
                totM2Produto, idProdPed);
        }

        /// <summary>
        /// Retorna o Código/Descrição do produto
        /// </summary>
        [Ajax.AjaxMethod()]
        public string GetProduto(string idPedidoStr, string codInterno, string tipoEntrega, string revenda,
            string idCliente, string percComissao, string tipoPedidoStr, string tipoVendaStr, string ambienteMaoObra,
            string percDescontoQtdeStr, string idLoja, bool produtoComposto)
        {
            return WebGlass.Business.Produto.Fluxo.BuscarEValidar.Ajax.GetProdutoPedido(idPedidoStr, codInterno,
                tipoEntrega, revenda, idCliente, percComissao, tipoPedidoStr, tipoVendaStr, ambienteMaoObra,
                percDescontoQtdeStr, idLoja, produtoComposto);
        }

        [Ajax.AjaxMethod]
        public string GetValorMinimo(string codInterno, string tipoPedido, string tipoEntrega, string tipoVenda, string idCliente,
            string revenda, string idProdPedStr, string percDescontoQtdeStr, string idPedido)
        {
            return WebGlass.Business.Produto.Fluxo.Valor.Ajax.GetValorMinimoPedido(codInterno, tipoPedido, tipoEntrega,
                tipoVenda, idCliente, revenda, idProdPedStr, percDescontoQtdeStr, idPedido);
        }

        /// <summary>
        /// Busca o cliente em tempo real
        /// </summary>
        /// <param name="idCli"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string GetCli(string idCli)
        {
            return WebGlass.Business.Cliente.Fluxo.BuscarEValidar.Ajax.GetCliPedido(idCli);
        }

        /// <summary>
        /// Verifica se o produto possui aplicação pré-cadastrada.
        /// </summary>
        /// <param name="codInterno">Código interno do produto.</param>
        /// <returns>Retorna "true" ou "false", de acordo com o cadastro de aplicação do produto.</returns>
        [Ajax.AjaxMethod]
        public string ProdutoPossuiAplPadrao(string codInterno)
        {
            try
            {
                var idProd = ProdutoDAO.Instance.ObtemIdProd(codInterno);

                return (ProdutoDAO.Instance.ObtemIdAplicacao(idProd).GetValueOrDefault() > 0).ToString().ToLower();
            }
            catch
            {
                return "false";
            }
        }

        #endregion

        #region Ambiente

        protected void grdAmbiente_RowCommand(object sender, GridViewCommandEventArgs e)
        {           
            if (e.CommandName == "ViewProd")
            {
                // Mostra os produtos relacionado ao ambiente selecionado
                hdfIdAmbiente.Value = e.CommandArgument.ToString();
                grdProdutos.Visible = true;
                grdProdutos.DataBind();

                // Mostra no label qual ambiente está sendo incluido produtos
                bool maoDeObra = IsPedidoMaoDeObra();
                AmbientePedido ambiente = AmbientePedidoDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(hdfIdAmbiente.Value));
                lblAmbiente.Text = "<br />" + ambiente.Ambiente;
                hdfAlturaAmbiente.Value = !maoDeObra ? "" : ambiente.Altura.Value.ToString();
                hdfLarguraAmbiente.Value = !maoDeObra ? "" : ambiente.Largura.Value.ToString();
                hdfQtdeAmbiente.Value = !maoDeObra ? "1" : ambiente.Qtde.Value.ToString();
                hdfRedondoAmbiente.Value = !maoDeObra ? "" : ambiente.Redondo.ToString().ToLower();
            }            
        }

        protected void grdAmbiente_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
            if (grdProdutos.Visible)
                grdProdutos.DataBind();
        }

        protected void grdAmbiente_PreRender(object sender, EventArgs e)
        {
            // Se não houver nenhum ambiente cadastrado para este pedido, esconde a primeira linha
            if (AmbientePedidoDAO.Instance.CountInPedido(Conversoes.StrParaUint(Request["idPedido"])) == 0)
                grdAmbiente.Rows[0].Visible = false;

            // Se a empresa trabalha com ambiente de pedido e não houver nenhum ambiente cadastrado, esconde grid de produtos
            var exibirAmbiente = PedidoConfig.DadosPedido.AmbientePedido || IsPedidoMaoDeObra();

            // Se a empresa não trabalha com Ambiente no pedido, esconde grdAmbiente
            grdAmbiente.Visible = exibirAmbiente;

            if (!exibirAmbiente &&
                !string.IsNullOrEmpty(Request["idPedido"]) &&
                PedidoDAO.Instance.PossuiCalculoProjeto(Request["idPedido"].StrParaUint()))
            {
                grdAmbiente.Visible = true;
                
            }
        }        

        #endregion 

        #region Fast Delivery

        protected void FastDelivery_Load(object sender, EventArgs e)
        {
            sender.GetType().GetProperty("Visible").SetValue(sender, PedidoConfig.Pedido_FastDelivery.FastDelivery, null);

            if (sender is CheckBox && ((CheckBox)sender).ID == "chkFastDelivery")
            {
                bool exibir = PedidoConfig.Pedido_FastDelivery.FastDelivery && Config.PossuiPermissao(Config.FuncaoMenuPedido.PermitirMarcarFastDelivery);
                ((CheckBox)sender).Style.Value = exibir ? "" : "display: none";
            }
        }

        #endregion

        #region Têmpera Fora

        protected void TemperaFora_Load(object sender, EventArgs e)
        {
            sender.GetType().GetProperty("Visible").SetValue(sender, PedidoConfig.TamanhoVidro.UsarTamanhoMaximoVidro, null);
        }

        #endregion

        #region Métodos usados para iniciar valores na página

        protected string GetPosValor()
        {
            if (!String.IsNullOrEmpty(Request["idPedido"]))
            {
                uint idPedido = Glass.Conversoes.StrParaUint(Request["idPedido"]);

                int tipoEntrega = PedidoDAO.Instance.ObtemTipoEntrega(idPedido);
                bool isRevenda = ClienteDAO.Instance.IsRevenda(PedidoDAO.Instance.ObtemIdCliente(idPedido));

                // Verifica qual valor será utilizado
                if (isRevenda) // Se for cliente revenda, valor de atacado
                    return "1";
                else if (tipoEntrega == 1 || tipoEntrega == 4) // Balcão ou Entrega
                    return "2";
                else if (tipoEntrega == 2 || tipoEntrega == 3 || tipoEntrega == 5 || tipoEntrega == 6) // Colocação Comum e Temperado
                    return "3";
                else
                    return "1";
            }
            else
                return "1";
        }
        
        protected bool IsPedidoMaoDeObra()
        {
            if (!String.IsNullOrEmpty(Request["idPedido"]))
                return PedidoDAO.Instance.IsMaoDeObra(Glass.Conversoes.StrParaUint(Request["idPedido"]));
            else
                return false;
        }

        protected bool IsPedidoMaoDeObraEspecial()
        {
            if (!String.IsNullOrEmpty(Request["idPedido"]))
                return PedidoDAO.Instance.IsMaoDeObraEspecial(Glass.Conversoes.StrParaUint(Request["idPedido"]));
            else
                return false;
        }

        protected bool IsPedidoProducao()
        {
            if (!String.IsNullOrEmpty(Request["idPedido"]))
                return PedidoDAO.Instance.IsProducao(Glass.Conversoes.StrParaUint(Request["idPedido"]));
            else
                return false;
        }

        #endregion

        #region ICMS/IPI

        protected void Icms_Load(object sender, EventArgs e)
        {
            var idPedido = Request["idPedido"];
            var idLoja = PedidoDAO.Instance.ObtemIdLoja(Conversoes.StrParaUint(idPedido));
            sender.GetType().GetProperty("Visible").SetValue(sender, LojaDAO.Instance.ObtemCalculaIcmsPedido(idLoja), null);
        }

        protected void Ipi_Load(object sender, EventArgs e)
        {
            var idPedido = Request["idPedido"];
            var idLoja = PedidoDAO.Instance.ObtemIdLoja(Conversoes.StrParaUint(idPedido));
            sender.GetType().GetProperty("Visible").SetValue(sender, LojaDAO.Instance.ObtemCalculaIpiPedido(idLoja), null);
        }

        #endregion

        #region Beneficiamentos

        protected void txtEspessura_DataBinding(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            GridViewRow linhaControle = txt.Parent.Parent as GridViewRow;

            ProdutosPedido prodPed = linhaControle.DataItem as ProdutosPedido;
            txt.Enabled = prodPed.Espessura <= 0;
        }

        protected void ctrlBenef_Load(object sender, EventArgs e)
        {
            Glass.UI.Web.Controls.ctrlBenef benef = (Glass.UI.Web.Controls.ctrlBenef)sender;
            GridViewRow linhaControle = benef.Parent.Parent as GridViewRow;

            var tipoPedido = PedidoDAO.Instance.GetTipoPedido(Conversoes.StrParaUint(Request["idPedido"]));

            Control codProd = null;
            if (linhaControle.FindControl("lblCodProdIns") != null)
                codProd = linhaControle.FindControl("lblCodProdIns");
            else
                codProd = linhaControle.FindControl("txtCodProdIns");

            TextBox txtAltura = (TextBox)linhaControle.FindControl("txtAlturaIns");
            TextBox txtEspessura = (TextBox)linhaControle.FindControl("txtEspessura");
            TextBox txtLargura = (TextBox)linhaControle.FindControl("txtLarguraIns");
            HiddenField hdfPercComissao = (HiddenField)dtvPedido.FindControl("hdfPercComissao");
            TextBox txtQuantidade = (TextBox)linhaControle.FindControl("txtQtdeIns");
            HiddenField hdfTipoEntrega = (HiddenField)dtvPedido.FindControl("hdfTipoEntrega");
            HiddenField hdfQtdAmbiente = new HiddenField();

            var usarQtdAmbiente = tipoPedido == Glass.Data.Model.Pedido.TipoPedidoEnum.MaoDeObra || tipoPedido == Glass.Data.Model.Pedido.TipoPedidoEnum.MaoDeObraEspecial;

            HiddenField hdfTotalM2 = null;
            if (!Beneficiamentos.UsarM2CalcBeneficiamentos)
            {
                if (linhaControle.FindControl("hdfTotM") != null)
                    hdfTotalM2 = (HiddenField)linhaControle.FindControl("hdfTotM");
                else if (linhaControle.FindControl("hdfTotM2Ins") != null)
                    hdfTotalM2 = (HiddenField)linhaControle.FindControl("hdfTotM2Ins");
            }
            else
            {
                if (linhaControle.FindControl("hdfTotM2Calc") != null)
                    hdfTotalM2 = (HiddenField)linhaControle.FindControl("hdfTotM2Calc");
                else if (linhaControle.FindControl("hdfTotM2CalcIns") != null)
                    hdfTotalM2 = (HiddenField)linhaControle.FindControl("hdfTotM2CalcIns");
            }

            TextBox txtValorIns = (TextBox)linhaControle.FindControl("txtValorIns");
            HiddenField hdfCliRevenda = (HiddenField)dtvPedido.FindControl("hdfCliRevenda");
            HiddenField hdfIdCliente = (HiddenField)dtvPedido.FindControl("hdfIdCliente");
            HiddenField hdfCustoProd = (HiddenField)linhaControle.FindControl("hdfCustoProd");

            benef.CampoAltura = txtAltura;
            benef.CampoEspessura = txtEspessura;
            benef.CampoLargura = txtLargura;
            benef.CampoPercComissao = hdfPercComissao;
            benef.CampoQuantidade = txtQuantidade;
            benef.CampoQuantidadeAmbiente = usarQtdAmbiente ? hdfQtdeAmbiente : null;
            benef.CampoTipoEntrega = hdfTipoEntrega;
            benef.CampoTotalM2 = hdfTotalM2;
            benef.CampoValorUnitario = txtValorIns;
            benef.CampoCusto = hdfCustoProd;
            benef.CampoProdutoID = codProd;
            benef.CampoRevenda = hdfCliRevenda;
            benef.CampoClienteID = hdfIdCliente;
            benef.CampoAplicacaoID = linhaControle.FindControl("hdfIdAplicacao");
            benef.CampoProcessoID = linhaControle.FindControl("hdfIdProcesso");
            benef.CampoAplicacao = linhaControle.FindControl("txtAplIns");
            benef.CampoProcesso = linhaControle.FindControl("txtProcIns");

            benef.TipoBenef = tipoPedido == Glass.Data.Model.Pedido.TipoPedidoEnum.MaoDeObraEspecial ?
                TipoBenef.MaoDeObraEspecial : TipoBenef.Venda;
        }

        #endregion

        protected void CalcularDataEntrega(ref string dataEntregaFD, ref string dataEntregaNormal, bool forcarAtualizacao)
        {
            var idPedido = Request["idPedido"] != null ? Request["idPedido"].StrParaUintNullable() : null;
            var idCli = idPedido > 0 ? PedidoDAO.Instance.GetIdCliente(idPedido.Value) : 0;
            var dataBase = idPedido > 0 ? PedidoDAO.Instance.ObtemDataPedido(idPedido.Value) : FuncionarioDAO.Instance.ObtemDataAtraso(UserInfo.GetUserInfo.CodUser);

            DateTime dataMinima, dataFastDelivery;

            if (((!IsPostBack || dtvPedido.CurrentMode == DetailsViewMode.Edit) || forcarAtualizacao) &&
                PedidoDAO.Instance.GetDataEntregaMinima(null, idCli, idPedido, out dataMinima, out dataFastDelivery))
            {
                dataEntregaFD = dataFastDelivery.ToString("dd/MM/yyyy");
                dataEntregaNormal = dataMinima.ToString("dd/MM/yyyy");
            }
        }

        protected void lblQtdeAmbiente_PreRender(object sender, EventArgs e)
        {
            ((Label)sender).Text = IsPedidoMaoDeObra() ? " x " + hdfQtdeAmbiente.Value + " peça(s) de vidro" : "";
        }

        protected string NomeControleBenef()
        {
            return grdProdutos.EditIndex == -1 ? "ctrlBenefInserir" : "ctrlBenefEditar";
        }        

        /// <summary>
        /// Mostra/Esconde campos do total bruto e líquido
        /// </summary>
        protected void lblTotalBrutoLiquido_Load(object sender, EventArgs e)
        {
            if (!Geral.NaoVendeVidro())
                ((WebControl)sender).Visible = false;
        }

        /// <summary>
        /// Mostra/Esconde campos do total geral
        /// </summary>
        protected void lblTotalGeral_Load(object sender, EventArgs e)
        {
            if (Geral.NaoVendeVidro())
                ((WebControl)sender).Visible = false;
        }

        /// <summary>
        /// Mostra/Esconde campos da loja
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Loja_Load(object sender, EventArgs e)
        {
            if (!OrdemCargaConfig.UsarControleOrdemCarga)
                ((Control)sender).Visible = false;

            if (!OrdemCargaConfig.UsarControleOrdemCarga && PedidoConfig.ExibirLoja)
            {
                ((Control)sender).Visible = true;

                if (sender is WebControl)
                    ((WebControl)sender).Enabled = false;
            }

            if (PedidoConfig.AlterarLojaPedido)
            {
                ((Control)sender).Visible = true;

                if (sender is WebControl)
                    ((WebControl)sender).Enabled = true;
            }

            if (((CheckBox)dtvPedido.FindControl("chkDeveTransferir")) != null)
                ((CheckBox)dtvPedido.FindControl("chkDeveTransferir")).Visible = PedidoConfig.ExibirOpcaoDeveTransferir;

            if (((Label)dtvPedido.FindControl("lblDeveTransferirTexto")) != null)
                ((Label)dtvPedido.FindControl("lblDeveTransferirTexto")).Visible = PedidoConfig.ExibirOpcaoDeveTransferir;

            if (((Label)dtvPedido.FindControl("lblDeveTransferirValor")) != null)
                ((Label)dtvPedido.FindControl("lblDeveTransferirValor")).Visible = PedidoConfig.ExibirOpcaoDeveTransferir;
        }

        [Ajax.AjaxMethod]
        public string PercDesconto(string idPedidoStr, string idFuncAtualStr, string alterouDesconto)
        {
            uint idPedido = Glass.Conversoes.StrParaUint(idPedidoStr);
            uint idFuncAtual = Glass.Conversoes.StrParaUint(idFuncAtualStr);
            uint idFuncDesc = Geral.ManterDescontoAdministrador ? PedidoDAO.Instance.ObtemIdFuncDesc(idPedido).GetValueOrDefault() : 0;

            return (idFuncDesc == 0 || UserInfo.IsAdministrador(idFuncAtual) || alterouDesconto.ToLower() == "true" ?
                PedidoConfig.Desconto.GetDescontoMaximoPedido(idFuncAtual, (int)PedidoDAO.Instance.GetTipoVenda(idPedido)) :
                PedidoConfig.Desconto.GetDescontoMaximoPedido(idFuncDesc, (int)PedidoDAO.Instance.GetTipoVenda(idPedido))).ToString().Replace(",", ".");
        }

        protected bool IsReposicao(object tipoVenda)
        {
            return Convert.ToInt32(tipoVenda) == (int)Glass.Data.Model.Pedido.TipoVendaPedido.Reposição;
        }

        protected bool UtilizarRoteiroProducao()
        {
            return PCPConfig.ControlarProducao && Data.Helper.Utils.GetSetores.Count(x => x.SetorPertenceARoteiro) > 0;
        }       

        protected void lblObsCliente_Load(object sender, EventArgs e)
        {
            (sender as Label).ForeColor = Color.Red;
        }
       
        protected void txtValorFrete_Load(object sender, EventArgs e)
        {
            if (!PedidoConfig.ExibirValorFretePedido)
                ((WebControl)sender).Style.Add("Display", "none");
        }
    }
}
