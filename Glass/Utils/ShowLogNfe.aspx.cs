using System;
using System.Web.UI;
using Glass.Data.DAL;
using Glass.Data.Model;

namespace Glass.UI.Web.Utils
{
    public partial class ShowLogNfe : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            uint idNf = Glass.Conversoes.StrParaUint(Request["idNf"]);
            int situacao = NotaFiscalDAO.Instance.ObtemSituacao(idNf);
            var numeroNfe = NotaFiscalDAO.Instance.ObtemNumerosNFePeloIdNf(idNf.ToString());

            /* Chamado 14827.
             * Caso a empresa não trabalhe com separação de valores ficais, o log de alterações de separação deve ser escondido. */
            if (!Configuracoes.FinanceiroConfig.SepararValoresFiscaisEReaisContasReceber &&
                !Configuracoes.FinanceiroConfig.SepararValoresFiscaisEReaisContasPagar)
            {
                grdLogSeparacaoValores.Visible = false;
                lblTituloSeparacaoValores.Visible = false;
            }
            else if (!IsPostBack)
                lblTituloSeparacaoValores.Text = "Log separação de valores da NF-e n.º " + numeroNfe;

            Page.Title += numeroNfe;
    
            if (situacao == (int)NotaFiscal.SituacaoEnum.Cancelada)
            {
                lblTituloMotivo.Text = "Motivo do Cancelamento:";
                lblTextoMotivo.Text = NotaFiscalDAO.Instance.ObtemMotivoCanc(idNf);
            }
            else if (situacao == (int)NotaFiscal.SituacaoEnum.Inutilizada)
            {
                lblTituloMotivo.Text = "Motivo da Inutilização:";
                lblTextoMotivo.Text = NotaFiscalDAO.Instance.ObtemMotivoInut(idNf);
            }
        }
    }
}
