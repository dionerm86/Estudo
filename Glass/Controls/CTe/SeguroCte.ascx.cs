using System;
using System.Web.UI.WebControls;
using WebGlass.Business.ConhecimentoTransporte.Entidade;
using System.Collections.Generic;

namespace Glass.UI.Web.Controls.CTe
{
    public partial class SeguroCte : CteBaseUserControl
    {
        #region propriedade

        public WebGlass.Business.ConhecimentoTransporte.Entidade.SeguroCte ObjSeguroCte
        {
            get
            {
                return new WebGlass.Business.ConhecimentoTransporte.Entidade.SeguroCte(
                       new Glass.Data.Model.Cte.SeguroCte
                       {
                           IdSeguradora = Glass.Conversoes.StrParaUint(drpSeguradora.SelectedValue),
                           NumeroApolice = txtNumApolice.Text,
                           NumeroAverbacao = txtNumAverbacao.Text,
                           ResponsavelSeguro = Glass.Conversoes.StrParaInt(drpRespSeguro.SelectedValue),
                           ValorCargaAverbacao = Glass.Conversoes.StrParaDecimal(txtValorCargaAverbacao.Text)
                       });
            }
            set
            {            
                drpRespSeguro.SelectedValue = value.ResponsavelSeguro.ToString();
                
                txtNumApolice.Text = value.NumeroApolice;
                txtNumAverbacao.Text = value.NumeroAverbacao;
                if(value.IdSeguradora.ToString() != "0")
                drpSeguradora.SelectedValue = value.IdSeguradora.ToString();
                txtValorCargaAverbacao.Text = value.ValorCargaAverbacao == 0 ? "" : value.ValorCargaAverbacao.ToString();
            }
        }
    
        #endregion
    
        public override IEnumerable<BaseValidator> ValidadoresObrigatoriosEntrada
        {
            get { return null; }
        }
    
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && ObjSeguroCte.IdCte == 0 && TipoDocumentoCte == Cte.TipoDocumentoCteEnum.Saida)
                drpRespSeguro.SelectedValue = Configuracoes.FiscalConfig.TelaCadastroCTe.ReponsavelSeguroCtePadraoCteSaida;
        }
    }
}
