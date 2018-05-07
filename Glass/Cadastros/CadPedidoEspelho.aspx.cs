using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Data.Model;
using System.Web.UI.HtmlControls;
using System.Linq;
using Glass.Configuracoes;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadPedidoEspelho : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadPedidoEspelho));
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            if (!IsPostBack && Request["idPedido"] != null)
            {
                var idPedido = Glass.Conversoes.StrParaUintNullable(Request["idPedido"]).GetValueOrDefault();

                if (idPedido > 0)
                {
                    var pedido = PedidoEspelhoDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(Request["idPedido"]));

                    // Se este pedido não puder ser editado, volta para lista de pedidos
                    if (pedido == null || !pedido.EditVisible)
                    {
                        Response.Redirect("../Listas/LstPedidosEspelho.aspx");
                        return;
                    }
                }
            }
    
            // Indica se o pedido é um pedido de mão de obra ou de produção
            bool isMaoDeObra = IsPedidoMaoDeObra();
            bool isProducao = IsPedidoProducao();
    
            if (Request["idPedido"] != null)
            {
                hdfTipoPedido.Value = ((int)PedidoDAO.Instance.GetTipoPedido(Glass.Conversoes.StrParaUint(Request["idPedido"]))).ToString();
                hdfPedidoMaoDeObra.Value = isMaoDeObra.ToString().ToLower();
                hdfPedidoProducao.Value = isProducao.ToString().ToLower();
            }
    
            // Indica se os produtos devem ser bloqueados de acordo com o tipo de pedido
            hdfBloquearMaoDeObra.Value = PedidoConfig.DadosPedido.BloqueioPedidoMaoDeObra.ToString().ToLower();
    
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
    
                inserirMaoObra.Visible = true;
                lbkInserirMaoObra.OnClientClick = "openWindow(500, 700, \"../Utils/SetProdMaoObra.aspx?idPedido=" + Request["idPedido"] + "&pcp=true\"); return false";
            }
    
            // Mostra a opção de inserir projeto apenas se for pedido pedido de venda, se a empresa tiver opção de usar projeto
            // e se o pedido tiver com opção readonly
            if (!IsPostBack)
            {
                uint? idObra = Request["idPedido"] != null ? PedidoDAO.Instance.GetIdObra(Glass.Conversoes.StrParaUint(Request["idPedido"])) : null;
                lnkProjeto.Visible = !isMaoDeObra &&
                    (idObra == null || idObra == 0) && !isProducao && dtvPedido.CurrentMode == DetailsViewMode.ReadOnly;
            }
    
            // Foi colocado o true para não deixar alterar acréscimo e desconto por ambiente
            if (true || !OrcamentoConfig.Desconto.DescontoAcrescimoItensOrcamento)
            {
                grdAmbiente.Columns[9].Visible = isMaoDeObra;
                grdAmbiente.Columns[10].Visible = false;
                grdAmbiente.Columns[11].Visible = false;
            }
    
            // Se a empresa trabalha com ambiente de pedido e não houver nenhum ambiente cadastrado, esconde grid de produtos
            bool exibirAmbiente = PedidoConfig.DadosPedido.AmbientePedido || IsPedidoMaoDeObra();
            grdProdutos.Visible = (exibirAmbiente && !String.IsNullOrEmpty(hdfIdAmbiente.Value) && hdfIdAmbiente.Value != "0") || !exibirAmbiente;
    
            // Se a empresa não trabalha com Ambiente no pedido, esconde grdAmbiente
            grdAmbiente.Visible = exibirAmbiente;
    
            if (!exibirAmbiente && !String.IsNullOrEmpty(Request["idPedido"]) && PedidoEspelhoDAO.Instance.PossuiCalculoProjeto(Glass.Conversoes.StrParaUint(Request["idPedido"])))
            {
                grdAmbiente.Visible = true;
                grdAmbiente.ShowFooter = false;
            }
        }
    
        protected void grdProdutos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdProdutos.ShowFooter = e.CommandName != "Edit";
        }
    
        protected void grdProdutos_PreRender(object sender, EventArgs e)
        {
            string ambiente = hdfIdAmbiente.Value;
    
            // Se não houver nenhum produto cadastrado no pedido (e no ambiente passado)
            if (ProdutosPedidoEspelhoDAO.Instance.CountInPedidoAmbiente(Glass.Conversoes.StrParaUint(Request["idPedido"]),
                !String.IsNullOrEmpty(ambiente) ? Glass.Conversoes.StrParaUint(ambiente) : 0) == 0)
                grdProdutos.Rows[0].Visible = false;
        }
    
        protected void grdProdutos_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
            dtvPedido.DataBind();
            grdAmbiente.DataBind();
        }
    
        protected void grdProdutos_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
            dtvPedido.DataBind();
            grdAmbiente.DataBind();
        }
    
        protected bool IsPedidoMaoDeObra()
        {
            if (Request["idPedido"] != null)
                return PedidoDAO.Instance.IsMaoDeObra(Glass.Conversoes.StrParaUint(Request["idPedido"]));
            else
                return false;
        }
    
        protected bool IsPedidoProducao()
        {
            if (Request["idPedido"] != null)
                return PedidoDAO.Instance.IsProducao(Glass.Conversoes.StrParaUint(Request["idPedido"]));
            else
                return false;
        }
    
        protected void ambMaoObra_Load(object sender, EventArgs e)
        {
            ((HtmlControl)sender).Visible = IsPedidoMaoDeObra();
        }
    
        protected void txtAmbiente_Load(object sender, EventArgs e)
        {
            ((TextBox)sender).Visible = !IsPedidoMaoDeObra();
        }
    
        #region Ambiente
    
        protected void grdAmbiente_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdAmbiente.ShowFooter = e.CommandName != "Edit";
    
            if (e.CommandName == "ViewProd")
            {
                // Mostra os produtos relacionado ao ambiente selecionado
                hdfIdAmbiente.Value = e.CommandArgument.ToString();
                grdProdutos.Visible = true;
                grdProdutos.DataBind();
    
                // Mostra no label qual ambiente está sendo incluido produtos
                AmbientePedidoEspelho ambiente = AmbientePedidoEspelhoDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(hdfIdAmbiente.Value));
                lblAmbiente.Text = ambiente.Ambiente;
                hdfAlturaAmbiente.Value = !IsPedidoMaoDeObra() ? "" : ambiente.Altura.Value.ToString();
                hdfLarguraAmbiente.Value = !IsPedidoMaoDeObra() ? "" : ambiente.Largura.Value.ToString();
                hdfQtdeAmbiente.Value = !IsPedidoMaoDeObra() ? "1" : ambiente.Qtde.Value.ToString();
                hdfRedondoAmbiente.Value = !IsPedidoMaoDeObra() ? "" : ambiente.Redondo.ToString().ToLower();
            }
            else if (e.CommandName == "Update")
                Glass.Validacoes.DisableRequiredFieldValidator(Page);
        }
    
        protected void grdAmbiente_PreRender(object sender, EventArgs e)
        {
            // Se não houver nenhum ambiente cadastrado para este pedido, esconde a primeira linha
            if (AmbientePedidoEspelhoDAO.Instance.CountInPedido(Glass.Conversoes.StrParaUint(Request["idPedido"])) == 0 && grdAmbiente.Rows.Count > 0)
                grdAmbiente.Rows[0].Visible = false;
        }
    
        protected void lnkInsAmbiente_Click(object sender, EventArgs e)
        {
            string ambiente = ((HiddenField)grdAmbiente.FooterRow.FindControl("hdfDescrAmbiente")).Value;
            string descricao = ((TextBox)grdAmbiente.FooterRow.FindControl("txtDescricao")).Text;
    
            if (ambiente == String.Empty)
            {
                Glass.MensagemAlerta.ShowMsg("Informe o ambiente.", Page);
                return;
            }
    
            AmbientePedidoEspelho ambPed = new AmbientePedidoEspelho();
            ambPed.IdPedido = Glass.Conversoes.StrParaUint(Request["idPedido"]);
            ambPed.Ambiente = ambiente;
            ambPed.Descricao = descricao;
    
            if (IsPedidoMaoDeObra())
            {
                string qtde = ((TextBox)grdAmbiente.FooterRow.FindControl("txtQtdeAmbiente")).Text;
                string altura = ((TextBox)grdAmbiente.FooterRow.FindControl("txtAlturaAmbiente")).Text;
                string largura = ((TextBox)grdAmbiente.FooterRow.FindControl("txtLarguraAmbiente")).Text;
                string idProd = ((HiddenField)grdAmbiente.FooterRow.FindControl("hdfAmbIdProd")).Value;
                bool redondo = ((CheckBox)grdAmbiente.FooterRow.FindControl("chkRedondoAmbiente")).Checked;
                string idAplicacao = ((HiddenField)grdAmbiente.FooterRow.FindControl("hdfAmbIdAplicacao")).Value;
                string idProcesso = ((HiddenField)grdAmbiente.FooterRow.FindControl("hdfAmbIdProcesso")).Value;
    
                ambPed.Qtde = !String.IsNullOrEmpty(qtde) ? (int?)Glass.Conversoes.StrParaInt(qtde) : null;
                ambPed.Altura = !String.IsNullOrEmpty(altura) ? (int?)Glass.Conversoes.StrParaInt(altura) : null;
                ambPed.Largura = !String.IsNullOrEmpty(largura) ? (int?)Glass.Conversoes.StrParaInt(largura) : null;
                ambPed.QtdeImpresso = 0;
                ambPed.IdProd = !String.IsNullOrEmpty(idProd) ? (uint?)Glass.Conversoes.StrParaUint(idProd) : null;
                ambPed.Redondo = redondo;
                ambPed.IdAplicacao = !String.IsNullOrEmpty(idAplicacao) ? (uint?)Glass.Conversoes.StrParaUint(idAplicacao) : null;
                ambPed.IdProcesso = !String.IsNullOrEmpty(idProcesso) ? (uint?)Glass.Conversoes.StrParaUint(idProcesso) : null;

                if (ambPed.Altura != ambPed.Largura && redondo)
                    throw new Exception("O beneficiamento Redondo pode ser marcado somente em peças de medidas iguais.");
            }
    
            try
            {
                // Cadastra um novo ambiente para o pedido
                hdfIdAmbiente.Value = AmbientePedidoEspelhoDAO.Instance.Insert(ambPed).ToString();
                lblAmbiente.Text = ambiente;
    
                hdfAlturaAmbiente.Value = ambPed.Altura != null ? ambPed.Altura.Value.ToString() : "";
                hdfLarguraAmbiente.Value = ambPed.Largura != null ? ambPed.Largura.Value.ToString() : "";
    
                grdProdutos.Visible = true;
                grdAmbiente.DataBind();
                grdProdutos.DataBind();
    
                // Esconde a 1ª linha da grdProduto, por não haver produtos cadastrados no ambiente
                grdProdutos.Rows[0].Visible = false;
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir ambiente.", ex, Page);
            }
        }
    
        protected void odsAmbiente_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao excluir ambiente.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
            {
                hdfIdAmbiente.Value = "";
                lblAmbiente.Text = "";
                grdProdutos.Visible = false;
            }
        }
    
        protected void odsAmbiente_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar ambiente.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
    
        #endregion
    
        #region Eventos
    
        protected void odsProdXPed_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg(null, e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
                dtvPedido.DataBind();
        }
    
        protected void odsProdXPed_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg(null, e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
                dtvPedido.DataBind();
        }
    
        #endregion
    
        #region Métodos Ajax
    
        [Ajax.AjaxMethod]
        public string IsProdutoObra(string idPedido, string codInterno, bool isComposicao)
        {
            return WebGlass.Business.Obra.Fluxo.DadosObra.Ajax.IsProdutoObraPcp(idPedido, codInterno, isComposicao);
        }
    
        [Ajax.AjaxMethod]
        public string GetTamanhoMaximoProduto(string idPedido, string codInterno, string totM2Produto, string idProdPed)
        {
            return WebGlass.Business.Obra.Fluxo.DadosObra.Ajax.GetTamanhoMaximoProdutoPcp(idPedido, codInterno,
                totM2Produto, idProdPed);
        }

        [Ajax.AjaxMethod]
        public string GetValorMinimo(string codInterno, string tipoPedido, string tipoEntrega, string idCliente, string revenda,
            string reposicao, string idProdPedStr, string percDescontoQtdeStr, string idPedido)
        {
            return WebGlass.Business.Produto.Fluxo.Valor.Ajax.GetValorMinimoPcp(codInterno, tipoPedido, tipoEntrega, idCliente,
                revenda, reposicao, idProdPedStr, percDescontoQtdeStr, idPedido);
        }
    
        /// <summary>
        /// Retorna o Código/Descrição do produto
        /// </summary>
        [Ajax.AjaxMethod()]
        public string GetProduto(string idPedidoStr, string codInterno, string tipoEntrega, string revenda,
            string idCliente, string tipoPedido, string ambienteMaoObra, string percDescontoQtdeStr, string idLoja)
        {
            return WebGlass.Business.Produto.Fluxo.BuscarEValidar.Ajax.GetProdutoPcp(idPedidoStr, codInterno, tipoEntrega,
                revenda, idCliente, tipoPedido, ambienteMaoObra, percDescontoQtdeStr, idLoja);
        }
    
        [Ajax.AjaxMethod]
        public string Desmembrar(string idProdPedStr, string qtdeStr)
        {
            return WebGlass.Business.PedidoEspelho.Fluxo.ProdutosPedido.Ajax.Desmembrar(idProdPedStr, qtdeStr);
        }
    
        [Ajax.AjaxMethod]
        public string VerificarProducaoSetor(string idPedidoStr, string totM2AdicionarStr, string idProcessoAdicionarStr)
        {
            try
            {
                uint idPedido = Glass.Conversoes.StrParaUint(idPedidoStr);
                float totM2Adicionar = Glass.Conversoes.StrParaFloat(totM2AdicionarStr);
                uint idProcessoAdicionar = Glass.Conversoes.StrParaUint(idProcessoAdicionarStr);
    
                PedidoEspelhoDAO.Instance.VerificaCapacidadeProducaoSetor(idPedido,
                    PedidoEspelhoDAO.Instance.ObtemDataFabrica(null, idPedido).GetValueOrDefault(), totM2Adicionar, idProcessoAdicionar);
                return "Ok";
            }
            catch (Exception ex)
            {
                return "Erro|" + Glass.MensagemAlerta.FormatErrorMsg("", ex);
            }
        }

        [Ajax.AjaxMethod()]
        public string ObterLojaSubgrupoProd(string codInterno)
        {
            var idProd = ProdutoDAO.Instance.ObtemIdProd(codInterno);
            var idLoja = SubgrupoProdDAO.Instance.ObterIdLojaPeloProduto(null, idProd);
            return idLoja.GetValueOrDefault(0).ToString();
        }

        /// <summary>
        /// Verifica se o pedido informado é um pedido de produção para corte
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string IsPedidoProducaoCorte(string idPedido)
        {
            return PedidoDAO.Instance.IsPedidoProducaoCorte(null, idPedido.StrParaUint()).ToString().ToLower();
        }

        [Ajax.AjaxMethod()]
        public string SalvaObsProdutoPedido(string idProdPed, string obs)
        {
            try
            {
                if (String.IsNullOrEmpty(idProdPed) || Conversoes.StrParaInt(idProdPed) == 0)
                    return "Erro;O produto não foi informado. informe o produto";

                ProdutosPedidoEspelhoDAO.Instance.AtualizaObs(Conversoes.StrParaUint(idProdPed), obs);

                return "ok";
            }
            catch (Exception ex)
            {
                return "Erro;" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao salvar observação.", ex);
            }
        }

        #endregion

        #region Finalizar Pedido

        protected void btnFinalizar_Click(object sender, EventArgs e)
        {
            var idPedidoEspelho = Glass.Conversoes.StrParaUint(Request["idPedido"]);

            try
            {
                WebGlass.Business.PedidoEspelho.Fluxo.Finalizar.Instance.FinalizarPedido(idPedidoEspelho);

                Response.Redirect("../Listas/LstPedidosEspelho.aspx", false);
            }
            catch (Exception ex)
            {
                var urlErro = Request.Url.ToString() == null || Request.Url.ToString() == "" ? "Finalizar Pedido Espelho" : Request.Url.ToString();
                ErroDAO.Instance.InserirFromException(urlErro, ex);
                Glass.MensagemAlerta.ErrorMsg("Falha ao finalizar Pedido.", ex, this);

                // Se o pedido estiver na situação Finalizado então o usuário não pode continuar na tela de cadastro do pedido,
                // por isso ele será redirecionado para a listagem de pedidos espelho ou para a tela de gerar pedido espelho.
                if (PedidoEspelhoDAO.Instance.ObtemSituacao(idPedidoEspelho) == PedidoEspelho.SituacaoPedido.Finalizado)
                {
                    Response.Redirect("../Listas/LstPedidosEspelho.aspx?dir=" + ex.Message, false);
                }

                return;
            }
        }
    
        #endregion
    
        #region Gera valor excedente da conferência
    
        /// <summary>
        /// Gera valor excedente da conferência
        /// </summary>
        protected void btnExcedente_Click(object sender, EventArgs e)
        {
            try
            {
                WebGlass.Business.PedidoEspelho.Fluxo.Finalizar.Instance.GerarValorExcedente(Glass.Conversoes.StrParaUint(Request["idPedido"]));
                Glass.MensagemAlerta.ShowMsg("Valor excedente gerado.", Page);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao gerar valor excedente do Pedido.", ex, Page);
                return;
            }
        }
    
        #endregion
    
        #region Insere ProdutoPedido
    
        protected void lnkInsProd_Click(object sender, EventArgs e)
        {
            bool isPedidoMaoDeObra = IsPedidoMaoDeObra();
    
            // Cria uma instância do ProdutosPedido
            ProdutosPedidoEspelho prodPed = new ProdutosPedidoEspelho();
            prodPed.IdPedido = Glass.Conversoes.StrParaUint(Request["IdPedido"]);
            prodPed.IdAmbientePedido = !String.IsNullOrEmpty(hdfIdAmbiente.Value) ? (uint?)Glass.Conversoes.StrParaUint(hdfIdAmbiente.Value) : null;
            prodPed.Qtde = float.Parse(((TextBox)grdProdutos.FooterRow.FindControl("txtQtdeIns")).Text.Replace('.', ','));
            prodPed.ValorVendido = Glass.Conversoes.StrParaDecimal(((HiddenField)grdProdutos.FooterRow.FindControl("hdfValorIns")).Value);
            prodPed.Altura = Glass.Conversoes.StrParaFloat(((HiddenField)grdProdutos.FooterRow.FindControl("hdfAlturaCalcIns")).Value);
            prodPed.AlturaReal = Glass.Conversoes.StrParaFloat(((TextBox)grdProdutos.FooterRow.FindControl("txtAlturaIns")).Text);
            prodPed.Largura = Glass.Conversoes.StrParaInt(((TextBox)grdProdutos.FooterRow.FindControl("txtLarguraIns")).Text);
            prodPed.LarguraReal = Glass.Conversoes.StrParaInt(((HiddenField)grdProdutos.FooterRow.FindControl("hdfLarguraCalc")).Value);
            prodPed.IdProd = Glass.Conversoes.StrParaUint(hdfIdProd.Value);
            prodPed.IdAplicacao = Glass.Conversoes.StrParaUintNullable(((HiddenField)grdProdutos.FooterRow.FindControl("hdfIdAplicacao")).Value);
            prodPed.IdProcesso = Glass.Conversoes.StrParaUintNullable(((HiddenField)grdProdutos.FooterRow.FindControl("hdfIdProcesso")).Value);
            prodPed.Espessura = Glass.Conversoes.StrParaInt(((TextBox)grdProdutos.FooterRow.FindControl("txtEspessura")).Text);
            prodPed.AlturaBenef = isPedidoMaoDeObra ? Glass.Conversoes.StrParaIntNullable(((DropDownList)grdProdutos.FooterRow.FindControl("drpAltBenef")).SelectedValue) : null;
            prodPed.LarguraBenef = isPedidoMaoDeObra ? Glass.Conversoes.StrParaIntNullable(((DropDownList)grdProdutos.FooterRow.FindControl("drpLargBenef")).SelectedValue) : null;
            prodPed.EspessuraBenef = isPedidoMaoDeObra ? Glass.Conversoes.StrParaFloat(((TextBox)grdProdutos.FooterRow.FindControl("txtEspBenef")).Text) : 0;
            prodPed.Beneficiamentos = ((Glass.UI.Web.Controls.ctrlBenef)grdProdutos.FooterRow.FindControl("ctrlBenefInserir")).Beneficiamentos;
            prodPed.PedCli = ((TextBox)grdProdutos.FooterRow.FindControl("txtPedCli")).Text;
            prodPed.AliqIcms = Convert.ToSingle(Math.Round(Glass.Conversoes.StrParaFloat(((HiddenField)grdProdutos.FooterRow.FindControl("hdfAliquotaIcmsProd")).Value), 2));
            prodPed.ValorIcms = Glass.Conversoes.StrParaDecimal(((HiddenField)grdProdutos.FooterRow.FindControl("hdfValorIcmsProd")).Value);
            prodPed.PercDescontoQtde = ((Glass.UI.Web.Controls.ctrlDescontoQtde)grdProdutos.FooterRow.FindControl("ctrlDescontoQtde")).PercDescontoQtde;
            prodPed.IdGrupoProd = (uint)ProdutoDAO.Instance.ObtemIdGrupoProd((int)prodPed.IdProd);
            prodPed.IdSubgrupoProd = (uint)ProdutoDAO.Instance.ObtemIdSubgrupoProd((int)prodPed.IdProd);
    
            if (prodPed.Largura == 0 && prodPed.LarguraReal > 0)
                prodPed.Largura = prodPed.LarguraReal;
    
            try
            {
                ProdutosPedidoEspelhoDAO.Instance.InsertComTransacao(prodPed);
    
                if (grdProdutos.PageCount > 1)
                    grdProdutos.PageIndex = grdProdutos.PageCount - 1;
    
                grdProdutos.DataBind();
                dtvPedido.DataBind();
                grdAmbiente.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao incluir produto no Pedido.", ex, Page);
                return;
            }
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
    
            ProdutosPedidoEspelho prodPed = linhaControle.DataItem as ProdutosPedidoEspelho;
            txt.Enabled = prodPed.Espessura <= 0;
        }
    
        protected void ctrlBenef_Load(object sender, EventArgs e)
        {
            Glass.UI.Web.Controls.ctrlBenef benef = (Glass.UI.Web.Controls.ctrlBenef)sender;
            GridViewRow linhaControle = benef.Parent.Parent as GridViewRow;
    
            Control codProd = null;
            if (linhaControle.FindControl("lblCodProdIns") != null)
                codProd = linhaControle.FindControl("lblCodProdIns");
            else
                codProd = linhaControle.FindControl("txtCodProdIns");
            HiddenField hdfAlturaCalc = (HiddenField)linhaControle.FindControl("hdfAlturaCalcIns");
            TextBox txtEspessura = (TextBox)linhaControle.FindControl("txtEspessura");
            TextBox txtLargura = (TextBox)linhaControle.FindControl("txtLarguraIns");
            HiddenField hdfPercComissao = (HiddenField)dtvPedido.FindControl("hdfPercComissao");
            TextBox txtQuantidade = (TextBox)linhaControle.FindControl("txtQtdeIns");
            HiddenField hdfTipoEntrega = (HiddenField)dtvPedido.FindControl("hdfTipoEntrega");
            HiddenField hdfCustoProd = (HiddenField)linhaControle.FindControl("hdfCustoProd");
    
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
    
            Label lblValorIns = (Label)linhaControle.FindControl("lblValorIns");
            HiddenField hdfCliRevenda = (HiddenField)dtvPedido.FindControl("hdfCliRevenda");
            HiddenField hdfIdCliente = (HiddenField)dtvPedido.FindControl("hdfIdCliente");
    
            benef.CampoAltura = hdfAlturaCalc;
            benef.CampoEspessura = txtEspessura;
            benef.CampoLargura = txtLargura;
            benef.CampoPercComissao = hdfPercComissao;
            benef.CampoQuantidade = txtQuantidade;
            benef.CampoTipoEntrega = hdfTipoEntrega;
            benef.CampoTotalM2 = hdfTotalM2;
            benef.CampoValorUnitario = lblValorIns;
            benef.CampoProdutoID = codProd;
            benef.CampoRevenda = hdfCliRevenda;
            benef.CampoClienteID = hdfIdCliente;
            benef.CampoAplicacaoID = linhaControle.FindControl("hdfIdAplicacao");
            benef.CampoProcessoID = linhaControle.FindControl("hdfIdProcesso");
            benef.CampoAplicacao = linhaControle.FindControl("txtAplIns");
            benef.CampoProcesso = linhaControle.FindControl("txtProcIns");
    
            benef.TipoBenef = PedidoDAO.Instance.GetTipoPedido(Conversoes.StrParaUint(Request["idPedido"])) == Glass.Data.Model.Pedido.TipoPedidoEnum.MaoDeObraEspecial ?
                TipoBenef.MaoDeObraEspecial : TipoBenef.Venda;
        }
    
        #endregion
    
        protected void grdAmbiente_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao excluir ambiente.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
    
        protected void btnExcedente_Load(object sender, EventArgs e)
        {
            ((Button)dtvPedido.FindControl("btnExcedente")).Visible = GerarCreditoValorExcedente();
        }
    
        protected void lblQtdeAmbiente_PreRender(object sender, EventArgs e)
        {
            ((Label)sender).Text = IsPedidoMaoDeObra() ? " x " + hdfQtdeAmbiente.Value + " peça(s) de vidro" : "";
        }
    
        protected string NomeControleBenef()
        {
            return grdProdutos.EditIndex == -1 ? "ctrlBenefInserir" : "ctrlBenefEditar";
        }
    
        protected void ctrlDescontoQtde_Load(object sender, EventArgs e)
        {
            Glass.UI.Web.Controls.ctrlDescontoQtde desc = (Glass.UI.Web.Controls.ctrlDescontoQtde)sender;
            GridViewRow linha = desc.Parent.Parent as GridViewRow;
    
            desc.CampoQtde = linha.FindControl("txtQtdeIns");
            desc.CampoProdutoID = linha.FindControl("hdfIdProd");
            desc.CampoClienteID = dtvPedido.FindControl("hdfIdCliente");
            desc.CampoTipoEntrega = dtvPedido.FindControl("hdfTipoEntrega");
            desc.CampoRevenda = dtvPedido.FindControl("hdfCliRevenda");
            desc.CampoReposicao = dtvPedido.FindControl("hdfIsReposicao");
            desc.CampoValorUnit = linha.FindControl("txtValorIns");
    
            if (desc.CampoProdutoID == null)
                desc.CampoProdutoID = hdfIdProd;
        }
    
        protected bool IsExportacaoOptyWay()
        {
            return EtiquetaConfig.TipoExportacaoEtiqueta == DataSources.TipoExportacaoEtiquetaEnum.OptyWay;
        }

        protected bool PerguntarGerarCreditoAoFinalizar()
        {
            return PCPConfig.TelaCadastro.PerguntarGerarCreditoAoFinalizar;
        }

        protected void dtvPedido_ItemCommand(object sender, DetailsViewCommandEventArgs e)
        {
            if (e.CommandName == "Atualizar")
            {
                try
                {
                    var pe = new PedidoEspelho();
                    pe.IdPedido = Glass.Conversoes.StrParaUint(Request["idPedido"]);
                    
                    var pedidoEspelho = PedidoEspelhoDAO.Instance.GetElementByPrimaryKey(null, pe.IdPedido);

                    pe.TipoDesconto = ((DropDownList)dtvPedido.FindControl("drpTipoDesconto")) != null ? ((DropDownList)dtvPedido.FindControl("drpTipoDesconto")).SelectedValue.StrParaInt() : pedidoEspelho.TipoDesconto;
                    pe.Desconto = ((TextBox)dtvPedido.FindControl("txtDesconto")) != null ? ((TextBox)dtvPedido.FindControl("txtDesconto")).Text.StrParaDecimal() : pedidoEspelho.Desconto;
                    pe.TipoAcrescimo = ((DropDownList)dtvPedido.FindControl("drpTipoAcrescimo")) != null ? ((DropDownList)dtvPedido.FindControl("drpTipoAcrescimo")).SelectedValue.StrParaInt() : pedidoEspelho.TipoAcrescimo;
                    pe.Acrescimo = ((TextBox)dtvPedido.FindControl("txtAcrescimo")) != null ? ((TextBox)dtvPedido.FindControl("txtAcrescimo")).Text.StrParaDecimal() : pedidoEspelho.Acrescimo;
                    pe.DataFabrica = ((Glass.UI.Web.Controls.ctrlData)dtvPedido.FindControl("ctrlDataFabrica")) != null ? ((Glass.UI.Web.Controls.ctrlData)dtvPedido.FindControl("ctrlDataFabrica")).Data : pedidoEspelho.DataFabrica;                    
                    pe.Obs = ((TextBox)dtvPedido.FindControl("txtObs")) != null ? ((TextBox)dtvPedido.FindControl("txtObs")).Text : pedidoEspelho.Obs;
                    pe.IdComissionado = ((HiddenField)dtvPedido.FindControl("hdfIdComissionado")) != null ? ((HiddenField)dtvPedido.FindControl("hdfIdComissionado")).Value.StrParaUintNullable() : pedidoEspelho.IdComissionado;
                    pe.PercComissao = ((HiddenField)dtvPedido.FindControl("hdfPercComissao")) != null ? ((HiddenField)dtvPedido.FindControl("hdfPercComissao")).Value.StrParaFloat() : pedidoEspelho.PercComissao;
    
                    PedidoEspelhoDAO.Instance.UpdateDados(null, pe);
    
                    dtvPedido.DataBind();
                    grdAmbiente.DataBind();
                    grdProdutos.DataBind();
    
                    Glass.MensagemAlerta.ShowMsg("Pedido atualizado/recalculado com sucesso!", Page);
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar/recalcular o pedido.", ex, Page);
                }
            }
        }
    
        [Ajax.AjaxMethod]
        public string PercDesconto(string idPedidoStr, string idFuncAtualStr, string alterouDesconto)
        {
            uint idPedido = Glass.Conversoes.StrParaUint(idPedidoStr);
            uint idFuncAtual = Glass.Conversoes.StrParaUint(idFuncAtualStr);
            uint idFuncDesc = Geral.ManterDescontoAdministrador ? PedidoDAO.Instance.ObtemIdFuncDesc(idPedido).GetValueOrDefault() : 0;
    
            return (idFuncDesc == 0 || UserInfo.IsAdministrador(idFuncAtual) || alterouDesconto.ToLower() == "true" ?
                PedidoConfig.Desconto.GetDescontoMaximoPedido(idFuncAtual, (int)PedidoDAO.Instance.GetTipoVenda(idPedido), (int)PedidoDAO.Instance.ObtemIdParcela(idPedido)) :
                PedidoConfig.Desconto.GetDescontoMaximoPedido(idFuncDesc, (int)PedidoDAO.Instance.GetTipoVenda(idPedido), (int)PedidoDAO.Instance.ObtemIdParcela(idPedido))).ToString().Replace(",", ".");
        }
    
        protected void txtPercentual_Load(object sender, EventArgs e)
        {
            if (PedidoConfig.Comissao.UsarComissionadoCliente)
                ((TextBox)sender).Style.Add("display", "none");
            else
                ((TextBox)sender).Enabled = PedidoConfig.Comissao.AlterarPercComissionado;
        }
            
        protected bool IsAVista(object tipoVenda)
        {
            return Convert.ToInt32(tipoVenda) == (int)Glass.Data.Model.Pedido.TipoVendaPedido.AVista;
        }
    
        protected bool IsReposicao(object tipoVenda)
        {
            return Convert.ToInt32(tipoVenda) == (int)Glass.Data.Model.Pedido.TipoVendaPedido.Reposição;
        }
    
        protected int CodigoTipoPedidoMaoObraEspecial()
        {
            return (int)Glass.Data.Model.Pedido.TipoPedidoEnum.MaoDeObraEspecial;
        }
    
        protected bool UtilizarRoteiroProducao()
        {
            return Data.Helper.Utils.GetSetores.Count(x => x.SetorPertenceARoteiro) > 0;
        }

        protected bool GerarCreditoValorExcedente()
        {
            return PCPConfig.TelaCadastro.GerarCreditoValorExcedente;
        }

        #region Verifica se o produto pode ser inserido/atualizado/apagado

        protected void odsProdXPed_Inserting(object sender, Colosoft.WebControls.VirtualObjectDataSourceMethodEventArgs e)
        {
            var idPedido = Request["idPedido"].StrParaInt();

            if (idPedido > 0)
            {
                if (!VerificarPodeApagarAtualizarInserir(idPedido))
                {
                    e.Cancel = true;
                    Page.ClientScript.RegisterClientScriptBlock(GetType(), "Erro", "alert('O pedido não está ativo, não é possível alterá-lo!'); " +
                "redirectUrl('../Listas/LstPedidos.aspx');", true);
                    MensagemAlerta.ShowMsg("O pedido não está ativo, não é possível alterá - lo.", Page);
                }
            }
        }

        protected void odsProdXPed_Updating(object sender, Colosoft.WebControls.VirtualObjectDataSourceMethodEventArgs e)
        {
            var idPedido = Request["idPedido"].StrParaInt();

            if (idPedido > 0)
            {
                if (!VerificarPodeApagarAtualizarInserir(idPedido))
                {
                    e.Cancel = true;
                    Page.ClientScript.RegisterClientScriptBlock(GetType(), "Erro", "alert('O pedido não está ativo, não é possível alterá-lo!'); " +
                "redirectUrl('../Listas/LstPedidos.aspx');", true);
                    MensagemAlerta.ShowMsg("O pedido não está ativo, não é possível alterá - lo.", Page);
                }
            }
        }

        protected void odsProdXPed_Deleting(object sender, Colosoft.WebControls.VirtualObjectDataSourceMethodEventArgs e)
        {
            var idPedido = Request["idPedido"].StrParaInt();

            if (idPedido > 0)
            {
                if (!VerificarPodeApagarAtualizarInserir(idPedido))
                {
                    e.Cancel = true;
                    Page.ClientScript.RegisterClientScriptBlock(GetType(), "Erro", "alert('O pedido não está ativo, não é possível alterá-lo!'); " +
                "redirectUrl('../Listas/LstPedidos.aspx');", true);
                    MensagemAlerta.ShowMsg("O pedido não está ativo, não é possível alterá - lo.", Page);
                }
            }
        }

        #endregion

        #region Verifica se o ambiente pode ser inserido/atualizado/apagado

        protected void odsAmbiente_Inserting(object sender, Colosoft.WebControls.VirtualObjectDataSourceMethodEventArgs e)
        {
            var idPedido = Request["idPedido"].StrParaInt();

            if (idPedido > 0)
            {
                if (!VerificarPodeApagarAtualizarInserir(idPedido))
                {
                    e.Cancel = true;
                    Page.ClientScript.RegisterClientScriptBlock(GetType(), "Erro", "alert('O pedido não está ativo, não é possível alterá-lo!'); " +
                "redirectUrl('../Listas/LstPedidos.aspx');", true);
                    MensagemAlerta.ShowMsg("O pedido não está ativo, não é possível alterá - lo.", Page);
                }
            }
        }

        protected void odsAmbiente_Updating(object sender, Colosoft.WebControls.VirtualObjectDataSourceMethodEventArgs e)
        {
            var idPedido = Request["idPedido"].StrParaInt();

            if (idPedido > 0)
            {
                if (!VerificarPodeApagarAtualizarInserir(idPedido))
                {
                    e.Cancel = true;
                    Page.ClientScript.RegisterClientScriptBlock(GetType(), "Erro", "alert('O pedido não está ativo, não é possível alterá-lo!'); " +
                "redirectUrl('../Listas/LstPedidos.aspx');", true);
                    MensagemAlerta.ShowMsg("O pedido não está ativo, não é possível alterá - lo.", Page);
                }
            }
        }

        protected void odsAmbiente_Deleting(object sender, Colosoft.WebControls.VirtualObjectDataSourceMethodEventArgs e)
        {
            var idPedido = Request["idPedido"].StrParaInt();

            if (idPedido > 0)
            {
                if (!VerificarPodeApagarAtualizarInserir(idPedido))
                {
                    e.Cancel = true;
                    Page.ClientScript.RegisterClientScriptBlock(GetType(), "Erro", "alert('O pedido não está ativo, não é possível alterá-lo!'); " +
                "redirectUrl('../Listas/LstPedidos.aspx');", true);
                    MensagemAlerta.ShowMsg("O pedido não está ativo, não é possível alterá - lo.", Page);
                }
            }
        }

        #endregion

        #region Verifica se o pedido pode ser atualizado

        protected void odsPedido_Updating(object sender, Colosoft.WebControls.VirtualObjectDataSourceMethodEventArgs e)
        {
            var idPedido = Request["idPedido"].StrParaInt();

            if (idPedido > 0)
            {
                if (!VerificarPodeApagarAtualizarInserir(idPedido))
                {
                    e.Cancel = true;
                    Page.ClientScript.RegisterClientScriptBlock(GetType(), "Erro", "alert('O pedido não está ativo, não é possível alterá-lo!'); " +
                "redirectUrl('../Listas/LstPedidos.aspx');", true);
                    MensagemAlerta.ShowMsg("O pedido não está ativo, não é possível alterá - lo.", Page);
                }
            }

        }

        #endregion

        #region Verifica se a situação do pedido espelho permite inserir/atualizar/apagar dados do mesmo

        public bool VerificarPodeApagarAtualizarInserir(int idPedido)
        {
            var situacao = PedidoEspelhoDAO.Instance.ObtemSituacao((uint)idPedido);

            if (situacao != PedidoEspelho.SituacaoPedido.Aberto &&
                situacao != PedidoEspelho.SituacaoPedido.ImpressoComum)
                return false;

            return true;
           
        }

        #endregion

        protected void LblValorFrete_Load(object sender, EventArgs e)
        {
            if (!PedidoConfig.ExibirValorFretePedido)
                ((WebControl)sender).Style.Add("Display", "none");
        }
    }
}
