using System;
using Glass.Data.DAL;
using GDA;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Utils
{
    public partial class SetMotivoCancCartaoNaoIdentificado : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            var idCartaoNaoIdentificado = Conversoes.StrParaUint(Request["IdCartaoNaoIdentificado"]);

            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();
                    CartaoNaoIdentificadoDAO.Instance.Cancelar(transaction, new[] { idCartaoNaoIdentificado }, txtMotivo.Text);
                    transaction.Commit();
                    transaction.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();
                    MensagemAlerta.ErrorMsg("Erro ao cancelar Arquivo.", ex, this);
                }
            }

            MensagemAlerta.ShowMsg("Arquivo cancelado.", this);

            ClientScript.RegisterClientScriptBlock(this.GetType(), "ok", "window.opener.redirectUrl(window.opener.location.href);closeWindow();", true);
        }

    }
}
