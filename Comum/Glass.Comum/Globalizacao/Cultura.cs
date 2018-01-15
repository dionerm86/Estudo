using System;
using System.Globalization;

namespace Glass.Globalizacao
{
    /// <summary>
    /// Armazena as configurações de cultura do sistema.
    /// </summary>
    public static class Cultura
    {
        #region Variáveis Locais

        private static readonly CultureInfo _culturaSistema;
        private static readonly StringComparer _comparadorTexto;
        private static readonly CultureInfo _inglesInvarianteEUA;

        #endregion

        #region Propriedades

        /// <summary>
        /// Cultura invariante para inglês dos Estados Unidos.
        /// </summary>
        public static CultureInfo InglesInvarianteEUA
        {
            get { return _inglesInvarianteEUA; }
        }

        /// <summary>
        /// Cultura padrão do sistema.
        /// </summary>
        public static CultureInfo CulturaSistema
        {
            get { return _culturaSistema; }
        }

        /// <summary>
        /// Instancia do comparador de string do sistema.
        /// </summary>
        public static StringComparer ComparadorTexto
        {
            get { return _comparadorTexto; }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor estático.
        /// </summary>
        static Cultura()
        {
            _culturaSistema = System.Globalization.CultureInfo.GetCultureInfo("pt-BR");
            _comparadorTexto = new ComparacaoSistemaTexto();
            _inglesInvarianteEUA = CultureInfo.ReadOnly(new CultureInfo("en-US", false));
        }

        #endregion
    }
}
