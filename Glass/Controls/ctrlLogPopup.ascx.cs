using System;
using System.Web.UI;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlLogPopup : BaseUserControl
    {
        #region Propriedades
    
        private LogAlteracao.TabelaAlteracao _tabela;
    
        public LogAlteracao.TabelaAlteracao Tabela
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

        private string _campo;

        public string Campo
        {
            get { return _campo; }
            set { _campo = value; }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.ClientScript.IsClientScriptBlockRegistered(GetType(), "ctrlLogPopup"))
            {
                string script = @"
                    function openLog(tabela, id)
                    {
                        return openLog(tabela, id, '');
                    }

                    function openLog(tabela, id, campo)
                    {
                        openWindow(500, 700, '" + ResolveClientUrl("~/Utils/ShowLogAlteracao.aspx") + @"?tabela=' + tabela +'&id=' + id + '&campo=' + campo);
                    }
                ";
    
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "ctrlLogPopup", script, true);
            }
        }
    
        protected void imgPopup_PreRender(object sender, EventArgs e)
        {
            imgPopup.Visible = _idRegistro != null && LogAlteracaoDAO.Instance.TemRegistro(_tabela, _idRegistro.Value, _campo);
            imgPopup.OnClientClick = "openLog(" + (int)_tabela + ", " + _idRegistro + ", '" + _campo + "'); return false";
        }
    }
}
