using System;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadMovInternaEstoqueFiscal : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(CadMovInternaEstoqueFiscal));
        }

        [Ajax.AjaxMethod()]
        public void MovimentarEstoque(string codProdOrigem, string codProdDestino, string qtdeOrigem, string qtdeDestino, string idLoja)
        {
                Data.DAL.MovInternaEstoqueFiscalDAO.Instance
                    .SalvarMovimentacao(codProdOrigem, codProdDestino, qtdeOrigem.StrParaDecimal(), qtdeDestino.StrParaDecimal(), idLoja.StrParaInt());
        }
    }
}