using System.Web.UI.WebControls;
using WebGlass.Business.ConhecimentoTransporte.Entidade;

namespace Glass.UI.Web.Controls.CTe
{
    public partial class ComplCte : CteBaseUserControl
    {
        #region Propriedades

        public WebGlass.Business.ConhecimentoTransporte.Entidade.ComplCte ObjComplCte
        {
            get
            {
                var obj = new WebGlass.Business.ConhecimentoTransporte.Entidade.ComplCte(
                    new Glass.Data.Model.Cte.ComplCte
                    {
                        CaractServico = txtCaractServico.Text,
                        CaractTransporte = txtCaractTransporte.Text,
                        IdRota = Glass.Conversoes.StrParaUint(drpRota.SelectedValue),                    
                        SiglaDestino = txtSiglaDestino.Text,
                        SiglaOrigem = txtSiglaOrigem.Text
                    });
    
                obj.ObjComplPassagemCte = ctrlComplPassagemCte.ObjComplPassagemCte;
    
                return obj;
            }
            set
            {
                txtCaractServico.Text = value.CaractServico;
                txtCaractTransporte.Text = value.CaractTransporte;
                drpRota.SelectedValue = value.IdRota.ToString();
                ctrlComplPassagemCte.ObjComplPassagemCte = value.ObjComplPassagemCte;
                txtSiglaDestino.Text = value.SiglaDestino;
                txtSiglaOrigem.Text = value.SiglaOrigem;
            }
        }
    
        public override Cte.TipoDocumentoCteEnum TipoDocumentoCte
        {
            get { return base.TipoDocumentoCte; }
            set
            {
                base.TipoDocumentoCte = value;
                ctrlComplPassagemCte.TipoDocumentoCte = value;
            }
        }
    
        #endregion
    
        public override System.Collections.Generic.IEnumerable<BaseValidator> ValidadoresObrigatoriosEntrada
        {
            get { return null; }
        }
    }
}
