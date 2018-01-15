using System;
using Glass.Data.DAL;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlSelProcesso : BaseUserControl
    {
            #region Propriedades
    
        public uint? CodigoProcesso
        {
            get { return Glass.Conversoes.StrParaUintNullable(selProcesso.Valor); }
            set
            {
                if (value != null)
                {
                    selProcesso.Valor = value.Value.ToString();
                    selProcesso.Descricao = EtiquetaProcessoDAO.Instance.ObtemCodInterno(value.Value);
                }
                else
                {
                    selProcesso.Valor = null;
                    selProcesso.Descricao = null;
                }
            }
        }
    
        public string ValidationGroup
        {
            get { return selProcesso.ValidationGroup; }
            set { selProcesso.ValidationGroup = value; }
        }
    
        public bool PermitirVazio
        {
            get { return selProcesso.PermitirVazio; }
            set { selProcesso.PermitirVazio = value; }
        }
    
        public bool FazerPostBackBotaoPesquisar
        {
            get { return selProcesso.FazerPostBackBotaoPesquisar; }
            set { selProcesso.FazerPostBackBotaoPesquisar = value; }
        }    

        public Controls.ctrlSelAplicacao ControleAplicacao { get; set; }
    
        #endregion
    
        #region Métodos Ajax
    
        [Ajax.AjaxMethod]
        public string ObtemAplicacao(string idProcessoStr)
        {
            uint idProcesso = Glass.Conversoes.StrParaUint(idProcessoStr);
            uint? idAplicacao = idProcesso > 0 ? EtiquetaProcessoDAO.Instance.ObtemIdAplicacao(idProcesso) : null;
            return idAplicacao != null ? idAplicacao.Value + "|" + EtiquetaAplicacaoDAO.Instance.ObtemCodInterno(idAplicacao.Value) : "";
        }
    
        #endregion
    
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Controls.ctrlSelProcesso));
        }
    }
}
