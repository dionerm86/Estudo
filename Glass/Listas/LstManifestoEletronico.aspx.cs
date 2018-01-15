using Glass.Data.DAL;
using Glass.Data.MDFeUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Listas
{
    public partial class LstManifestoEletronico : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                drpTipoContratante_SelectedIndexChanged(null, null);
            }

            // Exibe a mensagem de retorno ao usuário
            var retornoEmissao = Request["Retorno"];
            if(retornoEmissao != null)
            {
                MensagemAlerta.ShowMsg(retornoEmissao, Page);
            }

            Ajax.Utility.RegisterTypeForAjax(typeof(LstManifestoEletronico));
        }

        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdManifestoEletronico.PageIndex = 0;
        }

        protected void drpTipoContratante_SelectedIndexChanged(object sender, EventArgs e)
        {
            drpContratante.Items.Clear();
            drpContratante.Items.Add("");
            switch (drpTipoContratante.SelectedIndex)
            {
                case 0: //Loja
                    drpContratante.DataSourceID = "odsLoja";
                    drpContratante.DataValueField = "IdLoja";
                    drpContratante.DataTextField = "RazaoSocial";
                    break;
                case 1: //Fornecedor
                    drpContratante.DataSourceID = "odsFornecedor";
                    drpContratante.DataValueField = "IdFornec";
                    drpContratante.DataTextField = "RazaoSocial";
                    break;
                case 2: //Cliente
                    drpContratante.DataSourceID = "odsCliente";
                    drpContratante.DataValueField = "IdCli";
                    drpContratante.DataTextField = "Nome";
                    break;
                case 3: //Transportador
                    drpContratante.DataSourceID = "odsTransportador";
                    drpContratante.DataValueField = "IdTransportador";
                    drpContratante.DataTextField = "Nome";
                    break;
            }
        }

        protected void grdManifestoEletronico_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ConsultaSitLoteMDFe")
            {
                try
                {
                    // Consulta a situação do lote e da MDFe, caso o lote tenha sido processado
                    string msg = ConsultaSituacao.ConsultaSitLoteMDFe(Glass.Conversoes.StrParaInt(e.CommandArgument.ToString()));
                    MensagemAlerta.ShowMsg(msg, Page);

                    grdManifestoEletronico.DataBind();
                }
                catch (Exception ex)
                {
                    MensagemAlerta.ErrorMsg("Falha ao consultar situação.", ex, Page);
                }
            }
            else if (e.CommandName == "ConsultaSitMDFe")
            {
                try
                {
                    // Consulta a situação do lote e do MDFe, caso o lote tenha sido processado
                    string msg = ConsultaSituacao.ConsultaSitMDFe(Glass.Conversoes.StrParaInt(e.CommandArgument.ToString()));
                    MensagemAlerta.ShowMsg(msg, Page);

                    grdManifestoEletronico.DataBind();
                }
                catch (Exception ex)
                {
                    MensagemAlerta.ErrorMsg("Falha ao consultar situação.", ex, Page);
                }
            }
            else if (e.CommandName == "EmitirMDFeOffline")
            {
                try
                {
                    var idManifestoEletronico = Glass.Conversoes.StrParaInt(e.CommandArgument.ToString());
                    var retorno = ManifestoEletronicoDAO.Instance.EmitirMDFeOffline(idManifestoEletronico);

                    // Consulta a situação se o arquivo tiver sido enviado corretamente.
                    if (retorno == "Arquivo recebido com sucesso")
                    {
                        retorno = ConsultaSituacao.ConsultaSitLoteMDFe(idManifestoEletronico);
                    }

                    Response.Redirect("~/Listas/LstManifestoEletronico.aspx?Retorno=" + retorno);
                }
                catch(Exception ex)
                {
                    // Se for erro na validação do arquivo XML, abre popup para mostrar erros
                    if (ex.Message.Contains("XML inconsistente."))
                    {
                        string msg = MensagemAlerta.FormatErrorMsg("", ex).Replace("XML inconsistente.", "").Replace("Linha:", "%bl%%bl%Linha:");
                        ClientScript.RegisterClientScriptBlock(typeof(string), "msg", "openWindow(410, 540, '../Utils/ShowMsg.aspx?title=Falha na validação do arquivo do MDF-e&msg=" + msg + "')", true);
                    }
                    else
                    {
                        MensagemAlerta.ErrorMsg("Falha ao emitir o MDFe.", ex, Page);
                    }
                }
            }
        }

        [Ajax.AjaxMethod()]
        public string ImprimirMDFeContingencia(string idManifestoEletronico)
        {
            return ManifestoEletronicoDAO.Instance.ImprimirMDFeContingencia(idManifestoEletronico.StrParaInt());
        }

        protected void grdManifestoEletronico_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;

            var situacao = ((Glass.Data.Model.ManifestoEletronico)e.Row.DataItem).Situacao;

            var cor = situacao == Data.Model.SituacaoEnum.FalhaCancelar || situacao == Data.Model.SituacaoEnum.FalhaEmitir ||
                    situacao == Data.Model.SituacaoEnum.FalhaEncerrar ? System.Drawing.Color.Red :
                situacao == Data.Model.SituacaoEnum.Autorizado ||
                    situacao == Data.Model.SituacaoEnum.Encerrado ? System.Drawing.Color.Blue :
                System.Drawing.Color.Black;

            if (cor != System.Drawing.Color.Black)
                ((Label)e.Row.FindControl("lblSituacao")).ForeColor = cor;
        }

        protected void lnkConsultaNaoEncerrados_Click(object sender, EventArgs e)
        {
            try
            {
                var msg = string.Empty;
                var idLoja = Conversoes.StrParaUint(drpLojaConsultaNaoEncerrados.SelectedValue);
                if (idLoja > 0)
                    msg = Data.MDFeUtils.EnviaXML.EnviaConsultaNaoEncerrados(idLoja);
                else
                    msg = "Selecione uma loja para consultar.";

                Glass.MensagemAlerta.ShowMsg(msg, Page);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao consultar MDFe não encerrados.", ex, Page);
            }
        }
    }
}