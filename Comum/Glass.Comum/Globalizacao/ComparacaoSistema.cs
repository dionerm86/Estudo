namespace Glass.Globalizacao
{
    /// <summary>
    /// Implementação do comparador padrão do sistema.
    /// </summary>
    public class ComparacaoSistema : System.Collections.IComparer
    {
        #region Local Variables

        private static System.Collections.IComparer _default = new ComparacaoSistema();

        #endregion

        #region Properties

        /// <summary>
        /// Instancia do comparador padrão.
        /// </summary>
        public static System.Collections.IComparer Default
        {
            get { return _default; }
            set { _default = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Compara a instancia informadas.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(object x, object y)
        {
            if (x is string || y is string)
            {
                return Cultura.ComparadorTexto.Compare
                    (x is string ? (string)x : (x != null ? x.ToString() : null),
                    y is string ? (string)y : (y != null ? y.ToString() : null));
            }

            return System.Collections.Comparer.DefaultInvariant.Compare(x, y);
        }

        #endregion
    }
}
