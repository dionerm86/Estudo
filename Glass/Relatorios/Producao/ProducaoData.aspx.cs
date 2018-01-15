using System;
using System.Web.UI;
using Glass.Data.Model;

namespace Glass.UI.Web.Relatorios.Producao
{
    public partial class ProducaoData : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            if (!IsPostBack)
            {
                ctrlDataFim.Data = DateTime.Today;
                ctrlDataIni.Data = ctrlDataFim.Data.AddDays(-7);
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdProducaoData.PageIndex = 0;
        }
    
        protected void drpSituacao_DataBound(object sender, EventArgs e)
        {
            // Remove a situação "Cancelado"
            drpSituacao.Items.Remove(drpSituacao.Items.FindByValue(((int)Glass.Data.Model.Pedido.SituacaoPedido.Cancelado).ToString()));
        }
    }
}
