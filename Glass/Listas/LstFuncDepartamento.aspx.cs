using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Listas
{
    public partial class LstFuncDepartamento : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdFuncDepartamento.RowCommand += grdFuncDepartamento_RowCommand;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        private void grdFuncDepartamento_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Delete2")
            {
                odsFuncDepartamento.DeleteParameters.Add("idFunc", e.CommandArgument.ToString());
                try
                {
                    odsFuncDepartamento.Delete();
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao apagar o funcionário para o departamento.", ex, this);
                    return;
                }
                
                grdFuncDepartamento.DataBind();
                drpFunc.DataBind();
            }
        }
    
        protected void imgAdd_Click(object sender, ImageClickEventArgs e)
        {
            var idDepartamento = Request["idDepartamento"].StrParaInt();
            var idFunc = drpFunc.SelectedValue.StrParaInt();

            var funcionarioFluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<Glass.Global.Negocios.IFuncionarioFluxo>();

            var resultado = funcionarioFluxo.AdicionarFuncionarioDepartamento(idFunc, idDepartamento);

            if (!resultado)
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir o funcionário para o departamento.", resultado);
            else
            {
                grdFuncDepartamento.DataBind();
                drpFunc.DataBind();
            }
        }
    }
}
