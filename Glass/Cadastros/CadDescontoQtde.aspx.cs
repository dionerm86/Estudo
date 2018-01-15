using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadDescontoQtde : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(Request["idProd"] != null)
            lblProd.Text = ProdutoDAO.Instance.GetDescrProduto(Glass.Conversoes.StrParaInt(Request["idProd"]));
        }
    
        protected void imbAdd_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                string qtde = ((TextBox)grdDescontoQtde.FooterRow.FindControl("txtQtde")).Text;
                string percDesconto = ((TextBox)grdDescontoQtde.FooterRow.FindControl("txtPercDesconto")).Text;
    
                DescontoQtde desc = new DescontoQtde();
                desc.IdProd = Glass.Conversoes.StrParaUint(Request["idProd"]);
                desc.Qtde = Glass.Conversoes.StrParaInt(qtde);
                desc.PercDescontoMax = float.Parse(percDesconto);
    
                DescontoQtdeDAO.Instance.Insert(desc);
                grdDescontoQtde.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir desconto.", ex, Page);
            }
        }
    
        protected void grdDescontoQtde_PreRender(object sender, EventArgs e)
        {
            if (Request["idProd"] != null)
            if (DescontoQtdeDAO.Instance.GetCountByProd(Glass.Conversoes.StrParaUint(Request["idProd"])) == 0)
                grdDescontoQtde.Rows[0].Visible = false;
        }
    }
}
