using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Data.Model;

namespace Glass.UI.Web.Listas
{
    public partial class LstMovEstoqueCliente : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            if (!IsPostBack)
            {
                ((TextBox)ctrlDataIni.FindControl("txtData")).Text = DateTime.Now.AddDays(-15).ToString("dd/MM/yyyy");
                ((TextBox)ctrlDataFim.FindControl("txtData")).Text = DateTime.Now.ToString("dd/MM/yyyy");
            }
    
            grdMovEstoque.ShowFooter = true;
        }
    
        protected string GetCodigoTabela()
        {
            return ((int)LogCancelamento.TabelaCancelamento.MovEstoqueCliente).ToString();
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
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
                uint idCliente = Glass.Conversoes.StrParaUint((grdMovEstoque.FooterRow.FindControl("hdfCodigoCliente") as HiddenField).Value);
                uint idProd = Glass.Conversoes.StrParaUint((grdMovEstoque.FooterRow.FindControl("hdfCodigoProduto") as HiddenField).Value);
                uint idLoja = Glass.Conversoes.StrParaUint((grdMovEstoque.FooterRow.FindControl("hdfCodigoLoja") as HiddenField).Value);
                
                Glass.UI.Web.Controls.ctrlData data = grdMovEstoque.FooterRow.FindControl("ctrlDataMov") as Glass.UI.Web.Controls.ctrlData;
            TextBox qtde = grdMovEstoque.FooterRow.FindControl("txtQtde") as TextBox;
            DropDownList tipo = grdMovEstoque.FooterRow.FindControl("drpTipo") as DropDownList;
            TextBox valor = grdMovEstoque.FooterRow.FindControl("txtValor") as TextBox;
            TextBox obs = grdMovEstoque.FooterRow.FindControl("txtObservacao") as TextBox;
            
            if (tipo.SelectedValue == "1")
                WebGlass.Business.MovimentacaoEstoqueCliente.Fluxo.CRUD.Instance.CreditaEstoqueManual(idCliente, idProd, idLoja, 
                    Glass.Conversoes.StrParaDecimal(qtde.Text), Glass.Conversoes.StrParaDecimalNullable(valor.Text), data.Data, obs.Text);
            else
                WebGlass.Business.MovimentacaoEstoqueCliente.Fluxo.CRUD.Instance.BaixaEstoqueManual(idCliente, idProd, idLoja,
                    Glass.Conversoes.StrParaDecimal(qtde.Text), Glass.Conversoes.StrParaDecimalNullable(valor.Text), data.Data, obs.Text);

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
    }
}
