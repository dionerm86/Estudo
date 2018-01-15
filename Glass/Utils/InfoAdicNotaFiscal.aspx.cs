using System;
using System.Web.UI.WebControls;
using Glass.Data.Model;

namespace Glass.UI.Web.Utils
{
    public partial class InfoAdicNotaFiscal : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
    
        protected void dtvInfoAdicNf_ItemUpdated(object sender, DetailsViewUpdatedEventArgs e)
        {
            if (e.Exception == null)
                Glass.MensagemAlerta.ShowMsg("Informações adicionais salvas!", Page);
            else
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao salvar informações adicionais.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
    
        protected void dtvInfoAdicNf_DataBound(object sender, EventArgs e)
        {
            InfoAdicionalNf item = dtvInfoAdicNf.DataItem as InfoAdicionalNf;
    
            dtvInfoAdicNf.Rows[0].Visible = item.IsNfTransporte;
            dtvInfoAdicNf.Rows[1].Visible = !item.IsNfTransporte && !item.IsNfComunicacao && !item.IsNfTelecomunicacao;
            dtvInfoAdicNf.Fields[2].HeaderText = item.IsNfTransporte || item.IsNfComunicacao || item.IsNfTelecomunicacao ?
                "Valor Prest. Serviço" : "Valor Fornecido/Consumido";
            dtvInfoAdicNf.Rows[4].Visible = !item.IsNfTransporte;
            dtvInfoAdicNf.Rows[5].Visible = !item.IsNfTransporte && item.IsNfEnergiaEletrica;
            dtvInfoAdicNf.Rows[6].Visible = !item.IsNfTransporte && item.IsNfEnergiaEletrica;
            dtvInfoAdicNf.Rows[7].Visible = item.IsNfTransporte || item.IsNfComunicacao || item.IsNfTelecomunicacao;
            dtvInfoAdicNf.Rows[8].Visible = item.IsNfComunicacao || item.IsNfTelecomunicacao;
        }
    }
}
