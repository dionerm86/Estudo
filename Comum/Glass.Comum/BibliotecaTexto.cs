using System;

namespace Glass
{
    /// <summary>
    /// Classe com método que manipulam palavras.
    /// </summary>
    public class BibliotecaTexto
    {
        #region Retorna uma, duas ou três primeiras partes de um nome

        /// <summary>
        /// Retorna o primeiro nome do nome passado
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetFirstName(string name)
        {
            try
            {
                if (name == null)
                    return String.Empty;

                string result = name.IndexOf(' ') > 0 ? name.Remove(name.IndexOf(' ')) : name;

                return !String.IsNullOrEmpty(result) ? result : name;
            }
            catch
            {
                return name;
            }
        }

        /// <summary>
        /// Retorna os dois primeiros nomes do nome passado
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetTwoFirstNames(string name)
        {
            try
            {
                if (String.IsNullOrEmpty(name))
                    return String.Empty;

                string[] result = name.Split(' ');

                return result[0] + (result.Length > 1 ? " " + result[1] : String.Empty);
            }
            catch
            {
                return name;
            }
        }

        /// <summary>
        /// Retorna as três primeiras palavras da string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetThreeFirstWords(string value)
        {
            try
            {
                if (String.IsNullOrEmpty(value))
                    return String.Empty;

                string[] vet = value.Split(' ');
                string result = String.Empty;

                if (vet.Length >= 3)
                    result += vet[0] + " " + vet[1] + " " + vet[2];
                else if (vet.Length == 2)
                    result += vet[0] + " " + vet[1];
                else
                    result += vet[0];

                return result;
            }
            catch
            {
                return value;
            }
        }

        #endregion
    }
}
