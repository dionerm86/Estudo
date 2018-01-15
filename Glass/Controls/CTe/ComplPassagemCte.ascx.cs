using System;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Controls.CTe
{
    public partial class ComplPassagemCte : CteBaseUserControl
    {
        #region Properties

        public WebGlass.Business.ConhecimentoTransporte.Entidade.ComplPassagemCte ObjComplPassagemCte
        {
            get 
            {
                return new WebGlass.Business.ConhecimentoTransporte.Entidade.ComplPassagemCte(
                    new Glass.Data.Model.Cte.ComplPassagemCte
                    {
                        NumSeqPassagem = 1,
                        SiglaPassagem = txtSiglaPassagem.Text
                    });
            }
            set 
            {
                //txtNumSeqPassagem.Text = value.NumSeqPassagem.ToString();
                txtSiglaPassagem.Text = value.SiglaPassagem;
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
