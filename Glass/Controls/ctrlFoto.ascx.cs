using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Helper;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlFoto : CtrlFoto
    {
        #region Construtor
    
        public ctrlFoto()
        {
            LarguraTabela = new Unit("0px");
            imgImagem = new ImageButton();
            lblLegenda = new Label();
            lnkEditar = new LinkButton();
            lnkRemover = new LinkButton();
            lnkCalc = new LinkButton();
        }
    
        #endregion
    
        #region Page Load
    
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Foto != null && !String.IsNullOrEmpty(Foto.Descricao))
            {
                lblLegenda.Text = Foto.Descricao;
                txtEditar.Text = Foto.Descricao;
            }
    
            #region Informações da foto
    
            if (lblLegenda.Text != String.Empty)
                lblLegenda.Text += ". ";
    
            if (Foto != null)
            {
                if (!String.IsNullOrEmpty(Foto.CodInterno))
                    lblLegenda.Text += "Produto: " + Foto.CodInterno + " ";
    
                if (Foto.AreaQuadrada > 0)
                    lblLegenda.Text += "Área: " + Foto.AreaQuadrada + "m² Metro Linear: " + Foto.MetroLinear + "m ";
            }
    
            #endregion
    
            LoginUsuario login = UserInfo.GetUserInfo;
    
            txtEditar.Width = new Unit(LarguraTabela.Value - 47);
            imgImagem.ImageUrl = "../Handlers/LoadImage.ashx?path=" + Path + "&altura=100&largura=100";
            imgImagem.ToolTip = Arquivos.IsImagem(GetExtensao()) ? "Clique na imagem para vê-la em tamanho real" : "Fazer download do arquivo";
            imgImagem.OnClientClick = Arquivos.IsImagem(GetExtensao()) ? "openWindow(500, 700, '../Utils/ShowFoto.aspx?path=" + VirtualPath + "'); return false" :
                "redirectUrl('../Handlers/Download.ashx?filePath=" + Foto.FilePath.Replace("\\", "\\\\") + "&fileName=" + Foto.Tipo.ToString() + 
                Foto.Extensao + "'); return false";
    
            lnkEditar.Visible = Foto != null && EditVisible;
    
            lnkRemover.Visible = lnkEditar.Visible;
    
            lnkCalc.Visible = (Foto != null && Foto.PermiteCalcularArea) && (login.TipoUsuario == (uint)Data.Helper.Utils.TipoFuncionario.Administrador);
    
            if (Request["crud"] == "0")
            {
                lnkEditar.Visible = false;
                lnkRemover.Visible = false;
                lnkCalc.Visible = false;
            }
        }
    
        #endregion
    
        #region Excluir
    
        protected void lnkRemover_Click(object sender, EventArgs e)
        {
            try
            {
                Foto.Delete();
                Response.Redirect(Context.Request.Url.ToString());
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao excluir foto.", ex, Page);
            }
        }
    
        #endregion
    
        #region Editar
    
        protected void imgEditar_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                Foto.Descricao = txtEditar.Text;
                Foto.Update();
                Response.Redirect(Context.Request.Url.ToString());
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao editar foto.", ex, Page);
            }
        }
    
        #endregion
    
        protected void lnkCalc_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Utils/MedFoto.aspx?IdMedicao=" + Request["idMedicao"] + "&idFoto=" + Foto.IdFoto);
        }
    }
}
