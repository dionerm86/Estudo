using System;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Configuracoes;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlModeloProjeto : CtrlModeloProjeto
    {
        #region Construtor
    
        public ctrlModeloProjeto()
        {
            imgImagem = new System.Web.UI.WebControls.Image();
            lblDescricao = new Label();
            lnkCodigo = new LinkButton();
            hdfIdProjetoModelo = new HiddenField();
        }
    
        #endregion
    
        #region Page Load
    
        protected void Page_Load(object sender, EventArgs e)
        {
            drpCorVidro.Items.Clear();
            drpCorAluminio.Items.Clear();
            drpCorFerragem.Items.Clear();
    
            ListItem blank = new ListItem();
            drpCorVidro.Items.Add(blank);
    
            if ((ProjetoConfig.ControleModeloProjeto.ObrigarInformarCorAlumFerragem && Request["ApenasVidro"] != "true") || !ExibirCorAluminioFerragem)
            {   
                drpCorAluminio.Items.Add(blank);
                drpCorFerragem.Items.Add(blank);
            }
            
            lblDescricao.Text = Modelo.Descricao;
            lnkCodigo.Text = Modelo.Codigo;
    
            if (ProjetoConfig.ControleModeloProjeto.ApenasVidrosPadrao)
            {
                chkApenasVidro.Checked = true;
                drpEspessuraVidro.Attributes.Add("style", "display: block;");

                if (!ProjetoConfig.SelecionarEspessuraAoCalcularProjeto)
                {
                    drpEspessuraVidro.Visible = false;
                    lblEspessura.Visible = false;
                }
                else
                    lblEspessura.Attributes.Add("style", "display: block;");

                spnMedidaExata.Attributes.Add("style", "display: inline");
                
                if (ProjetoConfig.ControleModeloProjeto.MedidaExataPadrao)
                    chkMedidaExata.Checked = true;
            }
            else if (!ProjetoConfig.SelecionarEspessuraAoCalcularProjeto)
            {
                drpEspessuraVidro.Visible = false;
                lblEspessura.Visible = false;
            }
            else
            {
                drpEspessuraVidro.Attributes.Add("style", "display: none;");
                lblEspessura.Attributes.Add("style", "display: none;");
            }
            
            if (Modelo != null && Modelo.IdGrupoModelo > 0)
            {
                var idGrupoModelo = Modelo.IdGrupoModelo;
                var descricaoGrupoModelo = Data.DAL.GrupoModeloDAO.Instance.ObtemDescricao(idGrupoModelo);

                /* Chamado 48675. */
                if (FindControl("drpEspessuraVidro") != null)
                {
                    if (descricaoGrupoModelo.ToUpper().Contains("FIXO"))
                    {
                        if (((DropDownList)FindControl("drpEspessuraVidro")).Items.FindByValue("3") != null)
                            ((DropDownList)FindControl("drpEspessuraVidro")).Items.FindByValue("3").Enabled = true;
                        if (((DropDownList)FindControl("drpEspessuraVidro")).Items.FindByValue("4") != null)
                            ((DropDownList)FindControl("drpEspessuraVidro")).Items.FindByValue("4").Enabled = true;
                        if (((DropDownList)FindControl("drpEspessuraVidro")).Items.FindByValue("5") != null)
                            ((DropDownList)FindControl("drpEspessuraVidro")).Items.FindByValue("5").Enabled = true;
                    }
                    else
                    {
                        if (((DropDownList)FindControl("drpEspessuraVidro")).Items.FindByValue("3") != null)
                            ((DropDownList)FindControl("drpEspessuraVidro")).Items.FindByValue("3").Enabled = false;
                        if (((DropDownList)FindControl("drpEspessuraVidro")).Items.FindByValue("4") != null)
                            ((DropDownList)FindControl("drpEspessuraVidro")).Items.FindByValue("4").Enabled = false;
                        if (((DropDownList)FindControl("drpEspessuraVidro")).Items.FindByValue("5") != null)
                            ((DropDownList)FindControl("drpEspessuraVidro")).Items.FindByValue("5").Enabled = false;
                    }
                }
            }

            if (Request["ApenasVidro"] == "true")
            {
                chkApenasVidro.Checked = true;
                chkApenasVidro.Enabled = false;
                spnMedidaExata.Attributes.Add("style", "display: inline");
            }
            else
                chkApenasVidro.Attributes.Add("onclick", "document.getElementById('" + spnMedidaExata.ClientID + "').style.display=this.checked ? 'inline' : 'none'");
        
            hdfIdProjetoModelo.Value = Modelo.IdProjetoModelo.ToString();
            
            LoginUsuario login = UserInfo.GetUserInfo;
    
            int alturaImg = (int)(Modelo.AlturaFigura * 0.7);
            int larguraImg = (int)(Modelo.LarguraFigura * 0.7);
    
            imgImagem.ImageUrl = "~/Handlers/LoadImage.ashx" + "?path=" + Path + "&altura=" + alturaImg + "&largura=" + larguraImg;
    
            drpCorVidro.DataSource = Vidros;
            drpCorVidro.DataBind();
    
            drpCorAluminio.DataSource = Aluminios;
            drpCorAluminio.Visible = ExibirCorAluminioFerragem;
            lblAluminio.Visible = ExibirCorAluminioFerragem;
            drpCorAluminio.DataBind();
            
            drpCorFerragem.DataSource = Ferragens;
            drpCorFerragem.Visible = ExibirCorAluminioFerragem;
            lblFerragem.Visible = ExibirCorAluminioFerragem;
            drpCorFerragem.DataBind();
        }
    
        #endregion
    }
}
