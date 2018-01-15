using System;
using System.Collections.Generic;

namespace Glass
{
    /// <summary>
    /// Atribute do provedor de tradução
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum)]
    public class ProvedorTraducaoAttribute : Attribute
    {
        #region Properties

        /// <summary>
        /// Tipo do provedor.
        /// </summary>
        public Type Tipo { get; set; }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="tipo"></param>
        public ProvedorTraducaoAttribute(Type tipo)
        {
            Tipo = tipo;
        }

        #endregion
    }

    /// <summary>
    /// Assinatura de uma provedor de traducao.
    /// </summary>
    public interface IProvedorTraducao
    {
        /// <summary>
        /// Recupera as traduções.
        /// </summary>
        /// <returns></returns>
        IEnumerable<InformacaoEnumerador> ObterTraducoes();
    }

    /// <summary>
    /// Representa um provedor que possui multiplas traduções.
    /// </summary>
    public interface IProvedorMultiplaTraducao
    {
        /// <summary>
        /// Recupera as traduções.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IEnumerable<InformacaoEnumerador>> ObterTraducoes();
    }
}
