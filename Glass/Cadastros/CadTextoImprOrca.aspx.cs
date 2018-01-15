using System;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadTextoImprOrca : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void btnInserir_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtDescricao.Text))
            {
                Glass.MensagemAlerta.ShowMsg("Informe a descrição.", Page);
                return;
            }
    
            try
            {
                TextoImprOrca textoOrca = new TextoImprOrca();
                textoOrca.Titulo = txtTitulo.Text;
                textoOrca.Descricao = txtDescricao.Text;
                textoOrca.BuscarSempre = chkBuscarSempre.Checked;
                TextoImprOrcaDAO.Instance.Insert(textoOrca);
    
                grdTextoImprOrca.DataBind();
    
                txtTitulo.Text = String.Empty;
                txtDescricao.Text = String.Empty;
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir texto de orçamento.", ex, Page);
            }
        }
    
        protected void odsTextoImprOrca_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao excluir texto de Orçamento.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
    
        protected void odsTextoImprOrca_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar texto de Orçamento.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
    }
}
