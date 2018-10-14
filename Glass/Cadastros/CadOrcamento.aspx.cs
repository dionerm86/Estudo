using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Data.Model;
using System.IO;
using Glass.Configuracoes;
using System.Drawing;
using System.Collections.Generic;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadOrcamento : System.Web.UI.Page
    {
        private byte relatorio = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadOrcamento));
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(RecalcularOrcamento));

            hdfNaoVendeVidro.Value = Glass.Configuracoes.Geral.NaoVendeVidro().ToString().ToLower();
            grdProdutos.Columns[6].Visible = OrcamentoConfig.NegociarParcialmente;

            if (!IsPostBack && Request["idOrca"] != null && dtvOrcamento.CurrentMode != DetailsViewMode.ReadOnly)
            {
                hdfIdOrca.Value = Request["idOrca"];
                dtvOrcamento.ChangeMode(DetailsViewMode.ReadOnly);
                dtvOrcamento.DataBind();

                var orca = dtvOrcamento.DataItem as Data.Model.Orcamento;

                if (Request["atualizar"] == "1")
                {
                    OrcamentoDAO.Instance.Update(orca);
                    Response.Redirect(Request.Url.ToString().Replace("&atualizar=1", ""));
                    return;
                }

                // Se o usuário não tiver permissão para editar este orçamento, retorna para listagem de orçamentos
                if ((((HiddenField)dtvOrcamento.FindControl("hdfEditVisible")) != null &&
                    ((HiddenField)dtvOrcamento.FindControl("hdfEditVisible")).Value.ToLower() == "false") ||
                    !orca.EditVisible)
                    RedirecionarListagemOrcamento();


                if (Request["idOrca"] != null && !String.IsNullOrEmpty(Request["relatorio"]))
                {
                    switch (Request["relatorio"])
                    {
                        case "1":
                            Page.ClientScript.RegisterStartupScript(GetType(), "Relatorio", "openRpt('" + Request["idOrca"] + "');\n", true);
                            break;

                        case "2":
                            Page.ClientScript.RegisterStartupScript(GetType(), "Relatorio", "openRptMemoria('" + Request["idOrca"] + "');\n", true);
                            break;
                    }
                }
            }
            else if (!IsPostBack)
            {
                if (dtvOrcamento.CurrentMode == DetailsViewMode.Insert)
                {
                    if (((TextBox)dtvOrcamento.FindControl("txtPrazoEntregaIns")).Text == String.Empty)
                        ((TextBox)dtvOrcamento.FindControl("txtPrazoEntregaIns")).Text = OrcamentoConfig.DadosOrcamento.PrazoEntregaOrcamento;

                    if (((TextBox)dtvOrcamento.FindControl("txtValidadeIns")).Text == String.Empty)
                        ((TextBox)dtvOrcamento.FindControl("txtValidadeIns")).Text = OrcamentoConfig.DadosOrcamento.ValidadeOrcamento;

                    if (((TextBox)dtvOrcamento.FindControl("txtFormaPagtoIns")).Text == String.Empty)
                        ((TextBox)dtvOrcamento.FindControl("txtFormaPagtoIns")).Text = OrcamentoConfig.DadosOrcamento.FormaPagtoOrcamento;

                    ((DropDownList)dtvOrcamento.FindControl("drpFuncionario")).SelectedValue = UserInfo.GetUserInfo.CodUser.ToString();

                    ((DropDownList)dtvOrcamento.FindControl("drpTipoOrcamento")).SelectedValue = OrcamentoConfig.TelaCadastro.TipoOrcamentoPadrao == null ? "0" : ((int)OrcamentoConfig.TelaCadastro.TipoOrcamentoPadrao).ToString();
                }
            }

            hdfComissaoVisible.Value = PedidoConfig.Comissao.ComissaoPedido.ToString().ToLower();
            lnkProduto.Visible = grdProdutos.Visible && !Glass.Configuracoes.Geral.NaoVendeVidro();
            lnkProjeto.Visible = lnkProduto.Visible;
            grdProdutos.ShowFooter = !lnkProduto.Visible;

            grdProdutos.Columns[7].Visible = !grdProdutos.ShowFooter && OrcamentoConfig.Desconto.DescontoAcrescimoItensOrcamento;
            grdProdutos.Columns[8].Visible = !grdProdutos.ShowFooter && OrcamentoConfig.Desconto.DescontoAcrescimoItensOrcamento;
        }

        #region Métodos Ajax

        [Ajax.AjaxMethod]
        public string GetCli(string idCli)
        {
            return WebGlass.Business.Orcamento.Fluxo.BuscarEValidar.Ajax.GetCli(idCli);
        }

        #endregion

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            RedirecionarListagemOrcamento();
        }

        protected void btnCancelarEditar_Click(object sender, EventArgs e)
        {
            Response.Redirect(Request.Url.ToString());
        }

        protected void odsOrcamento_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                if (e.Exception.ToString().Contains("BLOQUEIO_ORCAMENTO"))
                {
                    Glass.MensagemAlerta.ShowMsg("Já existe orçamento cadastrado " +
                        "com estes mesmos dados em um determinado período de tempo e sendo necessário aguardar 1 minuto ou alterar dados do orçamento", Page);
                    e.ExceptionHandled = true;
                }
                else
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao cadastrar Orçamento.", e.Exception, Page);
                    e.ExceptionHandled = true;
                }
            }
            else
            {
                hdfIdOrca.Value = e.ReturnValue.ToString();
                Response.Redirect("CadOrcamento.aspx?IdOrca=" + hdfIdOrca.Value);
            }
        }

        protected void odsOrcamento_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                /*Chamado 65525 */
                if (e.Exception.ToString().Contains("BLOQUEIO_ORCAMENTO"))
                {
                    e.ExceptionHandled = true;
                    Response.Write("<script language = javascript > alert('Já existe orçamento cadastrado " +
                        "com estes mesmos dados em um determinado período de tempo e sendo necessário aguardar 1 minuto ou alterar dados do orçamento'); history.go(-1); </script>");
                }
                else
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar dados do Orçamento.", e.Exception, Page);
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "callback", "history.go(-1);", true);
                    e.ExceptionHandled = true;
                }
            }
            else
                Response.Redirect("CadOrcamento.aspx?IdOrca=" + hdfIdOrca.Value + (relatorio > 0 ? "&relatorio=" + relatorio : ""));
        }

        protected void ImageButton1_DataBinding(object sender, EventArgs e)
        {
            GridViewRow linha = ((ImageButton)sender).Parent.Parent as GridViewRow;
            ProdutosOrcamento item = linha.DataItem as ProdutosOrcamento;

            if (item == null)
            {
                ((ImageButton)sender).Visible = false;
                return;
            }

            if (item.IdProdPed != null)
            {
                ((ImageButton)sender).Visible = false;
                return;
            }

            ((ImageButton)sender).Visible = lnkProduto.Visible;
            ((ImageButton)sender).OnClientClick = "return openProdutos('" + item.IdProd + "', false);";
        }

        protected void grdAmbiente_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.Cells[0].FindControl("hdfIdAmbiente") != null)
            {
                uint idAmbiente = Glass.Conversoes.StrParaUint(((HiddenField)e.Row.Cells[0].FindControl("hdfIdAmbiente")).Value);
                if (idAmbiente == 0)
                    e.Row.Visible = false;
            }
        }

        protected void ctrlParcelasSelecionar1_Load(object sender, EventArgs e)
        {
            Glass.UI.Web.Controls.ctrlParcelasSelecionar parcSel = (Glass.UI.Web.Controls.ctrlParcelasSelecionar)sender;
            parcSel.ControleParcelas = dtvOrcamento.FindControl("ctrlParcelas1") as Glass.UI.Web.Controls.ctrlParcelas;
            parcSel.CampoClienteID = dtvOrcamento.FindControl("txtIdCliente");
        }

        protected void hdfDataBase_Load(object sender, EventArgs e)
        {
            ((HiddenField)sender).Value = DateTime.Now.ToString("dd/MM/yyyy");
        }

        protected void drpTipoPedido_DataBound(object sender, EventArgs e)
        {

            try
            {
                ((DropDownList)sender).Items.FindByValue(((int)Glass.Data.Model.Pedido.TipoPedidoEnum.MaoDeObra).ToString()).Enabled = false;
                ((DropDownList)sender).Items.FindByValue(((int)Glass.Data.Model.Pedido.TipoPedidoEnum.Producao).ToString()).Enabled = false;

                List<ListItem> colecao = new List<ListItem>();

                if (PedidoConfig.DadosPedido.BloquearItensTipoPedido)
                {
                    string tipoPedido = FuncionarioDAO.Instance.ObtemTipoPedido(UserInfo.GetUserInfo.CodUser);
                    string[] values = null;

                    if (!String.IsNullOrEmpty(tipoPedido))
                        values = tipoPedido.Split(',');

                    if (values != null)
                        foreach (string v in values)
                        {
                            if (((DropDownList)sender).Items.FindByValue(v) != null)
                                colecao.Add(((DropDownList)sender).Items.FindByValue(v));
                        }

                    ((DropDownList)sender).Items.Clear();
                    ((DropDownList)sender).Items.AddRange(colecao.ToArray());
                }
                else
                {
                    ((DropDownList)sender).Items.RemoveAt(0);
                }
            }
            catch { }
        }

        /// <summary>
        /// Controla se será mostrado o label ou a textBox do valor e da qtde
        /// </summary>
        protected void EditarValorQtde_DataBinding(object sender, EventArgs e)
        {
            GridViewRow linha = ((Control)sender).Parent.Parent as GridViewRow;
            ProdutosOrcamento item = linha.DataItem as ProdutosOrcamento;

            if (item == null)
            {
                ((Control)sender).Visible = !(sender is Label);
                return;
            }

            bool visivel = item.TemItensProduto || item.IdItemProjeto != null;

            ((Control)sender).Visible = sender is Label ? visivel : !visivel;
        }

        protected void btnEditar_Click(object sender, EventArgs e)
        {
            lnkProduto.Visible = false;
            lnkProjeto.Visible = false;
            grdProdutos.Visible = false;
        }

        protected void chkNegociar_CheckedChanged(object sender, EventArgs e)
        {
            if (!OrcamentoConfig.NegociarParcialmente)
                return;

            CheckBox chkNegociar = (CheckBox)sender;
            uint idProdOrca = Glass.Conversoes.StrParaUint(((HiddenField)chkNegociar.Parent.FindControl("hdfIdProd")).Value);

            ProdutosOrcamentoDAO.Instance.Negociar(idProdOrca, chkNegociar.Checked);
        }

        protected void lnkGerarPedido_Load(object sender, EventArgs e)
        {
            LinkButton lnkGerarPedido = (LinkButton)sender;
            HiddenField hdfIdCliente = (HiddenField)lnkGerarPedido.Parent.FindControl("hdfIdCliente");

            lnkGerarPedido.OnClientClick = String.Format(lnkGerarPedido.OnClientClick, hdfIdCliente.ClientID);
        }

        protected void lnkGerarPedido_Click(object sender, EventArgs e)
        {
            bool clienteGerado = false;

            try
            {
                uint idOrcamento = Glass.Conversoes.StrParaUint(Request["idOrca"]);
                var orca = OrcamentoDAO.Instance.GetElement(idOrcamento);

                LinkButton lnkGerarPedido = (LinkButton)sender;
                HiddenField hdfIdCliente = (HiddenField)lnkGerarPedido.Parent.FindControl("hdfIdCliente");

                if (orca.IdCliente == null)
                {
                    clienteGerado = true;
                    Cliente cli = ClienteDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(hdfIdCliente.Value));

                    orca.IdCliente = (uint)cli.IdCli;
                    orca.NomeCliente = cli.Nome;

                    if (String.IsNullOrEmpty(orca.Bairro))
                        orca.Bairro = cli.Bairro;
                    if (String.IsNullOrEmpty(orca.Cidade))
                        orca.Cidade = CidadeDAO.Instance.GetNome((uint?)cli.IdCidade);
                    if (String.IsNullOrEmpty(orca.Endereco))
                        orca.Endereco = cli.Endereco + (!String.IsNullOrEmpty(cli.Numero) ? ", " + cli.Numero : String.Empty);
                    if (String.IsNullOrEmpty(orca.TelCliente))
                        orca.TelCliente = !String.IsNullOrEmpty(cli.TelCont) ? cli.TelCont : cli.TelRes;
                    if (String.IsNullOrEmpty(orca.CelCliente))
                        orca.CelCliente = cli.TelCel;
                    if (String.IsNullOrEmpty(orca.Email))
                        orca.Email = (cli.Email != null ? cli.Email.Split(',')[0] : null);

                    OrcamentoDAO.Instance.Update(orca);
                }

                uint idPedido = PedidoDAO.Instance.GerarPedido(idOrcamento);
                Response.Redirect("../Cadastros/CadPedido.aspx?idPedido=" + idPedido);
            }
            catch (Exception ex)
            {
                if (clienteGerado)
                    dtvOrcamento.DataBind();

                Glass.MensagemAlerta.ErrorMsg("Falha ao gerar pedido.", ex, Page);
            }
        }

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
                if (!String.IsNullOrEmpty(Request["idOrca"]))
                    return Glass.Data.DAL.OrcamentoDAO.Instance.GetDescontoProdutos(Glass.Conversoes.StrParaUint(Request["idOrca"])).ToString().Replace(",", ".");
                else
                    return "0";
            }
            catch
            {
                return "0";
            }
        }

        protected string GetDescontoOrcamento()
        {
            try
            {
                if (!String.IsNullOrEmpty(Request["idOrca"]))
                    return Glass.Data.DAL.OrcamentoDAO.Instance.GetDescontoOrcamento(Glass.Conversoes.StrParaUint(Request["idOrca"])).ToString().Replace(",", ".");
                else
                    return "0";
            }
            catch
            {
                return "0";
            }
        }

        #region Recalcular

        protected void hdfRevenda_Load(object sender, EventArgs e)
        {
            uint idOrcamento = Glass.Conversoes.StrParaUint(Request["idOrca"]);
            uint? idCliente = OrcamentoDAO.Instance.ObtemIdCliente(idOrcamento);

            bool revenda = false;
            if (idCliente > 0)
                revenda = ClienteDAO.Instance.IsRevenda(idCliente.Value);

            ((HiddenField)sender).Value = revenda.ToString();
        }

        protected void ctrlBenef1_Load(object sender, EventArgs e)
        {
            ctrlBenef1.CampoClienteID = (HiddenField)dtvOrcamento.FindControl("hdfIdCliente");
            ctrlBenef1.CampoPercComissao = (HiddenField)dtvOrcamento.FindControl("hdfPercComissao");
            ctrlBenef1.CampoRevenda = (HiddenField)dtvOrcamento.FindControl("hdfRevenda");
            ctrlBenef1.CampoTipoEntrega = (HiddenField)dtvOrcamento.FindControl("hdfTipoEntrega");

            ctrlBenef1.CampoAltura = hdfBenefAltura;
            ctrlBenef1.CampoEspessura = hdfBenefEspessura;
            ctrlBenef1.CampoLargura = hdfBenefLargura;
            ctrlBenef1.CampoProdutoID = hdfBenefIdProd;
            ctrlBenef1.CampoQuantidade = hdfBenefQtde;
            ctrlBenef1.CampoTotalM2 = hdfBenefTotM;
            ctrlBenef1.CampoValorUnitario = hdfBenefValorUnit;
        }

        #endregion

        protected void ctrlMedicao_Load(object sender, EventArgs e)
        {
            // Esconde controles relacionados à medição
            if (!Geral.ControleMedicao)
                ((WebControl)sender).Visible = false;
        }

        protected void ctrlProjeto_Load(object sender, EventArgs e)
        {

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
        /// Adiciona um produto ao orçamento (para empresas que não vendem vidro).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void imgAdd_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string idProdStr = ((HiddenField)grdProdutos.FooterRow.FindControl("hdfIdProd")).Value;
                string qtdeStr = ((TextBox)grdProdutos.FooterRow.FindControl("txtQtde")).Text;
                string valorStr = ((Glass.UI.Web.Controls.ctrlTextBoxFloat)grdProdutos.FooterRow.FindControl("txtValorIns")).Value;
                Glass.UI.Web.Controls.ctrlDescontoQtde desc = (Glass.UI.Web.Controls.ctrlDescontoQtde)grdProdutos.FooterRow.FindControl("ctrlDescontoQtde1");

                ProdutosOrcamento prod = new ProdutosOrcamento();
                prod.IdOrcamento = Glass.Conversoes.StrParaUint(Request["idOrca"]);
                prod.IdProduto = !String.IsNullOrEmpty(idProdStr) ? (uint?)Glass.Conversoes.StrParaUint(idProdStr) : null;
                prod.Qtde = !String.IsNullOrEmpty(qtdeStr) ? float.Parse(qtdeStr.Replace(".", ",")) : 0;
                prod.ValorProd = !String.IsNullOrEmpty(valorStr) ? decimal.Parse(valorStr.Replace(".", ",")) : 0;
                prod.Descricao = prod.IdProduto > 0 ? ProdutoDAO.Instance.GetCodInterno((int)prod.IdProduto.Value) + " - " + ProdutoDAO.Instance.GetDescrProduto((int)prod.IdProduto.Value) : "";
                prod.PercDescontoQtde = desc.PercDescontoQtde;
                prod.TipoCalculoUsado = Glass.Conversoes.StrParaInt(((HiddenField)grdProdutos.FooterRow.FindControl("hdfTipoCalc")).Value);

                if (prod.IdProduto > 0)
                {
                    uint? idCliente = OrcamentoDAO.Instance.ObtemIdCliente(prod.IdOrcamento);
                    decimal custoProd = 0, total = 0;
                    float altura = 0, totM2 = 0;
                    Glass.Data.DAL.ProdutoDAO.Instance.CalcTotaisItemProd(idCliente.GetValueOrDefault(), (int)prod.IdProduto.Value, prod.Largura, prod.Qtde.Value, 1, prod.ValorProd.Value, 0,
                        false, 1, false, ref custoProd, ref altura, ref totM2, ref total, false, 0);

                    prod.Total = total;
                    prod.Custo = custoProd;
                }

                ProdutosOrcamentoDAO.Instance.Insert(prod);

                dtvOrcamento.DataBind();
                grdProdutos.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("", ex, Page);
            }
        }

        #region Métodos Ajax

        [Ajax.AjaxMethod]
        public string GetValorMinimo(string codInterno, string tipoEntrega, string idCliente, string revenda,
            string idProdOrcaStr, string percDescontoQtdeStr, string idOrcamento)
        {
            return WebGlass.Business.Produto.Fluxo.Valor.Ajax.GetValorMinimoOrca(codInterno, tipoEntrega,
                idCliente, revenda, idProdOrcaStr, percDescontoQtdeStr, idOrcamento);
        }

        /// <summary>
        /// Retorna o Código/Descrição do produto
        /// </summary>
        [Ajax.AjaxMethod()]
        public string GetProduto(string codInterno, string tipoEntrega, string revenda, string idCliente,
            string percComissao, string percDescontoQtdeStr, string idLoja, string idOrcamento)
        {
            return WebGlass.Business.Produto.Fluxo.BuscarEValidar.Ajax.GetProdutoOrca(codInterno, tipoEntrega,
                revenda, idCliente, percComissao, percDescontoQtdeStr, idLoja, idOrcamento);
        }

        #endregion

        protected void ctrlDescontoQtde1_Load(object sender, EventArgs e)
        {
            Glass.UI.Web.Controls.ctrlDescontoQtde desc = (Glass.UI.Web.Controls.ctrlDescontoQtde)sender;
            GridViewRow linha = desc.Parent.Parent as GridViewRow;
            if (linha == null)
                return;

            desc.CampoQtde = linha.FindControl("txtQtde");
            desc.CampoProdutoID = linha.FindControl("hdfIdProd");
            desc.CampoClienteID = dtvOrcamento.FindControl("hdfIdCliente");
            desc.CampoTipoEntrega = dtvOrcamento.FindControl("hdfTipoEntrega");
            desc.CampoRevenda = dtvOrcamento.FindControl("hdfRevenda");
            desc.CampoValorUnit = linha.FindControl("txtValorIns").FindControl("txtNumber");
            desc.CampoTotal = linha.FindControl("lblTotalProd");
        }

        protected void grdProdutos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdProdutos.ShowFooter = Glass.Configuracoes.Geral.NaoVendeVidro() && e.CommandName != "Edit";
        }

        protected void imagemProdutoOrca_Load(object sender, EventArgs e)
        {
            Control div = sender as Control;
            div.Visible = OrcamentoConfig.UploadImagensOrcamento;
        }

        protected void grdProdutos_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            FileUpload fluImagem = grdProdutos.Rows[e.RowIndex].FindControl("fluImagem") as FileUpload;
            if (fluImagem == null || !fluImagem.HasFile)
                return;

            uint idProd = Glass.Conversoes.StrParaUint(e.Keys[0].ToString());
            ManipulacaoImagem.SalvarImagem(Data.Helper.Utils.GetProdutosOrcamentoPath + idProd + ".jpg", fluImagem.FileBytes);
        }

        protected void imbExcluirImagem_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton imb = (ImageButton)sender;
            uint idProd = Glass.Conversoes.StrParaUint(imb.CommandArgument);

            if (File.Exists(Data.Helper.Utils.GetProdutosOrcamentoPath + idProd + ".jpg"))
                File.Delete(Data.Helper.Utils.GetProdutosOrcamentoPath + idProd + ".jpg");

            imb.Visible = false;
        }

        protected void lnkMedicaoDef_Click(object sender, EventArgs e)
        {
            uint idOrca = 0;
            if (uint.TryParse(Request["idOrca"], out idOrca))
            {
                try
                {
                    uint idMedicao = MedicaoDAO.Instance.GerarMedicaoDefinitivaOrca(idOrca);
                    Response.Redirect("../Cadastros/CadMedicao.aspx?idMedicao=" + idMedicao);
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao gerar medição definitiva.", ex, Page);
                }
            }
        }

        [Ajax.AjaxMethod]
        public string PercDesconto(string idOrcamentoStr, string idFuncAtualStr, string alterouDesconto)
        {
            uint idOrcamento = Glass.Conversoes.StrParaUint(idOrcamentoStr);
            uint idFuncAtual = Glass.Conversoes.StrParaUint(idFuncAtualStr);
            uint idFuncDesc = Geral.ManterDescontoAdministrador ? OrcamentoDAO.Instance.ObtemIdFuncDesc(idOrcamento).GetValueOrDefault() : 0;

            return (idFuncDesc == 0 || UserInfo.IsAdministrador(idFuncAtual) || alterouDesconto.ToLower() == "true" ?
                OrcamentoConfig.Desconto.GetDescontoMaximoOrcamento(idFuncAtual) : OrcamentoConfig.Desconto.GetDescontoMaximoOrcamento(idFuncDesc)).ToString();
        }

        protected void drpFuncionario_DataBound(object sender, EventArgs e)
        {
            ((DropDownList)sender).Enabled = Config.PossuiPermissao(Config.FuncaoMenuPedido.AlterarVendedorPedido);
        }

        public string AlterarLojaOrcamento()
        {
            return OrcamentoConfig.AlterarLojaOrcamento.ToString().ToLower();
        }

        protected void Loja_Load(object sender, EventArgs e)
        {
            if (!OrdemCargaConfig.UsarControleOrdemCarga && sender is WebControl)
                ((WebControl)sender).Enabled = false;

            if (OrcamentoConfig.AlterarLojaOrcamento && sender is WebControl)
                ((WebControl)sender).Enabled = true;
        }

        private void RedirecionarListagemOrcamento()
        {
            if (!string.IsNullOrWhiteSpace(Request["IdRelDinamico"]))
                Response.Redirect("~/Relatorios/Dinamicos/ListaDinamico.aspx?Id="+ Request["IdRelDinamico"]);
            else
                Response.Redirect("~/Listas/LstOrcamento.aspx");
        }

        protected void txtValorFrete_Load(object sender, EventArgs e)
        {
            if (!PedidoConfig.ExibirValorFretePedido)
                ((WebControl)sender).Style.Add("Display", "none");
        }

        protected Color GetCorObsCliente()
        {
            return Glass.Configuracoes.Liberacao.TelaLiberacao.CorExibirObservacaoCliente;
        }

        protected void lblObsCliente_Load(object sender, EventArgs e)
        {
            (sender as Label).ForeColor = Color.Red;
        }

        protected void lnkGerarPedidoAgrupado_Click(object sender, EventArgs e)
        {
            bool clienteGerado = false;

            try
            {
                uint idOrcamento = Glass.Conversoes.StrParaUint(Request["idOrca"]);
                var orca = OrcamentoDAO.Instance.GetElement(idOrcamento);

                LinkButton lnkGerarPedido = (LinkButton)sender;
                HiddenField hdfIdCliente = (HiddenField)lnkGerarPedido.Parent.FindControl("hdfIdCliente");

                if (orca.IdCliente == null)
                {
                    clienteGerado = true;
                    Cliente cli = ClienteDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(hdfIdCliente.Value));

                    orca.IdCliente = (uint)cli.IdCli;
                    orca.NomeCliente = cli.Nome;

                    if (String.IsNullOrEmpty(orca.Bairro))
                        orca.Bairro = cli.Bairro;
                    if (String.IsNullOrEmpty(orca.Cidade))
                        orca.Cidade = CidadeDAO.Instance.GetNome((uint?)cli.IdCidade);
                    if (String.IsNullOrEmpty(orca.Endereco))
                        orca.Endereco = cli.Endereco + (!String.IsNullOrEmpty(cli.Numero) ? ", " + cli.Numero : String.Empty);
                    if (String.IsNullOrEmpty(orca.TelCliente))
                        orca.TelCliente = !String.IsNullOrEmpty(cli.TelCont) ? cli.TelCont : cli.TelRes;
                    if (String.IsNullOrEmpty(orca.CelCliente))
                        orca.CelCliente = cli.TelCel;
                    if (String.IsNullOrEmpty(orca.Email))
                        orca.Email = (cli.Email != null ? cli.Email.Split(',')[0] : null);

                    OrcamentoDAO.Instance.Update(orca);
                }

                PedidoDAO.Instance.GerarPedidosAgrupados(idOrcamento);
                Response.Redirect("../Listas/LstPedidos.aspx");
            }
            catch (Exception ex)
            {
                if (clienteGerado)
                    dtvOrcamento.DataBind();

                Glass.MensagemAlerta.ErrorMsg("Falha ao gerar pedidos.", ex, Page);
            }
        }

        protected void lnkGerarPedidoAgrupado_Load(object sender, EventArgs e)
        {
            LinkButton lnkGerarPedidoAgrupado = (LinkButton)sender;
            HiddenField hdfIdCliente = (HiddenField)lnkGerarPedidoAgrupado.Parent.FindControl("hdfIdCliente");

            lnkGerarPedidoAgrupado.OnClientClick = String.Format(lnkGerarPedidoAgrupado.OnClientClick, hdfIdCliente.ClientID);
        }
    }
}
