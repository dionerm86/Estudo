using Glass.Configuracoes;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Data.Model;
using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadPedido : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(CadPedido));
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));

            bool isMaoDeObra = IsPedidoMaoDeObra();
            bool isProducao = IsPedidoProducao();

            // Indica se o pedido é um pedido de mão de obra ou de produção
            if (!String.IsNullOrEmpty(Request["idPedido"]))
            {
                hdfPedidoMaoDeObra.Value = isMaoDeObra.ToString().ToLower();
                hdfPedidoProducao.Value = isProducao.ToString().ToLower();
            }
            else
            {
                hdfPedidoMaoDeObra.Value = (Request["maoObra"] == "1").ToString().ToLower();
                hdfPedidoProducao.Value = (Request["producao"] == "1").ToString().ToLower();
            }

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
                lbkInserirMaoObra.OnClientClick = "openWindow(screen.height, screen.width, \"../Utils/SetProdMaoObra.aspx?idPedido=" + Request["idPedido"] + "\"); return false";
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

            if (!IsPostBack && Request["idPedido"] != null)
            {
                // Se este pedido não puder ser editado, volta para lista de pedidos
                if (!PedidoDAO.Instance.GetElementByPrimaryKey(Conversoes.StrParaUint(Request["idPedido"])).EditVisible)
                {
                    Response.Redirect(RedirecionarListagemPedido());
                    return;
                }
            }

            if (dtvPedido.CurrentMode == DetailsViewMode.Insert)
            {
                if (Request["idPedido"] == null)
                {
                    if (Request["producao"] == "1")
                    {
                        ((TextBox)dtvPedido.FindControl("txtNumCli")).Text = ClienteDAO.Instance.GetClienteProducao().ToString();
                    }
                }
                else
                {
                    hdfIdPedido.Value = Request["idPedido"];
                    dtvPedido.ChangeMode(DetailsViewMode.ReadOnly);
                }
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

            // Mostra a opção de inserir projeto apenas se for pedido pedido de venda, se a empresa tiver opção de usar projeto
            // e se o pedido tiver com opção readonly
            if (!IsPostBack)
            {
                lnkProjeto.Visible = !isMaoDeObra &&
                    !isProducao && dtvPedido.CurrentMode == DetailsViewMode.ReadOnly;
            }

            // Se a empresa não trabalha com Ambiente no pedido, esconde grdAmbiente
            grdAmbiente.Visible = exibirAmbiente;

            if (!exibirAmbiente && !String.IsNullOrEmpty(Request["idPedido"]) && PedidoDAO.Instance.PossuiCalculoProjeto(Conversoes.StrParaUint(Request["idPedido"])))
            {
                grdAmbiente.Visible = true;
                grdAmbiente.ShowFooter = false;
            }

            // Se der erro ao editar, abre tela já editando
            if (Request["edit"] == "1")
                dtvPedido.ChangeMode(DetailsViewMode.Edit);

            if (Glass.Configuracoes.Geral.SistemaLite)
            {
                grdAmbiente.Columns[6].Visible = false;
                grdAmbiente.Columns[7].Visible = false;
            }
        }

        protected void grdProdutos_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
            var idPedido = Request["idPedido"].StrParaIntNullable();

            dtvPedido.DataBind();
            grdAmbiente.DataBind();
        }

        protected void grdProdutos_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
            var idPedido = Request["idPedido"].StrParaIntNullable();

            dtvPedido.DataBind();
            grdAmbiente.DataBind();
        }

        protected void grdProdutos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdProdutos.ShowFooter = e.CommandName != "Edit";
        }

        protected void grdProdutos_PreRender(object sender, EventArgs e)
        {
            string ambiente = hdfIdAmbiente.Value;

            // Se não houver nenhum produto cadastrado no pedido (e no ambiente passado)
            if (ProdutosPedidoDAO.Instance.CountInPedidoAmbiente(Conversoes.StrParaUint(Request["idPedido"]), !String.IsNullOrEmpty(ambiente) ? Conversoes.StrParaUint(ambiente) : 0) == 0)
                grdProdutos.Rows[0].Visible = false;
        }

        protected void ambMaoObra_Load(object sender, EventArgs e)
        {
            ((HtmlControl)sender).Visible = IsPedidoMaoDeObra();
        }

        protected void txtAmbiente_Load(object sender, EventArgs e)
        {
            ((TextBox)sender).Visible = !IsPedidoMaoDeObra();
        }

        protected void odsProdXPed_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg(null, e.Exception, Page);
                e.ExceptionHandled = true;
            }

            dtvPedido.DataBind();
        }

        protected void odsProdXPed_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg(null, e.Exception, Page);
                e.ExceptionHandled = true;
            }

            dtvPedido.DataBind();
        }

        #region Métodos Ajax

        [Ajax.AjaxMethod]
        public string IsProdutoObra(string idPedido, string codInterno, bool isComposicao)
        {
            return WebGlass.Business.Obra.Fluxo.DadosObra.Ajax.IsProdutoObra(idPedido, codInterno, isComposicao);
        }

        [Ajax.AjaxMethod]
        public string GetTamanhoMaximoProduto(string idPedido, string codInterno, string totM2Produto, string idProdPed)
        {
            return WebGlass.Business.Obra.Fluxo.DadosObra.Ajax.GetTamanhoMaximoProduto(idPedido, codInterno,
                totM2Produto, idProdPed);
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
        public string GetValorMinimo(string codInterno, string tipoPedido, string tipoEntrega, string tipoVenda, string idCliente,
            string revenda, string idProdPedStr, string percDescontoQtdeStr, string idPedido)
        {
            return WebGlass.Business.Produto.Fluxo.Valor.Ajax.GetValorMinimoPedido(codInterno, tipoPedido, tipoEntrega,
                tipoVenda, idCliente, revenda, idProdPedStr, percDescontoQtdeStr, idPedido);
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

        /// <summary>
        /// Retorna o total dos produtos do pedido
        /// </summary>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string TotalProdPed(string idPedido)
        {
            return WebGlass.Business.Pedido.Fluxo.ProdutosPedido.Ajax.TotalProdPed(idPedido);
        }

        [Ajax.AjaxMethod]
        public string UsarDiferencaM2Prod(string codInternoProd)
        {
            return WebGlass.Business.Produto.Fluxo.BuscarEValidar.Ajax.UsarDiferencaM2Prod(codInternoProd);
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

        [Ajax.AjaxMethod()]
        public bool SubgrupoProdComposto(int idProd)
        {
            return SubgrupoProdDAO.Instance.ObtemTipoSubgrupo(idProd) == TipoSubgrupoProd.VidroDuplo;
        }

        [Ajax.AjaxMethod()]
        public string ObterLojaSubgrupoProd(string codInterno)
        {
            var idProd = ProdutoDAO.Instance.ObtemIdProd(codInterno);
            var idsLoja = SubgrupoProdDAO.Instance.ObterIdsLojaPeloProduto(null, idProd);
            return string.Join(", ", idsLoja);
        }

        [Ajax.AjaxMethod()]
        public string ObterSubgrupoProd(string codInterno)
        {
            var idProd = ProdutoDAO.Instance.ObtemIdProd(codInterno);
            var tipoSubGrupo = SubgrupoProdDAO.Instance.ObtemTipoSubgrupo(null, idProd);
            var idTipoSubGrupo = (int)tipoSubGrupo;

            return idTipoSubGrupo.ToString();
        }

        /// <summary>
        /// Verifica se o pedido é um pedido de corte
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string GerarPedidoProducaoCorte(string idPedido)
        {
            return PedidoDAO.Instance.GerarPedidoProducaoCorte(null, idPedido.StrParaUint()).ToString().ToLower();
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

                ProdutosPedidoDAO.Instance.AtualizaObs(Conversoes.StrParaUint(idProdPed), obs);

                return "ok";
            }
            catch (Exception ex)
            {
                return "Erro;" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao salvar observação.", ex);
            }
        }

        #endregion

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
                bool maoDeObra = IsPedidoMaoDeObra();
                AmbientePedido ambiente = AmbientePedidoDAO.Instance.GetElementByPrimaryKey(Conversoes.StrParaUint(hdfIdAmbiente.Value));
                lblAmbiente.Text = "<br />" + ambiente.Ambiente;
                hdfAlturaAmbiente.Value = !maoDeObra ? "" : ambiente.Altura.Value.ToString();
                hdfLarguraAmbiente.Value = !maoDeObra ? "" : ambiente.Largura.Value.ToString();
                hdfQtdeAmbiente.Value = !maoDeObra ? "1" : ambiente.Qtde.Value.ToString();
                hdfRedondoAmbiente.Value = !maoDeObra ? "" : ambiente.Redondo.ToString().ToLower();
            }
            else if (e.CommandName == "Update")
                Glass.Validacoes.DisableRequiredFieldValidator(Page);
        }

        protected void grdAmbiente_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
            if (grdProdutos.Visible)
                grdProdutos.DataBind();

            dtvPedido.DataBind();
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
                grdAmbiente.ShowFooter = false;
            }
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

            AmbientePedido ambPed = new AmbientePedido();
            ambPed.IdPedido = Conversoes.StrParaUint(Request["idPedido"]);
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

                ambPed.Qtde = !String.IsNullOrEmpty(qtde) ? (int?)Conversoes.StrParaInt(qtde) : null;
                ambPed.Altura = !String.IsNullOrEmpty(altura) ? (int?)Conversoes.StrParaInt(altura) : null;
                ambPed.Largura = !String.IsNullOrEmpty(largura) ? (int?)Conversoes.StrParaInt(largura) : null;
                ambPed.IdProd = !String.IsNullOrEmpty(idProd) ? (uint?)Conversoes.StrParaUint(idProd) : null;
                ambPed.Redondo = ambPed.Redondo = !redondo ? ProdutoDAO.Instance.IsRedondo(Conversoes.StrParaUint(idProd)) : redondo;
                ambPed.IdAplicacao = !String.IsNullOrEmpty(idAplicacao) ? (uint?)Conversoes.StrParaUint(idAplicacao) : null;
                ambPed.IdProcesso = !String.IsNullOrEmpty(idProcesso) ? (uint?)Conversoes.StrParaUint(idProcesso) : null;

                if (ambPed.Altura != ambPed.Largura && redondo)
                    throw new Exception("O beneficiamento Redondo pode ser marcado somente em peças de medidas iguais.");
            }

            try
            {
                // Cadastra um novo ambiente para o pedido
                hdfIdAmbiente.Value = AmbientePedidoDAO.Instance.Insert(ambPed).ToString();
                lblAmbiente.Text = "<br />" + ambiente;

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
                dtvPedido.DataBind();
            }
        }

        protected void odsAmbiente_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar ambiente.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
                dtvPedido.DataBind();
        }

        #endregion

        #region Em Conferência "Pedido"

        protected void btnEmConferencia_Click(object sender, EventArgs e)
        {
            uint idPedido = Conversoes.StrParaUint(Request["idPedido"]);

            // Verifica se o Pedido possui produtos
            if (ProdutosPedidoDAO.Instance.CountInPedido(idPedido) == 0)
            {
                Glass.MensagemAlerta.ShowMsg("Inclua pelo menos um produto no pedido para finalizá-lo.", Page);
                return;
            }

            try
            {
                // Cria um registro na tabela em conferencia para este pedido
                PedidoConferenciaDAO.Instance.NovaConferencia(idPedido, PedidoDAO.Instance.ObtemIdSinal(idPedido) > 0);
                Response.Redirect(RedirecionarListagemPedido());
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg(null, ex, Page);
            }
        }

        #endregion

        #region Insere ProdutoPedido

        protected void lnkInsProd_Click(object sender, EventArgs e)
        {
            if (grdProdutos.PageCount > 1)
                grdProdutos.PageIndex = grdProdutos.PageCount - 1;

            Controls.ctrlBenef benef = (Controls.ctrlBenef)grdProdutos.FooterRow.FindControl("ctrlBenefInserir");
            bool isPedidoMaoDeObra = IsPedidoMaoDeObra();

            uint idPedido = Conversoes.StrParaUint(Request["IdPedido"]);
            int idProd = !String.IsNullOrEmpty(hdfIdProd.Value) ? Conversoes.StrParaInt(hdfIdProd.Value) : 0;
            string idAmbiente = hdfIdAmbiente.Value;
            string alturaString = ((TextBox)grdProdutos.FooterRow.FindControl("txtAlturaIns")).Text;
            string alturaRealString = ((HiddenField)grdProdutos.FooterRow.FindControl("hdfAlturaRealIns")).Value;
            string larguraString = ((TextBox)grdProdutos.FooterRow.FindControl("txtLarguraIns")).Text;
            Single altura = Conversoes.StrParaFloat(alturaString);
            Single alturaReal = Conversoes.StrParaFloat(alturaRealString);
            int largura = !String.IsNullOrEmpty(larguraString) ? Conversoes.StrParaInt(larguraString) : 0;
            string idProcessoStr = ((HiddenField)grdProdutos.FooterRow.FindControl("hdfIdProcesso")).Value;
            string idAplicacaoStr = ((HiddenField)grdProdutos.FooterRow.FindControl("hdfIdAplicacao")).Value;
            string espessuraString = ((TextBox)grdProdutos.FooterRow.FindControl("txtEspessura")).Text;
            float espessura = !String.IsNullOrEmpty(espessuraString) ? Conversoes.StrParaFloat(espessuraString) : 0;
            bool redondo = ((CheckBox)benef.FindControl("Redondo_chkSelecao")) != null ? ((CheckBox)benef.FindControl("Redondo_chkSelecao")).Checked : false;
            float aliquotaIcms = Conversoes.StrParaFloat(((HiddenField)grdProdutos.FooterRow.FindControl("hdfAliquotaIcmsProd")).Value.Replace('.', ','));
            decimal valorIcms = Conversoes.StrParaDecimal(((HiddenField)grdProdutos.FooterRow.FindControl("hdfValorIcmsProd")).Value.Replace('.', ','));
            string alturaBenefString = ((DropDownList)grdProdutos.FooterRow.FindControl("drpAltBenef")).SelectedValue;
            string larguraBenefString = ((DropDownList)grdProdutos.FooterRow.FindControl("drpLargBenef")).SelectedValue;
            float espBenef = isPedidoMaoDeObra ? Conversoes.StrParaFloat(((TextBox)grdProdutos.FooterRow.FindControl("txtEspBenef")).Text) : 0;
            int? alturaBenef = isPedidoMaoDeObra && !String.IsNullOrEmpty(alturaBenefString) ? (int?)Conversoes.StrParaInt(alturaBenefString) : null;
            int? larguraBenef = isPedidoMaoDeObra && !String.IsNullOrEmpty(larguraBenefString) ? (int?)Conversoes.StrParaInt(larguraBenefString) : null;
            decimal percComissaoProd = Conversoes.StrParaDecimal(((TextBox)grdProdutos.FooterRow.FindControl("txtComissaoProd")).Text);

            int tipoEntrega = Conversoes.StrParaInt(((HiddenField)dtvPedido.FindControl("hdfTipoEntrega")).Value);
            uint idCliente = Conversoes.StrParaUint(((HiddenField)dtvPedido.FindControl("hdfIdCliente")).Value);
            bool reposicao = bool.Parse(((HiddenField)dtvPedido.FindControl("hdfIsReposicao")).Value);

            // Cria uma instância do ProdutosPedido
            ProdutosPedido prodPed = new ProdutosPedido();
            prodPed.IdPedido = idPedido;
            prodPed.Qtde = Conversoes.StrParaFloat(((TextBox)grdProdutos.FooterRow.FindControl("txtQtdeIns")).Text.Replace('.', ','));
            prodPed.ValorVendido = Conversoes.StrParaDecimal(((TextBox)grdProdutos.FooterRow.FindControl("txtValorIns")).Text);
            prodPed.PercDescontoQtde = ((Controls.ctrlDescontoQtde)grdProdutos.FooterRow.FindControl("ctrlDescontoQtde")).PercDescontoQtde;
            prodPed.ValorTabelaPedido = ProdutoDAO.Instance.GetValorTabela(idProd, tipoEntrega, idCliente, false, reposicao, prodPed.PercDescontoQtde, (int?)idPedido, null, null);
            prodPed.Altura = altura;
            prodPed.AlturaReal = alturaReal;
            prodPed.Largura = largura;
            prodPed.IdProd = (uint)idProd;
            prodPed.Espessura = espessura;
            prodPed.Redondo = !redondo ? ProdutoDAO.Instance.IsRedondo((uint)idProd) : redondo;
            if (!String.IsNullOrEmpty(idAmbiente)) prodPed.IdAmbientePedido = Conversoes.StrParaUint(idAmbiente);
            if (!String.IsNullOrEmpty(idAplicacaoStr)) prodPed.IdAplicacao = Conversoes.StrParaUint(idAplicacaoStr);
            if (!String.IsNullOrEmpty(idProcessoStr)) prodPed.IdProcesso = Conversoes.StrParaUint(idProcessoStr);
            prodPed.AliqIcms = aliquotaIcms;
            prodPed.ValorIcms = valorIcms;

            var idLoja = PedidoDAO.Instance.ObtemIdLoja(idPedido);
            if (LojaDAO.Instance.ObtemCalculaIpiPedido(idLoja) && ClienteDAO.Instance.IsCobrarIpi(null, idCliente))
                prodPed.AliqIpi = ProdutoDAO.Instance.ObtemAliqIpi(prodPed.IdProd);

            prodPed.AlturaBenef = alturaBenef;
            prodPed.LarguraBenef = larguraBenef;
            prodPed.EspessuraBenef = espBenef;
            prodPed.Beneficiamentos = benef.Beneficiamentos;
            prodPed.PedCli = ((TextBox)grdProdutos.FooterRow.FindControl("txtPedCli")).Text;
            prodPed.IdGrupoProd = (uint)ProdutoDAO.Instance.ObtemIdGrupoProd(idProd);
            prodPed.IdSubgrupoProd = (uint)ProdutoDAO.Instance.ObtemIdSubgrupoProd(idProd).GetValueOrDefault(0);
            prodPed.PercComissao = percComissaoProd;

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
                ProdutosPedidoDAO.Instance.InsertEAtualizaDataEntrega(prodPed);

                ((HiddenField)grdProdutos.FooterRow.FindControl("hdfAlturaRealIns")).Value = "";

                grdProdutos.DataBind();
                dtvPedido.DataBind();
                grdAmbiente.DataBind();

                if (PedidoConfig.TelaCadastro.ManterCodInternoCampoAoInserirProduto)
                    ClientScript.RegisterClientScriptBlock(typeof(string), "novoProd",
                        "ultimoCodProd = '" + ProdutoDAO.Instance.GetCodInterno((int)idProd) + "';", true);

                var produto = ProdutoDAO.Instance.GetElementByPrimaryKey((uint)idProd);

                if (produto.QtdeEstoque < produto.EstoqueMinimo && produto.EstoqueMinimo > 0)
                    MensagemAlerta.ShowMsg("Quantidade em estoque deste produto está abaixo do estoque mínimo.", Page);

            }
            catch (Exception ex)
            {
                MensagemAlerta.ErrorMsg("Falha ao incluir produto no Pedido.", ex, Page);
            }
        }

        #endregion

        #region Métodos usados para iniciar valores na página

        protected string GetTotalM2Pedido()
        {
            if (!String.IsNullOrEmpty(Request["idPedido"]))
            {
                var prodPed = ProdutosPedidoDAO.Instance.GetByPedido(Conversoes.StrParaUint(Request["idPedido"]));
                float m2 = 0f;

                foreach (ProdutosPedido p in prodPed)
                    if (Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro((int)p.IdGrupoProd) && p.TipoCalc != (int)Glass.Data.Model.TipoCalculoGrupoProd.Qtd)
                        m2 += p.TotM;

                return m2.ToString().Replace(',', '.');
            }
            else
                return "0";
        }

        protected bool IsPedidoMaoDeObra()
        {
            if (!String.IsNullOrEmpty(Request["idPedido"]))
                return PedidoDAO.Instance.IsMaoDeObra(Conversoes.StrParaUint(Request["idPedido"]));
            else
                return false;
        }

        protected bool IsPedidoProducao()
        {
            if (!String.IsNullOrEmpty(Request["idPedido"]))
                return PedidoDAO.Instance.IsProducao(Conversoes.StrParaUint(Request["idPedido"]));
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
            Controls.ctrlBenef benef = (Controls.ctrlBenef)sender;
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

        protected void grdAmbiente_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao excluir ambiente.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }

        protected string GetDescontoProdutos()
        {
            try
            {
                if (!String.IsNullOrEmpty(Request["idPedido"]))
                    return Glass.Data.DAL.PedidoDAO.Instance.GetDescontoProdutos(Conversoes.StrParaUint(Request["idPedido"])).ToString().Replace(",", ".");
                else
                    return "0";
            }
            catch
            {
                return "0";
            }
        }

        protected string GetDescontoPedido()
        {
            try
            {
                if (!String.IsNullOrEmpty(Request["idPedido"]))
                    return Glass.Data.DAL.PedidoDAO.Instance.GetDescontoPedido(Conversoes.StrParaUint(Request["idPedido"])).ToString().Replace(",", ".");
                else
                    return "0";
            }
            catch
            {
                return "0";
            }
        }

        protected void txtValorIns_Load(object sender, EventArgs e)
        {
            ((TextBox)sender).Enabled = PedidoConfig.DadosPedido.AlterarValorUnitarioProduto;
        }

        protected void lblQtdeAmbiente_PreRender(object sender, EventArgs e)
        {
            ((Label)sender).Text = IsPedidoMaoDeObra() ? " x " + hdfQtdeAmbiente.Value + " peça(s) de vidro" : "";
        }

        protected void ctrlDescontoQtde_Load(object sender, EventArgs e)
        {
            Controls.ctrlDescontoQtde desc = (Controls.ctrlDescontoQtde)sender;
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

        protected string GetTipoEntrega()
        {
            uint? tipoEntrega = DataSources.Instance.GetTipoEntregaEntrega();

            return tipoEntrega > 0 ? tipoEntrega.ToString() : "null";
        }

        protected string GetTipoEntregaBalcao()
        {
            uint? tipoEntrega = DataSources.Instance.GetTipoEntregaBalcao();

            return tipoEntrega > 0 ? tipoEntrega.ToString() : "null";
        }

        protected bool IsReposicao(object tipoVenda)
        {
            return Convert.ToInt32(tipoVenda) == (int)Glass.Data.Model.Pedido.TipoVendaPedido.Reposição;
        }

        protected bool UtilizarRoteiroProducao()
        {
            return PCPConfig.ControlarProducao && Data.Helper.Utils.GetSetores.Count(x => x.SetorPertenceARoteiro) > 0;
        }

        #region Verifica se o produto pode ser inserido/atualizado/apagado

        protected void odsProdXPed_Inserting(object sender, Colosoft.WebControls.VirtualObjectDataSourceMethodEventArgs e)
        {
            var idPedido = Request["idPedido"].StrParaInt();
            if (idPedido > 0)

                if (!VerificarPodeApagarAtualizarInserir(idPedido))
                {
                    e.Cancel = true;
                    Page.ClientScript.RegisterClientScriptBlock(GetType(), "Erro", "alert('O pedido não está ativo, não é possível alterá-lo!'); " +
                "redirectUrl('" + RedirecionarListagemPedido() + "');", true);
                    MensagemAlerta.ShowMsg("O pedido não está ativo, não é possível alterá - lo.", Page);
                }
        }

        protected void odsProdXPed_Updating(object sender, Colosoft.WebControls.VirtualObjectDataSourceMethodEventArgs e)
        {
            var idPedido = Request["idPedido"].StrParaInt();
            if (idPedido > 0)

                if (!VerificarPodeApagarAtualizarInserir(idPedido))
                {
                    e.Cancel = true;
                    Page.ClientScript.RegisterClientScriptBlock(GetType(), "Erro", "alert('O pedido não está ativo, não é possível alterá-lo!'); " +
                "redirectUrl('" + RedirecionarListagemPedido() + "');", true);
                    MensagemAlerta.ShowMsg("O pedido não está ativo, não é possível alterá - lo.", Page);
                }
        }

        protected void odsProdXPed_Deleting(object sender, Colosoft.WebControls.VirtualObjectDataSourceMethodEventArgs e)
        {
            var idPedido = Request["idPedido"].StrParaInt();
            if (idPedido > 0)

                if (!VerificarPodeApagarAtualizarInserir(idPedido))
                {
                    e.Cancel = true;
                    Page.ClientScript.RegisterClientScriptBlock(GetType(), "Erro", "alert('O pedido não está ativo, não é possível alterá-lo!'); " +
                "redirectUrl('" + RedirecionarListagemPedido() + "');", true);
                    MensagemAlerta.ShowMsg("O pedido não está ativo, não é possível alterá - lo.", Page);
                }
        }

        #endregion

        #region Verifica se o ambiente pode ser inserido/atualizado/apagado

        protected void odsAmbiente_Inserting(object sender, Colosoft.WebControls.VirtualObjectDataSourceMethodEventArgs e)
        {
            var idPedido = Request["idPedido"].StrParaInt();
            if (idPedido > 0)

                if (!VerificarPodeApagarAtualizarInserir(idPedido))
                {
                    if (!VerificarPodeApagarAtualizarInserir(idPedido))
                    {
                        e.Cancel = true;
                        Page.ClientScript.RegisterClientScriptBlock(GetType(), "Erro", "alert('O pedido não está ativo, não é possível alterá-lo!'); " +
                    "redirectUrl('" + RedirecionarListagemPedido() + "');", true);
                        MensagemAlerta.ShowMsg("O pedido não está ativo, não é possível alterá - lo.", Page);
                    }
                }
        }

        protected void odsAmbiente_Updating(object sender, Colosoft.WebControls.VirtualObjectDataSourceMethodEventArgs e)
        {
            var idPedido = Request["idPedido"].StrParaInt();
            if (idPedido > 0)

                if (!VerificarPodeApagarAtualizarInserir(idPedido))
                {
                    if (!VerificarPodeApagarAtualizarInserir(idPedido))
                    {
                        e.Cancel = true;
                        Page.ClientScript.RegisterClientScriptBlock(GetType(), "Erro", "alert('O pedido não está ativo, não é possível alterá-lo!'); " +
                    "redirectUrl('" + RedirecionarListagemPedido() + "');", true);
                        MensagemAlerta.ShowMsg("O pedido não está ativo, não é possível alterá - lo.", Page);
                    }
                }
        }

        protected void odsAmbiente_Deleting(object sender, Colosoft.WebControls.VirtualObjectDataSourceMethodEventArgs e)
        {
            var idPedido = Request["idPedido"].StrParaInt();
            if (idPedido > 0)

                if (!VerificarPodeApagarAtualizarInserir(idPedido))
                {
                    if (!VerificarPodeApagarAtualizarInserir(idPedido))
                    {
                        e.Cancel = true;
                        Page.ClientScript.RegisterClientScriptBlock(GetType(), "Erro", "alert('O pedido não está ativo, não é possível alterá-lo!'); " +
                    "redirectUrl('" + RedirecionarListagemPedido() + "');", true);
                        MensagemAlerta.ShowMsg("O pedido não está ativo, não é possível alterá - lo.", Page);
                    }
                }
        }

        #endregion

        #region Verifica se a situação do pedido permite inserir/atualizar/apagar dados do mesmo

        public bool VerificarPodeApagarAtualizarInserir(int idPedido)
        {
            var situacao = PedidoDAO.Instance.ObtemSituacao((uint)idPedido);

            if (situacao != Glass.Data.Model.Pedido.SituacaoPedido.Ativo &&
                situacao != Glass.Data.Model.Pedido.SituacaoPedido.AtivoConferencia)
                return false;

            return true;


        }

        #endregion

        private string RedirecionarListagemPedido()
        {
            if (!string.IsNullOrWhiteSpace(Request["IdRelDinamico"]))
                return "../Relatorios/Dinamicos/ListaDinamico.aspx?Id=" + Request["IdRelDinamico"];
            else if (Request["ByVend"] == "1")
                return "../Listas/LstPedidos.aspx?ByVend=1";
            else
                return "../Listas/LstPedidos.aspx";
        }
    }
}
