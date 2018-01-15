using System;
using System.Web.UI;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadAlterarGrupo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadAlterarGrupo));
    
            Page.ClientScript.RegisterStartupScript(GetType(), "loadSubgrupos", @"
                document.getElementById('" + drpGrupo.ClientID + @"').onchange();
                document.getElementById('" + drpNovoGrupo.ClientID + "').onchange(); ", true);
        }
    
        protected void drpNovoGrupo_DataBound(object sender, EventArgs e)
        {
            drpNovoGrupo.Items.RemoveAt(0);
        }
    
        [Ajax.AjaxMethod]
        public string LoadSubgrupos(string idGrupoProdStr, string isNenhum)
        {
            return WebGlass.Business.SubgrupoProd.Fluxo.BuscarEValidar.Ajax.LoadSubgruposAlterarGrupo(idGrupoProdStr, isNenhum);
        }
    
        [Ajax.AjaxMethod]
        public string BuscarProdutos(string codInterno, string descricao, string idGrupoProdStr, string idSubgrupoProdStr)
        {
            return WebGlass.Business.Produto.Fluxo.BuscarEValidar.Ajax.BuscarProdutosAlterarGrupo(codInterno, descricao, 
                idGrupoProdStr, idSubgrupoProdStr);
        }
    
        [Ajax.AjaxMethod]
        public string Alterar(string idsProd, string idNovoGrupo, string idNovoSubgrupo)
        {
            return WebGlass.Business.Produto.Fluxo.AlterarDados.Ajax.AlterarGrupo(idsProd, idNovoGrupo, idNovoSubgrupo);
        }
    }
}
