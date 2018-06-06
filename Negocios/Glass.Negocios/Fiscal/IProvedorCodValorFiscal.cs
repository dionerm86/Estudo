using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Fiscal.Negocios
{
    /// <summary>
    /// Assinatura do provedor do código do valor fiscal.
    /// </summary>
    public interface IProvedorCodValorFiscal
    {
        /// <summary>
        /// Recupera o código do valor fiscal.
        /// </summary>
        /// <param name="tipoDocumento"></param>
        /// <param name="loja"></param>
        /// <param name="cst"></param>
        /// <returns></returns>
        int? ObterCodValorFiscal(
            Sync.Fiscal.Enumeracao.NFe.TipoDocumento tipoDocumento,
            Global.Negocios.Entidades.Loja loja, 
            Sync.Fiscal.Enumeracao.Cst.CstIcms? cst);
    }
}
