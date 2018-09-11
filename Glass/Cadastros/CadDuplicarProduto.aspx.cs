using System;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadDuplicarProduto : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadDuplicarProduto));
        }

        [Ajax.AjaxMethod]
        public string GetSubgrupos(string idGrupo, string textoVazio)
        {
            return WebGlass.Business.SubgrupoProd.Fluxo.BuscarEValidar.Ajax.GetSubgrupos(idGrupo, textoVazio);
        }

        [Ajax.AjaxMethod]
        public string GetDadosProduto(string codInterno)
        {
            return WebGlass.Business.Produto.Fluxo.BuscarEValidar.Ajax.GetDadosProduto(codInterno);
        }

        [Ajax.AjaxMethod]
        public string GetProdutosGrupoSubgrupo(string idGrupo, string idSubgrupo)
        {
            return WebGlass.Business.Produto.Fluxo.BuscarEValidar.Ajax.GetProdutosGrupoSubgrupo(idGrupo, idSubgrupo, 1);
        }

        [Ajax.AjaxMethod]
        public string Duplicar(string idsProd, string idNovoGrupo, string idNovoSubgrupo, string codInternoRemover,
            string codInternoSubstituir, string descricaoRemover, string descricaoSubstituir, string novaAltura, string novaLargura,
            string processo, string aplicacao)
        {
            return WebGlass.Business.Produto.Fluxo.Duplicar.Ajax.DuplicarProduto(idsProd, idNovoGrupo, idNovoSubgrupo,
                codInternoRemover, codInternoSubstituir, descricaoRemover, descricaoSubstituir, novaAltura, novaLargura,
                processo, aplicacao);
        }
    }
}
