using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Listas
{
    public partial class LstEstoque : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["fiscal"] == "1")
                Page.Title = "Estoque Fiscal de Produtos";
            else
            {
                chkApenasPosseTerceiros.Visible = false;
                
                lblOrdenar.Visible = false;
                drpOrdenar.Style.Add("display", "none");
                lnkPesqSaidaEstoque0.Style.Add("display", "none");
            }
    
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Listas.LstEstoque));
    
            if (!IsPostBack)
            {
                drpLoja.SelectedValue = UserInfo.GetUserInfo.IdLoja.ToString();
    
                if (Request["fiscal"] == "1" || (PedidoConfig.LiberarPedido && Liberacao.Estoque.SaidaEstoqueAoLiberarPedido) ||
                    (!PedidoConfig.LiberarPedido && FinanceiroConfig.Estoque.SaidaEstoqueAutomaticaAoConfirmar))
                {
                    chkAguardSaidaEstoque.Style.Add("display", "none");
                    lnkPesqSaidaEstoque.Style.Add("display", "none");
                }

                // Esconde o log caso a tela n�o seja do estoque fiscal.
                grdEstoque.Columns[18].Visible = Request["fiscal"] == "1";
            }
    
            if (!Config.PossuiPermissao(Config.FuncaoMenuEstoque.AlterarEstoqueManualmente))
                chkInsercaoRapidaEstoque.Style.Add("display", "none");
        }
    
        protected void odsEstoque_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar estoque.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
                grdEstoque.DataBind();
        }
    
        protected void lnkPesq_Click(object sender, EventArgs e)
        {
            grdEstoque.DataBind();
            grdEstoque.PageIndex = 0;
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            lnkPesq_Click(sender, e);
        }
    
        protected void grdEstoque_DataBound(object sender, EventArgs e)
        {
            // Desabilita este campo at� que o m�todo ZeraEstoque abaixo zere tamb�m o extrato real/fiscal
            lnkLimparEstoque.Visible = false && UserInfo.GetUserInfo.IsAdministrador &&
                drpGrupo.SelectedValue != "0";
        }
    
        protected void lnkLimparEstoque_Click(object sender, EventArgs e)
        {
            try
            {
                uint idLoja = Glass.Conversoes.StrParaUint(drpLoja.SelectedValue);
                uint idGrupo = Glass.Conversoes.StrParaUint(drpGrupo.SelectedValue);
                uint? idSubgrupo = Glass.Conversoes.StrParaUintNullable(drpSubgrupo.SelectedValue);
    
                ProdutoLojaDAO.Instance.ZeraEstoque(idLoja, idGrupo, idSubgrupo);
                grdEstoque.DataBind();
    
                Glass.MensagemAlerta.ShowMsg("Estoque limpo com sucesso!", Page);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao limpar estoque.", ex, Page);
            }
        }
    
        /// <summary>
        /// M�todo acionado quando a op��o de inser��o r�pida de estoque
        /// for alterada.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void chkInsercaoRapidaEstoque_CheckedChanged(object sender, EventArgs e)
        {
            if (chkInsercaoRapidaEstoque.Checked)
            {
                grdEstoque.Columns[0].Visible = false;
                grdEstoque.Columns[4].Visible = false;
                grdEstoque.Columns[5].Visible = false;
                grdEstoque.Columns[6].Visible = false;
                grdEstoque.Columns[7].Visible = false;
                grdEstoque.Columns[8].Visible = false;
                grdEstoque.Columns[9].Visible = false;
                grdEstoque.Columns[10].Visible = false;
                grdEstoque.Columns[11].Visible = false;
                grdEstoque.Columns[12].Visible = false;
                grdEstoque.Columns[13].Visible = false;
                grdEstoque.Columns[14].Visible = false; 
    
                btnSalvarInsercaoRapida.Visible = true;
    
                if (Request["fiscal"] == "1")
                    grdEstoque.Columns[17].Visible = true;
                else
                    grdEstoque.Columns[16].Visible = true;            
            }
            else
            {
                grdEstoque.Columns[0].Visible = true;
                grdEstoque.Columns[4].Visible = true;
                grdEstoque.Columns[5].Visible = true;
                grdEstoque.Columns[6].Visible = true;
                grdEstoque.Columns[7].Visible = true;
                grdEstoque.Columns[8].Visible = true;
                grdEstoque.Columns[9].Visible = true;
                grdEstoque.Columns[10].Visible = true;
                grdEstoque.Columns[11].Visible = true;
                grdEstoque.Columns[12].Visible = true;
                grdEstoque.Columns[13].Visible = true;
                grdEstoque.Columns[14].Visible = true;
                grdEstoque.Columns[16].Visible = false;
                grdEstoque.Columns[17].Visible = false;
    
                btnSalvarInsercaoRapida.Visible = false;
            }
        }
    
        /// <summary>
        /// M�todo acionado para salvar a inser��o rapida de estoque
        /// </summary>
        protected void SalvarInsercaoRapida(object sender, EventArgs e)
        {
            try
            {
                Glass.Data.Model.ProdutoLoja produtoLoja;
                double qntdEstoque = 0, qntdEstoqueFiscal = 0;
    
                if (grdEstoque.Rows.Count < 1)
                {
                    Glass.MensagemAlerta.ShowMsg("N�o h� registros para inser��o r�pida", Page);
                    return;
                }
    
                foreach (GridViewRow row in grdEstoque.Rows)
                {
                    uint idLoja = Glass.Conversoes.StrParaUint(drpLoja.SelectedValue);
                    uint codProduto = Glass.Conversoes.StrParaUint(((HiddenField)row.Cells[15].FindControl("hdfCodProduto")).Value.Split(',')[1]);
    
                    produtoLoja = ProdutoLojaDAO.Instance.GetElement(idLoja, codProduto);
    
                    if (Request["fiscal"] == "1")
                    {
                        qntdEstoqueFiscal = Glass.Conversoes.StrParaDouble(((TextBox)row.Cells[17].FindControl("txtQtdEstoqueFiscalInsercaoRapida")).Text);
    
                        // S� altera a quantidade em estoque se tiver sido alterado na tela
                        if (qntdEstoqueFiscal == produtoLoja.EstoqueFiscal)
                            continue;
    
                        produtoLoja.EstoqueFiscal = qntdEstoqueFiscal;
                    }
                    else 
                    {
                        qntdEstoque = Glass.Conversoes.StrParaDouble(((TextBox)row.Cells[16].FindControl("txtQtdEstoqueInsercaoRapida")).Text);
    
                        // S� altera a quantidade em estoque se tiver sido alterado na tela
                        if (qntdEstoque == produtoLoja.QtdEstoque)
                            continue;
    
                        produtoLoja.QtdEstoque = qntdEstoque;
                    }
    
                    // Deve chamar este m�todo para que a movimenta��o de estoque fique correta.
                    ProdutoLojaDAO.Instance.AtualizaEstoque(produtoLoja);
                }
    
                Glass.MensagemAlerta.ShowMsg("Inser��o r�pida de estoque concluida", Page);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Erro", ex, Page);
            }
            finally
            {
                chkInsercaoRapidaEstoque.Checked = false;
                chkInsercaoRapidaEstoque_CheckedChanged(null, null);
                grdEstoque.DataBind();
            }       
        }
    
        protected void ctrlSelCorProd1_Load(object sender, EventArgs e)
        {
            (sender as Glass.UI.Web.Controls.ctrlSelCorProd).ControleGrupo = drpGrupo;
        }
    }
}
