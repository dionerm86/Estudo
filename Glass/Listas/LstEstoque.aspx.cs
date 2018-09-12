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

            if (!Config.PossuiPermissao(Config.FuncaoMenuEstoque.AlterarEstoqueManualmente))
                chkInsercaoRapidaEstoque.Style.Add("display", "none");
        }

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
        /// Método acionado para salvar a inserção rapida de estoque
        /// </summary>
        protected void SalvarInsercaoRapida(object sender, EventArgs e)
        {
            try
            {
                Glass.Data.Model.ProdutoLoja produtoLoja;
                double qntdEstoque = 0, qntdEstoqueFiscal = 0;

                if (grdEstoque.Rows.Count < 1)
                {
                    Glass.MensagemAlerta.ShowMsg("Não há registros para inserção rápida", Page);
                    return;
                }

                foreach (GridViewRow row in grdEstoque.Rows)
                {
                    uint idLoja = 0;//Glass.Conversoes.StrParaUint(drpLoja.SelectedValue);
                    uint codProduto = Glass.Conversoes.StrParaUint(((HiddenField)row.Cells[15].FindControl("hdfCodProduto")).Value.Split(',')[1]);

                    produtoLoja = ProdutoLojaDAO.Instance.GetElement(idLoja, codProduto);

                    if (Request["fiscal"] == "1")
                    {
                        qntdEstoqueFiscal = Glass.Conversoes.StrParaDouble(((TextBox)row.Cells[17].FindControl("txtQtdEstoqueFiscalInsercaoRapida")).Text);

                        // Só altera a quantidade em estoque se tiver sido alterado na tela
                        if (qntdEstoqueFiscal == produtoLoja.EstoqueFiscal)
                            continue;

                        produtoLoja.EstoqueFiscal = qntdEstoqueFiscal;
                    }
                    else
                    {
                        qntdEstoque = Glass.Conversoes.StrParaDouble(((TextBox)row.Cells[16].FindControl("txtQtdEstoqueInsercaoRapida")).Text);

                        // Só altera a quantidade em estoque se tiver sido alterado na tela
                        if (qntdEstoque == produtoLoja.QtdEstoque)
                            continue;

                        produtoLoja.QtdEstoque = qntdEstoque;
                    }

                    // Deve chamar este método para que a movimentação de estoque fique correta.
                    ProdutoLojaDAO.Instance.AtualizaEstoque(produtoLoja);
                }

                Glass.MensagemAlerta.ShowMsg("Inserção rápida de estoque concluida", Page);
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
    }
}
