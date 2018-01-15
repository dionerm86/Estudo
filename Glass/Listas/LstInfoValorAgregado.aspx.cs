using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Model;

namespace Glass.UI.Web.Listas
{
    public partial class LstInfoValorAgregado : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void grdInfoValorAgregado_DataBound(object sender, EventArgs e)
        {
            if (grdInfoValorAgregado.Rows.Count == 1)
                grdInfoValorAgregado.Rows[0].Visible = InfoValorAgregadoDAO.Instance.GetCountReal() > 0;
        }
    
        protected void grdInfoValorAgregado_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdInfoValorAgregado.ShowFooter = e.CommandName != "Edit";
        }
    
        protected void imgInserir_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                uint idProd = (grdInfoValorAgregado.FooterRow.FindControl("ctrlSelProduto")
                    as Glass.UI.Web.Controls.ctrlSelProduto).IdProd.GetValueOrDefault();
    
                uint idCidade = (grdInfoValorAgregado.FooterRow.FindControl("ctrlSelCidade")
                    as Glass.UI.Web.Controls.ctrlSelCidade).IdCidade.GetValueOrDefault();
    
                DateTime data = (grdInfoValorAgregado.FooterRow.FindControl("ctrlData")
                    as Glass.UI.Web.Controls.ctrlData).Data;
    
                decimal valor = Glass.Conversoes.StrParaDecimal((grdInfoValorAgregado.FooterRow.FindControl("txtValor")
                    as TextBox).Text);
    
                InfoValorAgregado info = new InfoValorAgregado();
                info.IdProd = idProd;
                info.IdCidade = idCidade;
                info.Data = data;
                info.Valor = valor;
    
                InfoValorAgregadoDAO.Instance.Insert(info);
                grdInfoValorAgregado.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir valor agregado.", ex, Page);
            }
        }
    }
}
