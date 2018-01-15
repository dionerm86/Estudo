using Microsoft.Practices.ServiceLocation;
using System;
using System.Web.UI;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadVincularPlanoConta : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void imgAdd_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                var idConta = hdfPlanoConta.Value.StrParaInt();

                var fluxo = ServiceLocator.Current.GetInstance<Financeiro.Negocios.IPlanoContasFluxo>();

                var conta = fluxo
                    .ObtemPlanoContas(idConta);

                if (conta == null)
                    throw new Exception("O plano de contas informado não foi encontrado.");

                conta.IdContaContabil = Request["idContaContabil"].StrParaIntNullable();

                var result = fluxo.SalvarPlanoContas(conta);

                if (!result)
                    MensagemAlerta.ErrorMsg("Falha ao vincular plano de contas. ", result);

                grdPlanoConta.DataBind();
            }
            catch (Exception ex)
            {
                MensagemAlerta.ErrorMsg("Falha ao vincular plano de contas", ex, Page);
            }
        }

        protected void grdPlanoConta_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if(e.CommandName == "Desvincular")
            {
                try
                {
                    var idConta = e.CommandArgument.ToString().StrParaInt();

                    var fluxo = ServiceLocator.Current.GetInstance<Financeiro.Negocios.IPlanoContasFluxo>();

                    var conta = fluxo
                        .ObtemPlanoContas(idConta);

                    if (conta == null)
                        throw new Exception("O plano de contas informado não foi encontrado.");

                    conta.IdContaContabil = null;

                    var result = fluxo.SalvarPlanoContas(conta);

                    if (!result)
                        MensagemAlerta.ErrorMsg("Falha ao desvincular plano de contas. ", result);

                    grdPlanoConta.DataBind();
                }
                catch (Exception ex)
                {
                    MensagemAlerta.ErrorMsg("Falha ao desvincular plano de contas", ex, Page);
                }
            }
        }
    }
}