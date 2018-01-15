using System;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.UI.Web.Cadastros.Projeto
{
    public partial class CadGrupoFiguraProjeto : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            try
            {
                GrupoFiguraProjeto grupo = new GrupoFiguraProjeto();
                grupo.Descricao = ((TextBox)grdGrupoFiguraProjeto.FooterRow.FindControl("txtDescricao")).Text;
                grupo.Situacao = Glass.Conversoes.StrParaInt(((DropDownList)grdGrupoFiguraProjeto.FooterRow.FindControl("drpSituacao")).SelectedValue);
    
                GrupoFiguraProjetoDAO.Instance.Insert(grupo);
    
                grdGrupoFiguraProjeto.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir grupo.", ex, Page);
            }
        }
    
        protected void odsGrupoFiguraProjeto_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao excluir grupo.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
    
        protected void odsGrupoFiguraProjeto_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar grupo.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
    }
}
