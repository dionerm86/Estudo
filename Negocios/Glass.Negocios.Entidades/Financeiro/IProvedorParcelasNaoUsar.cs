using System.Collections.Generic;

namespace Glass.Financeiro.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do provedor das parcelas que não devem ser usadas.
    /// </summary>
    public interface IProvedorParcelasNaoUsar
    {
        /// <summary>
        /// Crias as parcelas que não devem ser usadas associadas a parcela informada.
        /// </summary>
        /// <param name="parcela"></param>
        /// <returns></returns>
        IEnumerable<ParcelasNaoUsar> CriarParcelasNaoUsar(Parcelas parcela);

        /// <summary>
        /// Recupera a identificação da parcela;
        /// </summary>
        /// <param name="idParcela"></param>
        /// <returns></returns>
        string ObtemIdentificacao(int idParcela);
    }
}
