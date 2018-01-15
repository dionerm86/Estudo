using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Data.NFeUtils;
using System.Drawing;
using Glass.Configuracoes;

namespace Glass.UI.Web.Listas
{
    public partial class LstNotaFiscal : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["erroNf"] != null)
                ClientScript.RegisterClientScriptBlock(GetType(), "erroNf", "alert('O arquivo dessa nota fiscal não foi encontrado.')", true);
    
            bool isAutFin = IsAutorizadaFinalizada();
            grdNf.Visible = !isAutFin;
            grdNfAutFin.Visible = isAutFin;
            relatorioProd.Visible = isAutFin;
            relatorioProdExcel.Visible = isAutFin;
    
            if (!IsPostBack)
            {
                int numEmitir = NotaFiscalDAO.Instance.GetCountEmitirFs();
                divNumNotasFs.Visible = numEmitir > 0;
                lblNumNotasFs.Text = numEmitir.ToString();
    
                divContingencia.Visible = Config.PossuiPermissao(Config.FuncaoMenuFiscal.AtivarContingenciaNFe);
    
                if (EstoqueConfig.ControlarEstoqueVidrosClientes)
                    drpTipoNota.Items.Insert(4, new ListItem("Nota Fiscal de Cliente", "4"));

                ctrlDataIni.Data = DateTime.Parse(FuncoesData.ObtemDataPrimeiroDiaUltimoMes());
                ctrlDataFim.Data = DateTime.Now;
            }

            if (Request["autorizada"] != null)
            {
                if (Request["autorizada"] == "true")
                    MensagemAlerta.ShowMsg("Nota autorizada.", Page);
                else
                    MensagemAlerta.ShowMsg(Request["autorizada"], Page);
            }
    
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(LstNotaFiscal));
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            if (IsAutorizadaFinalizada())
            {
                grdNf.Visible = false;
                grdNfAutFin.Visible = true;
                grdNfAutFin.DataBind();
                grdNfAutFin.PageIndex = 0;
            }
            else
            {
                grdNfAutFin.Visible = false;
                grdNf.Visible = true;
                grdNf.DataBind();
                grdNf.PageIndex = 0;
            }
        }
    
        protected void drpLoja_SelectedIndexChanged(object sender, EventArgs e)
        {
            grdNf.DataBind();
            grdNf.PageIndex = 0;
    
            Loja l = LojaDAO.Instance.GetElement(Glass.Conversoes.StrParaUint(((DropDownList)sender).SelectedValue));
    
            if (l.DataVencimentoCertificado != null)
            {
                if (l.DataVencimentoCertificado.Value.Ticks > DateTime.Now.Ticks)
                {
                    if (l.DataVencimentoCertificado.Value.Subtract(DateTime.Now).Days + 1 <= 10)
                        lblDataVencimentoCertificado.ForeColor = Color.Red;
                    else
                        lblDataVencimentoCertificado.ForeColor = Color.Blue;
    
                    lblDataVencimentoCertificado.Text = string.Format("Falta(m) {0} dia(s) para a data de vencimento do Certificado : {1}", l.DataVencimentoCertificado.Value.Subtract(DateTime.Now).Days + 1, l.DataVencimentoCertificado.Value.ToString("dd/MM/yyyy"));
                }
                else
                    lblDataVencimentoCertificado.Text = string.Format("ATENÇÃO: Certificado vencido em {0}", l.DataVencimentoCertificado.Value.ToShortDateString());
            }
        }
    
        protected void grdNf_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ConsultaSitLote")
            {
                try
                {
                    // Consulta a situação do lote e da NFe, caso o lote tenha sido processado
                    string msg = ConsultaSituacao.ConsultaLote(Glass.Conversoes.StrParaUint(e.CommandArgument.ToString()));
                    Glass.MensagemAlerta.ShowMsg(msg, Page);
    
                    grdNf.DataBind();
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao consultar situação.", ex, Page);
                }
            }
            else if (e.CommandName == "ConsultaSitNFe")
            {
                try
                {
                    // Consulta a situação do lote e da NFe, caso o lote tenha sido processado
                    string msg = ConsultaSituacao.ConsultaSitNFe(Glass.Conversoes.StrParaUint(e.CommandArgument.ToString()));
                    Glass.MensagemAlerta.ShowMsg(msg, Page);
    
                    grdNf.DataBind();
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao consultar situação.", ex, Page);
                }
            }
            else if (e.CommandName == "Reabrir")
            {
                try
                {
                    uint idNf = Glass.Conversoes.StrParaUint(e.CommandArgument.ToString());
                    NotaFiscalDAO.Instance.ReabrirNotaEntradaTerceiros(idNf);
    
                    grdNf.DataBind();
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao reabrir nota fiscal.", ex, Page);
                }
            }
            else if (e.CommandName == "Complementar")
            {
                try
                {
                    uint idNfRef = Glass.Conversoes.StrParaUint(e.CommandArgument.ToString());
                    uint idNf = NotaFiscalDAO.Instance.GeraNFeComplementar(idNfRef);
    
                    Response.Redirect("../Cadastros/CadNotaFiscal.aspx?idNf=" + idNf + "&tipo=" + NotaFiscalDAO.Instance.GetTipoDocumento(idNf));
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao gerar nota fiscal complementar.", ex, Page);
                }
            }
            else if (e.CommandName == "Emitir")
            {
                try
                {
                    uint idNf = Glass.Conversoes.StrParaUint(e.CommandArgument.ToString());
                    NotaFiscalDAO.Instance.EmitirNfFS(idNf);
    
                    grdNf.DataBind();
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao emitir nota fiscal com formulário de segurança.", ex, Page);
                }
            }
            else if (e.CommandName == "ReenviarEmailXml")
            {
                try
                {
                    var idNf = Glass.Conversoes.StrParaUint(e.CommandArgument.ToString());
                    NotaFiscalDAO.Instance.EnviarEmailXml(idNf);

                    Glass.MensagemAlerta.ShowMsg("E-mail adicionado a fila de envios.", Page);
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao reenviar e-mail do XML / DANFE", ex, Page);
                }
            }
            else if (e.CommandName == "ReenviarEmailXmlCancelamento")
            {
                try
                {
                    var idNf = e.CommandArgument.ToString().StrParaUint();
                    NotaFiscalDAO.Instance.EnviarEmailXml(idNf, true);

                    MensagemAlerta.ShowMsg("E-mail adicionado a fila de envios.", Page);
                }
                catch (Exception ex)
                {
                    MensagemAlerta.ErrorMsg("Falha ao reenviar e-mail do XML / DANFE (Cancelamento)", ex, Page);
                }
            }
            else if (e.CommandName == "SepararValores")
            {
                try
                {
                    var idNf = Glass.Conversoes.StrParaUint(e.CommandArgument.ToString());
                    var tipoDocumento = NotaFiscalDAO.Instance.GetTipoDocumento(idNf);
                    var separouValores = false;

                    if (tipoDocumento == (int)Glass.Data.Model.NotaFiscal.TipoDoc.Saída)
                    {
                        if (NotaFiscalDAO.Instance.ObtemIdCliente(idNf).GetValueOrDefault(0) == 0)
                            throw new Exception("Não é possível fazer a vinculação de valores sem cliente.");

                        if (!Glass.Data.Helper.SeparacaoValoresFiscaisEReaisContasReceber.Instance.SepararComTransacao(idNf))
                            throw new Exception("Não foram encontradas contas a receber para realizar a vinculação.");
                    }
                    else
                        separouValores = true;

                    if (tipoDocumento == (int)Glass.Data.Model.NotaFiscal.TipoDoc.EntradaTerceiros &&
                        !Glass.Data.Helper.SeparacaoValoresFiscaisEReaisContasPagar.Instance.SepararComTransacao(idNf))
                        throw new Exception("Não foram encontradas contas a pagar para realizar a vinculação.");
                    else
                        separouValores = true;

                    if (idNf > 0)
                    {
                        if (!separouValores)
                        {
                            LogNfDAO.Instance.NewLog(idNf, "Separação Valores", 0, "Falha na vinculação de valores. " +
                                "Não foram encontradas contas para realizar a vinculação.");
                            Glass.MensagemAlerta.ShowMsg("Falha na vinculação de valores. Não foram encontradas contas para realizar a vinculação.", Page);
                        }
                        else
                            Glass.MensagemAlerta.ShowMsg("Vinculação feita com sucesso!", Page);
                    }
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao vincular os valores fiscais/reais", ex, Page);
                }
            }
            else if (e.CommandName == "CancelarSepararValores")
            {
                try
                {
                    var idNf = Glass.Conversoes.StrParaUint(e.CommandArgument.ToString());
                    var tipoDocumento = NotaFiscalDAO.Instance.GetTipoDocumento(idNf);

                    if (tipoDocumento == (int)Glass.Data.Model.NotaFiscal.TipoDoc.Saída)
                    {
                        Glass.Data.Helper.SeparacaoValoresFiscaisEReaisContasReceber.Instance.CancelarComTransacao(idNf);
                        NotaFiscalDAO.Instance.DesvinculaReferenciaPedidosAntecipados((int)idNf);
                    }

                    if (tipoDocumento == (int)Glass.Data.Model.NotaFiscal.TipoDoc.EntradaTerceiros)
                        Glass.Data.Helper.SeparacaoValoresFiscaisEReaisContasPagar.Instance.CancelarComTransacao(idNf);

                    Glass.MensagemAlerta.ShowMsg("Cancelamento feito com sucesso!", Page);
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao cancelar a vinculação dos valores fiscais/reais", ex, Page);
                }
            }
            else if (e.CommandName == "EmitirNFCe")
            {
                try
                {
                    uint idNf = e.CommandArgument.ToString().StrParaUint();
                    var retornoEmissao = NotaFiscalDAO.Instance.EmitirNfcOffline(idNf);

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
                        var msg = ConsultaSituacao.ConsultaSitNFe(idNf);

                        if (msg.ToLower().Contains("rejeicao"))
                            Response.Redirect("../Listas/LstNotaFiscal.aspx?autorizada=" + msg);
                        else
                            Response.Redirect("../Listas/LstNotaFiscal.aspx?autorizada=true", false);
                    }
                }
                catch (Exception ex)
                {
                    MensagemAlerta.ErrorMsg("Falha ao emitir NFC-e.", ex, Page);
                }
            }
        }
    
        protected bool IsAutorizadaFinalizada()
        {
            return cboSituacao.SelectedValue.Contains(((int)NotaFiscal.SituacaoEnum.Autorizada).ToString()) ||
                cboSituacao.SelectedValue.Contains(((int)NotaFiscal.SituacaoEnum.FinalizadaTerceiros).ToString());
        }

        protected void grdNf_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            HiddenField hdfCorLinha = e.Row.FindControl("hdfCorLinha") as HiddenField;
            if (hdfCorLinha == null)
                return;
    
            Color corLinha = Color.FromName(hdfCorLinha.Value);
            foreach (TableCell c in e.Row.Cells)
                c.ForeColor = corLinha;
        }
    
        protected void lnkEditar_Load(object sender, EventArgs e)
        {
            ((HyperLink)sender).Attributes.Add("OnClick", "if(this.href.indexOf('&manual=1') > -1) return confirm('Deseja realizar uma alteração manual de valores da nota fiscal?');");
    
            if (((HyperLink)sender).Text.IndexOf("script") == -1)
            {
                string nomeControle = "document.getElementById('" + ((HyperLink)sender).ClientID + "')";
                ((HyperLink)sender).Text += "<script type='text/javascript'>if(" + nomeControle + ".href.indexOf('&manual=1') > -1) { " + nomeControle + ".innerHTML = " +
                    nomeControle + ".innerHTML.replace('EditarGrid.gif', 'editarValor.gif'); " + nomeControle + ".title = 'Alteração manual de valores' }</script>";
            }
        }
    
        protected void lnkAlterarContingenciaNFe_Click(object sender, EventArgs e)
        {
            ConfigDAO.Instance.SetValue(Config.ConfigEnum.ContingenciaNFe, UserInfo.GetUserInfo.IdLoja, (uint)DataSources.TipoContingenciaNFe.SCAN);
        }
    
        protected void lnkAlterarContingenciaFsDa_Click(object sender, EventArgs e)
        {
            ConfigDAO.Instance.SetValue(Config.ConfigEnum.ContingenciaNFe, UserInfo.GetUserInfo.IdLoja, (uint)DataSources.TipoContingenciaNFe.FSDA);
        }
    
        protected string GetTipoContingenciaNfe()
        {
            string descr = FiscalConfig.NotaFiscalConfig.ContingenciaNFe == DataSources.TipoContingenciaNFe.NaoUtilizar ? null :
                DataSources.Instance.GetDescrTipoContingenciaNFe((int)FiscalConfig.NotaFiscalConfig.ContingenciaNFe);
    
            lblContingenciaNFe.Text = "Nota Fiscal em Contingência: " + descr;
            return descr;
        }
    
        protected void lnkDesabilitarContingenciaNFe_Click(object sender, EventArgs e)
        {
            ConfigDAO.Instance.SetValue(Config.ConfigEnum.ContingenciaNFe, UserInfo.GetUserInfo.IdLoja, (uint)DataSources.TipoContingenciaNFe.NaoUtilizar);
        }
    
        protected void grdNfAutFin_SelectedIndexChanged(object sender, EventArgs e)
        {
    
        }
    
        protected string LinkEditarNf(uint idNf, string tipo, int situacao)
        {
            string link = "../Cadastros/CadNotaFiscal.aspx?idNf=" + idNf + "&tipo=" + tipo;
    
            if (situacao == (int)Glass.Data.Model.NotaFiscal.SituacaoEnum.FinalizadaTerceiros ||
                (situacao == (int)Glass.Data.Model.NotaFiscal.SituacaoEnum.Autorizada &&
                NotaFiscalDAO.Instance.ExisteCartaCorrecaoRegistrada(null, idNf)))
                link += "&manual=1";
    
            return link;
        }


        [Ajax.AjaxMethod()]
        public string EmitirNfcOffline(string idNf)
        {
            return NotaFiscalDAO.Instance.EmitirNf(idNf.StrParaUint(), false, true, false);
        }
    }
}
