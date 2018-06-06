using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data
{
    /// <summary>
    /// Assinatura do resultado do calculo do imposto.
    /// </summary>
    public interface ICalculoImpostoResultado
    {
        #region Métodos

        /// <summary>
        /// Salva os dados do resultado na sessão informada.
        /// </summary>
        /// <param name="sessao"></param>
        void Salvar(GDA.GDASession sessao);

        #endregion
    }

    /// <summary>
    /// Assinatura complementar do resultado do calculo de imposto
    /// para aplicar os valores sobre itens de um determinado tipo.
    /// </summary>
    /// <typeparam name="T">Tipo do item que será tratado pelo resultado.</typeparam>
    public interface ICalculoImpostoResultado<in T> : ICalculoImpostoResultado
    {
        #region Métodos

        /// <summary>
        /// Aplica os impostos calculados para os itens informados.
        /// </summary>
        /// <param name="itens"></param>
        void AplicarImpostos(IEnumerable<T> itens);

        #endregion
    }
}
