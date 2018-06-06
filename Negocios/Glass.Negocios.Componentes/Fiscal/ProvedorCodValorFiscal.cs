using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Fiscal.Negocios.Componentes
{
    /// <summary>
    /// Implementação do provedor do código do valor fiscal.
    /// </summary>
    public class ProvedorCodValorFiscal : IProvedorCodValorFiscal
    {
        /// <summary>
        /// Recupera o código do valor fiscal.
        /// </summary>
        /// <param name="tipoDocumento"></param>
        /// <param name="loja"></param>
        /// <param name="cst"></param>
        /// <returns></returns>
        public int? ObterCodValorFiscal(
            Sync.Fiscal.Enumeracao.NFe.TipoDocumento tipoDocumento,
            Global.Negocios.Entidades.Loja loja,
            Sync.Fiscal.Enumeracao.Cst.CstIcms? cst)
        {
            var simplesNacional =
                loja.Crt == Data.Model.CrtLoja.SimplesNacional ||
                loja.Crt == Data.Model.CrtLoja.SimplesNacionalExcSub;

            return (int?)Data.DAL.NotaFiscalDAO.Instance.ObtemCodValorFiscal(
                (int)tipoDocumento, 
                cst.HasValue ? ((int)cst).ToString("00") : null, 
                simplesNacional);
        }
    }
}
