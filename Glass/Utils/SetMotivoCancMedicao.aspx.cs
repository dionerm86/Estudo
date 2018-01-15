using System;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.UI.Web.Utils
{
    public partial class SetMotivoCancMedicao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            Medicao med = MedicaoDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(Request["IdMedicao"]));
    
            // Concatena a observação da medição já existente com o motivo do cancelamento
            string motivo = "Motivo do Cancelamento: " + txtMotivo.Text;
            med.ObsMedicao = !String.IsNullOrEmpty(med.ObsMedicao) ? med.ObsMedicao + " " + motivo : motivo;
    
            // Se o tamanho do campo ObsMedicao exceder 500 caracteres, salva apenas os 500 primeiros, descartando o restante
            med.ObsMedicao = med.ObsMedicao.Length > 500 ? med.ObsMedicao.Substring(0, 500) : med.ObsMedicao;
    
            try
            {
                MedicaoDAO.Instance.CancelarMedicao(med.IdMedicao, med.ObsMedicao);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg(null, ex, Page);
                return;
            }
    
            ClientScript.RegisterClientScriptBlock(this.GetType(), "ok", "window.opener.redirectUrl(window.opener.location.href); closeWindow();", true);
        }
    }
}
