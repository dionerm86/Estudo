using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Listas
{
    public partial class LstSinaisRecebidos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            inserirPagtoAntecipado.Visible = Request["antecipado"] == "1";
            hdfPagtoAntecipado.Value = inserirPagtoAntecipado.Visible.ToString().ToLower();
    
            if (inserirPagtoAntecipado.Visible)
            {
                Page.Title = "Pagamentos Antecipados de Pedido Recebidos";
                
                grdSinaisReceber.Columns[1].HeaderText = "Núm. Pagto. Antecipado";
                grdSinaisReceber.Columns[3].HeaderText = "Valor Pagto. Antecipado";
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdSinaisReceber.PageIndex = 0;
        }
    
        protected void imbCancelar_Load(object sender, EventArgs e)
        {
            if (Request["antecipado"] == "1")
                ((ImageButton)sender).OnClientClick = ((ImageButton)sender).OnClientClick.Replace("sinal", "pagamento antecipado");
        }
    
        protected void lnkInserirPagto_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Cadastros/CadReceberSinal.aspx?antecipado=1" + (Request["cxDiario"] == "1" ? "&cxDiario=1" : ""));
        }
    }
}
