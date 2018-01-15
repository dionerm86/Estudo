using System;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Listas
{
    public partial class LstAssociarPropVeic : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
    
        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Cadastros/CadAssociarPropVeic.aspx");
        }
    
        protected void odsAssociarProprietarioVeiculo_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao desfazer associação.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
        protected void odsAssociarProprietarioVeiculo_Selected(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
           
        }
        protected void grdAssociarProprietarioVeiculo_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            var proprietarios = Glass.Data.DAL.CTe.ProprietarioVeiculoDAO.Instance.GetAll();
            if (e != null && e.Row != null && e.Row.DataItem != null && e.Row.Cells != null && e.Row.Cells.Count > 1 &&
                proprietarios != null && proprietarios.Count() > 0)
                e.Row.Cells[1].Text = proprietarios.Where(
                    f => f.IdPropVeic == ((Glass.Data.Model.Cte.ProprietarioVeiculo_Veiculo)e.Row.DataItem).IdPropVeic
                    ).FirstOrDefault().Nome;
        }
    }
}
