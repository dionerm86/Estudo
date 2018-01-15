using System;
using System.Web.UI;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlTipoFuncionario : BaseUserControl
    {
        #region Propriedades
    
        public bool IncluirSetor
        {
            get { return bool.Parse(hdfIncluirSetor.Value); }
            set { hdfIncluirSetor.Value = value.ToString(); }
        }
    
        public bool RemoverMarcadorProducaoSemSetor
        {
            get { return bool.Parse(hdfRemoverMarcadorProducaoSemSetor.Value); }
            set { hdfRemoverMarcadorProducaoSemSetor.Value = value.ToString(); }
        }
    
        public uint IdTipoFunc
        {
            get { return Glass.Conversoes.StrParaUint(hdfTipoFuncionario.Value); }
            set { hdfTipoFuncionario.Value = value.ToString(); }
        }
    
        public uint? IdSetorFunc
        {
            get { return Glass.Conversoes.StrParaUintNullable(hdfIdSetorFuncionario.Value); }
            set { hdfIdSetorFuncionario.Value = value != null ? value.ToString() : ""; }
        }
    
        public bool AutoPostBack
        {
            get { return drpTipoFuncionario.AutoPostBack; }
            set { drpTipoFuncionario.AutoPostBack = value; }
        }
    
        #endregion
    
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.ClientScript.IsClientScriptBlockRegistered(GetType(), "ctrlTipoFuncionario_Script"))
            {
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "ctrlTipoFuncionario_Script", @"
                    function alteraTipoFuncionario(nomeControle, tipoSetor)
                    {
                        var tipoFunc = document.getElementById(nomeControle + '_hdfTipoFuncionario');
                        var idSetor = document.getElementById(nomeControle + '_hdfIdSetorFuncionario');
                        
                        var dadosTipoSetor = tipoSetor.split(',');
                        tipoFunc.value = dadosTipoSetor[0];
                        idSetor.value = dadosTipoSetor.length > 1 ? dadosTipoSetor[1] : '';
                    }", true);
            }
    
            drpTipoFuncionario.Attributes.Add("OnChange", "alteraTipoFuncionario('" + this.ClientID + "', this.value)");
        }
    
        protected void drpTipoFuncionario_DataBound(object sender, EventArgs e)
        {
            string valor = IdTipoFunc.ToString();
            if (IdSetorFunc > 0)
                valor += "," + IdSetorFunc;
    
            drpTipoFuncionario.SelectedValue = valor;
        }
    }
}
