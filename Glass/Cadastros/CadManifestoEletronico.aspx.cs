using Glass.Data.DAL;
using Glass.Data.MDFeUtils;
using Glass.Data.Model;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadManifestoEletronico : System.Web.UI.Page
    {
        private static int? idMDFe = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadManifestoEletronico));

            if (!IsPostBack)
            {
                idMDFe = Conversoes.StrParaIntNullable(Request["IdMDFe"]);
                if (idMDFe != null && idMDFe.GetValueOrDefault() > 0)
                {
                    dtvManifestoEletronico.ChangeMode(DetailsViewMode.ReadOnly);
                }

                if (dtvManifestoEletronico.CurrentMode == DetailsViewMode.Insert)
                {
                    ((DropDownList)dtvManifestoEletronico.FindControl("drpUfInicio")).SelectedValue = Glass.Data.Helper.UserInfo.GetUserInfo.UfLoja;
                    ((DropDownList)dtvManifestoEletronico.FindControl("drpUfFim")).SelectedValue = Glass.Data.Helper.UserInfo.GetUserInfo.UfLoja;
                    ((DropDownList)dtvManifestoEletronico.FindControl("drpTipoEmitente")).SelectedValue = "TransportadorCargaPropria";
                    ((DropDownList)dtvManifestoEletronico.FindControl("drpTipoTransportador")).SelectedValue = "TAC";
                }
            }

            // Define que a Grid CidadeDescarga só fica visível após inserir o MDFe porque é necessário seu ID.
            grdCidadeDescarga.Visible = dtvManifestoEletronico.CurrentMode == DetailsViewMode.ReadOnly ||
                dtvManifestoEletronico.CurrentMode == DetailsViewMode.Edit;
        }

        #region Load

        protected void txtModelo_Load(object sender, EventArgs e)
        {
            ((TextBox)sender).Text = "58";
        }

        protected void txtSerie_Load(object sender, EventArgs e)
        {
            ((TextBox)sender).Enabled = dtvManifestoEletronico.CurrentMode == DetailsViewMode.Insert;

            if (dtvManifestoEletronico.CurrentMode == DetailsViewMode.Insert && ((TextBox)sender).Text == string.Empty)
                ((TextBox)sender).Text = "1";
        }

        protected void btnPreVisualizar_Load(object sender, EventArgs e)
        {
            var mdfe = Glass.Data.DAL.ManifestoEletronicoDAO.Instance.ObterManifestoEletronicoPeloId(idMDFe.GetValueOrDefault());
            if (mdfe != null)
            {
                ((Button)sender).Visible = (mdfe.Situacao == Data.Model.SituacaoEnum.Aberto || mdfe.Situacao == Data.Model.SituacaoEnum.FalhaEmitir);
            }
        }

        protected void btnEmitir_Load(object sender, EventArgs e)
        {
            var mdfe = Glass.Data.DAL.ManifestoEletronicoDAO.Instance.ObterManifestoEletronicoPeloId(idMDFe.GetValueOrDefault());
            if (mdfe != null)
            {
                ((Button)sender).Visible = (mdfe.Situacao == Data.Model.SituacaoEnum.Aberto || mdfe.Situacao == Data.Model.SituacaoEnum.FalhaEmitir) &&
                    mdfe.TipoEmissao == TipoEmissao.Normal;
            }
        }

        protected void btnImprimirContingencia_Load(object sender, EventArgs e)
        {
            var mdfe = Glass.Data.DAL.ManifestoEletronicoDAO.Instance.ObterManifestoEletronicoPeloId(idMDFe.GetValueOrDefault());
            if (mdfe != null)
            {
                ((Button)sender).Visible = (mdfe.Situacao == Data.Model.SituacaoEnum.Aberto || mdfe.Situacao == Data.Model.SituacaoEnum.FalhaEmitir) &&
                    mdfe.TipoEmissao == TipoEmissao.Contingencia;
            }
        }

        #endregion

        protected void odsManifestoEletronico_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception == null)
                Response.Redirect("CadManifestoEletronico.aspx?IdMDFe=" + e.ReturnValue);
            else
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao cadastrar MDFe.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }

        protected void odsManifestoEletronico_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception == null)
            {
                Glass.MensagemAlerta.ShowMsg("MDFe atualizado!", Page);
                Response.Redirect(Request.Url.ToString());
            }
            else
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar MDFe.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }

        #region Botões

        protected void btnEditar_Click(object sender, EventArgs e)
        {
            dtvManifestoEletronico.ChangeMode(DetailsViewMode.Edit);
        }

        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Listas/LstManifestoEletronico.aspx");
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Cadastros/CadManifestoEletronico.aspx?IdMDFe=" + Request["IdMDFe"]);
        }

        protected void btnEmitir_Click(object sender, EventArgs e)
        {
            try
            {
                var retornoEmissao = ManifestoEletronicoDAO.Instance.EmitirMDFe(idMDFe.GetValueOrDefault(), false);
                if(retornoEmissao != "Arquivo recebido com sucesso")
                {
                    // Se houver falha de emissão do MDFe por erro de conexão, verifica se o usuário deseja emitir em contingencia offline
                    if (retornoEmissao.Contains("Impossível conectar-se ao servidor remoto"))
                        Response.Redirect("~/Listas/LstManifestoEletronico.aspx?FalhaEmitirMDFe=true&IdMDFe=" + idMDFe.GetValueOrDefault());

                    // Redireciona para a lista com a mensagem de retorno
                    Response.Redirect("~/Listas/LstManifestoEletronico.aspx?Retorno=" + retornoEmissao);
                }

                // Consulta a situação
                var retorno = ConsultaSituacao.ConsultaSitLoteMDFe(idMDFe.GetValueOrDefault());
                Response.Redirect("~/Listas/LstManifestoEletronico.aspx?Retorno=" + retorno);
            }
            catch (Exception ex)
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

        protected void btnPreVisualizar_Click(object sender, EventArgs e)
        {
            try
            {
                ManifestoEletronicoDAO.Instance.EmitirMDFe(idMDFe.GetValueOrDefault(), true);

                ClientScript.RegisterStartupScript(typeof(string), "msg", "openWindow(600, 800, \"../Relatorios/MDFe/RelBase.aspx?rel=Damdfe&previsualizar=true&IdMDFe=\" + " +
                    idMDFe.GetValueOrDefault() + ");", true);
            }
            catch(Exception ex)
            {
                // Se for erro na validação do arquivo XML, abre popup para mostrar erros
                if (ex.Message.Contains("XML inconsistente."))
                {
                    string msg = Glass.MensagemAlerta.FormatErrorMsg("", ex).Replace("XML inconsistente.", "").Replace("Linha:", "%bl%%bl%Linha:");
                    ClientScript.RegisterClientScriptBlock(typeof(string), "msg", "openWindow(410, 540, '../Utils/ShowMsg.aspx?title=Falha na validação do arquivo do MDF-e&msg=" + msg + "')", true);
                }
                else
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao pré-visualizar MDFe.", ex, Page);
                }
            }
        }

        protected void btnImprimirContingencia_Click(object sender, EventArgs e)
        {
            try
            {
                var retorno = ManifestoEletronicoDAO.Instance.ImprimirMDFeContingencia(idMDFe.GetValueOrDefault());
                Response.Redirect("~/Listas/LstManifestoEletronico.aspx?Retorno=" + retorno);
            }
            catch (Exception ex)
            {
                // Se for erro na validação do arquivo XML, abre popup para mostrar erros
                if (ex.Message.Contains("XML inconsistente."))
                {
                    string msg = Glass.MensagemAlerta.FormatErrorMsg("", ex).Replace("XML inconsistente.", "").Replace("Linha:", "%bl%%bl%Linha:");
                    ClientScript.RegisterClientScriptBlock(typeof(string), "msg", "openWindow(410, 540, '../Utils/ShowMsg.aspx?title=Falha na validação do arquivo do MDF-e&msg=" + msg + "')", true);
                }
                else
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao emitir o MDFe.", ex, Page);
                }
            }
        }

        #endregion

        #region Métodos Ajax

        [Ajax.AjaxMethod()]
        public string InserirCidadeDescargaMdfe(string idCidadeDescarga)
        {
            try
            {
                var idCidade = idCidadeDescarga.StrParaInt();

                if (idCidade == 0 || idMDFe.Value == 0)
                {
                    return ("Erro|Falha ao recuperar dados da cidade.");
                }

                if (CidadeDescargaMDFeDAO.Instance.ValidarCidadeDescargaJaInserida(idCidade, idMDFe.Value))
                {
                    return ("Erro|Cidade já inserida.");
                }

                var cidadeDescargaMDFe = new CidadeDescargaMDFe();
                cidadeDescargaMDFe.IdManifestoEletronico = idMDFe.Value;
                cidadeDescargaMDFe.IdCidade = idCidade;

                CidadeDescargaMDFeDAO.Instance.Insert(cidadeDescargaMDFe);

            }
            catch (Exception ex)
            {
                return ("Erro|Falha ao associar Cidade." + ex.Message);
            }

            return "Cidade inserida com sucesso";
        }

        [Ajax.AjaxMethod]
        public string BuscarInfoNfe(string idNf)
        {
            var nfe = NotaFiscalDAO.Instance.GetElementByPrimaryKey(idNf.StrParaInt());
            return $"{nfe.ChaveAcesso}|{nfe.NumeroDocumentoFsda}";
        }

        [Ajax.AjaxMethod]
        public string InserirNfeCidadeDescarga(string idCidadeDesc, string idNf, string chaveAcesso, string numeroDocumentoFsdaStr)
        {
            try
            {
                var idCidadeDescarga = Conversoes.StrParaInt(idCidadeDesc);
                var idNFe = Conversoes.StrParaIntNullable(idNf);
                var numeroDocumentoFsda = string.IsNullOrWhiteSpace(numeroDocumentoFsdaStr) ? (long?)null : long.Parse(numeroDocumentoFsdaStr);

                if (idCidadeDescarga == 0)
                {
                    return "Erro|Falha ao recuperar dados da cidade.";
                }

                if (string.IsNullOrWhiteSpace(chaveAcesso) || chaveAcesso.Length != 44)
                {
                    return "Erro|A chave de acesso precisa ter 44 caracteres.";
                }

                if (NFeCidadeDescargaMDFeDAO.Instance.VerificarNfeJaInclusa(chaveAcesso))
                {
                    var numeroMdfe = NFeCidadeDescargaMDFeDAO.Instance.GetMdfeNfeInclusa(chaveAcesso);
                    return $"Erro|Nota Fiscal já inclusa no MDFe {numeroMdfe}.";
                }

                var nfeCidadeDescarga = new NFeCidadeDescargaMDFe();
                nfeCidadeDescarga.IdCidadeDescarga = idCidadeDescarga;
                nfeCidadeDescarga.IdNFe = idNFe;
                nfeCidadeDescarga.ChaveAcesso = chaveAcesso;
                nfeCidadeDescarga.NumeroDocumentoFsda = numeroDocumentoFsda;
                NFeCidadeDescargaMDFeDAO.Instance.Insert(null, nfeCidadeDescarga);
            }
            catch (Exception ex)
            {
                return "Erro|Falha ao associar NF-e." + ex.Message;
            }

            return "OK|Nfe Inserida";
        }

        [Ajax.AjaxMethod]
        public string BuscarInfoCte(string idCte)
        {
            return Glass.Data.DAL.CTe.ConhecimentoTransporteDAO.Instance.ObtemChaveAcesso(idCte.StrParaUint());
        }

        [Ajax.AjaxMethod]
        public string InserirCteCidadeDescarga(string idCidadeDesc, string idCte, string chaveAcesso, string numeroDocumentoFsdaStr)
        {
            try
            {
                var idCidadeDescarga = Conversoes.StrParaInt(idCidadeDesc);
                var idCTeDesc = Conversoes.StrParaIntNullable(idCte);
                var numeroDocumentoFsda = string.IsNullOrWhiteSpace(numeroDocumentoFsdaStr) ? (long?)null : long.Parse(numeroDocumentoFsdaStr);

                if (idCidadeDescarga == 0)
                {
                    return "Erro|Falha ao recuperar dados da cidade.";
                }

                if(string.IsNullOrWhiteSpace(chaveAcesso) || chaveAcesso.Length != 44)
                {
                    return "Erro|A chave de acesso precisa ter 44 caracteres.";
                }

                if (CTeCidadeDescargaMDFeDAO.Instance.VerificarCteJaIncluso(chaveAcesso))
                {
                    return "Erro|Conhecimento de transporte já incluso.";
                }

                var cteCidadeDescarga = new CTeCidadeDescargaMDFe();
                cteCidadeDescarga.IdCidadeDescarga = idCidadeDescarga;
                cteCidadeDescarga.IdCTe = idCTeDesc;
                cteCidadeDescarga.ChaveAcesso = chaveAcesso;
                cteCidadeDescarga.NumeroDocumentoFsda = numeroDocumentoFsda;
                CTeCidadeDescargaMDFeDAO.Instance.Insert(null, cteCidadeDescarga);
            }
            catch (Exception ex)
            {
                return "Erro|Falha ao associar CT-e." + ex.Message;
            }

            return "OK|Cte Inserido";
        }

        #endregion

        #region Métodos usados na página

        private bool corAlternada = true;

        protected string GetAlternateClass()
        {
            corAlternada = !corAlternada;
            return corAlternada ? "alt" : "";
        }

        #endregion

        #region Eventos da Grid

        protected void grdCidadeDescarga_DataBound(object sender, EventArgs e)
        {
            if (grdCidadeDescarga.Rows.Count == 1)
                grdCidadeDescarga.Rows[0].Visible = CidadeDescargaMDFeDAO.Instance.GetCountReal(idMDFe.GetValueOrDefault()) > 0;
            else
                grdCidadeDescarga.Rows[0].Visible = true;
        }

        protected void grdNFeCidadeDescarga_DataBound(object sender, EventArgs e)
        {
            // Recupera a grdNFeCidadeDescarga do sender porque a mesma só existe dentro de uma linha da grdCidadeDescarga
            var grdNFeCidadeDescarga = ((GridView)(System.Web.UI.WebControls.GridView)sender);
            // Recupera o idCidadeDescarga através di HiddenField para saber qual cidade está sendo exibida
            var idCidadeDescarga = Conversoes.StrParaInt(((HiddenField)grdNFeCidadeDescarga.Parent.Parent.FindControl("hdfIdCidadeDescargaNFe")).Value);

            if (grdNFeCidadeDescarga.Rows.Count == 1 && idCidadeDescarga > 0)
                grdNFeCidadeDescarga.Rows[0].Visible = NFeCidadeDescargaMDFeDAO.Instance.GetCountReal(idCidadeDescarga) > 0;
            else
                grdNFeCidadeDescarga.Rows[0].Visible = true;
        }

        protected void grdCTeCidadeDescarga_DataBound(object sender, EventArgs e)
        {
            // Recupera a grdCTeCidadeDescarga do sender porque a mesma só existe dentro de uma linha da grdCidadeDescarga
            var grdCTeCidadeDescarga = ((GridView)(System.Web.UI.WebControls.GridView)sender);
            // Recupera o idCidadeDescarga através di HiddenField para saber qual cidade está sendo exibida
            var idCidadeDescarga = Conversoes.StrParaInt(((HiddenField)grdCTeCidadeDescarga.Parent.Parent.FindControl("hdfIdCidadeDescargaCTe")).Value);

            if (grdCTeCidadeDescarga.Rows.Count == 1 && idCidadeDescarga > 0)
                grdCTeCidadeDescarga.Rows[0].Visible = CTeCidadeDescargaMDFeDAO.Instance.GetCountReal(idCidadeDescarga) > 0;
            else
                grdCTeCidadeDescarga.Rows[0].Visible = true;
        }

        protected void grdCidadeDescarga_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ExcluirCidadeDescarga")
            {
                try
                {
                    var cidadeDescarga = CidadeDescargaMDFeDAO.Instance.ObterCidadeDescargaMDFe(Glass.Conversoes.StrParaInt(e.CommandArgument.ToString()));
                    CidadeDescargaMDFeDAO.Instance.DeleteComTransacao(cidadeDescarga);
                    Glass.MensagemAlerta.ShowMsg("Cidade Descarga excluida!", Page);

                    grdCidadeDescarga.DataBind();
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao excluir cidade descarga.", ex, Page);
                }
            }
        }
        #endregion

    }
}
