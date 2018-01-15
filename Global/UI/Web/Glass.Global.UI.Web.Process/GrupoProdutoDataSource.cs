using Microsoft.Practices.ServiceLocation;
using System.Collections.Generic;
using System.Linq;

namespace Glass.Global.UI.Web.Process
{
    /// <summary>
    /// Classe com as fontes de dados dos grupos de produtos.
    /// </summary>
    public static class GrupoProdutoDataSource
    {
        /// <summary>
        /// Retorna uma lista com os tipos de cálculo.
        /// </summary>
        /// <param name="exibirDecimal">Identifica se é para exibir os tipos de calculo com decimal.</param>
        /// <param name="notaFiscal">Identifica se é para exibir os tipos de calculo de nota fiscal.</param>
        /// <returns></returns>
        public static IEnumerable<Colosoft.Globalization.TranslateInfo> ObtemTiposCalculo(bool exibirDecimal, bool notaFiscal)
        {
            var fluxo = ServiceLocator.Current.GetInstance<Global.Negocios.IGrupoProdutoFluxo>();

            return fluxo
                .ObtemTiposCalculo(exibirDecimal, notaFiscal)
                .Select(f => new Colosoft.Globalization.TranslateInfo(f, Colosoft.Translator.Translate(f)));
        }
    }
}
