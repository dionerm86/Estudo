using System;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Data.DAL.CTe;
using Glass.Data.Model.Cte;
using System.Drawing;
using Glass.Configuracoes;
using Glass.Data.DAL;
using Glass.Data.Model;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadConhecimentoTransporte : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadConhecimentoTransporte));
    
            if (!IsPostBack)
            {
                if (Request["idCte"] != null)
                    dtvConhecimentoTransporte.ChangeMode(DetailsViewMode.ReadOnly);
                else
                {
                    (dtvConhecimentoTransporte.FindControl("drpTipoDocumentoCte") as DropDownList).SelectedValue = Request["tipo"];

                    if (Request["tipo"] == "2")
                    {
                        (dtvConhecimentoTransporte.FindControl("drpTipoCte") as DropDownList).SelectedValue =
                        FiscalConfig.TelaCadastroCTe.TipoCtePadraoCteSaida;
                        (dtvConhecimentoTransporte.FindControl("drpTipoServico") as DropDownList).SelectedValue =
                            FiscalConfig.TelaCadastroCTe.TipoServicoPadraoCteSaida;
                        (dtvConhecimentoTransporte.FindControl("txtSerie") as TextBox).Text =
                            FiscalConfig.TelaCadastroCTe.SeriePadraoCteSaida;

                        #region Cidade

                        (dtvConhecimentoTransporte.FindControl("hdfCidadeCte") as HiddenField).Value =
                            FiscalConfig.TelaCadastroCTe.CidadeEnvioPadraoCteSaida;
                        (dtvConhecimentoTransporte.FindControl("hdfCidadeInicio") as HiddenField).Value =
                            FiscalConfig.TelaCadastroCTe.CidadeInicioPadraoCteSaida;

                        if (FiscalConfig.TelaCadastroCTe.CidadeEnvioPadraoCteSaida.StrParaIntNullable() > 0)
                            (dtvConhecimentoTransporte.FindControl("txtCidadeCte") as TextBox).Text =
                                CidadeDAO.Instance.GetNome(FiscalConfig.TelaCadastroCTe.CidadeEnvioPadraoCteSaida.StrParaUint());

                        if (FiscalConfig.TelaCadastroCTe.CidadeInicioPadraoCteSaida.StrParaIntNullable() > 0)
                            (dtvConhecimentoTransporte.FindControl("txtCidadeInicio") as TextBox).Text =
                                CidadeDAO.Instance.GetNome(FiscalConfig.TelaCadastroCTe.CidadeInicioPadraoCteSaida.StrParaUint());

                        #endregion
                    }
                }
            }
    
            grdNfCte.Visible =
                (dtvConhecimentoTransporte.CurrentMode == DetailsViewMode.ReadOnly ||
                dtvConhecimentoTransporte.CurrentMode == DetailsViewMode.Edit)
                && FiscalConfig.ConhecimentoTransporte.ExibirGridNotaFiscal ||
                GetTipoDocumentoCte() == WebGlass.Business.ConhecimentoTransporte.Entidade.Cte.TipoDocumentoCteEnum.EntradaTerceiros;

            grdChavesAcesso.Visible = GetTipoDocumentoCte() == WebGlass.Business.ConhecimentoTransporte.Entidade.Cte.TipoDocumentoCteEnum.Saida &&
                ((dtvConhecimentoTransporte.CurrentMode == DetailsViewMode.ReadOnly) || dtvConhecimentoTransporte.CurrentMode == DetailsViewMode.Edit);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdChavesAcesso.Register(true, true);
            odsChaveAcessoCte.Register();
        }
    
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Listas/LstConhecimentoTransporte.aspx");
        }
    
        protected void lnkInsCte_Click(object sender, EventArgs e)
        {
            WebGlass.Business.ConhecimentoTransporte.Entidade.NfeCte nfCte = new WebGlass.Business.ConhecimentoTransporte.Entidade.NfeCte
                (new Glass.Data.Model.Cte.NotaFiscalCte());
    
            nfCte.IdCte = Glass.Conversoes.StrParaUint(Request["IdCte"]);
    
            if (string.IsNullOrEmpty(((HiddenField)grdNfCte.FooterRow.FindControl("hdfIdNf")).Value))
                return;
    
            nfCte.IdNf = Convert.ToUInt32(((HiddenField)grdNfCte.FooterRow.FindControl("hdfIdNf")).Value);
    
            try
            {
                WebGlass.Business.ConhecimentoTransporte.Fluxo.CadastrarNotaFiscalCte.Instance.Insert(nfCte);
    
                dtvConhecimentoTransporte.DataBind();
    
                grdNfCte.DataBind();
                grdNfCte.PageIndex = grdNfCte.PageCount - 1;
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao associar NF.", ex, Page);
                return;
            }
        }

        protected void lnkAddChaveAcesso_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                var chaveAcessoCte = new Glass.Fiscal.Negocios.Entidades.Cte.ChaveAcessoCte();

                chaveAcessoCte.IdCte = Glass.Conversoes.StrParaInt(Request["IdCte"]);
                chaveAcessoCte.ChaveAcesso = ((TextBox)grdChavesAcesso.FooterRow.FindControl("txtNumChaveAcesso")).Text;
                chaveAcessoCte.PIN = ((TextBox)grdChavesAcesso.FooterRow.FindControl("txtNumPin")).Text;

                var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Glass.Fiscal.Negocios.ICTeFluxo>();

                var resultado = fluxo.SalvarChaveAcesso(chaveAcessoCte);

                if (resultado)
                {

                    dtvConhecimentoTransporte.DataBind();

                    grdChavesAcesso.DataBind();
                    grdChavesAcesso.PageIndex = grdChavesAcesso.PageCount - 1;
                }
                else
                    Glass.MensagemAlerta.ErrorMsg("Falha ao inserir chave de acesso.", resultado);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir chave de acesso.", ex, Page);
            }
        }
    
        #region Load
    
        protected void txtModelo_Load(object sender, EventArgs e)
        {
            ((TextBox)sender).Text = Glass.Data.CTeUtils.ConfigCTe.Modelo;
        }
    
        protected void txtSerie_Load(object sender, EventArgs e)
        {
            ((TextBox)sender).Enabled = IsEntradaTerceiros() || 
                dtvConhecimentoTransporte.CurrentMode == DetailsViewMode.Insert;
        }
    
        protected void txtTipoEmissao_Load(object sender, EventArgs e)
        {
            if (((TextBox)sender).Text == "")
                ((TextBox)sender).Text = "Normal";
    
            ((HiddenField)dtvConhecimentoTransporte.FindControl("hdfTipoEmissao")).Value = ((TextBox)sender).Text == "Normal" ? "1"
                : ((TextBox)sender).Text == "Contingencia Fsda" ? "5" : ((TextBox)sender).Text == "Autorizacao SvcRs" ? "7"
                : "8";
        }
    
        #endregion
    
        #region Grid
    
        protected void grdNf_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
            dtvConhecimentoTransporte.DataBind();
        }
    
        protected void grdNf_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdNfCte.ShowFooter = e.CommandName != "Edit";
        }
    
        protected void grdNf_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType.ToString() != "Footer" && e.Row.RowType.ToString() != "Header" && e.Row.RowType.ToString() != "Pager")
            {
                if (e.Row.DataItem != null && ((WebGlass.Business.ConhecimentoTransporte.Entidade.NfeCte)e.Row.DataItem).IdNf == 0)
                    e.Row.Visible = false;
            }
        }

        protected void grdChavesAcesso_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
            dtvConhecimentoTransporte.DataBind();
        }

        protected void grdChavesAcesso_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdChavesAcesso.ShowFooter = e.CommandName != "Edit";
        }

        #endregion
    
        #region Emitir/Pré-visualizar CTe
    
        protected void btnEmitir_Click(object sender, EventArgs e)
        {
            try
            {    
                // Se estiver em contingência mas a forma de emissão deste CTe for normal, gera um novo cte em contingência
                if (FiscalConfig.ConhecimentoTransporte.ContingenciaCTe == DataSources.TipoContingenciaCTe.SVC && ConhecimentoTransporteDAO.Instance.ObtemFormaEmissao(Glass.Conversoes.StrParaUint(Request["idCte"])) == (int)ConhecimentoTransporte.TipoEmissaoEnum.Normal)
                {
                    var cte = WebGlass.Business.ConhecimentoTransporte.Fluxo.BuscarCte.Instance.GetCte(Glass.Conversoes.StrParaUint(Request["idCte"]));
    
                    if (cte.TipoCte != (int)Glass.Data.Model.Cte.ConhecimentoTransporte.TipoCteEnum.Normal)
                        throw new Exception("Para emitir CTe em modo de contingência SVC o mesmo de ser do tipo Normal.");
    
                    uint idCte = WebGlass.Business.ConhecimentoTransporte.Fluxo.CadastrarCte.Instance.CriaCTeContingencia(cte);
    
                    Response.Redirect("CadConhecimentoTransporte.aspx?idCte=" + idCte + "&tipo=" + Request["tipo"] + "&isEmitirContingencia=true", false);
    
                    return;
                }
    
                WebGlass.Business.ConhecimentoTransporte.Fluxo.CadastrarCte.Instance.EmitirCTe(Glass.Conversoes.StrParaUint(Request["idCte"]), false);
                Response.Redirect(ResolveUrl("~/Listas/LstConhecimentoTransporte.aspx"), false);
            }
            catch (Exception ex)
            {            
                // Se for erro na validação do arquivo XML, abre popup para mostrar erros
                if (ex.Message.Contains("XML inconsistente."))
                {
                    string msg = Glass.MensagemAlerta.FormatErrorMsg("", ex).Replace("XML inconsistente.", "").Replace("Linha:", "%bl%%bl%Linha:");
                    ClientScript.RegisterClientScriptBlock(typeof(string), "msg", "openWindow(410, 540, '../Utils/ShowMsg.aspx?title=Falha na validação do arquivo do CT-e&msg=" + msg + "')", true);
                }
                else
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha na operação com o CTe.", ex, Page);             
                }
            }
        }
    
        protected void btnFinalizar_Click(object sender, EventArgs e)
        {
            try
            {
                // Recupera a nota fiscal
                uint idCte = Glass.Conversoes.StrParaUint(Request["idCte"]);
                
                // Finaliza a nota fiscal
                WebGlass.Business.ConhecimentoTransporte.Fluxo.FinalizarCte.Instance.Finalizar(idCte);
    
                // Redireciona para a lista
                Response.Redirect("~/Listas/LstConhecimentoTransporte.aspx", false);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao finalizar conhecimento de transporte.", ex, Page);
            }
        }
    
        protected void btnEmitirFinalizar_Load(object sender, EventArgs e)
        {
            var cte = ConhecimentoTransporteDAO.Instance.GetElement(Glass.Conversoes.StrParaUint(Request["idCte"]));
    
            ((Button)sender).Visible = ((Button)sender).ID == "btnEmitir" ? cte.TipoDocumentoCte == (int)WebGlass.Business.ConhecimentoTransporte.Entidade.Cte.TipoDocumentoCteEnum.Saida :
                cte.TipoDocumentoCte == (int)WebGlass.Business.ConhecimentoTransporte.Entidade.Cte.TipoDocumentoCteEnum.EntradaTerceiros && Request["manual"] != "1";
    
            if (((Button)sender).ID == "btnEmitir" && ((Button)sender).Visible)
            {
                // Se for para gerar contingência, altera a descrição do botão
                bool gerarCteContingencia = FiscalConfig.ConhecimentoTransporte.ContingenciaCTe == DataSources.TipoContingenciaCTe.SVC;
    
                if (Request["isEmitirContingencia"] == "true" || cte.TipoEmissao != (int)ConhecimentoTransporte.TipoEmissaoEnum.Normal)
                {
                    ((Button)sender).Text = "Emitir CTe em Contingência";
                    ((Button)sender).ForeColor = Color.Blue;
                    ((Button)sender).Width = new Unit("170px");
                }
                else
                {
                    ((Button)sender).Text = (!gerarCteContingencia ? "Emitir CTe" : "Gerar CTe de Contingência");
                    ((Button)sender).ForeColor = !gerarCteContingencia ? Color.Red : Color.Blue;
                    ((Button)sender).Width = !gerarCteContingencia ? new Unit("125px") : new Unit("170px");
                }
            }
        }
    
        protected void btnPreVisualizar_Load(object sender, EventArgs e)
        {
            var cte = ConhecimentoTransporteDAO.Instance.GetElement(Glass.Conversoes.StrParaUint(Request["idCte"]));
            ((Button)sender).Visible = cte.TipoDocumentoCte == (int)WebGlass.Business.ConhecimentoTransporte.Entidade.Cte.TipoDocumentoCteEnum.Saida;
        }
    
        protected void btnPreVisualizar_Click(object sender, EventArgs e)
        {
            try
            {
                Glass.Data.DAL.CTe.ConhecimentoTransporteDAO.Instance.EmitirCTe(Glass.Conversoes.StrParaUint(Request["idCte"]), true);
    
                var cte = ConhecimentoTransporteRodoviarioDAO.Instance.GetElement(Glass.Conversoes.StrParaUint(Request["idCte"]));
    
                if (cte != null && cte.Lotacao)
                    ClientScript.RegisterStartupScript(typeof(string), "msg", "openWindow(600, 800, \"../Relatorios/CTe/RelBase.aspx?rel=DacteLotacao&previsualizar=true&idCte=\" + " + Request["idCte"] + ");", true);
                else
                    ClientScript.RegisterStartupScript(typeof(string), "msg", "openWindow(600, 800, \"../Relatorios/CTe/RelBase.aspx?rel=DacteFracionada&previsualizar=true&idCte=\" + " + Request["idCte"] + ");", true);
            }
            catch (Exception ex)
            {
                // Se for erro na validação do arquivo XML, abre popup para mostrar erros
                if (ex.Message.Contains("XML inconsistente."))
                {
                    string msg = Glass.MensagemAlerta.FormatErrorMsg("", ex).Replace("XML inconsistente.", "").Replace("Linha:", "%bl%%bl%Linha:");
                    ClientScript.RegisterClientScriptBlock(typeof(string), "msg", "openWindow(410, 540, '../Utils/ShowMsg.aspx?title=Falha na validação do arquivo do CT-e&msg=" + msg + "')", true);
                }
                else
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao pré-visualizar CTe.", ex, Page);
                }
            }
        }
    
        #endregion
    
        #region DetailsView
    
        protected void odsConhecimentoTransporte_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception == null)
            {
                Glass.MensagemAlerta.ShowMsg("CTe atualizado!", Page);
                Response.Redirect(Request.Url.ToString());
            }
            else
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar CTe.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
    
        protected void odsConhecimentoTransporte_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception == null)
                Response.Redirect("CadConhecimentoTransporte.aspx?idCte=" + e.ReturnValue + "&tipo=" + Request["tipo"]);
            else
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao cadastrar CTe.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
    
        protected void dtvConhecimentoTransporte_ItemCommand(object sender, DetailsViewCommandEventArgs e)
        {
            grdNfCte.Visible = true;
            grdChavesAcesso.Visible = true;
    
            if (e.CommandName == "Edit")
            {
                dtvConhecimentoTransporte.ChangeMode(DetailsViewMode.Edit);
                grdNfCte.Visible = false;
                grdChavesAcesso.Visible = false;
            }
            if (e.CommandName == "Update" || e.CommandName == "Insert")
                Glass.Validacoes.DisableRequiredFieldValidator(Page);
        }
    
        protected void dtvConhecimentoTransporte_ItemInserting(object sender, DetailsViewInsertEventArgs e)
        {
            string[] chavesDecimal = new[] { "ValorTotal", "ValorReceber" };
            foreach (var c in chavesDecimal)
                e.Values[c] = Glass.Conversoes.StrParaDecimal(e.Values[c].ToString());
        }
    
        protected void dtvConhecimentoTransporte_ItemUpdating(object sender, DetailsViewUpdateEventArgs e)
        {
            string[] chavesDecimal = new[] { "ValorTotal", "ValorReceber" };
            foreach (var c in chavesDecimal)
                e.NewValues[c] = Glass.Conversoes.StrParaDecimal(e.NewValues[c].ToString());
        }
    
        #endregion
    
        #region Métodos AJAX
    
        [Ajax.AjaxMethod()]
        public string BuscarNF(string numNFe, string isEntradaTerceiros)
        {
            var nf = NotaFiscalDAO.Instance.GetByNumeroNFe(Glass.Conversoes.StrParaUint(numNFe), 0).ToList();
    
            if (nf == null || nf.Count == 0)
                throw new Exception("Erro: NF-e não encontrada.");
            if (nf.Count > 1)
                throw new Exception("Erro: Foi encontrado mais de uma NF-e.");
    
            if (bool.Parse(isEntradaTerceiros))
            {
                if (nf.Where(f => !("," + ((int)NotaFiscal.TipoDoc.EntradaTerceiros).ToString() + "," + ((int)NotaFiscal.TipoDoc.NotaCliente).ToString() + ",")
                    .Contains("," + f.TipoDocumento + ",")).Count() > 0)
                    throw new Exception("Erro: A NF-e informada não é uma nota de Entrada de Terceiros ou de Cliente.");
    
                if (nf.Where(f => f.Situacao != (int)NotaFiscal.SituacaoEnum.FinalizadaTerceiros).Count() > 0)
                    throw new Exception("Erro: A NF-e informada não esta finalizada.");
    
                return nf[0].IdNf + ";" + nf[0].NumeroNFe;
            }
    
            if (nf.Where(f => (int)NotaFiscal.TipoDoc.Saída != f.TipoDocumento).Count() > 0)
                throw new Exception("Erro: A NF-e informada não é uma nota de saída.");
    
            if (nf.Where(f => f.Situacao != (int)NotaFiscal.SituacaoEnum.Autorizada).Count() > 0)
                throw new Exception("Erro: A NF-e informada não esta autorizada.");
    
            return nf[0].IdNf + ";" + nf[0].NumeroNFe;
        }
    
        #endregion
    
        protected void ExibirDadosTerceiros(object sender, EventArgs e)
        {
            (sender as Control).Visible = IsEntradaTerceiros();
            
        }
    
        protected void NaoExibirDadosTerceiros(object sender, EventArgs e)
        {
            if (!IsEntradaTerceiros())
                return;
    
            if (sender is HtmlContainerControl)
                ((HtmlContainerControl)sender).Style.Add("display", "none");
        }
    
        private int? tipoDocumentoCte;
    
        public WebGlass.Business.ConhecimentoTransporte.Entidade.Cte.TipoDocumentoCteEnum GetTipoDocumentoCte()
        {
            if (tipoDocumentoCte == null)
            {
                tipoDocumentoCte = Glass.Conversoes.StrParaInt(Request["tipo"]);
                if (tipoDocumentoCte == 0)
                    tipoDocumentoCte = WebGlass.Business.ConhecimentoTransporte.Fluxo.BuscarCte.
                        Instance.GetCte(Glass.Conversoes.StrParaUint(Request["idCte"])).TipoDocumentoCte;
            }
    
            return (WebGlass.Business.ConhecimentoTransporte.Entidade.Cte.TipoDocumentoCteEnum)tipoDocumentoCte.GetValueOrDefault();
        }
    
        public bool IsEntradaTerceiros()
        {
            return GetTipoDocumentoCte() == WebGlass.Business.ConhecimentoTransporte.Entidade.Cte.TipoDocumentoCteEnum.EntradaTerceiros;
        }

        protected void divctrlComplCte_Load(object sender, EventArgs e)
        {
            if (!IsEntradaTerceiros() && !FiscalConfig.ConhecimentoTransporte.EsconderComplEDadosTransRodCteSaida)
                return;

            if (sender is HtmlContainerControl)
                ((HtmlContainerControl)sender).Style.Add("display", "none");
        }

        protected void div15_Load(object sender, EventArgs e)
        {
            if (!IsEntradaTerceiros() && Configuracoes.FiscalConfig.ConhecimentoTransporte.EsconderComplEDadosTransRodCteSaida)
            {
                if (sender is HtmlContainerControl)
                    ((HtmlContainerControl)sender).Style.Add("display", "none");
            }
        }
    }
}
