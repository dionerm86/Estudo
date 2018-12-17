using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Fiscal.Negocios
{
    /// <summary>
    /// Assinatura de uma item do resultado do calculo de imposto.
    /// </summary>
    public interface IItemCalculoImpostoResultado
    {
        #region Propriedades

        /// <summary>
        /// Referência do sobre o qual os impostos foram calculados.
        /// </summary>
        IItemImposto Referencia { get; }

        /// <summary>
        /// Natureza de operação.
        /// </summary>
        Entidades.NaturezaOperacao NaturezaOperacao { get; }

        /// <summary>
        /// Valor do frete.
        /// </summary>
        decimal ValorFrete { get; }

        /// <summary>
        /// Valor do seguro.
        /// </summary>
        decimal ValorSeguro { get; }

        /// <summary>
        /// Valor das outras despesas.
        /// </summary>
        decimal ValorOutrasDespesas { get; }

        /// <summary>
        /// Valor de desconto.
        /// </summary>
        decimal ValorDesconto { get; }

        /// <summary>
        /// Valor do IPI.
        /// </summary>
        decimal ValorIpi { get; }

        /// <summary>
        /// Aliquota do IPI.
        /// </summary>
        float AliqIpi { get; }

        /// <summary>
        /// Aliquota do ICMS.
        /// </summary>
        float AliqIcms { get; }

        /// <summary>
        /// Base de calc. do ICMS
        /// </summary>
        decimal BcIcms { get; }

        /// <summary>
        /// Base de calc. do ICMS sem Reducao.
        /// </summary>
        decimal BcIcmsSemReducao { get; }

        /// <summary>
        /// Valor do ICMS.
        /// </summary>
        decimal ValorIcms { get; }

        /// <summary>
        /// Aliquota do fundo de combate a pobreza.
        /// </summary>
        float AliqFcp { get; }

        /// <summary>
        /// Base de calc. do FCP
        /// </summary>
        decimal BcFcp { get; }

        /// <summary>
        /// Valor do fundo de combate a probreza.
        /// </summary>
        decimal ValorFcp { get; }

        /// <summary>
        /// Aliquota do ICMS ST
        /// </summary>
        float AliqIcmsSt { get; }

        /// <summary>
        /// Base de calc. do ICMS ST
        /// </summary>
        decimal BcIcmsSt { get; }

        /// <summary>
        /// Valor do calc. do ICMS ST.
        /// </summary>
        decimal ValorIcmsSt { get; }

        /// <summary>
        /// Aliquota do FCP ST
        /// </summary>
        float AliqFcpSt { get; }

        /// <summary>
        /// Valor do FCP ST
        /// </summary>
        decimal ValorFcpSt { get; }

        /// <summary>
        /// Base de calc. do FCP ST
        /// </summary>
        decimal BcFcpSt { get; }

        /// <summary>
        /// Base de calc. do PIS
        /// </summary>
        decimal BcPis { get; }

        /// <summary>
        /// Aliquota do PIS
        /// </summary>
        float AliqPis { get; }

        /// <summary>
        /// Valor do PIS.
        /// </summary>
        decimal ValorPis { get; }

        /// <summary>
        /// Aliquota do COFINS
        /// </summary>
        float AliqCofins { get; }

        /// <summary>
        /// Base de calc. do COFINS
        /// </summary>
        decimal BcCofins { get; }

        /// <summary>
        /// Valor do Cofins.
        /// </summary>
        decimal ValorCofins { get; }

        #endregion
    }
}
