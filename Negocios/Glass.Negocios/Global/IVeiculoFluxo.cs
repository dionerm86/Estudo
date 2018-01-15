using System.Collections.Generic;

namespace Glass.Global.Negocios
{
    /// <summary>
    /// Assinatura do fluxo de negócio dos veículos.
    /// </summary>
    public interface IVeiculoFluxo
    {
        /// <summary>
        /// Pesquisa os veículos do sistema.
        /// </summary>
        /// <returns></returns>
        IList<Entidades.Veiculo> PesquisarVeiculos();

        /// <summary>
        /// Recupera os descritores dos veículos.
        /// </summary>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemVeiculos();

        /// <summary>
        /// Recupera o veículo pela placa informada.
        /// </summary>
        /// <param name="placa"></param>
        /// <returns></returns>
        Entidades.Veiculo ObtemVeiculo(string placa);

        /// <summary>
        /// Salva os dados do veículo.
        /// </summary>
        /// <param name="veiculo"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarVeiculo(Entidades.Veiculo veiculo);

        /// <summary>
        /// Apaga os dados do veículo.
        /// </summary>
        /// <param name="veiculo"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarVeiculo(Entidades.Veiculo veiculo);
    }
}
