using System;
using System.Web.UI.WebControls;
using Glass.Data.EFD;
using Glass.Configuracoes;

namespace Glass.UI.Web.Relatorios.Fiscal
{
    public partial class EFD : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                drpMes_icmsIpi.DataBind();
                drpMes_icmsIpi.SelectedValue = DateTime.Now.Month.ToString();
                txtAno_icmsIpi.Text = DateTime.Now.Year.ToString();
    
                drpMes_pisCofins.DataBind();
                drpMes_pisCofins.SelectedValue = drpMes_icmsIpi.SelectedValue;
                txtAno_pisCofins.Text = txtAno_icmsIpi.Text;
    
                int? valor = (int?)FiscalConfig.IndicadorIncidenciaTributaria;
                if (valor != null)
                {
                    ctrlCodIncTrib.Valor = valor.ToString();
                    ctrlCodIncTrib.Descricao = DataSourcesEFD.Instance.GetDescrCodIncTrib(valor.Value);
                }
    
                valor = (int?)FiscalConfig.MetodoApropriacaoCreditos;
                if (valor != null)
                {
                    ctrlIndAproCred.Valor = valor.ToString();
                    ctrlIndAproCred.Descricao = DataSourcesEFD.Instance.GetDescrIndAproCred(valor.Value);
                }
    
                valor = (int?)FiscalConfig.TipoContribuicaoApurada;
                if (valor != null)
                {
                    ctrlCodTipoCont.Valor = valor.ToString();
                    ctrlCodTipoCont.Descricao = DataSourcesEFD.Instance.GetDescrCodTipoCont(valor.Value);
                }
            }
        }
    
        protected void drpLoja_DataBound(object sender, EventArgs e)
        {
            DropDownList drpLoja = (DropDownList)sender;
            uint idLoja = Glass.Conversoes.StrParaUint(drpLoja.SelectedValue);
            hdfIdLoja.Value = idLoja.ToString();
        }
    
        protected string ExibeSped()
        {
            if (FiscalConfig.NotaFiscalConfig.GerarEFD) return String.Empty;
    
            return @"<script type='text/javascript'>
                    document.getElementById('aba_icmsIpi').style.display = 'none';
                    document.getElementById('aba_pisCofins').style.display = 'none';
                </script>";
        }
    }
}
