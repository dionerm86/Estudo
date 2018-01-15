using Glass.Data.DAL;
using System;

namespace Glass.UI.Web.Utils
{
    public partial class DetalhesReposicaoPeca : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                lblEtiqueta.Text = ProdutoPedidoProducaoDAO.Instance.ObtemEtiqueta(Request["idProdPedProducao"].StrParaUint());
        }
    }
}