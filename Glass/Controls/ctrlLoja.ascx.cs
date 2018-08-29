using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Configuracoes;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlLoja : BaseUserControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (MostrarTodas)
                drpLoja.Items.Add(new ListItem("Todas", "", true));
            else if (MostrarVazia)
                drpLoja.Items.Add(new ListItem());
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if(!MostrarTodas && !MostrarVazia)
                {
                    uint idLoja = UserInfo.GetUserInfo.IdLoja;
                    drpLoja.SelectedValue = idLoja.ToString();
                }

                if (!SomenteAtivas)
                    drpLoja.DataSourceID = "odsLoja";
    
                if (VerificarSeControleDeveSerDesabilitado && Geral.FuncVisualizaDadosApenasSuaLoja && !Config.PossuiPermissao(Config.FuncaoMenuCadastro.IgnorarFuncVisualizarDadosApenasSuaLoja))
                {
                    uint idLoja = UserInfo.GetUserInfo.IdLoja;
                    drpLoja.SelectedValue = idLoja.ToString();
                    drpLoja.Enabled = false;
                }
            }
        }
    
        #region Variaveis Locais
    
        private bool _mostrarTodas = true;
        private bool _somenteAtivas = true;
        private bool _mostrarVazia = false;
        private bool _verificarSeControleDeveSerDesabilitado = true;

        #endregion

        #region Propiedades

        public string SelectedValue 
        { 
            get { return drpLoja.SelectedValue; }
            set { drpLoja.SelectedValue = value; }
        }
    
        public ListItem SelectedItem { get { return drpLoja.SelectedItem; } }
    
        public bool AutoPostBack
        {
            get { return drpLoja.AutoPostBack; }
            set { drpLoja.AutoPostBack = value; }
        }
    
        public event EventHandler SelectedIndexChanged
        {
            add { drpLoja.SelectedIndexChanged += value; }
            remove { drpLoja.SelectedIndexChanged -= value; }
        }
    
        public event EventHandler DataBound
        {
            add { drpLoja.DataBound += value; }
            remove { drpLoja.DataBound -= value; }
        }
    
        public CssStyleCollection Style { get { return drpLoja.Style; } }
    
        public bool MostrarTodas 
        {
            get
            {
                return _mostrarTodas;
            } 
            set
            {
                _mostrarTodas = value;
            }
        }
    
        public bool SomenteAtivas
        {
            get
            {
                return _somenteAtivas;
            }
            set
            {
                _somenteAtivas = value;
            }
        }

        public bool MostrarVazia
        {
            get
            {
                return _mostrarVazia;
            }
            set
            {
                _mostrarVazia = value;
            }
        }

        public bool VerificarSeControleDeveSerDesabilitado
        {
            get { return _verificarSeControleDeveSerDesabilitado; }
            set { _verificarSeControleDeveSerDesabilitado = value; }
        }

        public bool Enabled
        {
            get { return drpLoja.Enabled; }
            set { drpLoja.Enabled = value; }
        }
    
        public string OnChange
        {
            get { return drpLoja.Attributes["onchange"]; }
            set { drpLoja.Attributes.Add("onchange", value); }
        }
    
        #endregion
    }
}
