using System;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Model;
using System.IO;
using Sync.Controls;
using Glass.UI.Web.Controls;
using System.Linq;

namespace Glass.UI.Web.Cadastros.Projeto
{
    public partial class CadModeloProjeto : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.Projeto.CadModeloProjeto));
                
            if (Request["IdProjetoModelo"] != null)
            {
                hdfIdProjetoModelo.Value = Request["idProjetoModelo"];
    
                // Se o modelo não for configurável, esconde uma série de opções
                if (!ProjetoModeloDAO.Instance.IsConfiguravel(Glass.Conversoes.StrParaUint(Request["IdProjetoModelo"])))
                {
                    grdPecaProjetoModelo.ShowFooter = false;
                    grdMaterialProjetoModelo.ShowFooter = false;
                    grdPecaProjetoModelo.Columns[0].Visible = false;
                    grdMaterialProjetoModelo.Columns[0].Visible = false;
                }
            }

            if (Configuracoes.ProjetoConfig.SelecionarEspessuraAoCalcularProjeto)
            {
                grdPecaProjetoModelo.Columns[3].Visible = false;
                grdPecaProjetoModelo.Columns[4].Visible = false;
                grdMaterialProjetoModelo.Columns[4].Visible = true;
            }
            else
            {
                grdPecaProjetoModelo.Columns[5].Visible = false;
                grdPecaProjetoModelo.Columns[6].Visible = false;
                grdPecaProjetoModelo.Columns[7].Visible = false;
                grdPecaProjetoModelo.Columns[8].Visible = false;
                grdPecaProjetoModelo.Columns[9].Visible = false;
                grdPecaProjetoModelo.Columns[10].Visible = false;
                grdPecaProjetoModelo.Columns[11].Visible = false;
                grdPecaProjetoModelo.Columns[12].Visible = false;
                grdPecaProjetoModelo.Columns[13].Visible = false;
                grdPecaProjetoModelo.Columns[14].Visible = false;
                grdPecaProjetoModelo.Columns[15].Visible = false;
                grdPecaProjetoModelo.Columns[16].Visible = false;
                grdPecaProjetoModelo.Columns[17].Visible = false;
                grdPecaProjetoModelo.Columns[18].Visible = false;
                grdMaterialProjetoModelo.Columns[4].Visible = false;
            }
            
            if (dtvProjetoModelo.CurrentMode == DetailsViewMode.Insert && Request["IdProjetoModelo"] != null)
                dtvProjetoModelo.ChangeMode(DetailsViewMode.ReadOnly);
    
            bool gridVisib = dtvProjetoModelo.CurrentMode == DetailsViewMode.ReadOnly;
            grdPecaProjetoModelo.Visible = gridVisib;
            grdMaterialProjetoModelo.Visible = gridVisib;
            lblMateriais.Visible = gridVisib;
            lblPecas.Visible = gridVisib;
        }
    
        #region Métodos Ajax
    
        [Ajax.AjaxMethod()]
        public string GetProduto(string codInterno)
        {
            ProdutoProjeto prodProj = ProdutoProjetoDAO.Instance.GetByCodInterno(codInterno);
    
            if (prodProj == null)
                return "Erro;Não existe produto com o código informado.";
    
            string retorno = "Prod;" + prodProj.IdProdProj + ";" + prodProj.Descricao;
    
            return retorno;
        }
    
        #endregion
    
        #region Eventos DataSource
    
        protected void odsProjetoModelo_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir modelo de projeto.", e.Exception, Page);
                e.ExceptionHandled = true;
                return;
            }
    
            ProjetoModelo projMod = ProjetoModeloDAO.Instance.GetElement(Glass.Conversoes.StrParaUint(e.ReturnValue.ToString()));
    
            try
            {
                #region Figura Engenharia
    
                FileUpload fluFiguraEngenharia = (FileUpload)dtvProjetoModelo.FindControl("fluFiguraEngenharia");
    
                if (fluFiguraEngenharia.HasFile && e.ReturnValue != null)
                {
                    if (fluFiguraEngenharia.PostedFile.FileName.Substring(fluFiguraEngenharia.PostedFile.FileName.LastIndexOf(".")).ToLower() != ".jpg")
                    {
                        Glass.MensagemAlerta.ShowMsg("Imagem inválida.", Page);
                        return;
                    }

                    /* Chamado 47688 - Remover a busca pelo ID quando todas as imagens forem renomeadas */
                    string fotoPath = Data.Helper.Utils.GetModelosProjetoPath + projMod.IdProjetoModelo.ToString().PadLeft(3, '0') + ".jpg";
                    if (File.Exists(fotoPath))
                        File.Delete(fotoPath);

                    fotoPath = Data.Helper.Utils.GetModelosProjetoPath + projMod.Codigo + "§E.jpg";
                    if (File.Exists(fotoPath))
                        File.Delete(fotoPath);

                    fluFiguraEngenharia.SaveAs(fotoPath);
    
                    if (File.Exists(fotoPath))
                        ProjetoModeloDAO.Instance.AtualizaNomeFigura(projMod.IdProjetoModelo, projMod.Codigo + "§E.jpg", null);
                }
    
                #endregion
    
                #region Figura Modelo
    
                FileUpload fluFiguraModelo = (FileUpload)dtvProjetoModelo.FindControl("fluFiguraModelo");
    
                if (fluFiguraModelo.HasFile && e.ReturnValue != null)
                {
                    if (fluFiguraModelo.PostedFile.FileName.Substring(fluFiguraModelo.PostedFile.FileName.LastIndexOf(".")).ToLower() != ".jpg")
                    {
                        Glass.MensagemAlerta.ShowMsg("Imagem inválida.", Page);
                        return;
                    }

                    string fotoPath = Data.Helper.Utils.GetModelosProjetoPath + projMod.Codigo + ".jpg";
    
                    if (System.IO.File.Exists(fotoPath))
                        System.IO.File.Delete(fotoPath);
    
                    fluFiguraModelo.SaveAs(fotoPath);
    
                    if (File.Exists(fotoPath))
                        ProjetoModeloDAO.Instance.AtualizaNomeFigura(projMod.IdProjetoModelo, null, projMod.Codigo + ".jpg");
                }
    
                #endregion
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao cadastrar figura.", ex, Page);
                ProjetoModeloDAO.Instance.DeleteByPrimaryKey(projMod.IdProjetoModelo);
                return;
            }
    
            hdfIdProjetoModelo.Value = e.ReturnValue.ToString();
            Response.Redirect("CadModeloProjeto.aspx?idProjetoModelo=" + hdfIdProjetoModelo.Value);
        }
    
        protected void odsProjetoModelo_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar modelo de projeto.", e.Exception, Page);
                dtvProjetoModelo.ChangeMode(DetailsViewMode.ReadOnly);
                e.ExceptionHandled = true;
                return;
            }
    
            ProjetoModelo projMod = ProjetoModeloDAO.Instance.GetElement(Glass.Conversoes.StrParaUint(Request["idProjetoModelo"]));
    
            try
            {
                #region Figura Engenharia
    
                FileUpload fluFiguraEngenharia = (FileUpload)dtvProjetoModelo.FindControl("fluFiguraEngenharia");
    
                if (fluFiguraEngenharia.HasFile && e.ReturnValue != null)
                {
                    if (fluFiguraEngenharia.PostedFile.FileName.Substring(fluFiguraEngenharia.PostedFile.FileName.LastIndexOf(".")).ToLower() != ".jpg")
                    {
                        Glass.MensagemAlerta.ErrorMsg("Imagem inválida.", e.Exception, Page);
                        return;
                    }

                    /* Chamado 47688 - Remover a busca pelo ID quando todas as imagens forem renomeadas */
                    string fotoPath = Data.Helper.Utils.GetModelosProjetoPath + projMod.IdProjetoModelo.ToString().PadLeft(3, '0') + ".jpg";
                    if (File.Exists(fotoPath))
                        File.Delete(fotoPath);

                    fotoPath = Data.Helper.Utils.GetModelosProjetoPath + projMod.Codigo + "§E.jpg";
                    if (File.Exists(fotoPath))
                        File.Delete(fotoPath);

                    fluFiguraEngenharia.SaveAs(fotoPath);
    
                    if (File.Exists(fotoPath))
                        ProjetoModeloDAO.Instance.AtualizaNomeFigura(projMod.IdProjetoModelo, projMod.Codigo + "§E.jpg", null);
                }
    
                #endregion
    
                #region Figura Modelo
    
                FileUpload fluFiguraModelo = (FileUpload)dtvProjetoModelo.FindControl("fluFiguraModelo");
    
                if (fluFiguraModelo.HasFile && e.ReturnValue != null)
                {
                    if (fluFiguraModelo.PostedFile.FileName.Substring(fluFiguraModelo.PostedFile.FileName.LastIndexOf(".")).ToLower() != ".jpg")
                    {
                        Glass.MensagemAlerta.ShowMsg("Imagem inválida.", Page);
                        return;
                    }

                    string fotoPath = Data.Helper.Utils.GetModelosProjetoPath + projMod.Codigo + ".jpg";
    
                    if (System.IO.File.Exists(fotoPath))
                        System.IO.File.Delete(fotoPath);
    
                    fluFiguraModelo.SaveAs(fotoPath);
    
                    if (File.Exists(fotoPath))
                        ProjetoModeloDAO.Instance.AtualizaNomeFigura(projMod.IdProjetoModelo, null, projMod.Codigo + ".jpg");
                }
    
                #endregion
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao cadastrar figura.", ex, Page);
                return;
            }
    
            Response.Redirect("CadModeloProjeto.aspx?idProjetoModelo=" + hdfIdProjetoModelo.Value);
        }
    
        protected void odsProjetoModelo_Inserting(object sender, Colosoft.WebControls.VirtualObjectDataSourceMethodEventArgs e)
        {
            // Recupera as opções selecionadas das medidas
            ListItemCollection lstMedidas = ((CheckBoxList)dtvProjetoModelo.FindControl("cblMedidas")).Items;
    
            string items = String.Empty;
    
            foreach (ListItem li in lstMedidas)
                if (li.Selected)
                    items += li.Value + ",";
    
            ((ProjetoModelo)e.InputParameters[0]).MedidasProjMod = items.TrimEnd(',');
        }
    
        protected void odsProjetoModelo_Updating(object sender, Colosoft.WebControls.VirtualObjectDataSourceMethodEventArgs e)
        {
            // Recupera as opções selecionadas das medidas
            ListItemCollection lstMedidas = ((CheckBoxList)dtvProjetoModelo.FindControl("cblMedidas")).Items;
    
            string items = String.Empty;
    
            foreach (ListItem li in lstMedidas)
                if (li.Selected)
                    items += li.Value + ",";
    
            ((ProjetoModelo)e.InputParameters[0]).MedidasProjMod = items.TrimEnd(',');
        }
    
        protected void odsPecaProjetoModelo_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar peça.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
    
        protected void odsPecaProjetoModelo_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao excluir peça.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
    
        protected void odsMaterialProjetoModelo_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar material.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
    
        protected void odsMaterialProjetoModelo_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao excluir material.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
    
        #endregion
    
        #region Eventos Grid/Detail
    
        protected void dtvProjetoModelo_DataBound(object sender, EventArgs e)
        {
            // Marca as medidas já salvas neste modelo
            if (dtvProjetoModelo.CurrentMode == DetailsViewMode.Edit)
            {
                CheckBoxList cblMedidas = ((CheckBoxList)dtvProjetoModelo.FindControl("cblMedidas"));
                string medidas = ((HiddenField)dtvProjetoModelo.FindControl("hdfMedidas")).Value;
    
                if (String.IsNullOrEmpty(medidas))
                    return;
    
                foreach (string med in medidas.Split(','))
                    cblMedidas.Items.FindByValue(med).Selected = true;
            }
        }
    
        protected void grdPecaProjetoModelo_PreRender(object sender, EventArgs e)
        {
            string idProjetoModelo = hdfIdProjetoModelo.Value;
    
            // Se não houver nenhuma peça cadastrada no pedido
            if (!String.IsNullOrEmpty(idProjetoModelo) && PecaProjetoModeloDAO.Instance.GetCountRealIns(Glass.Conversoes.StrParaUint(idProjetoModelo)) == 0)
                grdPecaProjetoModelo.Rows[0].Visible = false;
        }
    
        protected void grdPecaProjetoModelo_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdPecaProjetoModelo.ShowFooter = e.CommandName != "Edit";
        }
    
        protected void grdMaterialProjetoModelo_PreRender(object sender, EventArgs e)
        {
            string idProjetoModelo = hdfIdProjetoModelo.Value;
    
            // Se não houver nenhuma peça cadastrada no pedido
            if (!String.IsNullOrEmpty(idProjetoModelo) && MaterialProjetoModeloDAO.Instance.GetCountRealIns(Glass.Conversoes.StrParaUint(idProjetoModelo)) == 0)
                grdMaterialProjetoModelo.Rows[0].Visible = false;
        }
    
        protected void grdPecaProjetoModelo_DataBound(object sender, EventArgs e)
        {
            // Cria link para inserir/editar images individuais
            foreach (GridViewRow row in grdPecaProjetoModelo.Rows)
            {
                HiddenField hdfIdPecaProjMod = (HiddenField)row.Cells[0].FindControl("hdfIdPecaProjMod");
    
                if (hdfIdPecaProjMod == null || hdfIdPecaProjMod.Value == "0")
                    continue;
    
                PecaProjetoModelo ppm = PecaProjetoModeloDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(hdfIdPecaProjMod.Value));
    
                // Imagem (Apenas para peça de instalação)
                if (ppm.Tipo == 1)
                {
                    // Formata o item
                    string itemTemp = ppm.Item.Replace(",", " ").Replace("e", " ").Replace("E", " ").Replace("-", "");
                    while (itemTemp.Contains("  "))
                        itemTemp = itemTemp.Replace("  ", " ");
    
                    // Insere uma imagem de edição para cada item
                    foreach (string item in itemTemp.Trim().Split(' '))
                    {
                        ImageButton cImb = new ImageButton();
                        cImb.OnClientClick = "openWindow(600, 800, 'CadPosicaoPecaModelo.aspx?tipo=pecaIndividual&idProjetoModelo=" +
                            Request["idProjetoModelo"] + "&idPecaProjMod=" + ppm.IdPecaProjMod + "&item=" + item.Trim() + "'); return false;";
                        
                        cImb.ImageUrl = "~/Images/clipboard.gif";
                        cImb.ToolTip = "Alterar imagem da peça (Item " + item.Trim() + ")";
    
                        row.Cells[22].Controls.Add(cImb);
                    }
                }
            }
        }
    
        #endregion
    
        #region Insere peça de vidro
    
        protected void lnkInsPeca_Click(object sender, EventArgs e)
        {
            var alturaString = ((TextBox)grdPecaProjetoModelo.FooterRow.FindControl("txtAlturaIns")).Text;
            var larguraString = ((TextBox)grdPecaProjetoModelo.FooterRow.FindControl("txtLarguraIns")).Text;
            var altura03MMString = ((TextBox)grdPecaProjetoModelo.FooterRow.FindControl("txtAltura03MMIns")).Text;
            var largura03MMString = ((TextBox)grdPecaProjetoModelo.FooterRow.FindControl("txtLargura03MMIns")).Text;
            var altura04MMString = ((TextBox)grdPecaProjetoModelo.FooterRow.FindControl("txtAltura04MMIns")).Text;
            var largura04MMString = ((TextBox)grdPecaProjetoModelo.FooterRow.FindControl("txtLargura04MMIns")).Text;
            var altura05MMString = ((TextBox)grdPecaProjetoModelo.FooterRow.FindControl("txtAltura05MMIns")).Text;
            var largura05MMString = ((TextBox)grdPecaProjetoModelo.FooterRow.FindControl("txtLargura05MMIns")).Text;
            var altura06MMString = ((TextBox)grdPecaProjetoModelo.FooterRow.FindControl("txtAltura06MMIns")).Text;
            var largura06MMString = ((TextBox)grdPecaProjetoModelo.FooterRow.FindControl("txtLargura06MMIns")).Text;
            var altura08MMString = ((TextBox)grdPecaProjetoModelo.FooterRow.FindControl("txtAltura08MMIns")).Text;
            var largura08MMString = ((TextBox)grdPecaProjetoModelo.FooterRow.FindControl("txtLargura08MMIns")).Text;
            var altura10MMString = ((TextBox)grdPecaProjetoModelo.FooterRow.FindControl("txtAltura10MMIns")).Text;
            var largura10MMString = ((TextBox)grdPecaProjetoModelo.FooterRow.FindControl("txtLargura10MMIns")).Text;
            var altura12MMString = ((TextBox)grdPecaProjetoModelo.FooterRow.FindControl("txtAltura12MMIns")).Text;
            var largura12MMString = ((TextBox)grdPecaProjetoModelo.FooterRow.FindControl("txtLargura12MMIns")).Text;

            var aplicacao = ((ctrlSelAplicacao)grdPecaProjetoModelo.FooterRow.FindControl("ctrlSelAplicacao")).CodigoAplicacao;
            var processo = ((ctrlSelProcesso)grdPecaProjetoModelo.FooterRow.FindControl("ctrlSelProcesso")).CodigoProcesso;
    
            var peca = new PecaProjetoModelo();
            peca.IdProjetoModelo = Glass.Conversoes.StrParaUint(hdfIdProjetoModelo.Value);
            peca.Tipo = Glass.Conversoes.StrParaInt(((DropDownList)grdPecaProjetoModelo.FooterRow.FindControl("drpTipoIns")).SelectedValue);

            peca.Altura = !String.IsNullOrEmpty(alturaString) ? Glass.Conversoes.StrParaInt(alturaString) : 0;
            peca.Largura = !String.IsNullOrEmpty(larguraString) ? Glass.Conversoes.StrParaInt(larguraString) : 0;
            peca.Altura03MM = !String.IsNullOrEmpty(altura03MMString) ? Glass.Conversoes.StrParaInt(altura03MMString) : 0;
            peca.Largura03MM = !String.IsNullOrEmpty(largura03MMString) ? Glass.Conversoes.StrParaInt(largura03MMString) : 0;
            peca.Altura04MM = !String.IsNullOrEmpty(altura04MMString) ? Glass.Conversoes.StrParaInt(altura04MMString) : 0;
            peca.Largura04MM = !String.IsNullOrEmpty(largura04MMString) ? Glass.Conversoes.StrParaInt(largura04MMString) : 0;
            peca.Altura05MM = !String.IsNullOrEmpty(altura05MMString) ? Glass.Conversoes.StrParaInt(altura05MMString) : 0;
            peca.Largura05MM = !String.IsNullOrEmpty(largura05MMString) ? Glass.Conversoes.StrParaInt(largura05MMString) : 0;
            peca.Altura06MM = !String.IsNullOrEmpty(altura06MMString) ? Glass.Conversoes.StrParaInt(altura06MMString) : 0;
            peca.Largura06MM = !String.IsNullOrEmpty(largura06MMString) ? Glass.Conversoes.StrParaInt(largura06MMString) : 0;
            peca.Altura08MM = !String.IsNullOrEmpty(altura08MMString) ? Glass.Conversoes.StrParaInt(altura08MMString) : 0;
            peca.Largura08MM = !String.IsNullOrEmpty(largura08MMString) ? Glass.Conversoes.StrParaInt(largura08MMString) : 0;
            peca.Altura10MM = !String.IsNullOrEmpty(altura10MMString) ? Glass.Conversoes.StrParaInt(altura10MMString) : 0;
            peca.Largura10MM = !String.IsNullOrEmpty(largura10MMString) ? Glass.Conversoes.StrParaInt(largura10MMString) : 0;
            peca.Altura12MM = !String.IsNullOrEmpty(altura12MMString) ? Glass.Conversoes.StrParaInt(altura12MMString) : 0;
            peca.Largura12MM = !String.IsNullOrEmpty(largura12MMString) ? Glass.Conversoes.StrParaInt(largura12MMString) : 0;
            peca.IdAplicacao = aplicacao;
            peca.IdProcesso = processo;
            peca.Item = ((TextBox)grdPecaProjetoModelo.FooterRow.FindControl("txtItemIns")).Text;
            peca.CalculoQtde = ((TextBox)grdPecaProjetoModelo.FooterRow.FindControl("txtCalcPecaQtde")).Text;
            peca.CalculoAltura = ((TextBox)grdPecaProjetoModelo.FooterRow.FindControl("txtCalcAltIns")).Text;
            peca.CalculoLargura = ((TextBox)grdPecaProjetoModelo.FooterRow.FindControl("txtCalcLargIns")).Text;
            peca.Obs = ((TextBox)grdPecaProjetoModelo.FooterRow.FindControl("txtObs")).Text;
            peca.IdArquivoMesaCorte = Glass.Conversoes.StrParaUintNullable(((DropDownList)grdPecaProjetoModelo.FooterRow.FindControl("drpArquivo")).SelectedValue);


            peca.FlagsArqMesa = ((Sync.Controls.CheckBoxListDropDown)grdPecaProjetoModelo.FooterRow.FindControl("drpFlagArqMesa")).SelectedValues;
            
            if (peca.IdArquivoMesaCorte.GetValueOrDefault() > 0)
                peca.TipoArquivo = (TipoArquivoMesaCorte?)Enum.Parse(typeof(TipoArquivoMesaCorte),
                    ((DropDownList)grdPecaProjetoModelo.FooterRow.FindControl("drpTipoArquivo")).SelectedValue, true);

            peca.Beneficiamentos = ((Glass.UI.Web.Controls.ctrlBenef)grdPecaProjetoModelo.FooterRow.FindControl("ctrlBenef1")).Beneficiamentos;
    
            try
            {
                PecaProjetoModeloDAO.Instance.Insert(peca);
                grdPecaProjetoModelo.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir peça.", ex, Page);
            }
        }
    
        #endregion
    
        #region Insere material
    
        protected void lnkInsMaterial_Click(object sender, EventArgs e)
        {
            MaterialProjetoModelo material = new MaterialProjetoModelo();

            var espessuras = ((CheckBoxListDropDown)grdMaterialProjetoModelo.FooterRow.FindControl("drpEspessuras")).SelectedValues;

            material.IdProjetoModelo = Glass.Conversoes.StrParaUint(hdfIdProjetoModelo.Value);
            material.IdProdProj = Glass.Conversoes.StrParaUint(((HiddenField)grdMaterialProjetoModelo.FooterRow.FindControl("hdfIdProdProj")).Value);
            material.CalculoQtde = ((TextBox)grdMaterialProjetoModelo.FooterRow.FindControl("txtCalcQtd")).Text;
            material.CalculoAltura = ((TextBox)grdMaterialProjetoModelo.FooterRow.FindControl("txtCalcMaterAlt")).Text;
            material.Espessuras = espessuras.Any(f => f > 0) ? string.Join(",", espessuras) : null;

            /* Chamado 53687. */
            var grauCorte = new GrauCorteEnum();
            if (Enum.TryParse(((DropDownList)grdMaterialProjetoModelo.FooterRow.FindControl("drpGrauCorte")).SelectedValue, out grauCorte))
                material.GrauCorte = grauCorte;

            try
            {
                MaterialProjetoModeloDAO.Instance.Insert(material);
                grdMaterialProjetoModelo.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir material.", ex, Page);
            }
        }
    
        #endregion
    
        #region Beneficiamentos
    
        protected void ctrlBenef1_Load(object sender, EventArgs e)
        {
            hdfIdProd.Value = ProdutoDAO.Instance.GetFirstProdutoCodInterno((int)Glass.Data.Model.NomeGrupoProd.Vidro);
    
            Glass.UI.Web.Controls.ctrlBenef ctrlBenef = (Glass.UI.Web.Controls.ctrlBenef)sender;
            ctrlBenef.CampoProdutoID = hdfIdProd;
        }
    
        #endregion
    
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("LstModeloProjeto.aspx");
        }
    
        protected void imgModelo_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string path = Data.Helper.Utils.GetModelosProjetoVirtualPath + ProjetoModeloDAO.Instance.ObtemNomeFigura(Glass.Conversoes.StrParaUint(Request["idProjetoModelo"]));
                imgModelo.ImageUrl = path;
            }
        }

        protected void drpArquivo_SelectedIndexChanged(object sender, EventArgs e)
        {
            hdfIdArquivoMesaCorte.Value = ((DropDownList)sender).SelectedValue;

            var linha = ((DropDownList)sender).Parent.Parent;
            ((CheckBoxListDropDown)linha.FindControl("drpFlagArqMesa")).Items.Clear();
        }

        protected void drpArquivo_DataBound(object sender, EventArgs e)
        {
            hdfIdArquivoMesaCorte.Value = ((DropDownList)sender).SelectedValue;
        }
    }
}
