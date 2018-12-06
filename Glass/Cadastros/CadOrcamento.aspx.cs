using Glass.Configuracoes;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Data.Model;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadOrcamento : System.Web.UI.Page
    {
        private byte relatorio = 0;

        #region Load

        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(CadOrcamento));
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(RecalcularOrcamento));

            grdProdutosAmbienteOrcamento.Columns[6].Visible = OrcamentoConfig.NegociarParcialmente;

            if (!IsPostBack && Request["idOrca"] != null && dtvOrcamento.CurrentMode != DetailsViewMode.ReadOnly)
            {
                hdfIdOrcamento.Value = Request["idOrca"];
                dtvOrcamento.ChangeMode(DetailsViewMode.ReadOnly);
                dtvOrcamento.DataBind();

                var orcamento = dtvOrcamento.DataItem as Data.Model.Orcamento;

                if (Request["atualizar"] == "1")
                {
                    var tipoEntrega = OrcamentoDAO.Instance.ObtemTipoEntrega(Conversoes.StrParaUint(hdfIdOrca.Value));
                    var idCliente = OrcamentoDAO.Instance.ObtemIdCliente(Conversoes.StrParaUint(hdfIdOrca.Value));
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "callback", $"recalcular({hdfIdOrca.Value}, false, {tipoEntrega}, {idCliente});", true);
                }

                // Se o usuário não tiver permissão para editar este orçamento, retorna para listagem de orçamentos
                if (((HiddenField)dtvOrcamento.FindControl("hdfEditVisible"))?.Value?.ToLower() == "false" || !orcamento.EditVisible)
                {
                    RedirecionarListagemOrcamento();
                }

                if (!string.IsNullOrWhiteSpace(hdfIdOrcamento.Value) && !string.IsNullOrEmpty(Request["relatorio"]))
                {
                    switch (Request["relatorio"])
                    {
                        case "1":
                            Page.ClientScript.RegisterStartupScript(GetType(), "Relatorio", $"openRpt('{ hdfIdOrcamento.Value }');\n", true);
                            break;

                        case "2":
                            Page.ClientScript.RegisterStartupScript(GetType(), "Relatorio", $"openRptMemoria('{ hdfIdOrcamento.Value }');\n", true);
                            break;
                    }
                }
            }
            else if (!IsPostBack)
            {
                if (dtvOrcamento.CurrentMode == DetailsViewMode.Insert)
                {
                    if (((TextBox)dtvOrcamento.FindControl("txtPrazoEntregaIns")).Text == string.Empty)
                    {
                        ((TextBox)dtvOrcamento.FindControl("txtPrazoEntregaIns")).Text = OrcamentoConfig.DadosOrcamento.PrazoEntregaOrcamento;
                    }

                    if (((TextBox)dtvOrcamento.FindControl("txtValidadeIns")).Text == string.Empty)
                    {
                        ((TextBox)dtvOrcamento.FindControl("txtValidadeIns")).Text = OrcamentoConfig.DadosOrcamento.ValidadeOrcamento;
                    }

                    if (((TextBox)dtvOrcamento.FindControl("txtFormaPagtoIns")).Text == string.Empty)
                    {
                        ((TextBox)dtvOrcamento.FindControl("txtFormaPagtoIns")).Text = OrcamentoConfig.DadosOrcamento.FormaPagtoOrcamento;
                    }

                    ((DropDownList)dtvOrcamento.FindControl("drpFuncionario")).SelectedValue = UserInfo.GetUserInfo.CodUser.ToString();
                    ((DropDownList)dtvOrcamento.FindControl("drpTipoOrcamento")).SelectedValue = OrcamentoConfig.TelaCadastro.TipoOrcamentoPadrao == null ? "0" : ((int)OrcamentoConfig.TelaCadastro.TipoOrcamentoPadrao).ToString();
                }
            }

            hdfComissaoVisible.Value = PedidoConfig.Comissao.ComissaoPedido.ToString().ToLower();
            grdProdutosAmbienteOrcamento.Visible = dtvOrcamento.CurrentMode == DetailsViewMode.ReadOnly;

            grdProdutosAmbienteOrcamento.Columns[4].Visible = OrcamentoConfig.Desconto.DescontoAcrescimoItensOrcamento;
            grdProdutosAmbienteOrcamento.Columns[5].Visible = OrcamentoConfig.Desconto.DescontoAcrescimoItensOrcamento;
        }

        protected void lnkGerarPedidoAgrupado_Load(object sender, EventArgs e)
        {
            LinkButton lnkGerarPedidoAgrupado = (LinkButton)sender;
            HiddenField hdfIdCliente = (HiddenField)lnkGerarPedidoAgrupado.Parent.FindControl("hdfIdCliente");

            lnkGerarPedidoAgrupado.OnClientClick = string.Format(lnkGerarPedidoAgrupado.OnClientClick, hdfIdCliente.ClientID);
        }

        protected void hdfCliRevenda_Load(object sender, EventArgs e)
        {
            var idOrcamento = Request["idOrca"].StrParaInt();
            var idCliente = OrcamentoDAO.Instance.ObterIdCliente(null, idOrcamento);
            var revenda = false;

            if (idCliente > 0)
            {
                revenda = ClienteDAO.Instance.IsRevenda(null, (uint)idCliente);
            }

            ((HiddenField)sender).Value = revenda.ToString();
        }

        protected void ctrlMedicao_Load(object sender, EventArgs e)
        {
            // Esconde controles relacionados à medição
            if (!Geral.ControleMedicao)
            {
                ((WebControl)sender).Visible = false;
            }
        }

        /// <summary>
        /// Mostra/Esconde campos do total bruto e líquido
        /// </summary>
        protected void lblTotalBrutoLiquido_Load(object sender, EventArgs e)
        {
            if (!Geral.NaoVendeVidro())
            {
                ((WebControl)sender).Visible = false;
            }
        }

        /// <summary>
        /// Mostra/Esconde campos do total geral
        /// </summary>
        protected void lblTotalGeral_Load(object sender, EventArgs e)
        {
            if (Geral.NaoVendeVidro())
            {
                ((WebControl)sender).Visible = false;
            }
        }

        protected void lnkGerarPedido_Load(object sender, EventArgs e)
        {
            LinkButton lnkGerarPedido = (LinkButton)sender;
            HiddenField hdfIdCliente = (HiddenField)lnkGerarPedido.Parent.FindControl("hdfIdCliente");

            lnkGerarPedido.OnClientClick = string.Format(lnkGerarPedido.OnClientClick, hdfIdCliente.ClientID);
        }

        protected void imagemProdutoOrca_Load(object sender, EventArgs e)
        {
            Control div = sender as Control;
            div.Visible = OrcamentoConfig.UploadImagensOrcamento;
        }

        protected void lblObsCliente_Load(object sender, EventArgs e)
        {
            (sender as Label).ForeColor = Color.Red;
        }

        protected void Loja_Load(object sender, EventArgs e)
        {
            if (!OrdemCargaConfig.UsarControleOrdemCarga && sender is WebControl)
            {
                ((WebControl)sender).Enabled = false;
            }

            if (OrcamentoConfig.AlterarLojaOrcamento && sender is WebControl)
            {
                ((WebControl)sender).Enabled = true;
            }
        }

        protected void txtValorFrete_Load(object sender, EventArgs e)
        {
            if (!PedidoConfig.ExibirValorFretePedido)
            {
                ((WebControl)sender).Style.Add("Display", "none");
            }
        }

        protected void ctrlDescontoQtde_Load(object sender, EventArgs e)
        {
            Controls.ctrlDescontoQtde desc = (Controls.ctrlDescontoQtde)sender;
            GridViewRow linha = desc.Parent.Parent as GridViewRow;

            desc.CampoQtde = linha.FindControl("txtQtdeIns");
            desc.CampoProdutoID = linha.FindControl("hdfIdProduto");
            desc.CampoClienteID = dtvOrcamento.FindControl("hdfIdCliente");
            desc.CampoTipoEntrega = dtvOrcamento.FindControl("hdfTipoEntrega");
            desc.CampoRevenda = dtvOrcamento.FindControl("hdfCliRevenda");
            desc.CampoValorUnit = linha.FindControl("txtValorIns");

            if (desc.CampoProdutoID == null)
            {
                desc.CampoProdutoID = hdfIdProduto;
            }
        }

        protected void ctrlBenef_Load(object sender, EventArgs e)
        {
            Controls.ctrlBenef benef = (Controls.ctrlBenef)sender;
            GridViewRow linhaControle = benef.Parent.Parent as GridViewRow;
            Control codProd = null;
            var tipoOrcamento = OrcamentoDAO.Instance.ObterTipoOrcamento(null, Request["idOrca"].StrParaInt());

            if (linhaControle.FindControl("lblCodProdIns") != null)
            {
                codProd = linhaControle.FindControl("lblCodProdIns");
            }
            else
            {
                codProd = linhaControle.FindControl("txtCodProdIns");
            }

            TextBox txtAltura = (TextBox)linhaControle.FindControl("txtAlturaIns");
            TextBox txtEspessura = (TextBox)linhaControle.FindControl("txtEspessura");
            TextBox txtLargura = (TextBox)linhaControle.FindControl("txtLarguraIns");
            HiddenField hdfPercComissao = (HiddenField)dtvOrcamento.FindControl("hdfPercComissao");
            TextBox txtQuantidade = (TextBox)linhaControle.FindControl("txtQtdeIns");
            HiddenField hdfTipoEntrega = (HiddenField)dtvOrcamento.FindControl("hdfTipoEntrega");
            HiddenField hdfTotalM2 = null;

            if (!Beneficiamentos.UsarM2CalcBeneficiamentos)
            {
                if (linhaControle.FindControl("hdfTotM") != null)
                {
                    hdfTotalM2 = (HiddenField)linhaControle.FindControl("hdfTotM");
                }
                else if (linhaControle.FindControl("hdfTotMIns") != null)
                {
                    hdfTotalM2 = (HiddenField)linhaControle.FindControl("hdfTotMIns");
                }
            }
            else
            {
                if (linhaControle.FindControl("hdfTotMCalc") != null)
                {
                    hdfTotalM2 = (HiddenField)linhaControle.FindControl("hdfTotMCalc");
                }
                else if (linhaControle.FindControl("hdfTotMCalcIns") != null)
                {
                    hdfTotalM2 = (HiddenField)linhaControle.FindControl("hdfTotMCalcIns");
                }
            }

            TextBox txtValorIns = (TextBox)linhaControle.FindControl("txtValorIns");
            HiddenField hdfCliRevenda = (HiddenField)dtvOrcamento.FindControl("hdfCliRevenda");
            HiddenField hdfIdCliente = (HiddenField)dtvOrcamento.FindControl("hdfIdCliente");
            HiddenField hdfCustoProd = (HiddenField)linhaControle.FindControl("hdfCustoProd");

            benef.CampoAltura = txtAltura;
            benef.CampoEspessura = txtEspessura;
            benef.CampoLargura = txtLargura;
            benef.CampoPercComissao = hdfPercComissao;
            benef.CampoQuantidade = txtQuantidade;
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
            benef.TipoBenef = TipoBenef.Venda;
        }

        protected void txtValorIns_Load(object sender, EventArgs e)
        {
            ((TextBox)sender).Enabled = PedidoConfig.DadosPedido.AlterarValorUnitarioProduto;
        }

        #endregion

        #region Click

        protected void lnkGerarPedidoAgrupado_Click(object sender, EventArgs e)
        {
            bool clienteGerado = false;

            try
            {
                var idOrcamento = Request["idOrca"].StrParaInt();
                var orcamento = OrcamentoDAO.Instance.GetElement(null, (uint)idOrcamento);

                LinkButton lnkGerarPedido = (LinkButton)sender;
                HiddenField hdfIdCliente = (HiddenField)lnkGerarPedido.Parent.FindControl("hdfIdCliente");

                if (orcamento.IdCliente == null)
                {
                    clienteGerado = true;
                    var cliente = ClienteDAO.Instance.GetElementByPrimaryKey((uint)hdfIdCliente.Value.StrParaInt());

                    if (cliente == null)
                    {
                        throw new Exception("Cliente não encontrado");
                    }

                    orcamento.IdCliente = (uint)cliente.IdCli;
                    orcamento.NomeCliente = cliente.Nome;

                    if (string.IsNullOrWhiteSpace(orcamento.Bairro))
                    {
                        orcamento.Bairro = cliente.Bairro;
                    }

                    if (string.IsNullOrWhiteSpace(orcamento.Cidade))
                    {
                        orcamento.Cidade = CidadeDAO.Instance.GetNome((uint?)cliente.IdCidade);
                    }

                    if (string.IsNullOrWhiteSpace(orcamento.Endereco))
                    {
                        orcamento.Endereco = cliente.Endereco + (!string.IsNullOrWhiteSpace(cliente.Numero) ? $", { cliente.Numero }" : string.Empty);
                    }

                    if (string.IsNullOrWhiteSpace(orcamento.TelCliente))
                    {
                        orcamento.TelCliente = !string.IsNullOrWhiteSpace(cliente.TelCont) ? cliente.TelCont : cliente.TelRes;
                    }

                    if (string.IsNullOrWhiteSpace(orcamento.CelCliente))
                    {
                        orcamento.CelCliente = cliente.TelCel;
                    }

                    if (string.IsNullOrWhiteSpace(orcamento.Email))
                    {
                        orcamento.Email = (cliente.Email != null ? cliente.Email.Split(',')[0] : null);
                    }

                    OrcamentoDAO.Instance.Update(orcamento);
                }

                PedidoDAO.Instance.GerarPedidosAgrupados(idOrcamento);
                Response.Redirect("../Listas/LstPedidos.aspx");
            }
            catch (Exception ex)
            {
                if (clienteGerado)
                {
                    dtvOrcamento.DataBind();
                }

                MensagemAlerta.ErrorMsg("Falha ao gerar pedidos.", ex, Page);
            }
        }

        protected void imbExcluirImagem_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton imb = (ImageButton)sender;
            var idProd = imb.CommandArgument.StrParaInt();
            var caminhoArquivoImagem = $"{Data.Helper.Utils.GetProdutosOrcamentoPath}{idProd}.jpg";

            if (File.Exists(caminhoArquivoImagem))
            {
                File.Delete(caminhoArquivoImagem);
            }

            imb.Visible = false;
        }

        protected void lnkMedicaoDef_Click(object sender, EventArgs e)
        {
            var idOrca = Request["idOrca"].StrParaInt();

            if (idOrca > 0)
            {
                try
                {
                    var idMedicao = MedicaoDAO.Instance.GerarMedicaoDefinitivaOrca((uint)idOrca);
                    Response.Redirect($"../Cadastros/CadMedicao.aspx?idMedicao={ idMedicao }");
                }
                catch (Exception ex)
                {
                    MensagemAlerta.ErrorMsg("Falha ao gerar medição definitiva.", ex, Page);
                }
            }
        }

        protected void btnEditar_Click(object sender, EventArgs e)
        {
            lnkProjeto.Visible = false;
            grdProdutosAmbienteOrcamento.Visible = false;
        }

        protected void lnkGerarPedido_Click(object sender, EventArgs e)
        {
            var clienteGerado = false;

            try
            {
                var idOrcamento = Request["idOrca"].StrParaUint();
                var orca = OrcamentoDAO.Instance.GetElement(idOrcamento);

                LinkButton lnkGerarPedido = (LinkButton)sender;
                HiddenField hdfIdCliente = (HiddenField)lnkGerarPedido.Parent.FindControl("hdfIdCliente");

                if (orca.IdCliente == null)
                {
                    clienteGerado = true;
                    var cliente = ClienteDAO.Instance.GetElementByPrimaryKey(hdfIdCliente.Value.StrParaUint());

                    if (cliente == null)
                    {
                        throw new Exception("Cliente não encontrado");
                    }

                    orca.IdCliente = (uint)cliente.IdCli;
                    orca.NomeCliente = cliente.Nome;

                    if (string.IsNullOrWhiteSpace(orca.Bairro))
                    {
                        orca.Bairro = cliente.Bairro;
                    }

                    if (string.IsNullOrWhiteSpace(orca.Cidade))
                    {
                        orca.Cidade = CidadeDAO.Instance.GetNome((uint?)cliente.IdCidade);
                    }

                    if (string.IsNullOrWhiteSpace(orca.Endereco))
                    {
                        orca.Endereco = $"{ cliente.Endereco }{ (!string.IsNullOrWhiteSpace(cliente.Numero) ? $", { cliente.Numero }" : string.Empty) }";
                    }

                    if (string.IsNullOrWhiteSpace(orca.TelCliente))
                    {
                        orca.TelCliente = !string.IsNullOrWhiteSpace(cliente.TelCont) ? cliente.TelCont : cliente.TelRes;
                    }

                    if (string.IsNullOrWhiteSpace(orca.CelCliente))
                    {
                        orca.CelCliente = cliente.TelCel;
                    }

                    if (string.IsNullOrWhiteSpace(orca.Email))
                    {
                        orca.Email = (cliente.Email != null ? cliente.Email.Split(',')[0] : null);
                    }

                    OrcamentoDAO.Instance.Update(orca);
                }

                var idPedido = PedidoDAO.Instance.GerarPedido(idOrcamento);
                Response.Redirect($"../Cadastros/CadPedido.aspx?idPedido={ idPedido }");
            }
            catch (Exception ex)
            {
                if (clienteGerado)
                {
                    dtvOrcamento.DataBind();
                }

                MensagemAlerta.ErrorMsg("Falha ao gerar pedido.", ex, Page);
            }
        }

        protected void lnkInsAmbiente_Click(object sender, EventArgs e)
        {
            try
            {
                var ambiente = ((HiddenField)grdProdutosAmbienteOrcamento.FooterRow.FindControl("hdfDescrAmbienteIns")).Value;
                var descricaoAmbiente = ((TextBox)grdProdutosAmbienteOrcamento.FooterRow.FindControl("txtDescricaoAmbienteIns")).Text;

                var produtoOrcamento = new ProdutosOrcamento();

                produtoOrcamento.IdOrcamento = Request["idOrca"].StrParaUint();
                produtoOrcamento.Ambiente = ambiente;
                produtoOrcamento.Descricao = descricaoAmbiente;
                produtoOrcamento.Qtde = 1;

                ProdutosOrcamentoDAO.Instance.InsertProdutoAmbienteComTransacao(produtoOrcamento);

                grdProdutosOrcamento.Visible = true;

                dtvOrcamento.DataBind();
                grdProdutosAmbienteOrcamento.DataBind();
            }
            catch (Exception ex)
            {
                MensagemAlerta.ErrorMsg(string.Empty, ex, Page);
            }
        }

        protected void lnkInsProd_Click(object sender, EventArgs e)
        {
            if (grdProdutosOrcamento.PageCount > 1)
            {
                grdProdutosOrcamento.PageIndex = grdProdutosOrcamento.PageCount - 1;
            }

            Controls.ctrlBenef benef = (Controls.ctrlBenef)grdProdutosOrcamento.FooterRow.FindControl("ctrlBenefInserir");

            var idOrcamento = Request["IdOrca"].StrParaInt();
            var idProduto = (hdfIdProduto?.Value?.StrParaInt()).GetValueOrDefault();
            var idAmbiente = hdfIdProdAmbienteOrcamento.Value;
            var valorVendido = (((TextBox)grdProdutosOrcamento.FooterRow.FindControl("txtValorIns"))?.Text?.StrParaDecimal()).GetValueOrDefault();
            var percentualDescontoQuantidade = (((Controls.ctrlDescontoQtde)grdProdutosOrcamento.FooterRow.FindControl("ctrlDescontoQtde"))?.PercDescontoQtde).GetValueOrDefault();
            var quantidade = (((TextBox)grdProdutosOrcamento.FooterRow.FindControl("txtQtdeIns"))?.Text?.Replace('.', ',')?.StrParaFloat()).GetValueOrDefault();
            var altura = (((TextBox)grdProdutosOrcamento.FooterRow.FindControl("txtAlturaIns"))?.Text?.StrParaFloat()).GetValueOrDefault();
            var alturaCalculada = (((HiddenField)grdProdutosOrcamento.FooterRow.FindControl("hdfAlturaCalcIns"))?.Value?.StrParaFloat()).GetValueOrDefault();
            var largura = (((TextBox)grdProdutosOrcamento.FooterRow.FindControl("txtLarguraIns"))?.Text?.StrParaInt()).GetValueOrDefault();
            var espessura = ((TextBox)grdProdutosOrcamento.FooterRow.FindControl("txtEspessura")).Text.StrParaFloat();
            var redondo = (((CheckBox)benef.FindControl("Redondo_chkSelecao"))?.Checked).GetValueOrDefault();
            var aliquotaIcms = (((HiddenField)grdProdutosOrcamento.FooterRow.FindControl("hdfAliquotaIcmsProd"))?.Value?.Replace('.', ',')?.StrParaFloat()).GetValueOrDefault();
            var valorIcms = (((HiddenField)grdProdutosOrcamento.FooterRow.FindControl("hdfValorIcmsProd"))?.Value?.Replace('.', ',')?.StrParaDecimal()).GetValueOrDefault();
            var tipoEntrega = ((HiddenField)dtvOrcamento.FindControl("hdfTipoEntrega")).Value.StrParaInt();
            var idCliente = ((HiddenField)dtvOrcamento.FindControl("hdfIdCliente")).Value.StrParaUint();
            var idAplicacao = ((HiddenField)grdProdutosOrcamento.FooterRow.FindControl("hdfIdAplicacao"))?.Value?.StrParaInt();
            var idProcesso = ((HiddenField)grdProdutosOrcamento.FooterRow.FindControl("hdfIdProcesso"))?.Value?.StrParaInt();
            var idProcessoFilha = ((HiddenField)grdProdutosOrcamento.FooterRow.FindControl("hdfIdProcessoFilhos"))?.Value?.StrParaInt();
            var idAplicacaoFilha = ((HiddenField)grdProdutosOrcamento.FooterRow.FindControl("hdfIdAplicacaoFilhos"))?.Value?.StrParaInt();
            var aplicarBenefComposicao = ((CheckBox)grdProdutosOrcamento.FooterRow.FindControl("chkAplicarBenefFilhos")).Checked;

            // Cria uma instância da classe ProdutosOrcamento.
            var produtoOrcamento = new ProdutosOrcamento();

            produtoOrcamento.IdOrcamento = (uint)idOrcamento;
            produtoOrcamento.Qtde = quantidade;
            produtoOrcamento.ValorProd = valorVendido;
            produtoOrcamento.PercDescontoQtde = percentualDescontoQuantidade;
            produtoOrcamento.Altura = altura;
            produtoOrcamento.AlturaCalc = alturaCalculada;
            produtoOrcamento.Largura = largura;
            produtoOrcamento.IdProduto = (uint)idProduto;
            produtoOrcamento.Espessura = espessura;
            produtoOrcamento.Redondo = redondo;
            produtoOrcamento.IdProdParent = idAmbiente.StrParaUint();
            produtoOrcamento.IdAplicacao = (uint?)idAplicacao;
            produtoOrcamento.IdProcesso = (uint?)idProcesso;
            produtoOrcamento.IdAplicacaoFilhas = idAplicacao;
            produtoOrcamento.IdProcessoFilhas = idProcesso;
            produtoOrcamento.AliquotaIcms = aliquotaIcms;
            produtoOrcamento.ValorIcms = valorIcms;
            produtoOrcamento.AplicarBenefComposicao = aplicarBenefComposicao;

            var idLoja = OrcamentoDAO.Instance.ObterIdLoja(null, idOrcamento);

            if (produtoOrcamento.IdProduto > 0 && LojaDAO.Instance.ObtemCalculaIpiPedido(null, (uint)idLoja) && ClienteDAO.Instance.IsCobrarIpi(null, idCliente))
            {
                produtoOrcamento.AliquotaIpi = ProdutoDAO.Instance.ObtemAliqIpi(produtoOrcamento.IdProduto.Value);
            }

            produtoOrcamento.Beneficiamentos = benef.Beneficiamentos;

            try
            {
                // Insere o produto orçamento.
                produtoOrcamento.IdProd = ProdutosOrcamentoDAO.Instance.Insert(produtoOrcamento);

                ((HiddenField)grdProdutosOrcamento.FooterRow.FindControl("hdfAlturaCalcIns")).Value = string.Empty;

                grdProdutosOrcamento.DataBind();
                dtvOrcamento.DataBind();
                grdProdutosAmbienteOrcamento.DataBind();

                if (PedidoConfig.TelaCadastro.ManterCodInternoCampoAoInserirProduto)
                {
                    ClientScript.RegisterClientScriptBlock(typeof(string), "novoProd", $"ultimoCodProd = '{ ProdutoDAO.Instance.GetCodInterno(null, (int)idProduto) }';", true);
                }
            }
            catch (Exception ex)
            {
                MensagemAlerta.ErrorMsg("Falha ao incluir produto no orçamento.", ex, Page);
                return;
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            RedirecionarListagemOrcamento();
        }

        protected void btnCancelarEditar_Click(object sender, EventArgs e)
        {
            Response.Redirect(Request.Url.ToString());
        }

        #endregion

        #region DataBinding/DataBound

        protected void drpFuncionario_DataBound(object sender, EventArgs e)
        {
            ((DropDownList)sender).Enabled = Config.PossuiPermissao(Config.FuncaoMenuPedido.AlterarVendedorPedido);
        }

        protected void txtEspessura_DataBinding(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            GridViewRow linhaControle = txt.Parent.Parent as GridViewRow;

            var produtoOrcamento = linhaControle.DataItem as ProdutosOrcamento;
            txt.Enabled = produtoOrcamento.Espessura <= 0;
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

            if (item.IdAmbientePedido > 0)
            {
                ((ImageButton)sender).Visible = false;
                return;
            }

            ((ImageButton)sender).Visible = true;
            ((ImageButton)sender).OnClientClick = "return openProdutos('" + item.IdProd + "', false);";
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

            var visivel = item.TemItensProdutoSession(null) || item.IdItemProjeto != null;

            ((Control)sender).Visible = sender is Label ? visivel : !visivel;
        }

        #endregion

        #region CheckedChanged

        protected void chkNegociar_CheckedChanged(object sender, EventArgs e)
        {
            if (!OrcamentoConfig.NegociarParcialmente)
            {
                return;
            }

            CheckBox chkNegociar = (CheckBox)sender;
            var idProdOrcamento = ((HiddenField)chkNegociar.Parent.FindControl("hdfIdProdOrcamento")).Value.StrParaInt();

            // Marca/desmarca o ambiente como negociável.
            ProdutosOrcamentoDAO.Instance.AtualizarNegociar(null, idProdOrcamento, chkNegociar.Checked);
        }

        #endregion

        #region Métodos Ajax

        [Ajax.AjaxMethod()]
        public string ObterLojaSubgrupoProd(string codInterno)
        {
            var idProd = ProdutoDAO.Instance.ObtemIdProd(codInterno);
            var idLoja = SubgrupoProdDAO.Instance.ObterIdsLojaPeloProduto(null, idProd);
            return idLoja?.FirstOrDefault().ToString() ?? string.Empty;
        }

        [Ajax.AjaxMethod]
        public string ProdutoPossuiAplPadrao(string codInterno)
        {
            try
            {
                var idProd = ProdutoDAO.Instance.ObtemIdProd(codInterno);
                var idAplicacao = ProdutoDAO.Instance.ObtemIdAplicacao(idProd);

                return (idAplicacao > 0).ToString().ToLower();
            }
            catch
            {
                return "false";
            }
        }

        [Ajax.AjaxMethod]
        public string GetCli(string idCli)
        {
            return WebGlass.Business.Orcamento.Fluxo.BuscarEValidar.Ajax.GetCli(idCli);
        }

        [Ajax.AjaxMethod]
        public string GetValorMinimo(string codInterno, string tipoEntrega, string idCliente, string revenda, string idProdOrcaStr, string percDescontoQtdeStr, string altura)
        {
            return WebGlass.Business.Produto.Fluxo.Valor.Ajax.GetValorMinimoOrca(codInterno, tipoEntrega, idCliente, revenda, idProdOrcaStr, percDescontoQtdeStr, Request["IdOrca"], altura);
        }

        /// <summary>
        /// Retorna o Código/Descrição do produto
        /// </summary>
        [Ajax.AjaxMethod()]
        public string GetProduto(string codInterno, string tipoEntrega, string revenda, string idCliente, string percComissao, string percDescontoQtdeStr, string idLoja)
        {
            return WebGlass.Business.Produto.Fluxo.BuscarEValidar.Ajax.GetProdutoOrca(codInterno, tipoEntrega, revenda, idCliente, percComissao, percDescontoQtdeStr, idLoja, Request["IdOrca"]);
        }

        [Ajax.AjaxMethod]
        public string PercDesconto(string idOrcamentoStr, string idFuncAtualStr, string alterouDesconto)
        {
            var idOrcamento = idOrcamentoStr.StrParaInt();
            var idFuncAtual = idFuncAtualStr.StrParaInt();
            var idFuncDesc = Geral.ManterDescontoAdministrador ? OrcamentoDAO.Instance.ObterIdFuncDesc(null, idOrcamento).GetValueOrDefault() : 0;
            var verificarFuncionarioAtualAdministrador = UserInfo.IsAdministrador((uint)idFuncAtual);

            return (idFuncDesc == 0 || verificarFuncionarioAtualAdministrador || alterouDesconto.ToLower() == "true" ?
                OrcamentoConfig.Desconto.GetDescontoMaximoOrcamento((uint)idFuncAtual) : OrcamentoConfig.Desconto.GetDescontoMaximoOrcamento((uint)idFuncDesc)).ToString();
        }

        [Ajax.AjaxMethod()]
        public bool SubgrupoProdComposto(int idProd)
        {
            return SubgrupoProdDAO.Instance.ObtemTipoSubgrupo(null, idProd) == TipoSubgrupoProd.VidroDuplo;
        }

        #endregion

        #region Métodos auxiliares

        protected Color GetCorObsCliente()
        {
            return Liberacao.TelaLiberacao.CorExibirObservacaoCliente;
        }

        public void VerificarPodeApagarAtualizarInserir(Colosoft.WebControls.VirtualObjectDataSourceMethodEventArgs e)
        {
            var idOrcamento = Request["idOrca"].StrParaInt();
            var situacao = OrcamentoDAO.Instance.ObterSituacao(null, idOrcamento);

            if (idOrcamento > 0)
            {
                if (situacao == (int)Data.Model.Orcamento.SituacaoOrcamento.Negociado)
                {
                    e.Cancel = true;
                    Page.ClientScript.RegisterClientScriptBlock(GetType(), "Erro", $"alert('O orçamento está negociado, não é possível alterá-lo!'); redirectUrl('{ CaminhoListagemOrcamento() }');", true);
                    MensagemAlerta.ShowMsg("O orçamento está negociado, não é possível alterá - lo.", Page);
                }
            }
        }

        private string CaminhoListagemOrcamento()
        {
            if (!string.IsNullOrWhiteSpace(Request["IdRelDinamico"]))
            {
                return $"../Relatorios/Dinamicos/ListaDinamico.aspx?Id={ Request["IdRelDinamico"] }";
            }
            else
            {
                return "../Listas/LstOrcamento.aspx";
            }
        }

        private void RedirecionarListagemOrcamento()
        {
            Response.Redirect(CaminhoListagemOrcamento());
        }

        protected string GetDescontoProdutos()
        {
            try
            {
                if (!string.IsNullOrEmpty(Request["idOrca"]))
                {
                    return OrcamentoDAO.Instance.ObterDescontoProdutos(null, Request["idOrca"].StrParaInt()).ToString().Replace(",", ".");
                }
                else
                {
                    return "0";
                }
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
                if (!string.IsNullOrEmpty(Request["idOrca"]))
                {
                    return OrcamentoDAO.Instance.ObterDescontoOrcamento(null, Request["idOrca"].StrParaInt()).ToString().Replace(",", ".");
                }
                else
                {
                    return "0";
                }
            }
            catch
            {
                return "0";
            }
        }

        protected bool UtilizarRoteiroProducao()
        {
            return PCPConfig.ControlarProducao && Data.Helper.Utils.GetSetores.Count(x => x.SetorPertenceARoteiro) > 0;
        }

        protected string NomeControleBenef()
        {
            return grdProdutosOrcamento.EditIndex == -1 ? "ctrlBenefInserir" : "ctrlBenefEditar";
        }

        #endregion

        #region odsOrcamento

        protected void odsOrcamento_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao cadastrar Orçamento.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
            {
                hdfIdOrcamento.Value = e.ReturnValue.ToString();
                Response.Redirect("CadOrcamento.aspx?IdOrca=" + hdfIdOrcamento.Value);
            }
        }

        protected void odsOrcamento_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar dados do Orçamento.", e.Exception, Page);
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "callback", "retornaPagina();", true);
                e.ExceptionHandled = true;
            }
            else
                Response.Redirect("CadOrcamento.aspx?IdOrca=" + hdfIdOrcamento.Value + (relatorio > 0 ? "&relatorio=" + relatorio : ""));
        }

        #endregion

        #region odsProdutosAmbienteOrcamento

        protected void odsProdutosAmbienteOrcamento_Inserting(object sender, Colosoft.WebControls.VirtualObjectDataSourceMethodEventArgs e)
        {
            VerificarPodeApagarAtualizarInserir(e);
        }

        protected void odsProdutosAmbienteOrcamento_Updating(object sender, Colosoft.WebControls.VirtualObjectDataSourceMethodEventArgs e)
        {
            VerificarPodeApagarAtualizarInserir(e);
        }

        protected void odsProdutosAmbienteOrcamento_Deleting(object sender, Colosoft.WebControls.VirtualObjectDataSourceMethodEventArgs e)
        {
            VerificarPodeApagarAtualizarInserir(e);
        }

        protected void odsProdutosAmbienteOrcamento_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                MensagemAlerta.ErrorMsg("Falha ao atualizar o ambiente.", e.Exception, Page);
                e.ExceptionHandled = true;
            }

            hdfIdProdAmbienteOrcamento.Value = string.Empty;
            lblAmbiente.Text = string.Empty;
            dtvOrcamento.DataBind();
        }

        protected void odsProdutosAmbienteOrcamento_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                MensagemAlerta.ErrorMsg("Falha ao remover o ambiente.", e.Exception, Page);
                e.ExceptionHandled = true;
            }

            hdfIdProdAmbienteOrcamento.Value = string.Empty;
            lblAmbiente.Text = string.Empty;
            dtvOrcamento.DataBind();
        }

        #endregion

        #region odsProdutosOrcamento

        protected void odsProdutosOrcamento_Inserting(object sender, Colosoft.WebControls.VirtualObjectDataSourceMethodEventArgs e)
        {
            VerificarPodeApagarAtualizarInserir(e);
        }

        protected void odsProdutosOrcamento_Updating(object sender, Colosoft.WebControls.VirtualObjectDataSourceMethodEventArgs e)
        {
            VerificarPodeApagarAtualizarInserir(e);
        }

        protected void odsProdutosOrcamento_Deleting(object sender, Colosoft.WebControls.VirtualObjectDataSourceMethodEventArgs e)
        {
            VerificarPodeApagarAtualizarInserir(e);
        }

        protected void odsProdutosOrcamento_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                MensagemAlerta.ErrorMsg("Falha ao atualizar o produto.", e.Exception, Page);
                e.ExceptionHandled = true;
            }

            dtvOrcamento.DataBind();
        }

        protected void odsProdutosOrcamento_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                MensagemAlerta.ErrorMsg("Falha ao remover o produto.", e.Exception, Page);
                e.ExceptionHandled = true;
            }

            dtvOrcamento.DataBind();
        }

        #endregion

        #region grdProdutosAmbienteOrcamento

        protected void grdProdutosAmbienteOrcamento_PreRender(object sender, EventArgs e)
        {
            if (grdProdutosAmbienteOrcamento?.Rows?.Count > 0)
            {
                // Exibe a primeira linha somente se houver produto ambiente cadastrado para o orçamento.
                grdProdutosAmbienteOrcamento.Rows[0].Visible = ProdutosOrcamentoDAO.Instance.PesquisarProdutosAmbienteOrcamentoCount(null, Request["idOrca"].StrParaIntNullable()) > 0;
            }
        }

        protected void grdProdutosAmbienteOrcamento_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdProdutosAmbienteOrcamento.ShowFooter = e.CommandName != "Edit";

            if (e.CommandName == "ViewProd")
            {
                var idProdAmbiente = (e?.CommandArgument?.ToString()?.StrParaInt()).GetValueOrDefault();

                if (idProdAmbiente == 0)
                {
                    return;
                }

                // Mostra os produtos relacionado ao ambiente selecionado
                hdfIdProdAmbienteOrcamento.Value = idProdAmbiente.ToString();
                grdProdutosOrcamento.Visible = true;
                grdProdutosOrcamento.DataBind();

                // Mostra no label qual ambiente está sendo incluido produtos.
                var nomeAmbiente = ProdutosOrcamentoDAO.Instance.ObterNomeAmbiente(null, idProdAmbiente);
                lblAmbiente.Text = $"<br />{ nomeAmbiente }";
            }
            else if (e.CommandName == "Update")
            {
                Validacoes.DisableRequiredFieldValidator(Page);
            }
        }

        protected void grdProdutosAmbienteOrcamento_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
            if (grdProdutosOrcamento.Visible)
            {
                grdProdutosOrcamento.DataBind();
            }
        }

        protected void grdProdutosAmbienteOrcamento_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
            if (e.Exception != null)
            {
                MensagemAlerta.ErrorMsg("Falha ao excluir o ambiente.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }

        #endregion

        #region grdProdutosOrcamento

        protected void grdProdutosOrcamento_PreRender(object sender, EventArgs e)
        {
            if (grdProdutosOrcamento?.Rows?.Count > 0)
            {
                var idProdAmbiente = hdfIdProdAmbienteOrcamento?.Value?.StrParaIntNullable();

                // Exibe a primeira linha somente se houver produto cadastrado para o orçamento.
                grdProdutosOrcamento.Rows[0].Visible = ProdutosOrcamentoDAO.Instance.PesquisarProdutosOrcamentoCount(null, Request["idOrca"].StrParaInt(), idProdAmbiente) > 0;
            }
        }

        protected void grdProdutosOrcamento_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            FileUpload fluImagem = grdProdutosAmbienteOrcamento.Rows[e.RowIndex].FindControl("fluImagem") as FileUpload;

            if (fluImagem == null || !fluImagem.HasFile)
            {
                return;
            }

            var idProd = e.Keys[0].ToString().StrParaInt();
            ManipulacaoImagem.SalvarImagem($"{ Data.Helper.Utils.GetProdutosOrcamentoPath }{ idProd }.jpg", fluImagem.FileBytes);
        }

        protected void grdProdutosOrcamento_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
            grdProdutosAmbienteOrcamento.DataBind();
            dtvOrcamento.DataBind();
        }

        protected void grdProdutosOrcamento_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
            grdProdutosAmbienteOrcamento.DataBind();
            dtvOrcamento.DataBind();
        }

        #endregion
    }
}
