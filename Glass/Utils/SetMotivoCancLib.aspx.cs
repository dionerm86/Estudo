using System;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.UI.Web.Utils
{
    public partial class SetMotivoCancLib : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                uint idLiberarPedido = Glass.Conversoes.StrParaUint(Request["IdLiberarPedido"]);
    
                ctrlDataEstorno.Data = DateTime.Now;
                estornoBanco.Visible = false;
    
                string aviso = "ATENÇÃO: Alguns cheques desta liberação foram utilizados em depósitos/pagamentos, cancelando a liberação os seguintes cheques serão marcados como cancelados: \r\n";
                string chequesDepositados = "";
                string chequesPagos = "";
    
                // Verifica se os cheques desta liberação foram utilizados em algum depósito/pagto
                foreach (Cheques c in ChequesDAO.Instance.GetByLiberacaoPedido(null, idLiberarPedido))
                {
                    if (c.IdDeposito > 0)
                        chequesDepositados = "Num. Cheque: " + c.Num + " Valor: " + c.Valor.ToString("C") + " Depósito: " + c.IdDeposito + "\r\n";
    
                    uint idPagto = PagtoChequeDAO.Instance.GetPagtoByCheque(c.IdCheque);
                    if (idPagto > 0)
                        chequesPagos = "Num. Cheque: " + c.Num + " Valor: " + " Pagamento: " + idPagto + "\r\n";
                }
    
                if (!String.IsNullOrEmpty(chequesDepositados) || !String.IsNullOrEmpty(chequesPagos))
                    lblAviso.Text = aviso + chequesDepositados + chequesPagos;
            }
        }
    
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            // Concatena a observação do pedido já existente com o motivo do cancelamento
            string motivo = "Motivo do Cancelamento: " + txtMotivo.Text;
    
            // Se o tamanho do campo ObsCanc exceder 300 caracteres, salva apenas os 300 primeiros, descartando o restante
            motivo = motivo.Length > 300 ? motivo.Substring(0, 300) : motivo;

            try
            {
                FilaOperacoes.CancelarLiberacao.AguardarVez();
                LiberarPedidoDAO.Instance.CancelarLiberacao(Request["IdLiberarPedido"].StrParaUint(),
                    motivo, ctrlDataEstorno.Data);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg(null, ex, Page);
                return;
            }
            finally
            {
                FilaOperacoes.CancelarLiberacao.ProximoFila();
            }
    
            ClientScript.RegisterClientScriptBlock(this.GetType(), "ok", "window.opener.redirectUrl(window.opener.location.href);closeWindow();", true);
        }
    }
}
