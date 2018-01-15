using System;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlConfigAresta : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (string.IsNullOrEmpty(_configAresta))
            {
                hdfSubgrupo.Value = "";
                hdfBenef.Value = "";
                hdfCondEspessura.Value = "";
                hdfEspessura.Value = "";
                hdfAresta.Value = "";
                hdfCodProc.Value = "";
                hdfIdProc.Value = "";

                return;
            }

            var dados = _configAresta.Split('|');

            hdfSubgrupo.Value = dados[0];
            hdfBenef.Value = dados[1];
            hdfCondEspessura.Value = dados[2];
            hdfEspessura.Value = dados[3];
            hdfAresta.Value = dados[4];
            hdfIdProc.Value = dados[5];
            hdfCodProc.Value = dados[6];

            Page.ClientScript.RegisterStartupScript(GetType(), "iniciar" + new Random().Next(0, 1000), this.ID + ".carregaConfigAresta();", true);
        }

        #region Variáveis Locais

        private string _configAresta;

        #endregion

        #region Propiedades

        public string ConfigAresta
        {
            get
            {
                _configAresta = hdfSubgrupo.Value + "|" + hdfBenef.Value + "|" + hdfCondEspessura.Value + "|" + hdfEspessura.Value + "|" + hdfAresta.Value + "|" + hdfIdProc.Value + "|" + hdfCodProc.Value;

                return _configAresta;
            }
            set { _configAresta = value; }
        }

        #endregion
    }
}