using System;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadPrecoProduto : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadPrecoProduto));
        }
    
        [Ajax.AjaxMethod()]
        public string GetProduto(string codInterno, string tipoPreco)
        {
            return WebGlass.Business.Produto.Fluxo.BuscarEValidar.Ajax.GetProdutoPreco(codInterno, tipoPreco);
        }
    
        [Ajax.AjaxMethod()]
        public string AtualizaPreco(string idProd, string tipoPreco, string preco)
        {
            return WebGlass.Business.Produto.Fluxo.Valor.Ajax.AtualizaPreco(idProd, tipoPreco, preco);
        }
    }
}
