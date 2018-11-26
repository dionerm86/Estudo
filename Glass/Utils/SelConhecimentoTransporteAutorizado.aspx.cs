using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Utils
{
    public partial class SelConhecimentoTransporteAutorizado : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Utils.SelConhecimentoTransporteAutorizado));
        }

        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {

        }

        /// <summary>
        /// Verifica se o CTe selecionado pode ser incluído no MDFe.
        /// </summary>
        /// <param name="idcte">Identificador do conhecimento de transporte a ser validado.</param>
        /// <returns>Resposta da validação com erro ou  ok.</returns>
        [Ajax.AjaxMethod()]
        public string VerificarDisponibilidadeCTeCidadeDescargaMdfe(string idCte)
        {
            var chaveAcesso = Data.DAL.CTe.ConhecimentoTransporteDAO.Instance.ObtemChaveAcesso(idCte.StrParaUint());

            if (string.IsNullOrWhiteSpace(chaveAcesso) || chaveAcesso.Length != 44)
            {
                return "Erro|A chave de acesso precisa ter 44 caracteres.";
            }

            if (Data.DAL.CTeCidadeDescargaMDFeDAO.Instance.VerificarCteJaIncluso(chaveAcesso))
            {
                var numeroMdfe = Data.DAL.CTeCidadeDescargaMDFeDAO.Instance.ObterNumeroMdfeAssociadoCte(null, chaveAcesso);
                return $"Erro|Conhecimento de transporte já incluso no MDFe {numeroMdfe}.";
            }

            return "ok|CTe válido";
        }
    }
}
