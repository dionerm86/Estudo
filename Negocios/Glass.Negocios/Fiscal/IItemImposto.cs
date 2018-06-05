using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Fiscal.Negocios
{
    /// <summary>
    /// Assinatura do um item sobre o calculo pode ser calculado os impostos.
    /// </summary>
    public interface IItemImposto
    {
        #region Propriedades

        /// <summary>
        /// Natureza de operação vinculada ao item.
        /// </summary>
        Entidades.NaturezaOperacao NaturezaOperacao { get; }

        /// <summary>
        /// Produto.
        /// </summary>
        Global.Negocios.Entidades.Produto Produto { get; }

        /// <summary>
        /// Valor do IOF
        /// </summary>
        decimal ValorIof { get; }

        /// <summary>
        /// Valor total.
        /// </summary>
        decimal Total { get; }

        /// <summary>
        /// Valor da despesa aduaneira
        /// </summary>
        decimal ValorDespesaAduaneira { get; }

        /// <summary>
        /// Código do valor fiscal
        /// </summary>
        int? CodValorFiscal { get; }

        /// <summary>
        /// Margem de valor agregado
        /// </summary>
        float Mva { get; }

        /// <summary>
        /// Código de situação tributária do IPI
        /// </summary>
        Sync.Fiscal.Enumeracao.Cst.CstIpi? CstIpi { get; }

        /// <summary>
        /// Aliquota do IPI
        /// </summary>
        float AliqIpi { get; }

        /// <summary>
        /// Código de situação tributária do COFINS
        /// </summary>
        Sync.Fiscal.Enumeracao.Cst.CstPisCofins? CstCofins { get; }

        /// <summary>
        /// Aliquota do COFINS
        /// </summary>
        float AliqCofins { get; }

        /// <summary>
        /// Código de situação tributária do PIS
        /// </summary>
        Sync.Fiscal.Enumeracao.Cst.CstPisCofins? CstPis { get; }

        /// <summary>
        /// Aliquota do PIS
        /// </summary>
        float AliqPis { get; }

        /// <summary>
        /// Aliquota do ICMS ST
        /// </summary>
        float AliqIcmsSt { get; }

        /// <summary>
        /// Aliquota do ICMS
        /// </summary>
        float AliqIcms { get; }

        /// <summary>
        /// Aliquota do FCP
        /// </summary>
        float AliqFcp { get; }

        /// <summary>
        /// Aliquota do FCP ST
        /// </summary>
        float AliqFcpSt { get; }

        /// <summary>
        /// Percentual de redução da base de calc. do ICMS
        /// </summary>
        float PercRedBcIcms { get; }

        /// <summary>
        /// Percentual de diferimento do ICMS
        /// </summary>
        float PercDiferimento { get; }

        /// <summary>
        /// Código de situação tributária do ICMS
        /// </summary>
        Sync.Fiscal.Enumeracao.Cst.CstIcms? Cst { get; }

        /// <summary>
        /// Código de situação tributária do ICMS SN
        /// </summary>
        Sync.Fiscal.Enumeracao.Cst.CsosnIcms? Csosn { get; }

        /// <summary>
        /// Verifica se os impostos devem ser calculados
        /// </summary>
        /// <returns></returns>
        bool CalcularImpostos { get; }

        #endregion
    }
}
