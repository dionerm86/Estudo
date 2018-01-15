using System;
using System.Web.UI;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlPopupTela : BaseUserControl
    {
        #region Enumerações
    
        public enum TipoExibicaoInterna : byte
        {
            Html,
            Pagina
        }
    
        #endregion
    
        #region Construtor
    
        public ctrlPopupTela()
        {
            ExibirFechar = true;
        }
    
        #endregion
    
        #region Propriedades
    
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public string InnerHTML { get; set; }
    
        public string UrlExterno { get; set; }
    
        public bool ExibirFechar { get; set; }
    
        public TipoExibicaoInterna TipoExibicao { get; set; }
    
        #endregion
    
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.ClientScript.IsClientScriptIncludeRegistered(GetType(), "ctrlPopupTela"))
                Page.ClientScript.RegisterClientScriptInclude(GetType(), "ctrlPopupTela", this.ResolveClientUrl("~/Scripts/ctrlPopupTela.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)));
    
            if (TipoExibicao == TipoExibicaoInterna.Pagina && !String.IsNullOrEmpty(UrlExterno))
                hdfInnerHTML.Value = this.ResolveClientUrl(UrlExterno);
        }
    
        protected string GetHtmlExibir()
        {
            if (this.TipoExibicao != TipoExibicaoInterna.Html || String.IsNullOrEmpty(InnerHTML))
                return String.Empty;
    
            return InnerHTML.Replace("~/", this.ResolveClientUrl("~/"));
        }
    
        protected string GetScriptVariavel()
        {
            return String.Format("var {0} = new PopupTelaType('{0}', {1}, {2}, '{3}');\n", this.ClientID, 
                ExibirFechar.ToString().ToLower(), (int)TipoExibicao, this.ResolveClientUrl("~/"));
        }
    }
}
