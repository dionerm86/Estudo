using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;

namespace Glass.UI.Web.Utils
{
    public partial class AtrasarPedidoFunc : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            grdFuncionario.Columns[2].Visible = LojaDAO.Instance.GetCount() > 1;
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
    
        }
    
        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow g in grdFuncionario.Rows)
            {
                uint idFunc = Glass.Conversoes.StrParaUint(((HiddenField)g.FindControl("hdfIdFunc")).Value);
                int numDiasAtraso = Glass.Conversoes.StrParaInt(((TextBox)g.FindControl("txtAtrasar")).Text);
                FuncionarioDAO.Instance.AtrasarFunc(idFunc, numDiasAtraso);
            }
    
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "fechar", "alert('Dados salvos com sucesso!'); closeWindow();\n", true);
        }
    }
}
