using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.CTeUtils;
using Glass.Data.Helper;
using Glass.Configuracoes;
using Glass.Data.DAL;

namespace Glass.UI.Web.Listas
{
    public partial class LstConhecimentoTransporte : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(LstConhecimentoTransporte));

            if (Request.QueryString["erroCte"] != null)
                ClientScript.RegisterClientScriptBlock(GetType(), "erroCte", "alert('O arquivo desse cte não foi encontrado.')", true);
    
            if (!IsPostBack)
            {
                // Se for para gerar contingência, altera a descrição do botão
                bool contingenciaHabilitada = FiscalConfig.ConhecimentoTransporte.ContingenciaCTe == DataSources.TipoContingenciaCTe.SVC;
    
                lnkDesabilitarContingenciaCTe.Visible = contingenciaHabilitada;
                lnkAlterarContingenciaCTe.Visible = !contingenciaHabilitada;
                drpTipoRemetente_SelectedIndexChanged(null, null);
                drpTipoDestinatario_SelectedIndexChanged(null, null);
                drpTipoRecebedor_SelectedIndexChanged(null, null);
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdCte.PageIndex = 0; 
        }
    
        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Cadastros/CadConhecimentoTransporte.aspx");
        }
    
        protected void grdCte_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ConsultaSitLote")
            {
                try
                {
                    // Consulta a situação do lote e da CTe, caso o lote tenha sido processado
                    string msg = ConsultaSituacao.ConsultaLote(Glass.Conversoes.StrParaUint(e.CommandArgument.ToString()));
                    Glass.MensagemAlerta.ShowMsg(msg, Page);
    
                    grdCte.DataBind();
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao consultar situação.", ex, Page);
                }
            }
            else if (e.CommandName == "ConsultaSitCTe")
            {
                try
                {
                    // Consulta a situação do lote e dO CTe, caso o lote tenha sido processado
                    string msg = ConsultaSituacao.ConsultaSitCTe(Glass.Conversoes.StrParaUint(e.CommandArgument.ToString()));
                    Glass.MensagemAlerta.ShowMsg(msg, Page);
    
                    grdCte.DataBind();
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
                    uint idCte = Glass.Conversoes.StrParaUint(e.CommandArgument.ToString());
                    WebGlass.Business.ConhecimentoTransporte.Fluxo.FinalizarCte.Instance.Reabrir(idCte);
    
                    grdCte.DataBind();
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao reabrir CT-e.", ex, Page);
                }
            }
        }
    
        protected void lnkDesabilitarContingenciaCTe_Click(object sender, EventArgs e)
        {
            ConfigDAO.Instance.SetValue(Config.ConfigEnum.ContingenciaCTe, UserInfo.GetUserInfo.IdLoja, (uint)DataSources.TipoContingenciaCTe.NaoUtilizar);
            lnkDesabilitarContingenciaCTe.Visible = false;
            lnkAlterarContingenciaCTe.Visible = true;
        }
    
        protected void lnkAlterarContingenciaCTe_Click(object sender, EventArgs e)
        {
            ConfigDAO.Instance.SetValue(Config.ConfigEnum.ContingenciaCTe, UserInfo.GetUserInfo.IdLoja, (uint)DataSources.TipoContingenciaCTe.SVC);
            lnkDesabilitarContingenciaCTe.Visible = true;
            lnkAlterarContingenciaCTe.Visible = false;
        }
    
        protected string GetTipoContingenciaCte()
        {
            string descr = FiscalConfig.ConhecimentoTransporte.ContingenciaCTe == DataSources.TipoContingenciaCTe.NaoUtilizar ? null :
                DataSources.Instance.GetDescrTipoContingenciaCTe((int)FiscalConfig.ConhecimentoTransporte.ContingenciaCTe);
    
            lblContingenciaCTe.Text = "CTe em Contingência: " + descr;
            return descr;
        }
    
        protected void grdCte_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;
    
            var situacao = ((WebGlass.Business.ConhecimentoTransporte.Entidade.Cte)e.Row.DataItem).Situacao;
    
            if (situacao == 1 || situacao == 10 || situacao == 12)
                ((HyperLink)e.Row.FindControl("lnkEditar")).Visible = true;
    
            if (situacao != 13 )
                ((PlaceHolder)e.Row.FindControl("phLog")).Visible = true;
    
            var cor = situacao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.FalhaCancelar
                || situacao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.FalhaEmitir ||
                        situacao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.FalhaInutilizar ? System.Drawing.Color.Red :
                        situacao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.Autorizado ||
                        situacao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.FinalizadoTerceiros ? System.Drawing.Color.Blue :
                        System.Drawing.Color.Black;
    
            if (cor != System.Drawing.Color.Black)
                ((Label)e.Row.FindControl("lblSituacao")).ForeColor = cor;
        }

        protected void drpTipoRemetente_SelectedIndexChanged(object sender, EventArgs e)
        {
            drpRemetente.Items.Clear();
            drpRemetente.Items.Add("");
            switch (drpTipoRemetente.SelectedIndex)
            {
                case 0: //Loja
                    drpRemetente.DataSourceID = "odsLoja";
                    drpRemetente.DataValueField = "IdLoja";
                    drpRemetente.DataTextField = "RazaoSocial";
                    break;
                case 1: //Fornecedor
                    drpRemetente.DataSourceID = "odsFornecedor";
                    drpRemetente.DataValueField = "IdFornec";
                    drpRemetente.DataTextField = "RazaoSocial";
                    break;
                case 2: //Cliente
                    drpRemetente.DataSourceID = "odsCliente";
                    drpRemetente.DataValueField = "IdCli";
                    drpRemetente.DataTextField = "Nome";
                    break;
                case 3: //Transportador
                    drpRemetente.DataSourceID = "odsTransportador";
                    drpRemetente.DataValueField = "IdTransportador";
                    drpRemetente.DataTextField = "Nome";
                    break;
            }
        }

        protected void drpTipoDestinatario_SelectedIndexChanged(object sender, EventArgs e)
        {
            drpDestinatario.Items.Clear();
            drpDestinatario.Items.Add("");
            switch (drpTipoDestinatario.SelectedIndex)
            {
                case 0: //Loja
                    drpDestinatario.DataSourceID = "odsLoja";
                    drpDestinatario.DataValueField = "IdLoja";
                    drpDestinatario.DataTextField = "RazaoSocial";
                    break;
                case 1: //Fornecedor
                    drpDestinatario.DataSourceID = "odsFornecedor";
                    drpDestinatario.DataValueField = "IdFornec";
                    drpDestinatario.DataTextField = "RazaoSocial";
                    break;
                case 2: //Cliente
                    drpDestinatario.DataSourceID = "odsCliente";
                    drpDestinatario.DataValueField = "IdCli";
                    drpDestinatario.DataTextField = "Nome";
                    break;
                case 3: //Transportador
                    drpDestinatario.DataSourceID = "odsTransportador";
                    drpDestinatario.DataValueField = "IdTransportador";
                    drpDestinatario.DataTextField = "Nome";
                    break;
            }
        }

        protected void drpTipoRecebedor_SelectedIndexChanged(object sender, EventArgs e)
        {
            drpRecebedor.Items.Clear();
            drpRecebedor.Items.Add("");
            switch (drpTipoRecebedor.SelectedIndex)
            {
                case 0: //Loja
                    drpRecebedor.DataSourceID = "odsLoja";
                    drpRecebedor.DataValueField = "IdLoja";
                    drpRecebedor.DataTextField = "RazaoSocial";
                    break;
                case 1: //Fornecedor
                    drpRecebedor.DataSourceID = "odsFornecedor";
                    drpRecebedor.DataValueField = "IdFornec";
                    drpRecebedor.DataTextField = "RazaoSocial";
                    break;
                case 2: //Cliente
                    drpRecebedor.DataSourceID = "odsCliente";
                    drpRecebedor.DataValueField = "IdCli";
                    drpRecebedor.DataTextField = "Nome";
                    break;
                case 3: //Transportador
                    drpRecebedor.DataSourceID = "odsTransportador";
                    drpRecebedor.DataValueField = "IdTransportador";
                    drpRecebedor.DataTextField = "Nome";
                    break;
            }
        }

        [Ajax.AjaxMethod]
        public string ObtemIdLoja(string idCte)
        {
            var contasPagar = ContasPagarDAO.Instance.GetByCte(idCte.StrParaUint());

            if (contasPagar.Length == 0)
                return "1";
            
            return contasPagar[0].IdLoja.ToString();
        }
    }
}
