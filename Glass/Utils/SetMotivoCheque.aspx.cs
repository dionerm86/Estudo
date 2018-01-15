using System;
using System.Web.UI;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.UI.Web.Utils
{
    public partial class SetMotivoCheque : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            uint idCheque = Glass.Conversoes.StrParaUint(Request["idCheque"]);
            int tipo = Glass.Conversoes.StrParaInt(Request["tipo"]);
    
            if (!IsPostBack)
                ctrlDataEstorno.Data = DateTime.Now;
    
            dataEstorno.Visible = tipo == 1;
    
            if (tipo == 1)
                Page.Title = "Motivo da Devolução";
            else if (tipo == 2)
                Page.Title = "Motivo do Protesto";
            else
                ClientScript.RegisterClientScriptBlock(typeof(string), "close", "closeWindow();", true);
        }
    
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            Cheques cheque = ChequesDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(Request["idCheque"]));
            int tipo = Glass.Conversoes.StrParaInt(Request["tipo"]);
    
            // Concatena a observação do cheque já existente com o motivo da devolução/protesto
            string motivo = Page.Title + ": " + txtMotivo.Text;
            motivo = !String.IsNullOrEmpty(cheque.Obs) ? cheque.Obs + ". " + motivo : motivo;
    
            // Se o tamanho do campo Obs exceder 500 caracteres, salva apenas os 500 primeiros, descartando o restante
            if (motivo.Length > 500) motivo = motivo.Substring(0, 500);
    
            try
            {
                // Cheque devolvido
                if (tipo == 1)
                {
                    if (!ChequesDAO.Instance.IsReapresentado(cheque.IdCheque))
                    {
                        if (cheque.Situacao == (int)Cheques.SituacaoCheque.Devolvido)
                        {
                            ClientScript.RegisterClientScriptBlock(this.GetType(), "ok", "alert('Este cheque já está marcado como devolvido.'); closeWindow();", true);
                            return;
                        }
    
                        DepositoChequeDAO.Instance.ChequeDevolvido(cheque.IdCheque, motivo, ctrlDataEstorno.Data);
                    }
                    else
                        ChequesDAO.Instance.DevolverChequeReapresentado(cheque.IdCheque, motivo, ctrlDataEstorno.Data);
                }            
                // Cheque protestado
                else if (tipo == 2)
                {
                    if (!ChequesDAO.Instance.IsReapresentado(cheque.IdCheque))
                    {
                        if (cheque.Situacao == (int)Cheques.SituacaoCheque.Protestado)
                        {
                            ClientScript.RegisterClientScriptBlock(this.GetType(), "ok", "alert('Este cheque já está marcado como protestado.'); closeWindow();", true);
                            return;
                        }
    
                        DepositoChequeDAO.Instance.ChequeProtestado(cheque.IdCheque, motivo);
                    }
                    else
                        ChequesDAO.Instance.ProtestarChequeReapresentado(cheque.IdCheque, motivo);
                }
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg(null, ex, Page);
                return;
            }
    
            string script = tipo == 1 ? "alert('Cheque marcado como Devolvido com sucesso');" : 
                tipo == 2 ? "alert('Cheque marcado como Protestado com sucesso');" : "";

            script += "window.opener.redirectUrl(window.opener.location.href.replace('#', '')); closeWindow();";
    
            ClientScript.RegisterClientScriptBlock(this.GetType(), "ok", script, true);
        }
    }
}
