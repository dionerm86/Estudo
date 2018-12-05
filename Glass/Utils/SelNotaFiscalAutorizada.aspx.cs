using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Utils
{
    public partial class SelNotaFiscalAutorizada : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Utils.SelNotaFiscalAutorizada));
        }

        protected void grdNf_RowCommand(object sender, GridViewCommandEventArgs e)
        {

        }

        protected void grdNf_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }

        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {

        }

        /// <summary>
        /// Verifica se a nota fiscal selecionada pode ser incluída no MDFe.
        /// </summary>
        /// <param name="idNfe">Identificador da nota a ser validada.</param>
        /// <returns>Resposta da validação com erro ou  ok.</returns>
        [Ajax.AjaxMethod]
        public string VerificarDisponibilidadeNfeCidadeDescargaMdfe(string idNfe)
        {
            var chaveAcesso = Data.DAL.NotaFiscalDAO.Instance.ObtemChaveAcesso(idNfe.StrParaUint());

            if (string.IsNullOrWhiteSpace(chaveAcesso) || chaveAcesso.Length != 44)
            {
                return "Erro|A chave de acesso precisa ter 44 caracteres.";
            }

            var numeroMdfeAssociadoNfe = Data.DAL.NFeCidadeDescargaMDFeDAO.Instance.ObterNumeroMdfeAssociadoNfe(null, chaveAcesso);

            if (!string.IsNullOrWhiteSpace(numeroMdfeAssociadoNfe))
            {
                return $"Erro|Nota fiscal já inclusa no MDFe {numeroMdfeAssociadoNfe}.";
            }

            return "ok|Nota válida";
        }
    }
}
