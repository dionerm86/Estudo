using System;
using System.Linq;

namespace Glass.Seguranca
{
    /// <summary>
    /// Classe que gerencia a parte de autenticação para acessar pastas remotas.
    /// </summary>
    public static class AutenticacaoRemota
    {
        /// <summary>
        /// Autentica no servidor e recupera a manipulada da sessão de autenticação.
        /// </summary>
        /// <returns></returns>
        public static IDisposable Autenticar()
        {
            var config = System.Configuration.ConfigurationManager.AppSettings["AutenticacaoRemota"];

            if (!string.IsNullOrEmpty(config))
            {
                var index = config.IndexOf(':');

                if (index >= 0)
                {
                    var part1 = config.Substring(0, index).Split('@');
                    var usuario = part1.First();
                    var dominio = (part1.Length > 1 ? part1.Last() : null);

                    var senha = config.Substring(index + 1);

                    return new Colosoft.Net.Impersonator(usuario, dominio, senha, Colosoft.Net.LogonType.LOGON32_LOGON_NEW_CREDENTIALS, Colosoft.Net.LogonProvider.LOGON32_PROVIDER_DEFAULT);
                }
            }

            return new AutenticacaoFake();
        }

        #region Tipos Aninhados

        public class AutenticacaoFake : IDisposable
        {
            public void Dispose()
            {
            }
        }

        #endregion
    }
}
