using System;
using System.Web.UI;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadAlterarDadosFiscais : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadAlterarDadosFiscais));
    
            Page.ClientScript.RegisterStartupScript(GetType(), "loadSubgrupos", @"
                document.getElementById('" + drpGrupo.ClientID + @"').onchange(); ", true);
    
            cbxICMS.Attributes.Add("OnClick", "enableControleTabela('cbxICMS', " + ctrlIcmsProdutoPorUf.ClientID + ")");
            cbxMVA.Attributes.Add("OnClick", "enableControleTabela('cbxMVA', " + ctrlMvaProdutoPorUf.ClientID + ")");
        }
    
        [Ajax.AjaxMethod]
        public string LoadSubgrupos(string idGrupoProdStr, string isNenhum)
        {
            return WebGlass.Business.SubgrupoProd.Fluxo.BuscarEValidar.Ajax.LoadSubgruposAlterarDados(idGrupoProdStr, isNenhum);
        }
    
        [Ajax.AjaxMethod]
        public string BuscarProdutos(string codInterno, string descricao, string idGrupoProdStr, string idSubgrupoProdStr,
            string ncmIni, string ncmFim)
        {
            return WebGlass.Business.Produto.Fluxo.BuscarEValidar.Ajax.BuscarProdutosAlterarDados(codInterno, descricao,
                idGrupoProdStr, idSubgrupoProdStr, ncmIni, ncmFim);
        }
    
        [Ajax.AjaxMethod]
        public string AlterarDados(string idsProd, string novaAliqICMS, string novaAliqICMSST, string novaAliqIPI, 
            string novaMVA, string novaNCM, string cst, string cstIpi, string csosn, string codEx, 
            string genProd, string tipoMerc, string planoContabil, string substituirICMS, string substituirMVA,
            string AlterarICMS, string alterarMVA, string cest)
        {
            return WebGlass.Business.Produto.Fluxo.AlterarDados.Ajax.AlterarDadosFiscais(idsProd, novaAliqICMS,
                novaAliqICMSST, novaAliqIPI, novaMVA, novaNCM, cst, cstIpi, csosn, codEx, genProd, tipoMerc,
                planoContabil, substituirICMS, substituirMVA, AlterarICMS, alterarMVA, cest);
        }
    }
}
