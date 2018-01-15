using Glass.Data.DAL;
using Glass.Data.Model;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros.Projeto
{
    public partial class CadGrupoMedidaProjeto : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void imbInserir_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                GrupoMedidaProjeto gmp = new GrupoMedidaProjeto();
                gmp.Descricao = ((TextBox)grdGrupoMedidaProjeto.FooterRow.FindControl("txtDescricaoIn")).Text;
                GrupoMedidaProjetoDAO.Instance.Insert(gmp);

                grdGrupoMedidaProjeto.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir grupo de medida de projeto.", ex, Page);
            }
        }

        protected void odsGrupoMedidaProjeto_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar grupo de medida de projeto.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }

        protected void odsGrupoMedidaProjeto_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao excluir grupo de medida de projeto.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
    }
}