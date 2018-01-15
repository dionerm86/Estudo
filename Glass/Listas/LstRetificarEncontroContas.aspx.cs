using System;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Listas
{
    public partial class LstRetificarEncontroContas : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            hdfIdEncontroContas.Value = selEncontroContas.Valor;
        }
    
        protected void btnRetificarEncontroContas_Click(object sender, EventArgs e)
        {
            try
            {
                int idEncontroContas = Glass.Conversoes.StrParaInt(hdfIdEncontroContas.Value);
                string dtVenc = ((TextBox)ctrlDataVenc.FindControl("txtData")).Text;
                string msg = WebGlass.Business.EncontroContas.Fluxo.EncontroContas.Instance.Retificar(idEncontroContas, hdfIdsContasPg.Value, hdfIdsContasR.Value, dtVenc);
    
                Glass.MensagemAlerta.ShowMsg(msg, Page);
                FuncoesGerais.RedirecionaPagina("../Listas/lstEncontroContas.aspx", Page);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao retificar sinal.", ex, Page);
            }
        }
    
        protected void grdContaPgEncontroContas_DataBound(object sender, EventArgs e)
        {
            bool visible = grdContaPgEncontroContas.Rows.Count > 0;
    
            captionContasPg.Visible = visible;
            captionContasR.Visible = visible;
            tbDtVenc.Visible = visible;
            btnRetificarEncontroContas.Visible = visible;
        }
    }
}
