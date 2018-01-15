using Glass.Configuracoes;
using System;

namespace Glass.UI.Web.Utils
{
    public partial class SelProdutoRepos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Geral.NaoVendeVidro())
            {
                // Esconde colunas Apl, Proc e V. Benef.
                grdProdutos.Columns[11].Visible = false;
                grdProdutos.Columns[12].Visible = false;
            }
            else
            {
                grdProdutos.Columns[grdProdutos.Columns.Count - 2].Visible = false;
                grdProdutos.Columns[grdProdutos.Columns.Count - 3].Visible = false;
            }
    
            grdProdutos.Columns[4].Visible = Glass.Configuracoes.ProducaoConfig.ReporApenasProduzidos;
            grdProdutos.Columns[5].Visible = Glass.Configuracoes.ProducaoConfig.ReporApenasProduzidos;
            grdProdutos.Columns[6].Visible = !Glass.Configuracoes.ProducaoConfig.ReporApenasProduzidos;
        }
    }
}
