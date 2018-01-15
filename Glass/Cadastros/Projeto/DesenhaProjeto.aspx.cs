using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Glass.Data.DAL;
using System.IO;

namespace Glass.UI.Web.Cadastros.Projeto
{
    public partial class DesenhaProjeto : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Busca o item projeto passado por query string
                ItemProjeto itemProj = ItemProjetoDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(Request["idItemProjeto"]));
                ProjetoModelo projMod = ProjetoModeloDAO.Instance.GetElementByPrimaryKey(itemProj.IdProjetoModelo);
    
                if (!Page.Title.Contains(":"))
                    Page.Title += ": " + projMod.Codigo;

                string nomeFigura = !String.IsNullOrEmpty(Request["item"]) ?
                    projMod.Codigo + "§" + Request["item"] + ".jpg" : projMod.NomeFiguraAssociada;

                // Carrega imagem na tela
                if (Data.Helper.Utils.ArquivoExiste("~/Upload/PecaProducao/" + nomeFigura))
                    imgFigura.ImageUrl = "../../Handlers/LoadFiguraAssociada.ashx?tipoDesenho=0&path=" + Server.MapPath("~/Upload/PecaProducao/" + nomeFigura);
                else if(File.Exists(Data.Helper.Utils.GetModelosProjetoPath + nomeFigura))
                    imgFigura.ImageUrl = "../../Handlers/LoadFiguraAssociada.ashx?tipoDesenho=0&path=" + Data.Helper.Utils.GetModelosProjetoPath + nomeFigura;
                else
                {
                    nomeFigura = projMod.IdProjetoModelo.ToString("0##") + "_" + Request["item"] + ".jpg";
                    if (Data.Helper.Utils.ArquivoExiste("~/Upload/PecaProducao/" + nomeFigura))
                        imgFigura.ImageUrl = "../../Handlers/LoadFiguraAssociada.ashx?tipoDesenho=0&path=" + Server.MapPath("~/Upload/PecaProducao/" + nomeFigura);
                    else
                        imgFigura.ImageUrl = "../../Handlers/LoadFiguraAssociada.ashx?tipoDesenho=0&path=" + Data.Helper.Utils.GetModelosProjetoPath + nomeFigura;
                }
    
                #region Monta tabela com figuras que podem ser selecionada para desenho
    
                foreach (GrupoFiguraProjeto grupo in GrupoFiguraProjetoDAO.Instance.GetOrdered())
                {
                    Table tb = new Table();
                    tb.ID = "tb" + grupo.IdGrupoFigProj;
    
                    // Monta o cabeçalho
                    TableRow trHeader = new TableRow();
                    TableCell tdHeader = new TableCell();
                    tdHeader.Text = grupo.Descricao;
                    tdHeader.ColumnSpan = 5;
                    tdHeader.Width = new Unit(200, UnitType.Percentage);
                    tdHeader.Style.Add("text-align", "left");
                    tdHeader.Attributes.Add("nowrap", "nowrap");
    
                    trHeader.CssClass = "toolGroup";
                    trHeader.Controls.Add(tdHeader);
                    tb.Controls.Add(trHeader);
    
                    var lstFigura = FiguraProjetoDAO.Instance.GetOrdered(grupo.IdGrupoFigProj);
    
                    TableRow tr = new TableRow();
                    
                    // Define um id para a tablerow para usar a função exibir/esconder
                    tr.ID = "tr" + grupo.IdGrupoFigProj + "_001";
                    tr.Style.Add("display", "none");
                    string onClickHeader = "exibeEscondeGrupo('" + tr.ID + "');";
    
                    // Monta as figuras na tabela
                    for (int i = 0; i < lstFigura.Count; i++)
                    {   
                        TableCell td = new TableCell();
    
                        ImageButton tool = new ImageButton();
                        tool.ImageUrl = Data.Helper.Utils.GetFullUrl(HttpContext.Current, Data.Helper.Utils.GetFigurasProjetoVirtualPath + lstFigura[i].IdFiguraProjeto + ".jpg"); //Utils.GetFigurasProjetoVirtualPath + lstFigura[i].IdFiguraProjeto + ".jpg";
                        tool.CssClass = "toolNormal";
                        tool.BorderWidth = new Unit(1);
                        tool.ID = "tool_" + lstFigura[i].IdFiguraProjeto + "_";
                        tool.Attributes.Add("onmouseover", "tool_OnMouseOver(this)");
                        tool.Attributes.Add("onmouseout", "tool_OnMouseOut(this)");
                        tool.OnClientClick = "setTool(this, " + lstFigura[i].IdFiguraProjeto + ", '" + tool.ImageUrl + "'); return false;";
                        
                        td.Controls.Add(tool);
                        tr.Controls.Add(td);
    
                        // Adiciona imagens à tabela de seu respectivo grupo e cria outra linha na tabela
                        if ((i + 1) % 5 == 0 || lstFigura.Count == i + 1)
                        {
                            tb.Controls.Add(tr);
    
                            tr = new TableRow();
                            tr.ID = "tr" + grupo.IdGrupoFigProj + "_" + i.ToString().PadLeft(3, '0');
                            tr.Style.Add("display", "none");
    
                            if (lstFigura.Count != i + 1)
                                onClickHeader += "exibeEscondeGrupo('" + tr.ID + "');";
                        }
                    }
    
                    // Adiciona comando no header para esconder/exibir imagens
                    trHeader.Attributes.Add("onClick", onClickHeader);
    
                    // Adiciona tabela de grupo de figura à tabela principal
                    pchTool.Controls.Add(tb);
                }
    
                #endregion
    
                #region Desenha figuras já inseridas nesta imagem
    
                var lstPosicao = FiguraPecaItemProjetoDAO.Instance.GetFigurasByPeca(Glass.Conversoes.StrParaUint(Request["idPecaItemProj"]),
                    Glass.Conversoes.StrParaInt(Request["item"]));
    
                if (lstPosicao.Count > 0)
                {
                    string vetIdFiguraProjeto = String.Empty;
                    string vetCoord = String.Empty;
    
                    foreach (FiguraPecaItemProjeto p in lstPosicao)
                    {
                        vetIdFiguraProjeto += p.IdFiguraProjeto + ";";
                        vetCoord += p.CoordX + ";" + p.CoordY + "|";
                    }
    
                    vetIdFiguraProjeto = vetIdFiguraProjeto.TrimEnd(';');
                    vetCoord = vetCoord.TrimEnd('|');
    
                    ClientScript.RegisterStartupScript(typeof(string), "carregar",
                        "vetIdFiguraProjetoReload='" + vetIdFiguraProjeto + "'; vetCoordReload='" + vetCoord + "'; carregaFigurasDesenhadas();", true);
                }
    
                #endregion
            }
    
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.Projeto.DesenhaProjeto));
        }
    
        [Ajax.AjaxMethod()]
        public string Salvar(string idItemProjeto, string idPecaItemProj, string item, string vetIdFiguraProjeto, string vetCoord)
        {
            try
            {
                // Insere/Atualiza figuras
                FiguraPecaItemProjetoDAO.Instance.InsereAtualizaFiguras(Glass.Conversoes.StrParaUint(idItemProjeto), Glass.Conversoes.StrParaUint(idPecaItemProj),
                    Glass.Conversoes.StrParaInt(item), vetIdFiguraProjeto.TrimEnd(';').Split(';'), vetCoord.TrimEnd('|').Split('|'));

                PecaItemProjetoDAO.Instance.ImagemEditada(Convert.ToUInt32(idPecaItemProj), true);

                return "ok";
            }
            catch (Exception ex)
            {
                return "Erro\t" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao salvar edição.", ex);
            }
        }
    }
}
