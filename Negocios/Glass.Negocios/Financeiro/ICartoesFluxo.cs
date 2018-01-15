namespace Glass.Financeiro.Negocios
{
    /// <summary>
    /// Assinatura do fluxo de cartões do sistema.
    /// </summary>
    public interface ICartoesFluxo
    {
        /// <summary>
        /// Cria novo tipo cartão de credito
        /// </summary>
        /// <returns></returns>
        Entidades.TipoCartaoCredito CriarTipoCartaoCredito();

        /// <summary>
        /// Salva o tipo de cartão de crédito
        /// </summary>
        Colosoft.Business.SaveResult SalvarTipoCartaoCredito(Entidades.TipoCartaoCredito tipoCartaoCredito);
    }
}
