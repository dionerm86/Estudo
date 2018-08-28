using Glass.Data.Helper;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Utils
{
    public partial class SetNaturezaOperacaoProdutoGerarNf : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(Utils.SetNaturezaOperacaoProdutoGerarNf));

            pedidos.Visible = Request["idsCompras"] == null;
            compras.Visible = !pedidos.Visible;
        }

        protected void ctrlNaturezaOperacao_PreRender(object sender, EventArgs e)
        {
            if (compras.Visible)
                return;

            uint idCliente = Glass.Conversoes.StrParaUint(Request["idCliente"]);
            uint? idNaturezaOperacaoNFe = Glass.Conversoes.StrParaUintNullable(Request["idNaturezaOperacao"]);
            int idProd = Glass.Conversoes.StrParaInt(((sender as Control).Parent.FindControl("hdfIdProd") as HiddenField).Value);

            var inst = WebGlass.Business.RegraNaturezaOperacao.Fluxo.BuscarEValidar.Instance;
            (sender as Controls.ctrlNaturezaOperacao).CodigoNaturezaOperacao = inst.BuscaCodigoNaturezaOperacaoPorRegra(true, UserInfo.GetUserInfo.IdLoja, idCliente, idProd);
        }
    }
}
