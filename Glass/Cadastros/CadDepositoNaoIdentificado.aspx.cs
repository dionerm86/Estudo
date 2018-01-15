using System;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Model;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadDepositoNaoIdentificado : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                uint idDeposito = Glass.Conversoes.StrParaUint(Request["idDepositoNaoIdentificado"]);
                if (idDeposito > 0)
                {
                    if (DepositoNaoIdentificadoDAO.Instance.ObtemSituacao(idDeposito)
                        != DepositoNaoIdentificado.SituacaoEnum.Ativo)
                        Response.Redirect("~/Listas/LstDepositoNaoIdentificado.aspx");
    
                    dtvDepositoNaoIdentificado.ChangeMode(DetailsViewMode.Edit);
                }
            }
        }
    
        protected void odsDepositoNaoIdentificado_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao cadastrar deposito não identificado.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
                Response.Redirect("../Listas/LstDepositoNaoIdentificado.aspx");
        }
    
        protected void odsDepositoNaoIdentificado_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar deposito não identificado.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
                Response.Redirect("../Listas/LstDepositoNaoIdentificado.aspx");
        }
    
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Listas/LstDepositoNaoIdentificado.aspx");
        }
    }
}
