using System;
using System.Web.UI;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlLogCancPopup : BaseUserControl
    {
        #region Propriedades
    
        private LogCancelamento.TabelaCancelamento _tabela;
    
        public LogCancelamento.TabelaCancelamento Tabela
        {
            get { return _tabela; }
            set { _tabela = value; }
        }
    
        private uint? _idRegistro;
    
        public uint? IdRegistro
        {
            get { return _idRegistro; }
            set { _idRegistro = value; }
        }
    
        #endregion
    
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.ClientScript.IsClientScriptBlockRegistered(GetType(), "ctrlLogCancPopup"))
            {
                string script = @"
                    function openLogCanc(tabela, id)
                    {
                        openWindow(500, 700, '" + ResolveClientUrl("~/Utils/ShowLogCancelamento.aspx") + @"?tabela=' + tabela +'&id=' + id);
                    }
                ";
    
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "ctrlLogCancPopup", script, true);
            }
        }
    
        protected void imgPopup_PreRender(object sender, EventArgs e)
        {
            imgPopup.Visible = _idRegistro != null && LogCancelamentoDAO.Instance.TemRegistro(_tabela, _idRegistro.Value);
            imgPopup.OnClientClick = "openLogCanc(" + (int)_tabela + ", " + _idRegistro + "); return false";
        }
    }
}
