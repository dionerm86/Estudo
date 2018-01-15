using System;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.UI.Web.Utils
{
    public partial class ShowPrecoAnterior : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            PrecoAnterior produto = PrecoAnteriorDAO.Instance.GetElement(Glass.Conversoes.StrParaUint(Request["idProd"]));
            lblProduto.Text = produto.Descricao;
            lblFabBase.Text = produto.Custofabbase.ToString("C");
            lblCustoCompra.Text = produto.CustoCompra.ToString("C");
            lblAtacado.Text = produto.ValorAtacado.ToString("C");
            lblBalcao.Text = produto.ValorBalcao.ToString("C");
            lblObra.Text = produto.ValorObra.ToString("C");
        }
    }
}
