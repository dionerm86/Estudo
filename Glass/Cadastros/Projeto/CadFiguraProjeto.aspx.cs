using System;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Glass.Data.DAL;
using System.IO;

namespace Glass.UI.Web.Cadastros.Projeto
{
    public partial class CadFiguraProjeto : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            try
            {
                if (drpGrupoFigura.SelectedValue == "0")
                {
                    Glass.MensagemAlerta.ShowMsg("Selecione o Grupo de Figura.", Page);
                    return;
                }
    
                FileUpload fluFiguraProjeto = (FileUpload)grdFiguraProjeto.FooterRow.FindControl("fluFigura");
    
                if (!fluFiguraProjeto.HasFile)
                {
                    Glass.MensagemAlerta.ShowMsg("Selecione a figura.", Page);
                    return;
                }
    
                FiguraProjeto figuraProjeto = new FiguraProjeto();
                figuraProjeto.IdGrupoFigProj = Glass.Conversoes.StrParaUint(drpGrupoFigura.SelectedValue);
                figuraProjeto.CodInterno = ((TextBox)grdFiguraProjeto.FooterRow.FindControl("txtCodInterno")).Text;
                figuraProjeto.Descricao = ((TextBox)grdFiguraProjeto.FooterRow.FindControl("txtDescricao")).Text;
                figuraProjeto.Situacao = Glass.Conversoes.StrParaInt(((DropDownList)grdFiguraProjeto.FooterRow.FindControl("drpSituacao")).SelectedValue);
    
                figuraProjeto.IdFiguraProjeto = FiguraProjetoDAO.Instance.Insert(figuraProjeto);
    
                #region Figura Projeto
    
                string extensao = fluFiguraProjeto.PostedFile.FileName.Substring(fluFiguraProjeto.PostedFile.FileName.LastIndexOf(".")).ToLower();
    
                if (fluFiguraProjeto.HasFile && figuraProjeto.IdFiguraProjeto > 0)
                {
                    if (extensao != ".jpg")
                    {
                        Glass.MensagemAlerta.ShowMsg("Apenas imagens jpg são aceitas.", Page);
                        FiguraProjetoDAO.Instance.Delete(figuraProjeto);
                        return;
                    }
    
                    string fotoPath = Data.Helper.Utils.GetFigurasProjetoPath + figuraProjeto.IdFiguraProjeto + extensao;
    
                    if (File.Exists(fotoPath))
                        File.Delete(fotoPath);
    
                    fluFiguraProjeto.SaveAs(fotoPath);
                }
    
                #endregion
    
                grdFiguraProjeto.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir Figura de Projeto.", ex, Page);
            }
        }
    
        protected void lblDescrGrupo_Load(object sender, EventArgs e)
        {
            if (drpGrupoFigura.SelectedItem != null && drpGrupoFigura.SelectedItem.Text.ToLower() != "todos")
                ((Label)sender).Text = drpGrupoFigura.SelectedItem.Text;
        }
    
        protected void odsFiguraProjeto_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir Figura de Projeto.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
                grdFiguraProjeto.DataBind();
        }
    
        protected void odsFiguraProjeto_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao excluir Figura de Projeto.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
                grdFiguraProjeto.DataBind();
        }
    }
}
