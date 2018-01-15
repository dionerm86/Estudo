using System.Collections.Generic;

namespace Glass.Api.Graficos.Producao
{
    #region Entidades

    #region Painel Produção

    public interface IProducaoDia
    {
        int Dia { get; }

        decimal Meta { get; set; }

        decimal Produzido { get; set; }
    }

    public interface IProducaoPendente
    {
        int Dia { get; set; }

        decimal Valor { get; set; }
    }

    public interface IPercentualPerdaMensal
    {
        string Mes { get; set; }

        decimal TotM2Perda { get; set; }

        decimal TotM2Produzido { get; set; }

        decimal PorcentagemPerda { get; set; }
    }

    public interface IValorPerdaMensal
    {
        string Mes { get; set; }

        decimal Valor { get; set; }
    }

    /// <summary>
    ///  Assinatura dos dados do Painel da Produção
    /// </summary>
    public interface IPainelProducao
    {
        IProducaoDia ProducaoDia { get; set; }

        List<IProducaoPendente> ProducaoPendente { get; set; }

        IPercentualPerdaMensal PercentualPerdaMensal { get; set; }

        List<IValorPerdaMensal> ValorPerdaMensal { get; set; }
    }

    #endregion

    /// <summary>
    /// Assinatura dos dados de Produção Diária Prevista / Realizada
    /// </summary>
    public interface IProducaoDiariaPrevistaRealizada
    {
        string Data { get; set; }

        decimal Previsto { get; set; }

        decimal Realizado { get; set; }

        decimal Pendente { get; set; }
    }

    /// <summary>
    /// Assinatura dos dados de Perda por Setor
    /// </summary>
    public interface IPerdaPorSetor
    {
        int IdSetor { get; set; }

        string Setor { get; set; }

        decimal Real { get; set; }

        decimal Desafio { get; set; }

        decimal Meta { get; set; }
    }

    /// <summary>
    /// Assinatura dos dados de Produção Diária por Setor
    /// </summary>
    public interface IProducaoDiariaPorSetor
    {
        int IdSetor { get; set; }

        string Setor { get; set; }

        decimal Produzido { get; set; }
    }

    /// <summary>
    /// Assinatura dos dados de Produção do Dia
    /// </summary>
    public interface IProducaoDoDia
    {
        string Dia { get; set; }

        decimal TotalM2 { get; set; }
    }

    #endregion

    #region Fluxos

    /// <summary>
    ///  Assinatura do fluxo de negocio dos graficos da produção
    /// </summary>
    public interface IProducaoFluxo
    {
        /// <summary>
        /// Recupera o Painel da produção
        /// </summary>
        IPainelProducao ObtemPainelProducao();

        /// <summary>
        /// Recupera os dados da produção diária prevista realizada
        /// </summary>
        /// <returns></returns>
        List<IProducaoDiariaPrevistaRealizada> ObtemProducaoDiariaPrevistaRealizada();

        /// <summary>
        /// Recupera os dados de Perda por Setor
        /// </summary>
        /// <returns></returns>
        List<IPerdaPorSetor> ObtemPerdaPorSetor();

        /// <summary>
        /// Recupera os dados de Produção Diária por Setor
        /// </summary>
        /// <returns></returns>
        List<IProducaoDiariaPorSetor> ObtemProducaoDiariaPorSetor();

        /// <summary>
        /// Recupera os dados de Produção do Dia
        /// </summary>
        /// <returns></returns>
        List<IProducaoDoDia> ObtemProducaoDia();
    }

    #endregion
}
