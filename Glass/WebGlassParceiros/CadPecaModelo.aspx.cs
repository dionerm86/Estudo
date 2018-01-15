using Glass.Data.DAL;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.WebGlassParceiros
{
    public partial class CadPecaModelo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
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
                }

                if (!ExibirFolgaPorEspessura())
                {
                    //Visualização
                    grdPecaProjMod.Columns[4].Visible = false;
                    grdPecaProjMod.Columns[5].Visible = false;
                    //Inserção Rapida
                    grdPecaProjMod.Columns[14].Visible = true;
                    grdPecaProjMod.Columns[15].Visible = true;
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
                    //Inserção Rapida
                    grdPecaProjMod.Columns[16].Visible = true;
                    grdPecaProjMod.Columns[17].Visible = true;
                    grdPecaProjMod.Columns[18].Visible = true;
                    grdPecaProjMod.Columns[19].Visible = true;
                    grdPecaProjMod.Columns[20].Visible = true;
                    grdPecaProjMod.Columns[21].Visible = true;
                    grdPecaProjMod.Columns[22].Visible = true;
                    grdPecaProjMod.Columns[23].Visible = true;
                }
                //Editar
                grdPecaProjMod.Columns[0].Visible = false;
                //Botão salvar
                btnSalvarInsercaoRapida.Visible = true;
            }
        }

        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdPecaProjMod.PageIndex = 0;
        }

        protected bool ExibirFolgaPorEspessura()
        {
            return Configuracoes.ProjetoConfig.SelecionarEspessuraAoCalcularProjeto;
        }

        //protected void chkInsercaoRapidaFolga_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (chkInsercaoRapidaFolga.Checked)
        //    {
        //        if (!ExibirFolgaPorEspessura())
        //        {
        //            //Visualização
        //            grdPecaProjMod.Columns[4].Visible = false;
        //            grdPecaProjMod.Columns[5].Visible = false;
        //            //Inserção Rapida
        //            grdPecaProjMod.Columns[14].Visible = true;
        //            grdPecaProjMod.Columns[15].Visible = true;
        //        }
        //        else
        //        {
        //            //Visualização
        //            grdPecaProjMod.Columns[6].Visible = false;
        //            grdPecaProjMod.Columns[7].Visible = false;
        //            grdPecaProjMod.Columns[8].Visible = false;
        //            grdPecaProjMod.Columns[9].Visible = false;
        //            grdPecaProjMod.Columns[10].Visible = false;
        //            grdPecaProjMod.Columns[11].Visible = false;
        //            grdPecaProjMod.Columns[12].Visible = false;
        //            grdPecaProjMod.Columns[13].Visible = false;
        //            //Inserção Rapida
        //            grdPecaProjMod.Columns[16].Visible = true;
        //            grdPecaProjMod.Columns[17].Visible = true;
        //            grdPecaProjMod.Columns[18].Visible = true;
        //            grdPecaProjMod.Columns[19].Visible = true;
        //            grdPecaProjMod.Columns[20].Visible = true;
        //            grdPecaProjMod.Columns[21].Visible = true;
        //            grdPecaProjMod.Columns[22].Visible = true;
        //            grdPecaProjMod.Columns[23].Visible = true;
        //        }
        //        //Editar
        //        grdPecaProjMod.Columns[0].Visible = false;
        //        //Botão salvar
        //        btnSalvarInsercaoRapida.Visible = true;
        //    }
        //    else
        //    {
        //        if (!ExibirFolgaPorEspessura())
        //        {
        //            //Visualização
        //            grdPecaProjMod.Columns[4].Visible = true;
        //            grdPecaProjMod.Columns[5].Visible = true;
        //            //Inserção Rapida
        //            grdPecaProjMod.Columns[14].Visible = false;
        //            grdPecaProjMod.Columns[15].Visible = false;
        //        }
        //        else
        //        {
        //            //Visualização
        //            grdPecaProjMod.Columns[6].Visible = true;
        //            grdPecaProjMod.Columns[7].Visible = true;
        //            grdPecaProjMod.Columns[8].Visible = true;
        //            grdPecaProjMod.Columns[9].Visible = true;
        //            grdPecaProjMod.Columns[10].Visible = true;
        //            grdPecaProjMod.Columns[11].Visible = true;
        //            grdPecaProjMod.Columns[12].Visible = true;
        //            grdPecaProjMod.Columns[13].Visible = true;
        //            //Inserção Rapida
        //            grdPecaProjMod.Columns[16].Visible = false;
        //            grdPecaProjMod.Columns[17].Visible = false;
        //            grdPecaProjMod.Columns[18].Visible = false;
        //            grdPecaProjMod.Columns[19].Visible = false;
        //            grdPecaProjMod.Columns[20].Visible = false;
        //            grdPecaProjMod.Columns[21].Visible = false;
        //            grdPecaProjMod.Columns[22].Visible = false;
        //            grdPecaProjMod.Columns[23].Visible = false;
        //        }
        //        //Editar
        //        grdPecaProjMod.Columns[0].Visible = true;
        //        //Botão salvar
        //        btnSalvarInsercaoRapida.Visible = false;
        //    }
        //}

        protected void btnSalvarInsercaoRapida_Click(object sender, EventArgs e)
        {
            try
            {
                Glass.Data.Model.FolgaPecaCliente folgaPecaCliente;
              
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
                    var idPecaProjetomodelo = ((HiddenField)row.Cells[26].FindControl("hdfIdPecaProjMod")).Value.StrParaUint();
                    folgaPecaCliente = FolgaPecaClienteDAO.Instance.GetElement(idPecaProjetomodelo, Data.Helper.UserInfo.GetUserInfo.IdCliente.Value);
                    
                    largura06MM = ((TextBox)row.Cells[16].FindControl("txtLargura06MMInsercaoRapida")).Text.StrParaInt();
                    largura08MM = ((TextBox)row.Cells[18].FindControl("txtLargura08MMInsercaoRapida")).Text.StrParaInt();
                    largura10MM = ((TextBox)row.Cells[20].FindControl("txtLargura10MMInsercaoRapida")).Text.StrParaInt();
                    largura12MM = ((TextBox)row.Cells[22].FindControl("txtLargura12MMInsercaoRapida")).Text.StrParaInt();

                    altura06MM = ((TextBox)row.Cells[17].FindControl("txtAltura06MMInsercaoRapida")).Text.StrParaInt();
                    altura08MM = ((TextBox)row.Cells[19].FindControl("txtAltura08MMInsercaoRapida")).Text.StrParaInt();
                    altura10MM = ((TextBox)row.Cells[21].FindControl("txtAltura10MMInsercaoRapida")).Text.StrParaInt();
                    altura12MM = ((TextBox)row.Cells[23].FindControl("txtAltura12MMInsercaoRapida")).Text.StrParaInt();

                    if (folgaPecaCliente == null)
                    {
                        folgaPecaCliente = new Data.Model.FolgaPecaCliente();
                        folgaPecaCliente.IdCliente = Data.Helper.UserInfo.GetUserInfo.IdCliente.Value;
                        folgaPecaCliente.IdPecaProjetoModelo = idPecaProjetomodelo;
                    }
                    else
                    {
                        if (folgaPecaCliente.FolgaLarg06MM == largura06MM && folgaPecaCliente.FolgaAlt06MM == altura06MM &&
                            folgaPecaCliente.FolgaLarg08MM == largura08MM && folgaPecaCliente.FolgaAlt08MM == altura08MM &&
                            folgaPecaCliente.FolgaLarg10MM == largura10MM && folgaPecaCliente.FolgaAlt10MM == altura10MM &&
                            folgaPecaCliente.FolgaLarg12MM == largura12MM && folgaPecaCliente.FolgaAlt12MM == altura12MM)
                            continue;
                    }

                    folgaPecaCliente.FolgaLarg06MM = largura06MM;
                    folgaPecaCliente.FolgaLarg08MM = largura08MM;
                    folgaPecaCliente.FolgaLarg10MM = largura10MM;
                    folgaPecaCliente.FolgaLarg12MM = largura12MM;

                    folgaPecaCliente.FolgaAlt06MM = altura06MM;
                    folgaPecaCliente.FolgaAlt08MM = altura08MM;
                    folgaPecaCliente.FolgaAlt10MM = altura10MM;
                    folgaPecaCliente.FolgaAlt12MM = altura12MM;

                    FolgaPecaClienteDAO.Instance.InsertOrUpdate(folgaPecaCliente);
                }

                Glass.MensagemAlerta.ShowMsg("Inserção rápida de folgas concluida", Page);
            }
            catch(Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Erro", ex, Page);
            }
            finally
            {                
                grdPecaProjMod.DataBind();
            }
        }
    }
}
