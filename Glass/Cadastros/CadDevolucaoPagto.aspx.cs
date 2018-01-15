using System;
using System.Web.UI;
using Glass.Data.DAL;
using Glass.Data.Helper;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadDevolucaoPagto : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            if (ctrlFormaPagto1.DataRecebimento == null)
                ctrlFormaPagto1.DataRecebimento = DateTime.Now;
    
            if (!IsPostBack)
                ctrlFormaPagto1.UsarCreditoMarcado = Configuracoes.FinanceiroConfig.OpcaoUsarCreditoMarcadaDevolucaoPagto;
        }
    
        protected void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                if (ctrlFormaPagto1.IdCliente == 0)
                    throw new Exception("Informe o cliente.");

                if (ctrlFormaPagto1.DataRecebimento == null)
                    throw new Exception("Informe a data de recebimento.");

                uint idDevolucaoPagto = DevolucaoPagtoDAO.Instance.Devolver(ctrlFormaPagto1.IdCliente,
                    ctrlFormaPagto1.DataRecebimento.Value, ctrlFormaPagto1.Valores,
                    ctrlFormaPagto1.FormasPagto, ctrlFormaPagto1.ContasBanco, ctrlFormaPagto1.DepositosNaoIdentificados,
                    ctrlFormaPagto1.TiposCartao, ctrlFormaPagto1.ParcelasCartao,
                    ctrlFormaPagto1.TiposBoleto, ctrlFormaPagto1.TaxasAntecipacao, ctrlFormaPagto1.ChequesString,
                    ctrlFormaPagto1.NumAutConstrucard,
                    ctrlFormaPagto1.CreditoUtilizado, txtObs.Text, Request["caixaDiario"] == "true");

                Page.ClientScript.RegisterClientScriptBlock(GetType(), "ok",
                    "alert('Devolução feita com sucesso! Código: " + idDevolucaoPagto +
                    "'); redirectUrl('../Listas/LstDevolucaoPagto.aspx" +
                    (Request["caixaDiario"] == "true" ? "?caixaDiario=true" : "") + "')", true);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("", ex, Page);
            }
        }
    
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Listas/LstDevolucaoPagto.aspx");
        }
    }
}
