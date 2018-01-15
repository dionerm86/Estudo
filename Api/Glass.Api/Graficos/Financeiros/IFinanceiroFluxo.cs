using System.Collections.Generic;
using System.ComponentModel;

namespace Glass.Api.Graficos.Financeiros
{
    #region Enumeradores

    /// <summary>
    /// Enum dos possiveis tipos de previsão financeira
    /// </summary>
    public enum TipoPrevisaoFinanceiraEnum
    {
        [Description("Não Definido")]
        NaoDefinido,

        [Description("Contas à Receber")]
        ContasReceber,

        [Description("Cheque de Terceiros")]
        ChequeTerceiros,

        [Description("Pedidos em Produção")]
        PedidosProducao,

        [Description("Contas à Pagar")]
        ContasPagar,

        [Description("Cheque Próprios")]
        ChequeProprios,

    }

    #endregion

    #region Entidades

    /// <summary>
    /// Assinatura dos dados do grafico de Previsão financeira
    /// </summary>
    public interface IPrevisaoFinanceira
    {
        TipoPrevisaoFinanceiraEnum TipoPrevisao { get; set; }

        string DescrTipoPrevisao { get; }

        decimal ValorHoje { get; set; }

        decimal Valor30Dias { get; set; }

        decimal Valor60Dias { get; set; }

        decimal Valor90Dias { get; set; }

        decimal ValorMais90Dias { get; set; }
    }

    /// <summary>
    /// Assinatura dos dados do grafico de Recebimento por Tipo
    /// </summary>
    public interface IRecebimentosPorTipo
    {
        string TipoRecebimento { get; set; }

        decimal Valor { get; set; }
    }

    /// <summary>
    /// Assinatura dos dados do grafico de pagamentos
    /// </summary>
    public interface IPagamentos
    {
        string Data { get; set; }

        string TipoPagamento { get; set; }

        decimal Valor { get; set; }
    }

    #endregion

    #region Fluxos

    /// <summary>
    /// Assinatura do fluxo de negocio dos graficos financeiros
    /// </summary>
    public interface IFinanceiroFluxo
    {
        /// <summary>
        /// Obtem os dados de previsão financeira a receber Vencida
        /// </summary>
        /// <returns></returns>
        List<IPrevisaoFinanceira> ObtemPrevisaoFinanceiraReceberVencida();

        /// <summary>
        /// Obtem os dados de previsão financeira a pagar Vencida.
        /// </summary>
        /// <returns></returns>
        List<IPrevisaoFinanceira> ObtemPrevisaoFinanceiraPagarVencida();

        /// <summary>
        /// Obtem os dados de previsão financeira a receber
        /// </summary>
        /// <returns></returns>
        List<IPrevisaoFinanceira> ObtemPrevisaoFinanceiraReceber();

        /// <summary>
        /// Obtem os dados de previsão financeira a pagar
        /// </summary>
        /// <returns></returns>
        List<IPrevisaoFinanceira> ObtemPrevisaoFinanceiraPagar();

        /// <summary>
        /// Obtem os dados de recebimento por tipo
        /// </summary>
        /// <returns></returns>
        List<IRecebimentosPorTipo> ObtemRecebimentoPorTipo();

        /// <summary>
        /// Obtem os dados do grafico de pagamentos
        /// </summary>
        /// <returns></returns>
        List<IPagamentos> ObtemPagamentos();
    }

    #endregion
}
