
namespace Glass.Financeiro.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do provedor que fornece informações de juros.
    /// </summary>
    public interface IProvedorJuros
    {
        /// <summary>
        /// Calcula a taxa de juros que ser aplica para o tipo de cartão.
        /// </summary>
        /// <param name="tipoCartao"></param>
        /// <param name="idLoja"></param>
        /// <param name="numeroParcelas"></param>
        /// <returns></returns>
        decimal CalcularTaxaJuros(TipoCartaoCredito tipoCartao, int? idLoja, int numeroParcelas);
    }
}
