using Glass.Data.DAL;
using System;
using System.Web.UI;

namespace Glass.UI.Web.Utils
{
    public partial class AvaliacaoAtendimento : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void grdAvaliacaoAtendimento_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Aprovar")
            {
                try
                {
                    AvaliacaoAtendimentoDAO.Instance.AvaliaAtendimento(uint.Parse(e.CommandArgument.ToString()), true);

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
                    AvaliacaoAtendimentoDAO.Instance.AvaliaAtendimento(uint.Parse(e.CommandArgument.ToString()), false);

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
    }
}
