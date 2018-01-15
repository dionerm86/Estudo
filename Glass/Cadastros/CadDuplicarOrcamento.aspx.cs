using System;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadDuplicarOrcamento : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadDuplicarOrcamento));
            Ajax.Utility.RegisterTypeForAjax(typeof(RecalcularOrcamento));
        }
    
        [Ajax.AjaxMethod]
        public string GetDadosOrcamento(string idOrcamentoStr)
        {
            return WebGlass.Business.Orcamento.Fluxo.BuscarEValidar.Ajax.GetDadosOrcamento(idOrcamentoStr);
        }
    
        [Ajax.AjaxMethod]
        public string Duplicar(string idOrcamentoStr)
        {
            return WebGlass.Business.Orcamento.Fluxo.Duplicar.Ajax.DuplicarOrcamento(idOrcamentoStr);
        }
    
        protected void ctrlBenef1_Load(object sender, EventArgs e)
        {
            ctrlBenef1.CampoClienteID = hdfIdCliente;
            ctrlBenef1.CampoPercComissao = hdfPercComissao;
            ctrlBenef1.CampoRevenda = hdfRevenda;
            ctrlBenef1.CampoTipoEntrega = hdfTipoEntrega;
            ctrlBenef1.CampoAltura = hdfBenefAltura;
            ctrlBenef1.CampoEspessura = hdfBenefEspessura;
            ctrlBenef1.CampoLargura = hdfBenefLargura;
            ctrlBenef1.CampoProdutoID = hdfBenefIdProd;
            ctrlBenef1.CampoQuantidade = hdfBenefQtde;
            ctrlBenef1.CampoTotalM2 = hdfBenefTotM;
            ctrlBenef1.CampoValorUnitario = hdfBenefValorUnit;
        }
    }
}
