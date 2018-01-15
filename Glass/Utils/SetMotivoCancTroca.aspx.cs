using System;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.UI.Web.Utils
{
    public partial class SetMotivoCancTroca : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ctrlDataEstorno.Data = DateTime.Now;
                estornoBanco.Visible = false;
            }
        }
    
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            TrocaDevolucao trocaDev = TrocaDevolucaoDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(Request["IdTrocaDev"]));
    
            // Concatena a observação da troca já existente com o motivo do cancelamento
            string motivo = "Motivo do Cancelamento: " + txtMotivo.Text;
            trocaDev.Obs = !String.IsNullOrEmpty(trocaDev.Obs) ? trocaDev.Obs + " " + motivo : motivo;
    
            // Se o tamanho do campo Obs exceder 300 caracteres, salva apenas os 300 primeiros, descartando o restante
            trocaDev.Obs = trocaDev.Obs.Length > 300 ? trocaDev.Obs.Substring(0, 300) : trocaDev.Obs;
    
            try
            {
                TrocaDevolucaoDAO.Instance.Cancelar(trocaDev.IdTrocaDevolucao, trocaDev.Obs, ctrlDataEstorno.Data);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg(null, ex, Page);
                return;
            }
    
            ClientScript.RegisterClientScriptBlock(this.GetType(), "ok", "window.opener.redirectUrl(window.opener.location.href);closeWindow();", true);
        }
    }
}
