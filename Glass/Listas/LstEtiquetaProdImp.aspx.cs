using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Data.Helper;
using System.Drawing;
using Glass.Configuracoes;

namespace Glass.UI.Web.Listas
{
    public partial class LstEtiquetaProdImp : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                Page.Title += " " + Request["idImpressao"];
    
            pchArqOtimiz.Visible = !PCPConfig.Etiqueta.UsarPlanoCorte;
            lnkImprimirApenasPlano.Visible = PCPConfig.Etiqueta.UsarPlanoCorte;
    
            try
            {
                uint idImpressao = Glass.Conversoes.StrParaUint(Request["idImpressao"]);
                impressao.Visible = Config.PossuiPermissao(Config.FuncaoMenuPCP.ReimprimirEtiquetas) && 
                    ImpressaoEtiquetaDAO.Instance.ObtemSituacao(idImpressao) == (int)ImpressaoEtiqueta.SituacaoImpressaoEtiqueta.Ativa;
    
                ProdutoImpressaoDAO.TipoEtiqueta tipoImpressao = ImpressaoEtiquetaDAO.Instance.GetTipoImpressao(idImpressao);
    
                if (tipoImpressao == ProdutoImpressaoDAO.TipoEtiqueta.Pedido)
                {
                    grdProduto.Columns[4].Visible = PedidoConfig.DadosPedido.AmbientePedido;
                    grdProduto.Columns[5].Visible = false;
                    grdProduto.Columns[6].Visible = false;
                }
                else if (tipoImpressao == ProdutoImpressaoDAO.TipoEtiqueta.NotaFiscal)
                {
                    Label2.Visible = false;
                    Label14.Visible = true;
    
                    txtNumPedido.Visible = false;
                    txtNumNFe.Visible = true;
    
                    grdProduto.Columns[2].Visible = false;
                    grdProduto.Columns[3].Visible = false;
                    grdProduto.Columns[4].Visible = false;
                    grdProduto.Columns[10].Visible = false;
                    grdProduto.Columns[11].Visible = false;
                }
                else if (tipoImpressao == ProdutoImpressaoDAO.TipoEtiqueta.Box)
                {
                    grdProduto.Columns[4].Visible = false;
                    grdProduto.Columns[5].Visible = false;
                    grdProduto.Columns[6].Visible = false;
                    grdProduto.Columns[12].Visible = false;
                    grdProduto.Columns[13].Visible = false;

                    lnkImprimirApenasPlano.Visible = false;
                    lnkImprimirCorEspessura.Visible = false;
                }
                else
                {
                    grdProduto.Columns[2].Visible = false;
                    grdProduto.Columns[3].Visible = false;
                    grdProduto.Columns[4].Visible = false;
                    grdProduto.Columns[5].Visible = false;
                    grdProduto.Columns[6].Visible = false;
                    grdProduto.Columns[10].Visible = false;
                    grdProduto.Columns[11].Visible = false;
                }
            }
            catch { }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdProduto.PageIndex = 0;
        }

        protected void lnkImprimir_Click(object sender, EventArgs e)
        {
            if (ImpressaoEtiquetaDAO.Instance.GetTipoImpressao(Glass.Conversoes.StrParaUint(Request["idImpressao"])) == ProdutoImpressaoDAO.TipoEtiqueta.Box)
            {
                if(!Config.PossuiPermissao(Config.FuncaoMenuPCP.ReimprimirEtiquetaBox))
                {
                    Glass.MensagemAlerta.ShowMsg("Você não tem permissão para reimprimir etiquetas de box.", Page);
                    return;
                }

                Response.Redirect("../Relatorios/RelBase.aspx?rel=EtiquetaBox&idImpressao=" + Request["idImpressao"]);
                return;
            }

            Response.Redirect("../Relatorios/RelEtiquetas.aspx?idImpressao=" + Request["idImpressao"] + "&tipo=0");
        }
    
        protected void lnkImprimirApenasPlano_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Relatorios/RelEtiquetas.aspx?idImpressao=" + Request["idImpressao"] + "&tipo=0&apenasPlano=true");
        }
    
        protected void lnkImprimirCorEspessura_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Relatorios/RelBase.aspx?rel=PedidoPcpAgrupado&idImpressao=" + Request["idImpressao"] + "&idPedido" + txtNumPedido.Text + 
                "&descrProduto=" + txtDescr.Text + "&etiqueta=" + txtEtiqueta.Text + "&altura=" + txtAltura.Text + "&largura=" + txtLargura.Text);
        }
    
        protected void grdProduto_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;
    
            if ((e.Row.DataItem as ProdutoImpressao).Cancelado)
                foreach (TableCell c in e.Row.Cells)
                    c.ForeColor = Color.Red;
        }
    
        protected void grdProduto_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "addObs")
            {
                try
                {
                    var prodImpressao = ProdutoImpressaoDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(e.CommandArgument.ToString()));
                    uint idProdPedEsp = ProdutosPedidoEspelhoDAO.Instance.GetIdProdPedByEtiqueta(prodImpressao.NumEtiqueta);
                    string obsAntiga = ((HiddenField)grdProduto.Rows[grdProduto.EditIndex].FindControl("hdfObs")).Value;
                    string obsNova = ((TextBox)grdProduto.Rows[grdProduto.EditIndex].FindControl("txtObs")).Text;
    
                    if (obsNova != null)
                    {
                        ProdutosPedidoEspelhoDAO.Instance.AtualizaObs(null, idProdPedEsp, obsNova);
    
                        // Salva no log esta alteração, mesmo que o campo obs não exista nesta tabela
    
                        // Põe a antiga observação em um objeto
                        prodImpressao.Obs = obsAntiga;
    
                        // Cria um objeto com a nova observação
                        ProdutoImpressao prodImpressaoNovo = MetodosExtensao.Clonar(prodImpressao);
                        prodImpressaoNovo.Obs = obsNova;
    
                        LogAlteracaoDAO.Instance.LogProdutoImpressao(prodImpressao, prodImpressaoNovo);
                    }
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao adicionar observação", ex, Page);
                }
                finally
                {
                    grdProduto.EditIndex = -1;
                    grdProduto.DataBind();
                }
            }
        }

        protected void grdProduto_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && PedidoConfig.EmpresaTrabalhaAlturaLargura)
            {
                grdProduto.Columns[8].HeaderText = "Altura";
                grdProduto.Columns[9].HeaderText = "Largura";
            }
        }
    }
}
