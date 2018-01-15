using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.UI;
using Glass.Data.DAL;
using Glass.Data.CFeUtils;

namespace Glass.UI.Web.CFe
{
    public partial class ConsultaCFe : System.Web.UI.Page
    {
        #region Eventos
    
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(CFe.ConsultaCFe));
        }
    
        protected void btnStatusOperacional_Click(object sender, EventArgs e)
        {
            try
            {
                StatusOperacionalSAT statusSat = CupomFiscalDAO.Instance.MontarStatusOperacional(this.hidStatusOperacional.Value.Split('|'));
    
                List<StatusOperacionalSAT> lstStatus = new List<StatusOperacionalSAT>();
                lstStatus.Add(statusSat);
    
                this.dtvStatusOp.DataSource = lstStatus;
                this.dtvStatusOp.DataBind();
            }
            catch
            {
                string script = @"<script>"
                               + "  alert('Não foi possível obter o status operacional do aparelho.');"
                               + "</script>";
    
                Page.RegisterClientScriptBlock(Guid.NewGuid().ToString(), script);
            }
        }
    
        #endregion
    
        #region Metodos Ajax
    
        [Ajax.AjaxMethod]
        public string ObterNumeroSessao()
        {
            string numSessao = CupomFiscalDAO.Instance.GerarNumeroSessao().ToString();
            return numSessao;
        }
    
        [Ajax.AjaxMethod]
        public string ObterCodigoAtivacao()
        {
            return ConfigurationManager.AppSettings.Get("CodigoAtivacaoSAT");
        }
    
        #endregion
    }
}
