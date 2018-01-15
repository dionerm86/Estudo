using System;
using System.Linq;

namespace Glass.Data.Helper
{
    /// <summary>
    /// Classe que gerencia a string de conexão.
    /// </summary>
    public class ConnectionString
    {
        /// <summary>
        /// Descriptograda a string de conexão.
        /// </summary>
        /// <param name="texto"></param>
        /// <returns></returns>
        public static string Decrypt(string texto)
        {
            var index = 0;
            var parts = texto.Split(';')
                .Where(f => (index = f.IndexOf("=")) > 0)
                .Select(f => new[] { f.Substring(0, index), f.Substring(index + 1) })
                .ToArray();

            var crypto = new Glass.Seguranca.Crypto();

            // Se o banco não tiver criptografado (senha com menos de 20 caracteres), retorna a mesma string passada por parâmetro
            foreach (var i in parts)
            {
                if ((StringComparer.InvariantCultureIgnoreCase.Equals(i[0], "Password") && i[1].Length < 20) ||
                    (StringComparer.InvariantCultureIgnoreCase.Equals(i[0], "Pwd") && i[1].Length < 20))
                    return texto;
            }

            foreach (var i in parts)
            {
                if (StringComparer.InvariantCultureIgnoreCase.Equals(i[0], "Password") ||
                    StringComparer.InvariantCultureIgnoreCase.Equals(i[0], "User Id") ||
                    StringComparer.InvariantCultureIgnoreCase.Equals(i[0], "Uid") ||
                    StringComparer.InvariantCultureIgnoreCase.Equals(i[0], "Pwd"))
                    i[1] = crypto.Decrypt(i[1]);
            }

            return string.Join(";", parts.Select(f => string.Join("=", f)).ToArray());
        }
    }
}
