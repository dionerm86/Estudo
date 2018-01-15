using System;
using System.Web.UI;

namespace Glass.UI.Web.Listas
{
    public partial class LstPecasSemEntrada : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            /* filtros.Visible = txtNumCli.Text != "" || txtNome.Text != "";
            separador.Visible = filtros.Visible;
            lista.Visible = filtros.Visible; */
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdProdutoSemEntrada.PageIndex = 0;
        }
    
        protected bool EsconderColunas()
        {
            return Glass.Configuracoes.Geral.NaoVendeVidro();
        }
    }
}
