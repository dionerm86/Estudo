using System;
using System.Linq;
using Glass.Data.DAL;
using Glass.Data.Model;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadArquivoFCI : System.Web.UI.Page
    { 
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadArquivoFCI));
        }
    
        [Ajax.AjaxMethod()]
        public string AddProduto(string prodNf, string codProd, string idsProd, bool alertaProdAdd)
        {
            var idProd = Glass.Conversoes.StrParaInt(codProd);
            var idProdNf = Glass.Conversoes.StrParaUint(prodNf); 
    
            if (idProd == 0)
                throw new Exception("Nenhum produto foi informado.");
    
            if (idsProd.Split(',').Where(f => Glass.Conversoes.StrParaUint(f) == idProd).Count() > 0)
            {
                if (alertaProdAdd)
                {
                    var descricao = ProdutoDAO.Instance.ObtemDescricao(idProd);
                    var codInterno = ProdutoDAO.Instance.GetCodInterno(idProd);
                    throw new Exception("O produto " + codInterno + " - " + descricao + " já foi informado.");
                }
                else
                    throw new Exception("");
            }
    
            decimal parcelaImportada = 0; 
            decimal saidaInterestadual = 0;
            decimal conteudoImportação = 0;
    
            if (idProdNf > 0)
            {
                var pnf = ProdutosNfDAO.Instance.GetElement(idProdNf);
    
                parcelaImportada = pnf.ParcelaImportada;
                saidaInterestadual = pnf.SaidaInterestadual;
                conteudoImportação = pnf.ConteudoImportacao;
            }
            else
            {
                parcelaImportada = ProdutosNfDAO.Instance.CalculaValorParcelaImportada((uint)idProd, DateTime.Now);
                saidaInterestadual = ProdutosNfDAO.Instance.CalculaValorSaidaInterestadual((uint)idProd, DateTime.Now);
                conteudoImportação = ProdutosNfDAO.Instance.CalculaConteudoImportacao((uint)idProd, DateTime.Now);
            }
    
            var retorno = new string[] 
            {
                idProd.ToString(),
                ProdutoDAO.Instance.GetCodInterno(idProd) + " - " + ProdutoDAO.Instance.ObtemDescricao(idProd),
                parcelaImportada.ToString(),
                saidaInterestadual.ToString(),
                conteudoImportação.ToString()
            };
    
            return string.Join(";", retorno);
    }

    [Ajax.AjaxMethod()]
    public string BuscaProdutosNf(string numeroNFe)
    {
        var nota = NotaFiscalDAO.Instance.GetByNumeroNFe(Conversoes.StrParaUint(numeroNFe), (int)NotaFiscal.TipoDoc.Saída);

        if (nota.Length == 0)
            throw new Exception("Nenhum produto foi encontrado");

        var prodsNf = ProdutosNfDAO.Instance.GetIdsProdForGerarFci(((NotaFiscal)nota.GetValue(0)).IdNf);

        if (string.IsNullOrEmpty(prodsNf))
            throw new Exception("Nenhum produto foi encontrado");

            return prodsNf;
        }
    
        [Ajax.AjaxMethod()]
        public string GeraArquivoFci(string dados)
        {
            return WebGlass.Business.FCI.Fluxo.ArquivoFCIFluxo.Instance.GerarArquivoFci(dados).ToString();
        }   
    }
}
