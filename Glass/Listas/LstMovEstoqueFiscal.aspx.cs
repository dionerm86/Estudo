using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Model;

namespace Glass.UI.Web.Listas
{
    public partial class LstMovEstoqueFiscal : System.Web.UI.Page
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
            return ((int)LogCancelamento.TabelaCancelamento.MovEstoqueFiscal).ToString();
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            /* Chamado 17611. */
            grdMovEstoqueFiscal.DataBind();
            grdMovEstoqueFiscal.PageIndex = 0;
        }
    
        protected void grdMovEstoqueFiscal_DataBound(object sender, EventArgs e)
        {
            foreach (GridViewRow row in grdMovEstoqueFiscal.Rows)
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
                uint idProd = Glass.Conversoes.StrParaUint((grdMovEstoqueFiscal.FooterRow.FindControl("hdfIdProd") as HiddenField).Value);
                uint idLoja = Glass.Conversoes.StrParaUint((grdMovEstoqueFiscal.FooterRow.FindControl("hdfIdLoja") as HiddenField).Value);
    
                Glass.UI.Web.Controls.ctrlData data = grdMovEstoqueFiscal.FooterRow.FindControl("ctrlDataMov") as Glass.UI.Web.Controls.ctrlData;
                TextBox qtde = grdMovEstoqueFiscal.FooterRow.FindControl("txtQtde") as TextBox;
                DropDownList tipo = grdMovEstoqueFiscal.FooterRow.FindControl("drpTipo") as DropDownList;
                TextBox valor = grdMovEstoqueFiscal.FooterRow.FindControl("txtValor") as TextBox;
                TextBox obs = grdMovEstoqueFiscal.FooterRow.FindControl("txtObs") as TextBox;
    
                if (tipo.SelectedValue == "1")
                    MovEstoqueFiscalDAO.Instance.CreditaEstoqueManual(idProd, idLoja, Glass.Conversoes.StrParaDecimal(qtde.Text),
                        Glass.Conversoes.StrParaDecimalNullable(valor.Text), data.Data, obs.Text);
                else
                    MovEstoqueFiscalDAO.Instance.BaixaEstoqueManual(idProd, idLoja, Glass.Conversoes.StrParaDecimal(qtde.Text),
                        Glass.Conversoes.StrParaDecimalNullable(valor.Text), data.Data, obs.Text);
    
                data.DataString = null;
                qtde.Text = null;
                tipo.SelectedValue = null;
                grdMovEstoqueFiscal.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir movimentação retroativa.", ex, Page);
            }
        }
    }
}
