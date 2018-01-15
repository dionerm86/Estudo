using System;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Utils
{
    public partial class SetMotivoCancReceb : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ctrlDataEstorno.Data = DateTime.Now;
                estornoBanco.Visible = ExibirEstornoBanco();
            }
        }
    
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            try
            {
                uint id = Glass.Conversoes.StrParaUint(Request["id"]);
    
                switch (Request["tipo"])
                {
                    case "contaR":
                        ContasReceberDAO.Instance.CancelarConta(id, txtMotivo.Text, ctrlDataEstorno.Data, false, true);
                        break;
    
                    case "sinal":
                        SinalDAO.Instance.CancelarComTransacao(id, null, false, false, txtMotivo.Text, ctrlDataEstorno.Data, false, true);
                        break;
    
                    case "acerto":
                        ContasReceberDAO.Instance.CancelarAcerto(id, txtMotivo.Text, ctrlDataEstorno.Data, false, true);
                        break;
    
                    case "acertoCheque":
                        AcertoChequeDAO.Instance.CancelarAcertoCheque(id, txtMotivo.Text, ctrlDataEstorno.Data, false, true);
                        break;
    
                    case "obra":
                        ObraDAO.Instance.CancelaObra(id, txtMotivo.Text, ctrlDataEstorno.Data, false, true);
                        break;
    
                    case "devolucaoPagto":
                        DevolucaoPagtoDAO.Instance.Cancelar(id, txtMotivo.Text, ctrlDataEstorno.Data);
                        break;
    
                    case "movBanco":
                        MovBancoDAO.Instance.Cancelar(id, txtMotivo.Text, true);
                        break;
    
                    case "credFornec" :
                        CreditoFornecedorDAO.Instance.Cancela(id, txtMotivo.Text);
                        break;
                }
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg(null, ex, Page);
                return;
            }
    
            ClientScript.RegisterClientScriptBlock(this.GetType(), "ok", "window.opener.atualizarPagina(); closeWindow();", true);
        }
    
        protected bool ExibirEstornoBanco()
        {    
            try
            {
                uint id = Glass.Conversoes.StrParaUint(Request["id"]);
                string nomeCampo = "";
    
                switch (Request["tipo"])
                {
                    case "contaR":
                        nomeCampo = "idContaR";
                        break;
    
                    case "sinal":
                        nomeCampo = "idSinal";
                        break;
    
                    case "acerto":
                        nomeCampo = "idAcerto";
                        break;
    
                    case "acertoCheque":
                        nomeCampo = "idAcertoCheque";
                        break;
    
                    case "obra":
                        nomeCampo = "idObra";
                        break;
    
                    case "devolucaoPagto":
                        nomeCampo = "idDevolucaoPagto";
                        break;
                }
    
                return !String.IsNullOrEmpty(nomeCampo) ? MovBancoDAO.Instance.ExistsByCampo(nomeCampo, id) : false;
            }
            catch
            {
                return false;
            }
        }
    }
}
