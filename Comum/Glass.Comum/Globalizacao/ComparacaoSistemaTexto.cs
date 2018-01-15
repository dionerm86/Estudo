using System;
using System.Globalization;

namespace Glass.Globalizacao
{
    /// <summary>
    /// Representa o comparador d string padrão do sistema.
    /// </summary>
    public class ComparacaoSistemaTexto : StringComparer
    {
        #region Local Variables

        private static readonly ComparacaoSistemaTexto _default = new ComparacaoSistemaTexto();

        #endregion

        #region Properties

        /// <summary>
        /// Instancia padrão do comparador.
        /// </summary>
        public static ComparacaoSistemaTexto Default
        {
            get { return _default; }
        }

        #endregion

        /// <summary>
        /// Compara as instancia informadas.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static int GlobalCompare(object x, object y)
        {
            return Default.Compare(x, y);
        }

        /// <summary>
        /// Compara a duas strings informadas.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override int Compare(string x, string y)
        {
            return Cultura.CulturaSistema.CompareInfo.Compare(x, y,
                CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace |
                CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth);
        }

        /// <summary>
        /// Verifica se as duas string informadas são iguais.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override bool Equals(string x, string y)
        {
            return Cultura.CulturaSistema.CompareInfo.Compare(x, y,
                CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace |
                CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth) == 0;
        }

        /// <summary>
        /// Recupera o hashcode da string informada.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override int GetHashCode(string obj)
        {
            return Cultura.CulturaSistema.CompareInfo.GetSortKey(obj,
                CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace |
                CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth).GetHashCode();
        }
    }
}
