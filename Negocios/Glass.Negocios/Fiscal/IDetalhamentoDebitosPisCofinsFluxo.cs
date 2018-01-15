using System.Collections.Generic;

namespace Glass.Fiscal.Negocios
{
    /// <summary>
    /// Assinatura do fluxo de negócio dos débitos de PIS e Cofins.
    /// </summary>
    public interface IDetalhamentoDebitosPisCofinsFluxo
    {
        /// <summary>
        /// Recupera os dados de débitos de PIS/Cofins.
        /// </summary>
        /// <returns></returns>
        IList<Entidades.DetalhamentoDebitosPisCofins> ObtemDebitosPisCofins();

        /// <summary>
        /// Cria um item para cadastro.
        /// </summary>
        /// <returns></returns>
        Entidades.DetalhamentoDebitosPisCofins CriarDebitoPisCofins();

        /// <summary>
        /// Recupera um item de débitos de PIS/Cofins.
        /// </summary>
        /// <param name="IdDetalhamentoPisCofins"></param>
        /// <returns></returns>
        Entidades.DetalhamentoDebitosPisCofins ObtemDebitoPisCofins(int IdDetalhamentoPisCofins);

        /// <summary>
        /// Salva os dados do débito de PIS/Cofins.
        /// </summary>
        /// <param name="debitoPisCofins"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarDebitoPisCofins(Entidades.DetalhamentoDebitosPisCofins debitoPisCofins);

        /// <summary>
        /// Apaga os dados de PIS/Cofins.
        /// </summary>
        /// <param name="debitoPisCofins"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarDebitoPisCofins(Entidades.DetalhamentoDebitosPisCofins debitoPisCofins);
    }
}
