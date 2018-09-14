using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Data.Model;

namespace Glass.UI.Web.Listas
{
    public partial class LstMovEstoque : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            if (!IsPostBack)
            {
                ((TextBox)ctrlDataIni.FindControl("txtData")).Text = DateTime.Now.AddDays(-15).ToString("dd/MM/yyyy");
                ((TextBox)ctrlDataFim.FindControl("txtData")).Text = DateTime.Now.ToString("dd/MM/yyyy");
            }
        }
    
        protected string GetCodigoTabela()
        {
            return ((int)LogCancelamento.TabelaCancelamento.MovEstoque).ToString();
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            /* Chamado 17611. */
            grdMovEstoque.DataBind();
            grdMovEstoque.PageIndex = 0;
        }
    
        protected void grdMovEstoque_DataBound(object sender, EventArgs e)
        {
            foreach (GridViewRow row in grdMovEstoque.Rows)
            {
                if (((HiddenField)row.Cells[0].FindControl("hdfTipoMov")).Value == "2")
                    foreach (TableCell c in row.Cells)
                        c.ForeColor = System.Drawing.Color.Red;
            }
        }
    
        protected void imgAdd_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (!Config.PossuiPermissao(Config.FuncaoMenuEstoque.AlterarEstoqueManualmente))
                {
                    MensagemAlerta.ShowMsg("Você não tem permissão para alterar estoque manualmente.", Page);
                    return;
                }

                uint idProd = Glass.Conversoes.StrParaUint((grdMovEstoque.FooterRow.FindControl("hdfIdProd") as HiddenField).Value);
                uint idLoja = Glass.Conversoes.StrParaUint((grdMovEstoque.FooterRow.FindControl("hdfIdLoja") as HiddenField).Value);
    
                Glass.UI.Web.Controls.ctrlData data = grdMovEstoque.FooterRow.FindControl("ctrlDataMov") as Glass.UI.Web.Controls.ctrlData;
                TextBox qtde = grdMovEstoque.FooterRow.FindControl("txtQtde") as TextBox;
                DropDownList tipo = grdMovEstoque.FooterRow.FindControl("drpTipo") as DropDownList;
                TextBox valor = grdMovEstoque.FooterRow.FindControl("txtValor") as TextBox;
                TextBox obs = grdMovEstoque.FooterRow.FindControl("txtObs") as TextBox;
                
                if (tipo.SelectedValue == "1")
                    MovEstoqueDAO.Instance.CreditaEstoqueManualComTransacao(idProd, idLoja, Glass.Conversoes.StrParaDecimal(qtde.Text),
                        Glass.Conversoes.StrParaDecimalNullable(valor.Text), data.Data, obs.Text);
                else
                    MovEstoqueDAO.Instance.BaixaEstoqueManualComTransacao(idProd, idLoja, Glass.Conversoes.StrParaDecimal(qtde.Text),
                        Glass.Conversoes.StrParaDecimalNullable(valor.Text), data.Data, obs.Text);
    
                data.DataString = null;
                qtde.Text = null;
                tipo.SelectedValue = null;
                grdMovEstoque.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir movimentação retroativa.", ex, Page);
            }
        }

        protected void cbdSubgrupoProd_DataBound(object sender, EventArgs e)
        {
            //Remove a opção de Todos, pois caso a mesma seja selecionada não busca todos, busca nada.
            cbdSubgrupoProd.Items.RemoveAt(0);
        }
    }
}
