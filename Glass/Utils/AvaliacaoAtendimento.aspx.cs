using Glass.Data.DAL;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Utils
{
    public partial class AvaliacaoAtendimento : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(AvaliacaoAtendimento));
        }

        protected void grdAvaliacaoAtendimento_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Aprovar")
            {
                try
                {
                    if (!AvaliacaoAtendimentoDAO.Instance.PossuiAvaliacaoPendente())
                        ClientScript.RegisterClientScriptBlock(typeof(AvaliacaoAtendimento), "ok", "alert('Agradecemos seu retorno!'); window.close()", true);
                    else
                        grdAvaliacaoAtendimento.DataBind();
                }
                catch (Exception ex)
                {
                    MensagemAlerta.ErrorMsg("Falha ao aprovar atendimento.", ex, Page);
                }
            }
            else if (e.CommandName == "Negar")
            {
                try
                {
                    if (!AvaliacaoAtendimentoDAO.Instance.PossuiAvaliacaoPendente())
                        ClientScript.RegisterClientScriptBlock(typeof(AvaliacaoAtendimento), "ok", "alert('Agradecemos seu retorno, entraremos em contato em breve!'); window.close()", true);
                    else
                        grdAvaliacaoAtendimento.DataBind();
                }
                catch (Exception ex)
                {
                    MensagemAlerta.ErrorMsg("Falha ao negar resolução do atendimento.", ex, Page);
                }
            }
        }

        [Ajax.AjaxMethod]
        public string AvaliaAtendimentoAjax(string idAvaliacaoAtendimentoString, string satisfacaoString, string obs, string aprovadoString)
        {
            try
            {
                var idAvaliacaoAtendimento = Glass.Conversoes.StrParaUint(idAvaliacaoAtendimentoString);
                var satisfacao = Glass.Data.Helper.DataSources.GetSatisfacaoAvaliacaoAtendimento(Glass.Conversoes.StrParaUint(satisfacaoString));
                var aprovado = aprovadoString == "true" ? true : false;

                AvaliacaoAtendimentoDAO.Instance.AvaliaAtendimento(idAvaliacaoAtendimento, satisfacao, obs, aprovado);

                return "";
            }
            catch (Exception ex)
            {
                return "Falha ao avaliar resolução do atendimento. " + ex.Message;
            }
        }
    }
}
