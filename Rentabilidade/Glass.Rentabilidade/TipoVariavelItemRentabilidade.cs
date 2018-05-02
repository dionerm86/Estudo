using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Glass.Rentabilidade
{
    /// <summary>
    /// Possíveis tipos de variaveis associadas aos itens da rentabilidade.
    /// </summary>
    public enum TipoVariavelItemRentabilidade
    {
        /// <summary>
        /// Preço de venda.
        /// </summary>
        [Description("Preço Venda")]
        PrecoVenda = 1,
        /// <summary>
        /// Preço de custo.
        /// </summary>
        [Description("Preço Custo")]
        PrecoCusto,
        /// <summary>
        /// Prazo médio.
        /// </summary>
        [Description("Prazo Médio")]
        PrazoMedio,
        /// <summary>
        /// Percentual do ICMS de compra.
        /// </summary>
        [Description("% ICMS Compra")]
        PICMSCompra,
        /// <summary>
        /// Percentual do ICMS de venda.
        /// </summary>
        [Description("% ICMS Venda")]
        PICMSVenda,
        /// <summary>
        /// Fator do ICMS de substituição.
        /// </summary>
        [Description("% ICMS Substituição")]
        FatorICMSSubstituicao,
        /// <summary>
        /// Percentual do IPI de compra;
        /// </summary>
        [Description("% IPI Compra")]
        PIPICompra,
        /// <summary>
        /// Percentual do IPI de venda.
        /// </summary>
        [Description("% IPI Venda")]
        PIPIVenda,
        /// <summary>
        /// Percentual de Comissão.
        /// </summary>
        [Description("% Comissão")]
        PComissao,
        /// <summary>
        /// Valor dos custos extras.
        /// </summary>
        [Description("Custos Extras")]
        CustosExtras
    }
}
