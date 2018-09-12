using System;
using System.Web.UI;

namespace Glass.UI.Web.Listas
{
    public partial class LstEstoque : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["fiscal"] == "1")
                Page.Title = "Estoque Fiscal de Produtos";

            //if (!Config.PossuiPermissao(Config.FuncaoMenuEstoque.AlterarEstoqueManualmente))
            //    chkInsercaoRapidaEstoque.Style.Add("display", "none");
        }

        /// <summary>
        /// Método acionado para salvar a inserção rapida de estoque
        /// </summary>
        protected void SalvarInsercaoRapida(object sender, EventArgs e)
        {/*
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
            }*/
        }
    }
}
