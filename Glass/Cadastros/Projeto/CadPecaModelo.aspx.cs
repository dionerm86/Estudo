using Glass.Data.DAL;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros.Projeto
{
    public partial class CadPecaModelo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                if (ExibirFolgaPorEspessura())
                {
                    grdPecaProjMod.Columns[4].Visible = false;
                    grdPecaProjMod.Columns[5].Visible = false;
                }
                else
                {
                    grdPecaProjMod.Columns[6].Visible = false;
                    grdPecaProjMod.Columns[7].Visible = false;
                    grdPecaProjMod.Columns[8].Visible = false;
                    grdPecaProjMod.Columns[9].Visible = false;
                    grdPecaProjMod.Columns[10].Visible = false;
                    grdPecaProjMod.Columns[11].Visible = false;
                    grdPecaProjMod.Columns[12].Visible = false;
                    grdPecaProjMod.Columns[13].Visible = false;
                    grdPecaProjMod.Columns[14].Visible = false;
                    grdPecaProjMod.Columns[15].Visible = false;
                    grdPecaProjMod.Columns[16].Visible = false;
                    grdPecaProjMod.Columns[17].Visible = false;
                    grdPecaProjMod.Columns[18].Visible = false;
                    grdPecaProjMod.Columns[19].Visible = false;
                }
            
            if(IsPostBack && !string.IsNullOrEmpty(txtCodCliente.Text))
                chkInsercaoRapidaFolga.Style["display"] = "none";
        }

        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdPecaProjMod.PageIndex = 0;
        }

        protected bool ExibirFolgaPorEspessura()
        {
            return Configuracoes.ProjetoConfig.SelecionarEspessuraAoCalcularProjeto;
        }

        protected void chkInsercaoRapidaFolga_CheckedChanged(object sender, EventArgs e)
        {
            if (chkInsercaoRapidaFolga.Checked)
            {
                if (!ExibirFolgaPorEspessura())
                {
                    //Visualização
                    grdPecaProjMod.Columns[4].Visible = false;
                    grdPecaProjMod.Columns[5].Visible = false;
                    //Inserção Rapida
                    grdPecaProjMod.Columns[20].Visible = true;
                    grdPecaProjMod.Columns[21].Visible = true;
                }
                else
                {
                    //Visualização
                    grdPecaProjMod.Columns[6].Visible = false;
                    grdPecaProjMod.Columns[7].Visible = false;
                    grdPecaProjMod.Columns[8].Visible = false;
                    grdPecaProjMod.Columns[9].Visible = false;
                    grdPecaProjMod.Columns[10].Visible = false;
                    grdPecaProjMod.Columns[11].Visible = false;
                    grdPecaProjMod.Columns[12].Visible = false;
                    grdPecaProjMod.Columns[13].Visible = false;
                    grdPecaProjMod.Columns[14].Visible = false;
                    grdPecaProjMod.Columns[15].Visible = false;
                    grdPecaProjMod.Columns[16].Visible = false;
                    grdPecaProjMod.Columns[17].Visible = false;
                    grdPecaProjMod.Columns[18].Visible = false;
                    grdPecaProjMod.Columns[19].Visible = false;
                    //Inserção Rapida
                    grdPecaProjMod.Columns[22].Visible = true;
                    grdPecaProjMod.Columns[23].Visible = true;
                    grdPecaProjMod.Columns[24].Visible = true;
                    grdPecaProjMod.Columns[25].Visible = true;
                    grdPecaProjMod.Columns[26].Visible = true;
                    grdPecaProjMod.Columns[27].Visible = true;
                    grdPecaProjMod.Columns[28].Visible = true;
                    grdPecaProjMod.Columns[29].Visible = true;
                    grdPecaProjMod.Columns[30].Visible = true;
                    grdPecaProjMod.Columns[31].Visible = true;
                    grdPecaProjMod.Columns[32].Visible = true;
                    grdPecaProjMod.Columns[33].Visible = true;
                    grdPecaProjMod.Columns[34].Visible = true;
                    grdPecaProjMod.Columns[35].Visible = true;
                }
                //Editar
                grdPecaProjMod.Columns[0].Visible = false;
                //Botão salvar
                btnSalvarInsercaoRapida.Visible = true;
            }
            else
            {
                if (!ExibirFolgaPorEspessura())
                {
                    //Visualização
                    grdPecaProjMod.Columns[4].Visible = true;
                    grdPecaProjMod.Columns[5].Visible = true;
                    //Inserção Rapida
                    grdPecaProjMod.Columns[20].Visible = false;
                    grdPecaProjMod.Columns[21].Visible = false;
                }
                else
                {
                    //Visualização
                    grdPecaProjMod.Columns[6].Visible = true;
                    grdPecaProjMod.Columns[7].Visible = true;
                    grdPecaProjMod.Columns[8].Visible = true;
                    grdPecaProjMod.Columns[9].Visible = true;
                    grdPecaProjMod.Columns[10].Visible = true;
                    grdPecaProjMod.Columns[11].Visible = true;
                    grdPecaProjMod.Columns[12].Visible = true;
                    grdPecaProjMod.Columns[13].Visible = true;
                    grdPecaProjMod.Columns[14].Visible = true;
                    grdPecaProjMod.Columns[15].Visible = true;
                    grdPecaProjMod.Columns[16].Visible = true;
                    grdPecaProjMod.Columns[17].Visible = true;
                    grdPecaProjMod.Columns[18].Visible = true;
                    grdPecaProjMod.Columns[19].Visible = true;
                    //Inserção Rapida
                    grdPecaProjMod.Columns[22].Visible = false;
                    grdPecaProjMod.Columns[23].Visible = false;
                    grdPecaProjMod.Columns[24].Visible = false;
                    grdPecaProjMod.Columns[25].Visible = false;
                    grdPecaProjMod.Columns[26].Visible = false;
                    grdPecaProjMod.Columns[27].Visible = false;
                    grdPecaProjMod.Columns[28].Visible = false;
                    grdPecaProjMod.Columns[29].Visible = false;
                    grdPecaProjMod.Columns[30].Visible = false;
                    grdPecaProjMod.Columns[31].Visible = false;
                    grdPecaProjMod.Columns[32].Visible = false;
                    grdPecaProjMod.Columns[33].Visible = false;
                    grdPecaProjMod.Columns[34].Visible = false;
                    grdPecaProjMod.Columns[35].Visible = false;
                }
                //Editar
                grdPecaProjMod.Columns[0].Visible = true;
                //Botão salvar
                btnSalvarInsercaoRapida.Visible = false;
            }
        }

        protected void btnSalvarInsercaoRapida_Click(object sender, EventArgs e)
        {
            try
            {
                Data.Model.PecaProjetoModelo pecaProjetoModelo;
                int largura;
                int altura;
                int largura03MM;
                int altura03MM;
                int largura04MM;
                int altura04MM;
                int largura05MM;
                int altura05MM;
                int largura06MM;
                int altura06MM;
                int largura08MM;
                int altura08MM;
                int largura10MM;
                int altura10MM;
                int largura12MM;
                int altura12MM;

                if (grdPecaProjMod.Rows.Count < 1)
                {
                    Glass.MensagemAlerta.ShowMsg("Não há registros para inserção rápida", Page);
                    return;
                }

                foreach(GridViewRow row in grdPecaProjMod.Rows)
                {
                    uint idPecaProjetomodelo = ((HiddenField)row.Cells[38].FindControl("hdfIdPecaProjMod")).Value.StrParaUint();
                    pecaProjetoModelo = PecaProjetoModeloDAO.Instance.ObtemPeloId(idPecaProjetomodelo);

                    if (!ExibirFolgaPorEspessura())
                    {
                        largura = ((TextBox)row.Cells[20].FindControl("txtLarguraInsercaoRapida")).Text.StrParaInt();
                        altura = ((TextBox)row.Cells[21].FindControl("txtAlturaInsercaoRapida")).Text.StrParaInt();

                        if (pecaProjetoModelo.Largura == largura && pecaProjetoModelo.Altura == altura)
                            continue;

                        pecaProjetoModelo.Largura = largura;
                        pecaProjetoModelo.Altura = altura;
                    }
                    else
                    {
                        largura03MM = ((TextBox)row.Cells[22].FindControl("txtLargura03MMInsercaoRapida")).Text.StrParaInt();
                        largura04MM = ((TextBox)row.Cells[24].FindControl("txtLargura04MMInsercaoRapida")).Text.StrParaInt();
                        largura05MM = ((TextBox)row.Cells[26].FindControl("txtLargura05MMInsercaoRapida")).Text.StrParaInt();
                        largura06MM = ((TextBox)row.Cells[28].FindControl("txtLargura06MMInsercaoRapida")).Text.StrParaInt();
                        largura08MM = ((TextBox)row.Cells[30].FindControl("txtLargura08MMInsercaoRapida")).Text.StrParaInt();
                        largura10MM = ((TextBox)row.Cells[32].FindControl("txtLargura10MMInsercaoRapida")).Text.StrParaInt();
                        largura12MM = ((TextBox)row.Cells[34].FindControl("txtLargura12MMInsercaoRapida")).Text.StrParaInt();

                        altura03MM = ((TextBox)row.Cells[23].FindControl("txtAltura03MMInsercaoRapida")).Text.StrParaInt();
                        altura04MM = ((TextBox)row.Cells[25].FindControl("txtAltura04MMInsercaoRapida")).Text.StrParaInt();
                        altura05MM = ((TextBox)row.Cells[27].FindControl("txtAltura05MMInsercaoRapida")).Text.StrParaInt();
                        altura06MM = ((TextBox)row.Cells[29].FindControl("txtAltura06MMInsercaoRapida")).Text.StrParaInt();
                        altura08MM = ((TextBox)row.Cells[31].FindControl("txtAltura08MMInsercaoRapida")).Text.StrParaInt();
                        altura10MM = ((TextBox)row.Cells[33].FindControl("txtAltura10MMInsercaoRapida")).Text.StrParaInt();
                        altura12MM = ((TextBox)row.Cells[35].FindControl("txtAltura12MMInsercaoRapida")).Text.StrParaInt();

                        if (pecaProjetoModelo.Largura03MM == largura03MM && pecaProjetoModelo.Altura03MM == altura03MM &&
                            pecaProjetoModelo.Largura04MM == largura04MM && pecaProjetoModelo.Altura04MM == altura04MM &&
                            pecaProjetoModelo.Largura05MM == largura05MM && pecaProjetoModelo.Altura05MM == altura05MM &&
                            pecaProjetoModelo.Largura06MM == largura06MM && pecaProjetoModelo.Altura06MM == altura06MM &&
                            pecaProjetoModelo.Largura08MM == largura08MM && pecaProjetoModelo.Altura08MM == altura08MM &&
                            pecaProjetoModelo.Largura10MM == largura10MM && pecaProjetoModelo.Altura10MM == altura10MM &&
                            pecaProjetoModelo.Largura12MM == largura12MM && pecaProjetoModelo.Altura12MM == altura12MM)
                            continue;
                        
                        pecaProjetoModelo.Largura03MM = largura03MM;
                        pecaProjetoModelo.Largura04MM = largura04MM;
                        pecaProjetoModelo.Largura05MM = largura05MM;
                        pecaProjetoModelo.Largura06MM = largura06MM;
                        pecaProjetoModelo.Largura08MM = largura08MM;
                        pecaProjetoModelo.Largura10MM = largura10MM;
                        pecaProjetoModelo.Largura12MM = largura12MM;

                        pecaProjetoModelo.Altura03MM = altura03MM;
                        pecaProjetoModelo.Altura04MM = altura04MM;
                        pecaProjetoModelo.Altura05MM = altura05MM;
                        pecaProjetoModelo.Altura06MM = altura06MM;
                        pecaProjetoModelo.Altura08MM = altura08MM;
                        pecaProjetoModelo.Altura10MM = altura10MM;
                        pecaProjetoModelo.Altura12MM = altura12MM;
                    }

                    PecaProjetoModeloDAO.Instance.Update(pecaProjetoModelo);
                }

                Glass.MensagemAlerta.ShowMsg("Inserção rápida de folgas concluida", Page);
            }
            catch(Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Erro", ex, Page);
            }
            finally
            {
                chkInsercaoRapidaFolga.Checked = false;
                chkInsercaoRapidaFolga_CheckedChanged(null, null);
                grdPecaProjMod.DataBind();
            }
        }
    }
}
