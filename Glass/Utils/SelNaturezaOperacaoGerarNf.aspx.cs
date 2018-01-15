using System;
using Glass.Data.DAL;

namespace Glass.UI.Web.Utils
{
    public partial class SelNaturezaOperacaoGerarNf : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(SelNaturezaOperacaoGerarNf));
        }
    
        #region Métodos AJAX
    
        [Ajax.AjaxMethod()]
        public string GerarNf(string idCompra, string idNaturezaOperacao)
        {
            var produtos = Data.DAL.ProdutosCompraDAO.Instance.GetByCompra(idCompra.StrParaUint());
            return NotaFiscalDAO.Instance.GerarNfCompraComTransacao(idCompra.StrParaUint(), idNaturezaOperacao.StrParaUintNullable(), produtos).ToString();
        }
    
        #endregion
    }
}
