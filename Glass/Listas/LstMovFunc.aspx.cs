using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using System.Drawing;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Listas
{
    public partial class LstMovFunc : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ((TextBox)ctrlDataIni.FindControl("txtData")).Text = DateTime.Now.AddDays(-15).ToString("dd/MM/yyyy");
                ((TextBox)ctrlDataFim.FindControl("txtData")).Text = DateTime.Now.ToString("dd/MM/yyyy");
            }
    
            grdMovFunc.Columns[2].Visible = PedidoConfig.LiberarPedido;
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
    
        }
    
        protected void grdMovFunc_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.DataItem == null)
                return;
    
            int tipo = ((MovFunc)e.Row.DataItem).TipoMov;
            if (tipo == 2)
                foreach (TableCell c in e.Row.Cells)
                    c.ForeColor = Color.Red;
    
            foreach (GridViewRow row in grdMovFunc.Rows)
            {
                if (string.IsNullOrEmpty(((Label)row.Cells[0].FindControl("lblFunc")).Text))
                {
                    row.ForeColor = System.Drawing.Color.Black;
                    row.Font.Bold = true;
                }
            }
    
        }
    
        protected void grdMovFunc_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                uint idMovFunc = Glass.Conversoes.StrParaUint(((ImageButton)e.CommandSource).CommandArgument);
                MovFuncDAO.Instance.DeletaMov(idMovFunc);
                this.grdMovFunc.DataBind();
            }
            catch
            {
                Glass.MensagemAlerta.ErrorMsg("Não foi possível cancelar a movimentação.", null, this);
            }
        }
    }
}
