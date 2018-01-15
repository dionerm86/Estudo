using System;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.UI.Web.Cadastros.Projeto
{
    public partial class CadGrupoModelo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            try
            {
                GrupoModelo grupo = new GrupoModelo();
                grupo.Descricao = ((TextBox)grdGrupoModelo.FooterRow.FindControl("txtDescricao")).Text;
                grupo.Situacao = Glass.Conversoes.StrParaInt(((DropDownList)grdGrupoModelo.FooterRow.FindControl("drpSituacao")).SelectedValue);
                grupo.BoxPadrao = ((CheckBox)grdGrupoModelo.FooterRow.FindControl("chkBoxPadrao")).Checked;
                grupo.Esquadria = ((CheckBox)grdGrupoModelo.FooterRow.FindControl("chkEsquadria")).Checked;

                GrupoModeloDAO.Instance.Insert(grupo);
    
                grdGrupoModelo.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir grupo.", ex, Page);
            }
        }
    
        protected void odsGrupoModelo_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao excluir grupo.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
    
        protected void odsGrupoModelo_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar grupo.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
    }
}
