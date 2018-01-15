using System;
using Glass.Configuracoes;

namespace Glass.UI.Web.Listas
{
    public partial class LstConsultaProd : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                grdProduto.Columns[7].Visible = PedidoConfig.LiberarPedido;
                hdfApenasProdAtivos.Value = ProdutosAtivos.ToString();
            }
        }
    
        protected void lnkPesq_Click(object sender, EventArgs e)
        {
            grdProduto.PageIndex = 0;
        }
    
        /// <summary>
        /// Verifica se na lista de produtos deve ser mostrado apenas
        /// os produtos ativos.
        /// </summary>
        /// <returns></returns>
        protected int ProdutosAtivos
        {
            get
            {
                if (ProdutoConfig.TelaListagem.BuscarApenasProdutosAtivosConsultaProd)
                    return 1;
    
                return 0;
            }
        }
    }
}
