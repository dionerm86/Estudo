using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.EFD;
using Glass.Data.DAL;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlSelParticipante : BaseUserControl
    {
        #region Propriedades
    
        public bool TemParticipanteSelecionado
        {
            get { return IdCliente > 0 || IdFornec > 0 || IdTransportador > 0 || IdLoja > 0 || IdAdminCartao > 0; }
        }
    
        public uint? IdCliente
        {
            get 
            { 
                return TipoPart == DataSourcesEFD.TipoPartEnum.Cliente ? 
                    Glass.Conversoes.StrParaUintNullable(hdfIdPart.Value) : null; 
            }
            set 
            {
                if (value > 0)
                {
                    TipoPart = DataSourcesEFD.TipoPartEnum.Cliente;
                    hdfIdPart.Value = value.ToString();
                }
            }
        }
    
        public uint? IdFornec
        {
            get
            {
                return TipoPart == DataSourcesEFD.TipoPartEnum.Fornecedor ?
                    Glass.Conversoes.StrParaUintNullable(hdfIdPart.Value) : null;
            }
            set
            {
                if (value > 0)
                {
                    TipoPart = DataSourcesEFD.TipoPartEnum.Fornecedor;
                    hdfIdPart.Value = value.ToString();
                }
            }
        }
    
        public uint? IdTransportador
        {
            get
            {
                return TipoPart == DataSourcesEFD.TipoPartEnum.Transportador ?
                    Glass.Conversoes.StrParaUintNullable(hdfIdPart.Value) : null;
            }
            set
            {
                if (value > 0)
                {
                    TipoPart = DataSourcesEFD.TipoPartEnum.Transportador;
                    hdfIdPart.Value = value.ToString();
                }
            }
        }
    
        public uint? IdLoja
        {
            get
            {
                return TipoPart == DataSourcesEFD.TipoPartEnum.Loja ?
                    Glass.Conversoes.StrParaUintNullable(hdfIdPart.Value) : null;
            }
            set
            {
                if (value > 0)
                {
                    TipoPart = DataSourcesEFD.TipoPartEnum.Loja;
                    hdfIdPart.Value = value.ToString();
                }
            }
        }
    
        public uint? IdAdminCartao
        {
            get
            {
                return TipoPart == DataSourcesEFD.TipoPartEnum.AdministradoraCartao ?
                    Glass.Conversoes.StrParaUintNullable(hdfIdPart.Value) : null;
            }
            set
            {
                if (value > 0)
                {
                    TipoPart = DataSourcesEFD.TipoPartEnum.AdministradoraCartao;
                    hdfIdPart.Value = value.ToString();
                }
            }
        }
    
        public bool ExibirAdminCartao
        {
            get { return bool.Parse(hdfExibirAdminCartao.Value); }
            set { hdfExibirAdminCartao.Value = value.ToString(); }
        }
    
        protected DataSourcesEFD.TipoPartEnum TipoPart
        {
            get { return (DataSourcesEFD.TipoPartEnum)Glass.Conversoes.StrParaInt(drpPart.SelectedValue); }
            set 
            {
                if (drpPart.Items.Count == 0)
                    drpPart.DataBind();
    
                drpPart.SelectedValue = ((int)value).ToString(); 
            }
        }
    
        protected uint? IdPart
        {
            get
            {
                switch (TipoPart)
                {
                    case DataSourcesEFD.TipoPartEnum.Fornecedor: return IdFornec;
                    case DataSourcesEFD.TipoPartEnum.Loja: return IdLoja;
                    case DataSourcesEFD.TipoPartEnum.Transportador: return IdTransportador;
                    case DataSourcesEFD.TipoPartEnum.AdministradoraCartao: return IdAdminCartao;
                    default: return IdCliente;
                }
            }
        }
    
        public CustomValidator Validador
        {
            get { return ctvPart; }
        }
    
        public bool PermitirVazio
        {
            get { return !ctvPart.Enabled; }
            set { ctvPart.Enabled = !value; }
        }
    
        public string ValidationGroup
        {
            get { return ctvPart.ValidationGroup; }
            set { ctvPart.ValidationGroup = value; }
        }
    
        #endregion
    
        #region Métodos Ajax
    
        [Ajax.AjaxMethod]
        public string GetNomePart(string tipoPart, string idPartStr)
        {
            try
            {
                uint idPart = Glass.Conversoes.StrParaUint(idPartStr);
                switch ((DataSourcesEFD.TipoPartEnum)Glass.Conversoes.StrParaUint(tipoPart))
                {
                    case DataSourcesEFD.TipoPartEnum.Cliente: return "Ok;" + ClienteDAO.Instance.GetNome(idPart);
                    case DataSourcesEFD.TipoPartEnum.Fornecedor: return "Ok;" + FornecedorDAO.Instance.GetNome(idPart);
                    case DataSourcesEFD.TipoPartEnum.Transportador: return "Ok;" + TransportadorDAO.Instance.GetNome(idPart);
                    case DataSourcesEFD.TipoPartEnum.Loja: return "Ok;" + LojaDAO.Instance.GetNome(idPart);
                    case DataSourcesEFD.TipoPartEnum.AdministradoraCartao: return "Ok;" + AdministradoraCartaoDAO.Instance.GetNome(idPart);
                }
    
                throw new Exception("Tipo de participante inválido.");
            }
            catch (Exception ex)
            {
                return "Erro;" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao buscar nome do participante.", ex);
            }
        }
    
        #endregion
    
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Controls.ctrlSelParticipante));
    
            imgPart.OnClientClick = String.Format("ctrlSelParticipante_buscarPart('{0}', '{1}'); return false",
                this.ClientID, this.ResolveClientUrl("~/Utils/"));
    
            drpPart.Attributes["onchange"] = String.Format("ctrlSelParticipante_limparPart('{0}')", this.ClientID);
    
            if (!Page.ClientScript.IsClientScriptIncludeRegistered(GetType(), "ctrlSelParticipante_script"))
                Page.ClientScript.RegisterClientScriptInclude(GetType(), "ctrlSelParticipante_script", 
                    this.ResolveClientUrl("~/Scripts/ctrlSelParticipante.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)));
    
            if (!Page.ClientScript.IsOnSubmitStatementRegistered(GetType(), "ctrlSelParticipante_submit_" + this.ClientID))
                Page.ClientScript.RegisterOnSubmitStatement(GetType(), "ctrlSelParticipante_submit_" + this.ClientID,
                    String.Format("if (document.getElementById('{0}') != null) document.getElementById('{0}').disabled = false;", drpPart.ClientID));
        }
    
        protected void drpPart_DataBound(object sender, EventArgs e)
        {
            string[] temp = GetNomePart(drpPart.SelectedValue, IdPart != null ? IdPart.ToString() : "").Split(';');
            if (temp[0] == "Ok")
                lblDescrPart.Text = temp[1];
        }
    }
}
