using System;
using System.Web.UI.WebControls;
using WebGlass.Business.ConhecimentoTransporte.Entidade;

namespace Glass.UI.Web.Controls.CTe
{
    public partial class InfoCte : CteBaseUserControl
    {
        #region Propriedades

        public WebGlass.Business.ConhecimentoTransporte.Entidade.InfoCte ObjInfoCte
        {
            get
            {
                var obj = new WebGlass.Business.ConhecimentoTransporte.Entidade.InfoCte(
                    new Glass.Data.Model.Cte.InfoCte
                    {
                        ProdutoPredominante = txtProdutoPredominante.Text,
                        OutrasCaract = txtOutrasCaract.Text,
                        ValorCarga = Glass.Conversoes.StrParaDecimal(txtValorCarga.Text)
                    });

                obj.ObjInfoCargaCte = ctrlCargaCte.ObjInfoCarga;

                return obj;
            }
            set
            {
                txtProdutoPredominante.Text = value.ProdutoPredominante;
                txtOutrasCaract.Text = value.OutrasCaract;
                txtValorCarga.Text = value.ValorCarga.ToString() == "0,00" ? "" : Glass.Conversoes.StrParaFloat(value.ValorCarga.ToString()).ToString();
                ctrlCargaCte.ObjInfoCarga = value.ObjInfoCargaCte;
            }
        }

        public override Cte.TipoDocumentoCteEnum TipoDocumentoCte
        {
            get { return base.TipoDocumentoCte; }
            set
            {
                base.TipoDocumentoCte = value;
                ctrlCargaCte.TipoDocumentoCte = value;
            }
        }

        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && ObjInfoCte.IdCte == 0 && TipoDocumentoCte == Cte.TipoDocumentoCteEnum.Saida)
                txtProdutoPredominante.Text = Configuracoes.FiscalConfig.TelaCadastroCTe.ProdutoPredominanteInfoCtePadraoCteSaida;
        }

        public override System.Collections.Generic.IEnumerable<BaseValidator> ValidadoresObrigatoriosEntrada
        {
            get { return null; }
        }
    }
}
