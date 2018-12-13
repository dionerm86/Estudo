using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Data.Model;
using System.Drawing;
using Glass.Data.NFeUtils;
using Glass.Data.EFD;
using System.Linq;
using Glass.Configuracoes;
using System.Web.UI.HtmlControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadNotaFiscal : System.Web.UI.Page
    {
        bool simplesNacional = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadNotaFiscal));
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));

            uint idNf = 0;

            if (Request["idNf"] != null && Request["idNf"].ToString() == "0")
            {
                Response.Redirect("CadNotaFiscal.aspx?tipo=" + Request["tipo"]);
            }

            if (!IsPostBack && Request["idNf"] != null)
            {
                idNf = Glass.Conversoes.StrParaUint(Request["idNF"]);

                bool isNfeImportacao = NotaFiscalDAO.Instance.IsNotaFiscalImportacao(idNf);

                // Mesmo que não possa excluir produto, esta opção deve ficar assim, para que preencha exatamente o que é preciso na NF
                if (isNfeImportacao && String.IsNullOrEmpty(Request["manual"]))
                    Response.Redirect("CadNotaFiscal.aspx?idNf=" + idNf + "&tipo=" + Request["tipo"] + "&manual=1");

                // Se esta NF não puder ser editada, volta para lista de NF
                if (!NotaFiscalDAO.Instance.PodeSerEditada(idNf) || (Request["manual"] != "1" && Request["tipo"] == "3" &&
                    NotaFiscalDAO.Instance.ObtemSituacao(idNf) == (int)NotaFiscal.SituacaoEnum.FinalizadaTerceiros))
                {
                    Response.Redirect("../Listas/LstNotaFiscal.aspx");
                    return;
                }

                // Verifica se o usuário tem permissão de alteração manual de NFe
                if (Request["manual"] == "1" && !Config.PossuiPermissao(Config.FuncaoMenuFiscal.AlteracaoManualNFe) && !isNfeImportacao)
                {
                    Response.Redirect("../Listas/LstNotaFiscal.aspx");
                    return;
                }

                grdProdutos.ShowFooter = Request["manual"] != "1" || isNfeImportacao;
                grdProdutos.Columns[15].Visible = Request["manual"] == "1";
                grdProdutos.Columns[18].Visible = Request["manual"] == "1";
                grdProdutos.Columns[22].Visible = Request["manual"] == "1";
                // Caso a nota fiscal seja do tipo Entrada de terceiros ou do tipo Entrada o campo Lote deve aparecer na Grid de produtos, caso contrário deve ser escondido.
                grdProdutos.Columns[25].Visible = isNfeImportacao || Request["tipo"] == "3" || Request["tipo"] == "4";

                int crt = LojaDAO.Instance.BuscaCrtLoja(null, NotaFiscalDAO.Instance.ObtemIdLoja(idNf));
                int crtForn = FornecedorDAO.Instance.BuscaRegimeFornec(idNf);
                int tipoDocumento = NotaFiscalDAO.Instance.GetTipoDocumento(idNf);

                simplesNacional = crt == 1 || crt == 2;

                //if (tipoDocumento == 3)
                //    simplesNacional = crtForn == 2;
                //else if (tipoDocumento == 2)
                //    simplesNacional = crt == 1 || crt == 2;

                hdfSimplesNacional.Value = simplesNacional.ToString();
                hdfTipoDocumento.Value = tipoDocumento.ToString();

                // Mostra CST ou CSOSN
                if (tipoDocumento == 3 && FornecedorDAO.Instance.BuscaRegimeFornec(idNf) == (int)RegimeFornecedor.SimplesNacional)
                {
                    //grdProdutos.Columns[11].Visible = false;
                    // Se a loja também for simples
                    if (simplesNacional)
                        /* Chamado 49020 */
                        grdProdutos.Columns[11].Visible = false;
                    else
                        /* Chamado 43993. */
                        grdProdutos.Columns[12].Visible = false;
                }
                else if ((tipoDocumento != 3 && (crt == (int)CrtLoja.LucroPresumido || crt == (int)CrtLoja.LucroReal)) ||
                    tipoDocumento == 3)//(tipoDocumento == 3 && crtForn == (int)Fornecedor.RegimeFornec.RegimeNormal))
                    grdProdutos.Columns[12].Visible = false;
                else
                    grdProdutos.Columns[11].Visible = false;

                /* Chamado 40005. */
                if (tipoDocumento != (int)NotaFiscal.TipoDoc.EntradaTerceiros ||
                    !CfopDAO.Instance.IsCfopDevolucao(CfopDAO.Instance.ObtemCodInterno
                        (NaturezaOperacaoDAO.Instance.ObtemIdCfop(NotaFiscalDAO.Instance.ObtemIdNaturezaOperacao(idNf)))))
                    // Sempre esconde o MVA, pois o mesmo deve ser preenchido apenas no cadastro de produto.
                    grdProdutos.Columns[9].Visible = false; // MVA

                if (!FiscalConfig.UtilizaFCI)
                {
                    grdProdutos.Columns[26].Visible = false;
                    grdProdutos.Columns[27].Visible = false;
                    grdProdutos.Columns[28].Visible = false;
                    grdProdutos.Columns[29].Visible = false;
                }
            }

            if (dtvNf.CurrentMode == DetailsViewMode.Insert)
            {
                if (Request["idNf"] != null) // Se for edição
                {
                    hdfIdNf.Value = Request["idNf"];

                    if (ProdutosNfDAO.Instance.CountInNf(Glass.Conversoes.StrParaUint(Request["idNf"])) == 0)
                        grdProdutos.Rows[0].Visible = false;

                    dtvNf.ChangeMode(DetailsViewMode.ReadOnly);
                }
                else // Se for inserção
                {
                    // Se o tipo da nota não tiver sido informado, volta para listagem de notas
                    if (Request["tipo"] != "1" && Request["tipo"] != "2" && Request["tipo"] != "3" && Request["tipo"] != "4")
                    {
                        Response.Redirect("../Listas/LstNotaFiscal.aspx");
                        return;
                    }

                    // Muda valor da drpTipoDocumento
                    ((DropDownList)dtvNf.FindControl("drpTipoDocumento")).SelectedValue = Request["tipo"];

                    //Se for NFC-e
                    if (IsConsumidor())
                        ((HiddenField)dtvNf.FindControl("hdfConsumidor")).Value = "true";
                }
            }

            if (dtvNf.FindControl("planoContas") != null)
                dtvNf.FindControl("planoContas").Visible = Request["tipo"] == "3";

            // Se for NF-e Complementar alterar o título da página
            if (dtvNf.FindControl("hdfFinalidade") != null && ((HiddenField)dtvNf.FindControl("hdfFinalidade")).Value == ((int)NotaFiscal.FinalidadeEmissaoEnum.Complementar).ToString())
            {
                Page.Title = "Emissão de NF-e Complementar";
                var idsNfRef = NotaFiscalDAO.Instance.ObtemIdsNfRef(Request["idNf"].StrParaUint());

                if (!string.IsNullOrEmpty(idsNfRef))
                    lblSubtitle.Text = "Ref. a NF-e " + NotaFiscalDAO.Instance.ObtemNumerosNFePeloIdNf(NotaFiscalDAO.Instance.ObtemIdsNfRef(Glass.Conversoes.StrParaUint(Request["idNf"])).ToString()) + "<br />";
            }
            // Se for NF-e de Ajuste altera o título da página
            else if (Glass.Conversoes.StrParaUint(Request["Finalidade"]) == (int)NotaFiscal.FinalidadeEmissaoEnum.Ajuste)
            {
                Page.Title = "Emissão de NF-e de Ajuste";

                if (dtvNf.FindControl("hdfFinalidade") != null)
                    ((HiddenField)dtvNf.FindControl("hdfFinalidade")).Value = Request["Finalidade"];
            }
            else if (dtvNf.FindControl("hdfFinalidade") != null && ((HiddenField)dtvNf.FindControl("hdfFinalidade")).Value == ((int)NotaFiscal.FinalidadeEmissaoEnum.Ajuste).ToString())
                Page.Title = "Emissão de NF-e de Ajuste";

            if (!IsPostBack)
            {
                hdfNaoVendeVidro.Value = Glass.Configuracoes.Geral.NaoVendeVidro().ToString().ToLower();
            }

            hdfMaxNumParc.Value = FiscalConfig.NotaFiscalConfig.NumeroParcelasNFe.ToString();
            grdProdutos.Visible = dtvNf.CurrentMode == DetailsViewMode.ReadOnly && !NotaFiscalDAO.Instance.IsSerieU(idNf);


            //Exibe a mensagem de diferença de calculo caso deva
            if (Request["exibirMensagem"] == "true")
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "script", "$(function () { alert('A diferença entre os valores da nota e da(s) liberação(ões) se dá por um ou mais pedidos das liberações possuir Icms e Desconto, além da configuração Agrupar produtos do(s) pedido(s) ao gerar NF-e estar marcada'); });", true);
            }
        }

        #region Eventos de produtos

        protected void lnkInsProd_Click(object sender, EventArgs e)
        {
            try
            {
                var produto = grdProdutos.FooterRow.FindControl("ctrlSelProd") as Glass.UI.Web.Controls.ctrlSelProduto;
                var idNf = Glass.Conversoes.StrParaUint(Request["IdNf"]);
                var crtsCsons = new List<CrtLoja> { CrtLoja.SimplesNacional, CrtLoja.SimplesNacionalExcSub };

                // Cria uma instância do ProdutosPedido
                ProdutosNf prodNf = new ProdutosNf();
                prodNf.IdNf = idNf;
                prodNf.IdProd = produto.IdProd.GetValueOrDefault();
                prodNf.DescricaoItemGenerico = produto.DescricaoItemGenerico;
                prodNf.CstOrig = Conversoes.StrParaInt(((DropDownList)grdProdutos.FooterRow.FindControl("drpOrigCst")).SelectedValue);
                prodNf.Cst = ((DropDownList)grdProdutos.FooterRow.FindControl("drpCst")).SelectedValue;
                prodNf.Csosn = ((DropDownList)grdProdutos.FooterRow.FindControl("drpCsosnIns")).SelectedValue;
                prodNf.IdNaturezaOperacao = (grdProdutos.FooterRow.FindControl("ctrlNaturezaOperacaoProd") as UI.Web.Controls.ctrlNaturezaOperacao).CodigoNaturezaOperacao;
                prodNf.Qtde = Conversoes.StrParaFloat(((TextBox)grdProdutos.FooterRow.FindControl("txtQtdeIns")).Text);
                prodNf.QtdeTrib = Conversoes.StrParaFloat(((TextBox)grdProdutos.FooterRow.FindControl("txtQtdeTrib")).Text);
                prodNf.ValorUnitario = Conversoes.StrParaDecimal(((TextBox)grdProdutos.FooterRow.FindControl("txtValorIns")).Text);
                prodNf.Altura = Conversoes.StrParaFloat(((TextBox)grdProdutos.FooterRow.FindControl("txtAlturaIns")).Text);
                prodNf.Largura = Conversoes.StrParaInt(((TextBox)grdProdutos.FooterRow.FindControl("txtLarguraIns")).Text);
                prodNf.TotM = Conversoes.StrParaFloat(((TextBox)grdProdutos.FooterRow.FindControl("txtTotM2Ins")).Text);

                if (!string.IsNullOrWhiteSpace(prodNf.Cst))
                {
                    prodNf.PercRedBcIcms = ((TextBox)grdProdutos.FooterRow.FindControl("txtPercRedBcIcms")).Text.StrParaFloat();
                    prodNf.PercRedBcIcmsSt = ((TextBox)grdProdutos.FooterRow.FindControl("txtPercRedBcIcmsSt")).Text.StrParaFloat();
                }
                else if (!string.IsNullOrWhiteSpace(prodNf.Csosn))
                {
                    prodNf.PercRedBcIcms = ((TextBox)grdProdutos.FooterRow.FindControl("txtCsosnPercRedBcIcms")).Text.StrParaFloat();
                    prodNf.PercRedBcIcmsSt = ((TextBox)grdProdutos.FooterRow.FindControl("txtCsosnPercRedBcIcmsSt")).Text.StrParaFloat();
                }

                var tipoDocumento = Request["tipo"].StrParaInt();

                var idLojaNf = NotaFiscalDAO.Instance.ObtemIdLoja(null, idNf);
                var crtEmit = LojaDAO.Instance.BuscaCrtLoja(null, idLojaNf);

                if (crtsCsons.Any(csosn => (int)csosn == crtEmit))
                {
                    prodNf.Csosn = ((DropDownList)grdProdutos.FooterRow.FindControl("drpCsosnIns")).SelectedValue;
                }
                else
                {
                    prodNf.Csosn = string.Empty;
                }                

                prodNf.AliqIcms = Conversoes.StrParaFloat(((TextBox)grdProdutos.FooterRow.FindControl("txtAliqIcmsIns")).Text);
                prodNf.Mva = Conversoes.StrParaFloat(((TextBox)grdProdutos.FooterRow.FindControl("txtMva")).Text);
                prodNf.AliqIcmsSt = Conversoes.StrParaFloat(((TextBox)grdProdutos.FooterRow.FindControl("txtAliqIcmsStIns")).Text);
                prodNf.AliqFcp = ((TextBox)grdProdutos.FooterRow.FindControl("txtAliqFcp")).Text.StrParaFloat();
                prodNf.AliqFcpSt = ((TextBox)grdProdutos.FooterRow.FindControl("txtAliqFcpSt")).Text.StrParaFloat();
                prodNf.AliqIpi = Conversoes.StrParaFloat(((TextBox)grdProdutos.FooterRow.FindControl("txtAliqIpiIns")).Text);
                prodNf.ValorIpi = Conversoes.StrParaDecimal(((TextBox)grdProdutos.FooterRow.FindControl("txtValorIpiIns")).Text);
                prodNf.Ncm = ((TextBox)grdProdutos.FooterRow.FindControl("txtNcm")).Text;
                prodNf.ParcelaImportada = Glass.Conversoes.StrParaDecimal(((TextBox)grdProdutos.FooterRow.FindControl("txtParcelaImportada")).Text);
                prodNf.SaidaInterestadual = Glass.Conversoes.StrParaDecimal(((TextBox)grdProdutos.FooterRow.FindControl("txtSaidaInterestadual")).Text);
                prodNf.ConteudoImportacao = Glass.Conversoes.StrParaDecimal(((TextBox)grdProdutos.FooterRow.FindControl("txtConteudoImportacao")).Text);
                prodNf.CodValorFiscal = Glass.Conversoes.StrParaUintNullable(((DropDownList)grdProdutos.FooterRow.FindControl("ddlCodValorFiscal")).SelectedValue);
                prodNf.ValorTotalTrib = Glass.Conversoes.StrParaDecimal(((TextBox)grdProdutos.FooterRow.FindControl("txtTotalTrib")).Text);
                prodNf.PercDiferimento = Glass.Conversoes.StrParaFloat(((TextBox)grdProdutos.FooterRow.FindControl("txtPercDiferimento")).Text);
                prodNf.ValorIcmsDesonerado = Glass.Conversoes.StrParaDecimal(((TextBox)grdProdutos.FooterRow.FindControl("txtValorIcmsDeson")).Text);
                prodNf.ValorIpiDevolvido = Conversoes.StrParaDecimal(((TextBox)grdProdutos.FooterRow.FindControl("txtValorIpiDevolvidoProd")).Text);

                var motivoDeson = (prodNf.Cst == "20" || prodNf.Cst == "30" || prodNf.Cst == "70" || prodNf.Cst == "90") ?
                    ((DropDownList)grdProdutos.FooterRow.FindControl("drpMotivoIcmsDeson")).SelectedValue : "";

                if (!string.IsNullOrEmpty(motivoDeson) && motivoDeson != "0")
                    prodNf.MotivoDesoneracao = Conversoes.StrParaInt(motivoDeson);

                prodNf.Lote = ((TextBox)grdProdutos.FooterRow.FindControl("txtLote")).Text;

                var numControleFci = ((TextBox)grdProdutos.FooterRow.FindControl("txtNumControleFci")).Text;
                if (NotaFiscalDAO.Instance.IsGuid(numControleFci))
                    prodNf.NumControleFciStr = numControleFci;

                if (FiscalConfig.NotaFiscalConfig.GerarEFD && (prodNf.CodValorFiscal == null || prodNf.CodValorFiscal == 0))
                {
                    throw new Exception("O código do valor fiscal ICMS deve ser selecionado.");
                }

                if (!NaturezaOperacaoDAO.Instance.ValidarCfop((int)prodNf.IdNaturezaOperacao.GetValueOrDefault(0), tipoDocumento))
                {
                    throw new Exception("A Natureza de operação selecionada não pode ser utilizada em notas desse tipo.");
                }

                string cstIpi = ((Glass.UI.Web.Controls.ctrlSelPopup)grdProdutos.FooterRow.FindControl("selCstIpi")).Valor;
                prodNf.CstIpi = Glass.Conversoes.StrParaIntNullable(cstIpi);

                string contaContabil = ((DropDownList)grdProdutos.FooterRow.FindControl("drpContaContabil")).SelectedValue;
                prodNf.IdContaContabil = Glass.Conversoes.StrParaUintNullable(contaContabil);

                string naturezaBcCredito = ((Glass.UI.Web.Controls.ctrlSelPopup)grdProdutos.FooterRow.FindControl("selNatBcCred")).Valor;
                prodNf.NaturezaBcCred = Glass.Conversoes.StrParaIntNullable(naturezaBcCredito);

                string indNaturezaFrete = ((Glass.UI.Web.Controls.ctrlSelPopup)grdProdutos.FooterRow.FindControl("selIndNatFrete")).Valor;
                prodNf.IndNaturezaFrete = Glass.Conversoes.StrParaIntNullable(indNaturezaFrete);

                string codCont = ((Glass.UI.Web.Controls.ctrlSelPopup)grdProdutos.FooterRow.FindControl("selCodCont")).Valor;
                prodNf.CodCont = Glass.Conversoes.StrParaIntNullable(codCont);

                string codCred = ((Glass.UI.Web.Controls.ctrlSelPopup)grdProdutos.FooterRow.FindControl("selCodCred")).Valor;
                prodNf.CodCred = Glass.Conversoes.StrParaIntNullable(codCred);

                string cstPis = ((Glass.UI.Web.Controls.ctrlSelPopup)grdProdutos.FooterRow.FindControl("selCstPis")).Valor;
                prodNf.CstPis = Glass.Conversoes.StrParaIntNullable(cstPis);

                prodNf.BcPis = Glass.Conversoes.StrParaDecimal(((TextBox)grdProdutos.FooterRow.FindControl("txtBcPis")).Text);
                prodNf.AliqPis = Glass.Conversoes.StrParaFloat(((TextBox)grdProdutos.FooterRow.FindControl("txtAliqPis")).Text);
                prodNf.ValorPis = Glass.Conversoes.StrParaDecimal(((TextBox)grdProdutos.FooterRow.FindControl("txtValorPis")).Text);

                /* string cstCofins = ((ctrlSelPopup)grdProdutos.FooterRow.FindControl("selCstCofins")).Valor;
                prodNf.CstCofins = Glass.Conversoes.StrParaIntNullable(cstCofins);

                prodNf.BcCofinsString = ((TextBox)grdProdutos.FooterRow.FindControl("txtBcCofins")).Text; */
                prodNf.AliqCofins = Glass.Conversoes.StrParaFloat(((TextBox)grdProdutos.FooterRow.FindControl("txtAliqCofins")).Text);
                prodNf.ValorCofins = Glass.Conversoes.StrParaDecimal(((TextBox)grdProdutos.FooterRow.FindControl("txtValorCofins")).Text);

                prodNf.NumACDrawback = ((TextBox)grdProdutos.FooterRow.FindControl("txtNumACDrawback")).Text;

                ProdutosNfDAO.Instance.Insert(prodNf);
                dtvNf.DataBind();

                grdProdutos.DataBind();
                grdProdutos.PageIndex = grdProdutos.PageCount - 1;
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao incluir produto na NF.", ex, Page);
                return;
            }
        }

        public bool IsNfFinalidadeDevolucao()
        {
            return NotaFiscalDAO.Instance.ObtemFinalidade(Request["idNf"].StrParaUint()) == (int)Glass.Data.Model.NotaFiscal.FinalidadeEmissaoEnum.Devolucao;
        }

        protected void odsProdutos_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg(null, e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
                dtvNf.DataBind();
        }

        protected void odsProdutos_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg(null, e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
                dtvNf.DataBind();
        }

        protected void grdProdutos_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
            dtvNf.DataBind();
        }

        protected void grdProdutos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdProdutos.ShowFooter = e.CommandName != "Edit";
        }

        #endregion

        #region Eventos DataSource

        protected void odsNf_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao cadastrar Nota Fiscal.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
            {
                hdfIdNf.Value = e.ReturnValue.ToString();
                Response.Redirect("CadNotaFiscal.aspx?IdNf=" + hdfIdNf.Value + "&tipo=" + Request["tipo"] + (Request["manual"] == "1" ? "&manual=1" : ""));
            }
        }

        protected void odsNf_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                e.ExceptionHandled = true;
                dtvNf.ChangeMode(DetailsViewMode.ReadOnly);
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar dados da Nota Fiscal.", e.Exception, Page);
            }
            else
            {
                if (String.IsNullOrEmpty(hdfIdNf.Value))
                    hdfIdNf.Value = Request["idNf"];

                Response.Redirect("CadNotaFiscal.aspx?IdNf=" + hdfIdNf.Value + "&tipo=" + Request["tipo"] + (Request["manual"] == "1" ? "&manual=1" : ""));
            }
        }

        #endregion

        #region Métodos AJAX

        /// <summary>
        /// Retorna o Código/Descrição do produto/kit
        /// </summary>
        [Ajax.AjaxMethod()]
        public string GetProduto(string idProd, string tipoEntrega, string idCliente, string idFornecedor, string idNf)
        {
            return WebGlass.Business.Produto.Fluxo.BuscarEValidar.Ajax.GetProdutoNotaFiscal(idProd, tipoEntrega, idCliente, idFornecedor, idNf);
        }

        [Ajax.AjaxMethod()]
        public string BuscaDadosFci(string idProd, string cstOrigStr)
        {
            var cstOrig = Glass.Conversoes.StrParaInt(cstOrigStr);

            decimal conteudoImportacao = 0;

            if (cstOrig == 5)
                conteudoImportacao = 40;
            else if (cstOrig == 3)
                conteudoImportacao = 70;
            else if (cstOrig == 8)
                conteudoImportacao = 100;

            var numControleFci = ProdutosArquivoFCIDAO.Instance.ObtemNumControleFci(Glass.Conversoes.StrParaUint(idProd), conteudoImportacao);

            if (!string.IsNullOrEmpty(numControleFci))
            {
                return numControleFci;
            }
            else
            {
                var valorParcelaImportada = ProdutosNfDAO.Instance.CalculaValorParcelaImportada(Glass.Conversoes.StrParaUint(idProd), DateTime.Now);
                var valorSaidaInterestadual = ProdutosNfDAO.Instance.CalculaValorSaidaInterestadual(Glass.Conversoes.StrParaUint(idProd), DateTime.Now);
                var valorConteudoImportacao = Math.Round((valorParcelaImportada / valorSaidaInterestadual) * 100, 2);

                return ";" + valorParcelaImportada + ";" + valorSaidaInterestadual + ";" + valorConteudoImportacao;
            }
        }

        /// <summary>
        /// Valida ao finalizar ou emitir uma nf se o fci deve ser informado
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string validaFciFinalizarEmitir(string idNf)
        {
            var prodsNf = ProdutosNfDAO.Instance.GetByNf(Glass.Conversoes.StrParaUint(idNf));

            string retorno = "";

            foreach (var p in prodsNf)
                if ((p.CstOrig == 3 || p.CstOrig == 5 || p.CstOrig == 8) && string.IsNullOrEmpty(p.NumControleFciStr))
                    retorno += p.CodInterno + " - " + p.DescrProduto + ";";

            return retorno.Trim(';');
        }

        /// <summary>
        /// Verifica se a natureza de operação da nota fiscal foi informada.
        /// </summary>
        [Ajax.AjaxMethod()]
        public string ValidaNaturezaOperacao(string idNf)
        {
            var idNaturezaOperacao = NotaFiscalDAO.Instance.ObtemIdNaturezaOperacao(idNf.StrParaUint());
            var naturezaOperacao = NaturezaOperacaoDAO.Instance.GetElementByPrimaryKey(idNaturezaOperacao);

            if (idNaturezaOperacao == 0 || naturezaOperacao == null)
                return "nao";

            return "sim";
        }

        /// <summary>
        /// Confirma se a nota fiscal deve ser finalizada caso a natureza de operação não esteja configurada para alterar estoque fiscal.
        /// </summary>
        [Ajax.AjaxMethod()]
        public string VerificarNaturezaOperacaoAlteraEstoqueFiscal(string idNf)
        {
            var idNaturezaOperacao = NotaFiscalDAO.Instance.ObtemIdNaturezaOperacao(idNf.StrParaUint());
            var naturezaOperacao = NaturezaOperacaoDAO.Instance.GetElementByPrimaryKey(idNaturezaOperacao);

            if (naturezaOperacao.AlterarEstoqueFiscal)
                return "sim";
            else
                return "nao";
        }

        [Ajax.AjaxMethod()]
        public string ClienteTemTransportador(string idCliente)
        {
            return (ClienteDAO.Instance.ObtemIdTransportador(Glass.Conversoes.StrParaUint(idCliente)).GetValueOrDefault(0) > 0).ToString().ToLower();
        }

        /// <summary>
        /// Valida ao finalizar ou emitir uma nf se o fci deve ser informado
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string habilitarReferenciaNFe(string idNf, string idCfop, string tipo)
        {
            if (idCfop.Length > 4)
                idCfop = idCfop.Substring(0, 4);

            var devolucaoRef = CfopDAO.Instance.IsCfopDevolucaoNFeRefereciada(idCfop);
            var notaAjuste = NotaFiscalDAO.Instance.ObtemFinalidade(Conversoes.StrParaUint(idNf)) == (int)NotaFiscal.FinalidadeEmissaoEnum.Ajuste;
            var consumidor = NotaFiscalDAO.Instance.IsNotaFiscalConsumidor(idNf.StrParaUint());

            if ((tipo == "1" || (tipo == "2" && devolucaoRef) || notaAjuste) && !consumidor)
                return "true";
            else if (tipo == "2" && idCfop == "5929")
                return "true";
            else if (tipo == "1" && consumidor && devolucaoRef)
                return "true";
            else
                return "false";
        }

        [Ajax.AjaxMethod()]
        public string ObterCalcularIcmsIpi(string idNaturezaOperacaoStr)
        {
            return WebGlass.Business.NaturezaOperacao.Fluxo.BuscarEValidar.Ajax.ObterCalcularIcmsIpi(idNaturezaOperacaoStr);
        }

        #endregion

        #region Editar NF

        protected void btnEditar_Click(object sender, EventArgs e)
        {
            lnkProduto.Visible = false;
            grdProdutos.Visible = false;
        }

        #endregion

        #region Cancelar

        protected void btnCancelarEdit_Click(object sender, EventArgs e)
        {
            hdfIdNf.Value = Request["idNf"];

            dtvNf.ChangeMode(DetailsViewMode.ReadOnly);

            lnkProduto.Visible = dtvNf.CurrentMode == DetailsViewMode.ReadOnly;
            grdProdutos.Visible = lnkProduto.Visible;
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Listas/LstNotaFiscal.aspx?tipo=" + Request["tipo"]);
        }

        #endregion

        #region Emitir/Pré-visualizar NFe

        protected void btnEmitir_Click(object sender, EventArgs e)
        {
            try
            {
                var notaFiscal = NotaFiscalDAO.Instance.GetElement(Request["idNf"].StrParaUint());

                if (notaFiscal.Situacao == (int)NotaFiscal.SituacaoEnum.Autorizada)
                    throw new Exception("A nota já se encontra autorizada");

                //Chamado 66308
                var usuarioFinalização = UserInfo.GetUserInfo != null && UserInfo.GetUserInfo.CodUser > 0 ? UserInfo.GetUserInfo.Nome : " ";
                LogNfDAO.Instance.NewLog(notaFiscal.IdNf, "Emissão ", 0, string.Format("Tentativa Emissão : {0}", usuarioFinalização));

                /* Chamado 21050. */
                if (PedidoConfig.LiberarPedido &&
                    FiscalConfig.NotaFiscalConfig.BloquearEmissaoAVistaComContaAReceberDeLiberacao &&
                    notaFiscal.FormaPagto > 0 &&
                    (NotaFiscal.FormaPagtoEnum)notaFiscal.FormaPagto == NotaFiscal.FormaPagtoEnum.AVista)
                {
                    var pedidosNotaFiscal = PedidosNotaFiscalDAO.Instance.GetByNf(notaFiscal.IdNf);

                    if (pedidosNotaFiscal != null && pedidosNotaFiscal.Any())
                        foreach (var pnf in pedidosNotaFiscal)
                            if (pnf.IdLiberarPedido.GetValueOrDefault() > 0 &&
                                ContasReceberDAO.Instance.ExisteReceberLiberacao(pnf.IdLiberarPedido.Value))
                                throw new Exception(
                                    string.Format(
                                        "Não é possível emitir esta nota fiscal com a forma de pagamento à vista, pois, existem contas a receber da liberação {0}",
                                            pnf.IdLiberarPedido.Value));
                }

                /* Chamado 34537. */
                if (notaFiscal.GerarEstoqueReal &&
                    notaFiscal.TipoDocumento == (int)NotaFiscal.TipoDoc.Saída &&
                    !CfopDAO.Instance.IsCfopDevolucao(NaturezaOperacaoDAO.Instance.ObtemIdCfop(notaFiscal.IdNaturezaOperacao.Value)))
                {
                    foreach (var produtoNotaFiscal in ProdutosNfDAO.Instance.GetByNf(notaFiscal.IdNf))
                        MovEstoqueDAO.Instance.ValidarMovimentarEstoque(null, (int)produtoNotaFiscal.IdProd, (int)notaFiscal.IdLoja, DateTime.Now,
                            MovEstoque.TipoMovEnum.Saida,
                            (decimal)ProdutosNfDAO.Instance.ObtemQtdDanfe(null, produtoNotaFiscal.IdProd, produtoNotaFiscal.TotM,
                                produtoNotaFiscal.Qtde, produtoNotaFiscal.Altura, produtoNotaFiscal.Largura, false, false), true);
                }

                //var prodsNf = ProdutosNfDAO.Instance.GetByNf(Glass.Conversoes.StrParaUint(Request["idNf"]));
                //if (prodsNf.Where(f => (f.CstOrig == 3 || f.CstOrig == 8 || f.CstOrig == 8) && string.IsNullOrEmpty(f.NumControleFciStr)).Count() > 0)
                //    throw new Exception("É necessário enviar um arquivo FCI para essa nota fiscal.");

                // Se estiver em contingência mas a forma de emissão desta nota for normal, gera uma nova nota em contingência
                if (FiscalConfig.NotaFiscalConfig.ContingenciaNFe == DataSources.TipoContingenciaNFe.SCAN &&
                    NotaFiscalDAO.Instance.ObtemFormaEmissao(Glass.Conversoes.StrParaUint(Request["idNf"])) == (int)NotaFiscal.TipoEmissao.Normal)
                {
                    uint idNf = NotaFiscalDAO.Instance.CriaNFeContingencia(NotaFiscalDAO.Instance.GetElement(Glass.Conversoes.StrParaUint(Request["idNf"])));

                    Response.Redirect("CadNotaFiscal.aspx?idNf=" + idNf + "&tipo=2");

                    return;
                }

                var retornoEmissao = NotaFiscalDAO.Instance.EmitirNf(Glass.Conversoes.StrParaUint(Request["idNf"]), false, false);
                if (retornoEmissao != "Lote processado.")
                {
                    //Se houver falha de emissão da NFC-e por erro de conexão, verifica se o usuário deseja emitir em contingencia offline
                    if (retornoEmissao.Contains("Impossível conectar-se ao servidor remoto") && NotaFiscalDAO.Instance.IsNotaFiscalConsumidor(Request["idNf"].StrParaUint()))
                        Response.Redirect("../Listas/LstNotaFiscal.aspx?falhaEmitirNFce=true&idNf=" + Request["idNf"]);

                    Response.Redirect("../Listas/LstNotaFiscal.aspx?autorizada=" + retornoEmissao);
                }

                if (UserInfo.GetUserInfo.UfLoja.ToUpper() != "BA" && UserInfo.GetUserInfo.UfLoja.ToUpper() != "SP")
                {
                    // Consulta a situação do lote e da NFe, caso o lote tenha sido processado
                    string msg = ConsultaSituacao.ConsultaSitNFe(Glass.Conversoes.StrParaUint(Request["idNf"]));

                    if (msg.ToLower().Contains("rejeicao"))
                        Response.Redirect("../Listas/LstNotaFiscal.aspx?autorizada=" + msg);
                    else
                        Response.Redirect("../Listas/LstNotaFiscal.aspx?autorizada=true", false);
                }
            }
            catch (Exception ex)
            {
                // Se for erro na validação do arquivo XML, abre popup para mostrar erros
                if (ex.Message.Contains("XML inconsistente."))
                {
                    string msg = Glass.MensagemAlerta.FormatErrorMsg("", ex).Replace("XML inconsistente.", "").Replace("Linha:", "%bl%%bl%Linha:");
                    ClientScript.RegisterClientScriptBlock(typeof(string), "msg", "openWindow(410, 540, '../Utils/ShowMsg.aspx?title=Falha na validação do arquivo da NF-e&msg=" + msg + "')", true);
                }
                else
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao emitir NF.", ex, Page);
                }
            }
        }

        protected void btnPreVisualizar_Click(object sender, EventArgs e)
        {
            try
            {
                var idNf = Conversoes.StrParaUint(Request["idNf"]);
                var tipoDoc = NotaFiscalDAO.Instance.GetTipoDocumento(idNf);

                NotaFiscalDAO.Instance.EmitirNf(idNf, true, false, false);

                if (tipoDoc == (int)NotaFiscal.TipoDoc.Entrada || tipoDoc == (int)NotaFiscal.TipoDoc.Saída)
                {
                    //NotaFiscalDAO.Instance.EmitirNf(idNf, true);
                    ClientScript.RegisterClientScriptBlock(typeof(string), "msg", "openWindow(600, 800, \"../Relatorios/NFe/RelBase.aspx?rel=Danfe&previsualizar=true&idNf=\" + " + idNf + ");", true);
                }
                else
                    ClientScript.RegisterClientScriptBlock(typeof(string), "msg", "openWindow(600, 800, '../Relatorios/NFe/RelBase.aspx?rel=NfTerceiros&idNf=" + idNf + "');", true);
            }
            catch (Exception ex)
            {
                // Se for erro na validação do arquivo XML, abre popup para mostrar erros
                if (ex.Message.Contains("XML inconsistente."))
                {
                    string msg = Glass.MensagemAlerta.FormatErrorMsg("", ex).Replace("XML inconsistente.", "").Replace("Linha:", "%bl%%bl%Linha:");
                    ClientScript.RegisterStartupScript(typeof(string), "msg", "openWindow(410, 540, '../Utils/ShowMsg.aspx?title=Falha na validação do arquivo da NF-e&msg=" + msg + "')", true);
                }
                else
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao pré-visualizar NF.", ex, Page);
                }
            }
        }

        #endregion

        #region Finalizar nota de entrada (terceiros)

        protected void btnFinalizar_Click(object sender, EventArgs e)
        {
            NotaFiscal objNf;
            IList<Compra> objCompra = null;

            try
            {
                // Recupera a nota fiscal
                uint idNf = Glass.Conversoes.StrParaUint(Request["idNF"]);
                objNf = NotaFiscalDAO.Instance.GetElement(idNf);

                if (objNf.ExibirComprasVisible)
                    objCompra = CompraDAO.Instance.GetByString(objNf.IdCompras);

                if (objCompra != null && objCompra.Sum(x => x.ValorTributado) > 0 && !objNf.GerarContasPagar)
                {
                    Glass.MensagemAlerta.ShowMsg("A compra referente a esta nota possui valor tributado. "
                                                 + "Neste caso é necessário gerar contas a pagar. "
                                                 + "Clique no botão Editar e marque a opção Gerar contas a pagar.", Page);

                    return;
                }

                if (objCompra != null && objCompra.Sum(x => x.ValorTributado) > 0 &&
                    Math.Round(objNf.TotalNota, 2) != Math.Round(objCompra.Sum(x => x.ValorTributado), 2))
                {
                    Glass.MensagemAlerta.ShowMsg(
                        "Esta Nota não pode ser finalizada. O valor da Nota Fiscal (" + objNf.TotalNota.ToString("C")
                        + ") diverge do valor tributado declarado na(s) compra(s) Nº "
                        + objNf.IdCompras + " (" + objCompra.Sum(x => x.ValorTributado).ToString("C") + ").", Page);
                    return;
                }

                // Finaliza a nota fiscal
                NotaFiscalDAO.Instance.FinalizarNotaEntradaTerceiros(idNf);

                // Redireciona para a lista
                Response.Redirect("~/Listas/LstNotaFiscal.aspx");
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao finalizar nota fiscal.", ex, Page);
            }
        }

        #endregion

        #region Finalizar nota com formulário de segurança

        protected void btnFinalizarFs_Click(object sender, EventArgs e)
        {
            try
            {
                NotaFiscalDAO.Instance.FinalizarNfFS(Glass.Conversoes.StrParaUint(Request["idNf"]));
                ClientScript.RegisterClientScriptBlock(typeof(string), "msg", "openWindow(600, 800, \"../Relatorios/NFe/RelBase.aspx?rel=Danfe&idNf=\" + " +
                    Request["idNf"] + "); redirectUrl(\"../Listas/LstNotaFiscal.aspx\");", true);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao finalizar NF.", ex, Page);
            }
        }

        #endregion

        #region Consulta Situação do Cadastro Contribuinte

        /// <summary>
        /// Verifica se e possivel consultar a situação do cliente em questão
        /// </summary>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string PodeConsultarCadastro(string idContribuinte, string tipoContr)
        {
            uint idContr = Glass.Conversoes.StrParaUint(idContribuinte);

            if (tipoContr == "cliente")
                return ConsultaSituacao.HabilitadoConsultaCadastro(ClienteDAO.Instance.ObtemUf(idContr)).ToString();
            else
                return ConsultaSituacao.HabilitadoConsultaCadastro(FornecedorDAO.Instance.GetElement(idContr).Uf).ToString();
        }

        protected void ctrlConsultaCadCliSintegra1_Load(object sender, EventArgs e)
        {
            Glass.UI.Web.Controls.ctrlConsultaCadCliSintegra ConsCad = (Glass.UI.Web.Controls.ctrlConsultaCadCliSintegra)sender;

            HiddenField idCliFornc = null;

            if ((HiddenField)dtvNf.FindControl("hdfIdCliente") != null)
                idCliFornc = (HiddenField)dtvNf.FindControl("hdfIdCliente");
            else if ((HiddenField)dtvNf.FindControl("hdfIdFornec") != null)
                idCliFornc = (HiddenField)dtvNf.FindControl("hdfIdFornec");

            ConsCad.CtrlCliente = idCliFornc;
        }

        #endregion

        #region Antecipação de Fornecedor

        protected void drpFormaPagto_Load(object sender, EventArgs e)
        {
            //Se não for entrada de terceiros.
            if (Request["tipo"] != "3" || !FinanceiroConfig.UsarPgtoAntecipFornec ||
                FornecedorConfig.TipoUsoAntecipacaoFornecedor != DataSources.TipoUsoAntecipacaoFornecedor.CompraOuNotaFiscal)
                return;

            DropDownList drpFormaPagto = (DropDownList)sender;
            drpFormaPagto.Items.Add(new ListItem("Antecipação", ((int)Glass.Data.Model.Pagto.FormaPagto.AntecipFornec).ToString()));
            drpFormaPagto.Attributes.Add("onchange", "formaPagtoChanged(this.value);");
        }

        [Ajax.AjaxMethod()]
        public string GetDescrAntecipFornec(string idAntecipFornec)
        {
            uint idAntecip = Glass.Conversoes.StrParaUint(idAntecipFornec);

            if (idAntecip == 0)
                return "";

            return AntecipacaoFornecedorDAO.Instance.GetDescricao(idAntecip);
        }

        #endregion

        #region Vigência tabela preço fornecedor

        [Ajax.AjaxMethod()]
        public string VigenciaPrecoFornec(string idFornec)
        {
            uint id = Glass.Conversoes.StrParaUint(idFornec);

            if (FornecedorDAO.Instance.VigenciaPrecoExpirada(id))
                return "Erro;Este fornecedor está inativo por data de vigência da tabela de preço expirada.";
            else
                return "Ok;";
        }

        #endregion

        #region Loads

        protected void ctrlParcelas1_Load(object sender, EventArgs e)
        {
            Glass.UI.Web.Controls.ctrlParcelas ctrlParcelas = (Glass.UI.Web.Controls.ctrlParcelas)sender;
            HiddenField hdfCalcularParcelas = (HiddenField)dtvNf.FindControl("hdfCalcularParcelas");
            HiddenField hdfExibirParcelas = (HiddenField)dtvNf.FindControl("hdfExibirParcelas");
            TextBox txtNumParc = (TextBox)dtvNf.FindControl("txtNumParc");
            HiddenField hdfTotal = (HiddenField)dtvNf.FindControl("hdfTotalParcelas");
            //TextBox txtDesconto = (TextBox)dtvNf.FindControl("ctrDesconto").FindControl("txtNumber");
            //HiddenField hdfDesconto = (HiddenField)dtvNf.FindControl("hdfDesconto");
            //HiddenField hdfAcrescimo = (HiddenField)dtvNf.FindControl("hdfAcrescimo");
            //HiddenField hdfAcrescimoAnterior = (HiddenField)dtvNf.FindControl("hdfAcrescimoAnterior");
            //TextBox txtEntrada = (TextBox)dtvNf.FindControl("ctrValEntrada").FindControl("txtNumber");

            ctrlParcelas.CampoCalcularParcelas = hdfCalcularParcelas;
            ctrlParcelas.CampoExibirParcelas = hdfExibirParcelas;
            ctrlParcelas.CampoParcelasVisiveis = txtNumParc;
            ctrlParcelas.CampoValorTotal = hdfTotal;
            ctrlParcelas.NumParcelas = FiscalConfig.NotaFiscalConfig.NumeroParcelasNFe;
            ctrlParcelas.DiasSomarDataVazia = 30;
            //ctrlParcelas.CampoValorDescontoAnterior = hdfDesconto;
            //ctrlParcelas.CampoValorDescontoAtual = txtDesconto;
            //ctrlParcelas.CampoValorAcrescimoAnterior = hdfAcrescimoAnterior;
            //ctrlParcelas.CampoValorAcrescimoAtual = hdfAcrescimo;
            //ctrlParcelas.CampoValorEntrada = txtEntrada;
        }

        protected void txtSerie_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var tipo = Request["tipo"].StrParaIntNullable();
                var finalidade = Request["tipo"].StrParaIntNullable();
                var serie = "";

                if (tipo.GetValueOrDefault(0) != (int)NotaFiscal.TipoDoc.EntradaTerceiros && tipo.GetValueOrDefault(0) != (int)NotaFiscal.TipoDoc.NotaCliente)
                {
                    serie = FiscalConfig.NotaFiscalConfig.SeriePadraoNFe(null, null, finalidade.GetValueOrDefault(0) == (int)NotaFiscal.FinalidadeEmissaoEnum.Ajuste).ToString();
                }

                ((TextBox)sender).Text = serie;
            }
        }

        protected void btnEmitirFinalizar_Load(object sender, EventArgs e)
        {
            uint idNf = Glass.Conversoes.StrParaUint(Request["idNF"]);
            int tipoDocumento = NotaFiscalDAO.Instance.GetTipoDocumento(idNf);

            var tiposComparar = new List<int>() {
                (int)Glass.Data.Model.NotaFiscal.TipoDoc.EntradaTerceiros,
                (int)Glass.Data.Model.NotaFiscal.TipoDoc.NotaCliente
            };

            ((Button)sender).Visible = ((Button)sender).ID == "btnEmitir" ? !tiposComparar.Contains(tipoDocumento) &&
                FiscalConfig.NotaFiscalConfig.ContingenciaNFe != DataSources.TipoContingenciaNFe.FSDA : ((Button)sender).ID == "btnFinalizarFs" ?
                IsContingenciaFsda() : tiposComparar.Contains(tipoDocumento) && Request["manual"] != "1";

            if (((Button)sender).ID == "btnEmitir" && tipoDocumento == (int)Glass.Data.Model.NotaFiscal.TipoDoc.Saída && Request["manual"] == "1")
                ((Button)sender).Visible = false;

            if (((Button)sender).ID == "btnEmitir" && ((Button)sender).Visible)
            {
                // Se for para gerar contingência, altera a descrição do botão
                bool gerarNfContingencia = FiscalConfig.NotaFiscalConfig.ContingenciaNFe == DataSources.TipoContingenciaNFe.SCAN &&
                    NotaFiscalDAO.Instance.ObtemFormaEmissao(Glass.Conversoes.StrParaUint(Request["idNf"])) == (int)NotaFiscal.TipoEmissao.Normal;

                ((Button)sender).Text = (!gerarNfContingencia ? "Emitir Nota Fiscal" : "Gerar NFe de Contingência");
                ((Button)sender).ForeColor = !gerarNfContingencia ? Color.Red : Color.Blue;
                ((Button)sender).Width = !gerarNfContingencia ? new Unit("125px") : new Unit("170px");
            }
        }

        /// <summary>
        /// Todos os controles que referenciarem esse evento ficarão visíveis somente se for NF de entrada de terceiros
        /// </summary>
        protected void EntradaTerceiros_Load(object sender, EventArgs e)
        {
            if (sender is WebControl)
            {
                ((WebControl)sender).Visible = IsNfEntradaTerceiros();

                // Não exibe se for alteração manual
                if (((WebControl)sender).ID == "chkCompl")
                    ((WebControl)sender).Visible = ((WebControl)sender).Visible && Request["manual"] != "1";
                else if (((WebControl)sender).ID == "chkGerarEstoqueReal")
                    ((WebControl)sender).Visible = (((WebControl)sender).Visible || IsNfImportacao()) && !EstoqueConfig.EntradaEstoqueManual;
            }
            else if (sender is System.Web.UI.HtmlControls.HtmlGenericControl)
                ((System.Web.UI.HtmlControls.HtmlGenericControl)sender).Visible = IsNfEntradaTerceiros();
        }

        protected void chkGerarEtiqueta_Load(object sender, EventArgs e)
        {
            var idNf = Glass.Conversoes.StrParaUint(hdfIdNf.Value);

            // Não permite usar/modificar a opção de gerar etiqueta caso a nota seja de saída ou seja alteração manual de valores
            if (Request["tipo"] == ((int)NotaFiscal.TipoDoc.Saída).ToString() ||
                (Request["manual"] == "1" && NotaFiscalDAO.Instance.ObtemSituacao(idNf) != (int)NotaFiscal.SituacaoEnum.Aberta))
                ((CheckBox)sender).Visible = false;
            else if (dtvNf.CurrentMode == DetailsViewMode.Insert && String.IsNullOrEmpty(hdfIdNf.Value))
                ((CheckBox)dtvNf.FindControl("chkGerarEtiqueta")).Checked = true;
        }

        protected void GerarContasPagar_Load(object sender, EventArgs e)
        {
            uint idNf = Glass.Conversoes.StrParaUint(hdfIdNf.Value);

            if (idNf > 0 && IsNfEntradaTerceiros() && CompraNotaFiscalDAO.Instance.PossuiCompra(idNf) && FinanceiroConfig.SepararValoresFiscaisEReaisContasPagar)
                (sender as CheckBox).Enabled = false;
            else
                EntradaTerceiros_Load(sender, e);
        }

        protected void GerarEstoqueReal_Load(object sender, EventArgs e)
        {
            string idNf = hdfIdNf.Value;

            int tipoDocumento = dtvNf.CurrentMode == DetailsViewMode.Insert && String.IsNullOrEmpty(idNf) ? Glass.Conversoes.StrParaInt(Request["tipo"]) :
                NotaFiscalDAO.Instance.GetTipoDocumento(Glass.Conversoes.StrParaUint(idNf));

            ((CheckBox)sender).Visible = (((CheckBox)sender).Visible ||
                    tipoDocumento == (int)NotaFiscal.TipoDoc.Saída || !EstoqueConfig.EntradaEstoqueManual) && !IsConsumidor();
        }

        protected void CamposEditarManual_Load(object sender, EventArgs e)
        {
            // Se for nota de ajuste, permite alterar o valor do icms do produto
            bool isNotaAjuste = (((WebControl)sender).ID.Contains("ValorIcmsIns") && dtvNf.FindControl("hdfFinalidade") != null &&
                ((HiddenField)dtvNf.FindControl("hdfFinalidade")).Value == ((int)NotaFiscal.FinalidadeEmissaoEnum.Ajuste).ToString());

            bool manual = Request["manual"] == "1" || isNotaAjuste;
            ((WebControl)sender).Visible = sender is Label ? !manual : manual;
        }

        protected void AlteracaoManual_Load(object sender, EventArgs e)
        {
            if (Request["manual"] == "1" && Request["tipo"] == "3")
                ((System.Web.UI.HtmlControls.HtmlContainerControl)sender).Style.Add("display", "none");
        }

        protected void btnPreVisualizar_Load(object sender, EventArgs e)
        {
            ((Button)sender).Visible = Request["tipo"] != ((int)NotaFiscal.TipoDoc.EntradaTerceiros).ToString() &&
                FiscalConfig.NotaFiscalConfig.ContingenciaNFe == DataSources.TipoContingenciaNFe.NaoUtilizar;

            ((Button)sender).Visible = Request["tipo"] == ((int)NotaFiscal.TipoDoc.Saída).ToString() ||
                Request["tipo"] == ((int)NotaFiscal.TipoDoc.Entrada).ToString();
        }

        protected void txtNumNfe_Load(object sender, EventArgs e)
        {
            var tipos = new List<int>() {
                (int)NotaFiscal.TipoDoc.EntradaTerceiros,
                (int)NotaFiscal.TipoDoc.NotaCliente
            }.ConvertAll(x => x.ToString());

            // Habilita o campo para preencher o número da nota fiscal se for entrada de terceiros
            // ou se for a primeira nota de saída
            ((WebControl)sender).Enabled = tipos.Contains(Request["tipo"]) ||
                !NotaFiscalDAO.Instance.ExisteNotaSaida(null, Glass.Conversoes.StrParaUint(Request["idNf"]));
        }

        protected void txtAliqPis_Load(object sender, EventArgs e)
        {
            if ((sender as TextBox).Text == "")
                (sender as TextBox).Text = ConfigNFe.AliqPis(GetIdLoja()).ToString();
        }

        protected void txtAliqCofins_Load(object sender, EventArgs e)
        {
            if ((sender as TextBox).Text == "")
                (sender as TextBox).Text = ConfigNFe.AliqCofins(GetIdLoja()).ToString();
        }

        protected void selCstIpi_Load(object sender, EventArgs e)
        {
            if ((sender as Glass.UI.Web.Controls.ctrlSelPopup).Valor != "")
                return;

            var cst = ConfigNFe.CstIpi(0);
            (sender as Glass.UI.Web.Controls.ctrlSelPopup).Valor = ((int)cst).ToString();
            (sender as Glass.UI.Web.Controls.ctrlSelPopup).Descricao = Colosoft.Translator.Translate(cst).Format();
        }

        protected void selCstPisCofins_Load(object sender, EventArgs e)
        {
            if ((sender as Glass.UI.Web.Controls.ctrlSelPopup).Valor != "")
                return;

            uint idNf = Glass.Conversoes.StrParaUint(Request["idNf"]);
            int cst = ConfigNFe.CstPisCofins(idNf);
            (sender as Glass.UI.Web.Controls.ctrlSelPopup).Valor = cst.ToString();
            (sender as Glass.UI.Web.Controls.ctrlSelPopup).Descricao = DataSourcesEFD.Instance.GetDescrCstPisCofins(cst);
        }

        protected void selCodCont_Load(object sender, EventArgs e)
        {
            if ((sender as Glass.UI.Web.Controls.ctrlSelPopup).Valor != "" || FiscalConfig.NotaFiscalConfig.TipoNotaBuscarContribuicaoSocialPadrao == null)
                return;

            uint idNf = Glass.Conversoes.StrParaUint(Request["idNf"]);
            int tipoDoc = NotaFiscalDAO.Instance.GetTipoDocumento(idNf);

            if (tipoDoc == (int)NotaFiscal.TipoDoc.Saída && FiscalConfig.NotaFiscalConfig.TipoNotaBuscarContribuicaoSocialPadrao
                != DataSourcesEFD.TipoUsoCredCont.Entrada ||
                tipoDoc != (int)NotaFiscal.TipoDoc.Saída && FiscalConfig.NotaFiscalConfig.TipoNotaBuscarContribuicaoSocialPadrao != DataSourcesEFD.TipoUsoCredCont.Saída)
            {
                int? valor = (int?)FiscalConfig.NotaFiscalConfig.TipoContribuicaoSocialPadrao;
                (sender as Glass.UI.Web.Controls.ctrlSelPopup).Valor = valor != null ? valor.ToString() : "";
                (sender as Glass.UI.Web.Controls.ctrlSelPopup).Descricao = DataSourcesEFD.Instance.GetDescrCodCont(valor);
            }
        }

        protected void selCodCred_Load(object sender, EventArgs e)
        {
            if ((sender as Glass.UI.Web.Controls.ctrlSelPopup).Valor != "" || FiscalConfig.NotaFiscalConfig.TipoNotaBuscarCreditoPadrao == null)
                return;

            uint idNf = Glass.Conversoes.StrParaUint(Request["idNf"]);
            int tipoDoc = NotaFiscalDAO.Instance.GetTipoDocumento(idNf);

            if (tipoDoc == (int)NotaFiscal.TipoDoc.Saída && FiscalConfig.NotaFiscalConfig.TipoNotaBuscarCreditoPadrao
                != DataSourcesEFD.TipoUsoCredCont.Entrada ||
                tipoDoc != (int)NotaFiscal.TipoDoc.Saída && FiscalConfig.NotaFiscalConfig.TipoNotaBuscarCreditoPadrao
                != DataSourcesEFD.TipoUsoCredCont.Saída)
            {
                int? valor = (int?)FiscalConfig.NotaFiscalConfig.TipoCreditoPadrao;
                (sender as Glass.UI.Web.Controls.ctrlSelPopup).Valor = valor != null ? valor.ToString() : "";
                (sender as Glass.UI.Web.Controls.ctrlSelPopup).Descricao = DataSourcesEFD.Instance.GetDescrCodCred(valor);
            }
        }

        #endregion

        /// <summary>
        /// Verifica se e possivel consultar a situação do cliente em questão
        /// </summary>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string VerificaSeFornecImportacao(string idFornecedor)
        {
            var idFornec = Conversoes.StrParaInt(idFornecedor);

            var fornec = Microsoft.Practices.ServiceLocation.ServiceLocator.Current
                .GetInstance<Glass.Global.Negocios.IFornecedorFluxo>()
                .ObtemFornecedor(idFornec);

            return (fornec.IdPais != 30).ToString().ToLower();
        }

        protected int TipoDocumentoNF()
        {
            uint idNf = Glass.Conversoes.StrParaUint(Request["idNF"]);

            if (idNf == 0)
                return 0;

            return NotaFiscalDAO.Instance.GetTipoDocumento(idNf);
        }

        protected bool IsContingenciaFsda()
        {
            int tipoDocumento = TipoDocumentoNF();
            int tipoComparar = (int)Glass.Data.Model.NotaFiscal.TipoDoc.EntradaTerceiros;

            return tipoDocumento != tipoComparar &&
                FiscalConfig.NotaFiscalConfig.ContingenciaNFe == DataSources.TipoContingenciaNFe.FSDA;
        }

        protected bool IsNfImportacao()
        {
            uint idNf = Glass.Conversoes.StrParaUint(Request["idNF"]);
            if (idNf == 0)
                return false;

            return NotaFiscalDAO.Instance.IsNotaFiscalImportacao(idNf);
        }

        protected bool IsNfExportacao()
        {
            uint idNf = Request["idNF"].StrParaUint();
            if (idNf == 0)
                return false;

            return NotaFiscalDAO.Instance.IsNotaFiscalExportacao(null, idNf);
        }

        protected bool EmiteEFD()
        {
            return FiscalConfig.NotaFiscalConfig.GerarEFD;
        }

        protected bool IsNfEntradaTerceiros()
        {
            uint idNf = Glass.Conversoes.StrParaUint(Request["idNf"]);

            if (idNf == 0)
                return Glass.Conversoes.StrParaInt(Request["tipo"]) == 3;

            return NotaFiscalDAO.Instance.GetTipoDocumento(idNf) == (int)NotaFiscal.TipoDoc.EntradaTerceiros;
        }

        protected bool UtilizaFCI()
        {
            return FiscalConfig.UtilizaFCI;
        }

        protected bool IsNfComplementar()
        {
            return NotaFiscalDAO.Instance.IsComplementar(Glass.Conversoes.StrParaUint(Request["idNf"]));
        }

        protected bool HabilitarPisCofins()
        {
            return IsNfEntradaTerceiros() || IsNfComplementar();
        }

        protected void drpPeriodoIpi_DataBound(object sender, EventArgs e)
        {
            ((DropDownList)sender).SelectedValue = ((int)FiscalConfig.NotaFiscalConfig.PeriodoApuracaoIpi).ToString();
        }

        protected void grdProdutos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            var footer = e.Row.RowType == DataControlRowType.Footer;

            if (footer)
            {
                DropDownList ddlCodValorFiscal = e.Row.FindControl("ddlCodValorFiscal") as DropDownList;

                if (simplesNacional)
                    ddlCodValorFiscal.SelectedValue = "3";
            }
        }

        protected uint GetIdLoja()
        {
            uint idNf = Glass.Conversoes.StrParaUint(Request["idNf"]);
            return NotaFiscalDAO.Instance.ObtemIdLoja(idNf);
        }

        protected bool ExibirRemoverProduto(object idNf)
        {
            return Request["manual"] != "1" || NotaFiscalDAO.Instance.IsNotaFiscalImportacao(Convert.ToUInt32(idNf));
        }

        protected void dtvNf_DataBound(object sender, EventArgs e)
        {
            //Se for nota de entrada ou de saída habilita os campos de nf referenciada
            if ((Request["tipo"] == "1" || Request["tipo"] == "2") && dtvNf.CurrentMode != DetailsViewMode.ReadOnly && Request["mod"] != "65")
            {
                uint idNf = Glass.Conversoes.StrParaUint(Request["idNf"]);
                var numNfRef = string.Empty;

                if (dtvNf.FindControl("lblNfReferenciada") != null)
                    ((Label)dtvNf.FindControl("lblNfReferenciada")).Visible = true;

                if (dtvNf.FindControl("lblNotaFiscalConsumidorReferenciada") != null)
                    ((Label)dtvNf.FindControl("lblNotaFiscalConsumidorReferenciada")).Style.Add("display", "none");

                if (dtvNf.FindControl("txtNfReferenciada") != null)
                {
                    ((TextBox)dtvNf.FindControl("txtNfReferenciada")).Visible = true;

                    if (idNf > 0)
                    {
                        var idsNfRef = NotaFiscalDAO.Instance.ObtemIdsNfRef(idNf);
                        if (!string.IsNullOrEmpty(idsNfRef) && idsNfRef.Contains(','))
                        {
                            foreach (var i in idsNfRef.Split(','))
                                numNfRef += NotaFiscalDAO.Instance.ObtemNumeroNf(null, Conversoes.StrParaUint(i)) + ", ";
                        }
                        else if (!string.IsNullOrEmpty(idsNfRef))
                            numNfRef = NotaFiscalDAO.Instance.ObtemNumeroNf(null, Conversoes.StrParaUint(idsNfRef)).ToString();

                        if (!string.IsNullOrEmpty(numNfRef))
                            ((TextBox)dtvNf.FindControl("txtNfReferenciada")).Text = numNfRef;
                    }
                }

                if (dtvNf.FindControl("imbOpenNfReferenciada") != null)
                    ((ImageButton)dtvNf.FindControl("imbOpenNfReferenciada")).Visible = true;
            }
        }

        protected void txtSuframa_DataBinding(object sender, EventArgs e)
        {
            ((TextBox)sender).Style.Add("display", String.IsNullOrEmpty(((TextBox)sender).Text) ? "none" : "inline");
            ((Label)dtvNf.FindControl("lblSuframa")).Style.Add("display", String.IsNullOrEmpty(((TextBox)sender).Text) ? "none" : "inline");
        }

        protected void ctrlSelProd_Load(object sender, EventArgs e)
        {
            (sender as Glass.UI.Web.Controls.ctrlSelProduto).Compra = TipoDocumentoNF() != (int)NotaFiscal.TipoDoc.Saída;
        }

        protected void ctrlNaturezaOperacaoProd_Load(object sender, EventArgs e)
        {
            var linha = (sender as Control).Parent.Parent;

            (sender as Glass.UI.Web.Controls.ctrlNaturezaOperacao).CampoCstIcms = linha.FindControl("drpCst") ?? linha.FindControl("drpCstIns");
            (sender as Glass.UI.Web.Controls.ctrlNaturezaOperacao).CampoPercReducaoBcIcms = linha.FindControl("txtPercRedBcIcms") ?? linha.FindControl("txtCsosnPercRedBcIcms");
            (sender as Glass.UI.Web.Controls.ctrlNaturezaOperacao).CampoPercDiferimento = linha.FindControl("txtPercDiferimento");
            (sender as Glass.UI.Web.Controls.ctrlNaturezaOperacao).CampoCstIpi = linha.FindControl("selCstIpi").FindControl("txtDescr");
            (sender as Glass.UI.Web.Controls.ctrlNaturezaOperacao).CampoCstPisCofins = linha.FindControl("selCstPis").FindControl("txtDescr");
            (sender as Glass.UI.Web.Controls.ctrlNaturezaOperacao).CampoCsosn = linha.FindControl("drpCsosn") ?? linha.FindControl("drpCsosnIns");
        }

        private bool LojaEstoque()
        {
            return EstoqueConfig.ControlarEstoqueVidrosClientes && Request["tipo"] == "4";
        }

        protected string ExibirLojaEstoque()
        {
            return LojaEstoque() ? "" : "display: none";
        }

        protected string ObtemNomeLoja(object idLoja)
        {
            if (idLoja == null || !(idLoja is uint) || !LojaEstoque())
                return String.Empty;

            return WebGlass.Business.Loja.Fluxo.BuscarEValidar.Instance.ObtemNomeLoja((uint)idLoja);
        }

        protected void txtAliquota_Load(object sender, EventArgs e)
        {
            // Esconde campos de impostos
            if (FiscalConfig.NotaFiscalConfig.EsconderIcmsIcmsStIpiNotaSaida && TipoDocumentoNF() == (int)NotaFiscal.TipoDoc.Saída)
                ((TextBox)sender).Style.Add("display", "none");
        }

        protected void Nfce_Load(object sender, EventArgs e)
        {
            if (sender is WebControl && IsConsumidor())
            {
                ((WebControl)sender).Visible = false;
            }
        }

        protected bool IsConsumidor()
        {
            return (TipoDocumentoNF() == (int)NotaFiscal.TipoDoc.Saída && NotaFiscalDAO.Instance.IsNotaFiscalConsumidor(hdfIdNf.Value.StrParaUint())) ||
                (Request["tipo"] == "2" && Request["mod"] == "65");
        }

        [Ajax.AjaxMethod()]
        public bool HabilitarCpfCnpj(string idCli, string tipo, string mod, string idNf)
        {
            var tipoDoc = string.IsNullOrEmpty("idNf") ? 0 : NotaFiscalDAO.Instance.GetTipoDocumento(idNf.StrParaUint());
            var isNotaFiscalConsumidor = NotaFiscalDAO.Instance.IsNotaFiscalConsumidor(idNf.StrParaUint());
            var isConsumidorFinal = ClienteDAO.Instance.IsConsumidorFinal(idCli.StrParaUint());

            return ((tipoDoc == (int)NotaFiscal.TipoDoc.Saída && isNotaFiscalConsumidor) || (tipo == "2" && mod == "65")) && isConsumidorFinal;
        }

        /// <summary>
        /// Obtem os dados do veiculo atravez da placa
        /// </summary>
        /// <param name="placa"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string ObterDadosVeiculo(string placa)
        {
            if (string.IsNullOrEmpty(placa))
            {
                return string.Empty;
            }

            var veiculo = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Glass.Global.Negocios.IVeiculoFluxo>().ObtemVeiculo(placa);

            if (veiculo != null)
            {
                return $"{veiculo.RNTC},{veiculo.UfLicenc}";
            }

            return string.Empty;
        }
    }
}
