using System;
using System.Collections.Generic;

namespace Glass.Api.Graficos.Vendas
{
    #region Entidades

    /// <summary>
    /// Assinatura dos dados do grafico de Vendas (Curva ABC)
    /// </summary>
    public interface IVendasCurvaAbc
    {
        int IdLoja { get; set; }

        string Loja { get; set; }

        string Mes { get; set; }

        decimal Valor { get; set; }
    }

    /// <summary>
    /// Assinatura dos dados do gráfico de Vendas por Cliente (Curva ABC)
    /// </summary>
    public interface IVendasPorClienteCurvaAbc
    {
        int IdCliente { get; set; }

        string Cliente { get; set; }

        string Mes { get; set; }

        decimal Valor { get; set; }
    }

    /// <summary>
    /// Assinatura dos dados do gráfico de Vendas por Pedido
    /// </summary>
    public interface IVendasPorPedido
    {
        string TipoPedido { get; set; }

        string Data { get; set; }

        decimal Valor { get; set; }
    }

    /// <summary>
    /// Assinatura dos dados do gráfico de Vendas por Produto
    /// </summary>
    public interface IVendasPorProduto
    {
        int IdProd { get; set; }

        string Descricao { get; set; }

        decimal M2 { get; set; }
    }

    /// <summary>
    /// Assinatura dos dados do grafico de Vendas por Vendedor
    /// </summary>
    public interface IVendasPorVendedor
    {
        int IdVendedor { get; set; }

        string Nome { get; set; }

        string Mes { get; set; }

        decimal Valor { get; set; }
    }

    #endregion

    #region Fluxos

    /// <summary>
    /// Assinatura do fluxo de negocio dos graficos de vendas
    /// </summary>
    public interface IVendasFluxo
    {
        /// <summary>
        /// Recupera o grafico de Vendas (Curva ABC)
        /// </summary>
        List<IVendasCurvaAbc> ObtemVendasCurvaAbc();

        /// <summary>
        /// Recupera o grafico de Vendas por Cliente (Curva ABC)
        /// </summary>
        List<IVendasPorClienteCurvaAbc> ObtemVendasPorClienteCurvaAbc();

        /// <summary>
        /// Recupera o grafico de Vendas por Pedido
        /// </summary>
        List<IVendasPorPedido> ObtemVendasPorPedido();

        /// <summary>
        /// Recupera o grafico de Vendas por Produto
        /// </summary>
        List<IVendasPorProduto> ObtemVendasPorProduto();

        /// <summary>
        /// Recupera o grafico de Vendas por Vendedor
        /// </summary>
        /// <returns></returns>
        List<IVendasPorVendedor> ObtemVendasPorVendedor();
    }

    #endregion
}
