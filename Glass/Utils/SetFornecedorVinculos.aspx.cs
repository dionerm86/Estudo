using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using System.Linq;

namespace Glass.UI.Web.Utils
{
    public partial class SetFornecedorVinculos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Title = "Fornecedor: " + FornecedorDAO.Instance.GetNome(Request["idFornec"].StrParaUint());
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdVinculados.PageIndex = 0;
        }
    
        protected void grdCli_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "CriarVinculo")
            {
                try
                {
                    if(FornecedorDAO.Instance.GetVinculados(Request["idFornec"].StrParaInt())
                        .Any(f=> f.IdFornec == e.CommandArgument.ToString().StrParaInt()))
                    {
                        MensagemAlerta.ShowMsg("Fornecedor já vinculado.", Page);
                        return;
                    }
                        
                    FornecedorVinculoDAO.Instance.CriarVinculo(Request["idFornec"].StrParaInt(), e.CommandArgument.ToString().StrParaInt());
    
                    grdVinculados.DataBind();
                }
                catch (Exception ex)
                {
                    MensagemAlerta.ErrorMsg("Falha ao vincular fornecedor.", ex, Page);
                }
            }
        }
    
        protected void grdVinculados_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "RemoverVinculo")
            {
                try
                {
                    FornecedorVinculoDAO.Instance.RemoverVinculo(Request["idFornec"].StrParaInt(), e.CommandArgument.ToString().StrParaInt());
    
                    grdVinculados.DataBind();
                }
                catch (Exception ex)
                {
                    MensagemAlerta.ErrorMsg("Falha ao remover vínculo.", ex, Page);
                }
            }
        }
    }
}
