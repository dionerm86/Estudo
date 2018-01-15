

using System.Collections.Generic;

namespace Glass.Api.Graficos.Administrativos
{
    #region Entidades

    /// <summary>
    /// Assinatura dos dados da DRE
    /// </summary>
    public interface IDre
    {
        string Loja { get; set; }

        string Data { get; set; }

        decimal Valor { get; set; }
    }

    /// <summary>
    /// Assinatura dos dados do ponto de equilibrio
    /// </summary>
    public interface IPontoEquilibrio
    {
        string Item { get; set; }

        decimal Valor { get; set; }

        string Percentual { get; set; }
    }

    /// <summary>
    /// Assinatura dos dados do tempo gasto por etapa
    /// </summary>
    public interface ITempoGastoPorEtapa
    {
        string Data { get; set; }

        string Dias { get; set; }
    }

    /// <summary>
    /// Assinatura dos dados da metragem produzida
    /// </summary>
    public interface IMetragemProduzir
    {
        string Cor { get; set; }

        float Espessura { get; set; }

        decimal M2 { get; set; }

        int Qtde { get; set; }
    }

    #endregion

    #region Fluxos

    /// <summary>
    /// Assinatura do fluxo de negocio dos graficos administrativos
    /// </summary>
    public interface IAdministrativoFluxo
    {
        /// <summary>
        /// Obtem os dados do DRE
        /// </summary>
        /// <returns></returns>
        List<IDre> ObtemDre();

        /// <summary>
        /// Obtem os dados do ponto de equilibrio
        /// </summary>
        /// <returns></returns>
        List<IPontoEquilibrio> ObtemPontoEquilibrio();

        /// <summary>
        /// Obtem os dados do tempo gasto por etapa
        /// </summary>
        /// <returns></returns>
        List<ITempoGastoPorEtapa> ObtemTempoGastoPorEtapa();

        /// <summary>
        /// Obtem os dados da metragem a produzir
        /// </summary>
        /// <returns></returns>
        List<IMetragemProduzir> ObtemMetragemProduzir();
    }

    #endregion
}
