using System;
using System.Web.UI.WebControls;
using WebGlass.Business.ConciliacaoBancaria.Entidade;
using System.Drawing;
using Glass.Data.Helper;

namespace Glass.UI.Web.Listas
{
    public partial class LstConciliacaoBancaria : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            grdConciliacaoBancaria.Columns[0].Visible = Config.PossuiPermissao(Config.FuncaoMenuFinanceiroPagto.RealizarConciliacaoBancaria);
        }
    
        protected void grdConciliacaoBancaria_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;
    
            var item = e.Row.DataItem as ConciliacaoBancaria;
            if (item == null)
                return;
    
            if (item.Situacao == ConciliacaoBancaria.SituacaoEnum.Cancelada)
                foreach (TableCell c in e.Row.Cells)
                    c.ForeColor = Color.Red;
        }
    
        protected string ExibirConciliar()
        {
            return Config.PossuiPermissao(Config.FuncaoMenuFinanceiroPagto.RealizarConciliacaoBancaria) ? "" : "display: none";
        }
    }
}
