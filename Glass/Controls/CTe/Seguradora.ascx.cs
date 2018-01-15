using System;
using System.Web.UI.WebControls;


namespace Glass.UI.Web.Controls.CTe
{
    public partial class Seguradora : CteBaseUserControl
    {
        #region Propriedades

        public WebGlass.Business.ConhecimentoTransporte.Entidade.Seguradora ObjSeguradora
        {
            get
            {
                return new WebGlass.Business.ConhecimentoTransporte.Entidade.Seguradora(
                    new Glass.Data.Model.Cte.Seguradora
                    {
                        NomeSeguradora = drpSeguradora.SelectedItem.Text
                    });
            }
            set
            {
                drpSeguradora.SelectedValue = value.IdSeguradora.ToString();
            }
        }
    
        #endregion
    
        public override System.Collections.Generic.IEnumerable<BaseValidator> ValidadoresObrigatoriosEntrada
        {
            get { return null; }
        }
    
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    }
}
