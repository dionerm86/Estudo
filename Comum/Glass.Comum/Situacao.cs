using Colosoft;
using Colosoft.Globalization;
using System.Collections.Generic;

namespace Glass
{
    /// <summary>
    /// Situação padrão do sistema.
    /// </summary>
    [Colosoft.Translate(typeof(SituacaoTranslateProvider))]
    public enum Situacao : long
    {
        /// <summary>
        /// Ativo.
        /// </summary>
        Ativo = 1,
        /// <summary>
        /// Inativo.
        /// </summary>
        Inativo
    }

    /// <summary>
    /// Provedor de tradução da situação.
    /// </summary>
    class SituacaoTranslateProvider : Colosoft.Globalization.IMultipleTranslateProvider
    {
        /// <summary>
        /// Recupera as traduções do grupo de tradução informado.
        /// </summary>
        /// <param name="groupKey"></param>
        /// <returns></returns>
        public IEnumerable<Colosoft.Globalization.TranslateInfo> GetTranslates(object groupKey)
        {
            if ((groupKey as string) != "fem")
                return new[] {
                    new TranslateInfo(Situacao.Ativo, "Ativo".GetFormatter()),
                    new TranslateInfo(Situacao.Inativo, "Inativo".GetFormatter())
                };
            else
                return new[] {
                    new TranslateInfo(Situacao.Ativo, "Ativa".GetFormatter()),
                    new TranslateInfo(Situacao.Inativo, "Inativa".GetFormatter())
                };
        }

        /// <summary>
        /// Recupera as traduções.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Colosoft.Globalization.TranslateInfo> GetTranslates()
        {
            return GetTranslates(null);
        }
    }
}
