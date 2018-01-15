using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Seguranca
{
    public static class AutenticacaoAutomatica
    {
        /// <summary>
        /// Autentica o usuário utilizando token
        /// </summary>
        /// <param name="token"></param>
        /// <returns>Se autenticar, retorna os dados do usuário caso contrário, retorna null</returns>
        public static string Autenticar(string token, string empresa)
        {
            var dadosLogin = Descriptografar(token);

            if (dadosLogin == null || dadosLogin.Length < 4 || dadosLogin[2] == null)
                return null;

            // Verifica se os dados são novos
            if (dadosLogin[0].StrParaInt() != DateTime.Now.Month || dadosLogin[1].StrParaInt() != DateTime.Now.Year || dadosLogin[2].ToLower() != empresa.ToLower())
                return "Erro|Dados de login inválidos.";

            /* Chamado 51923. */
            return dadosLogin[3].Replace("#", "|");
        }

        private static string[] Descriptografar(string texto)
        {
            var crypto = new Glass.Seguranca.Crypto();
            crypto.Key = "z16y$3EmA4*!F(@!(zH))tgf}}6v8*c9";

            // Descriptografa o texto
            var dados = crypto.Decrypt(texto.Replace(" ", "+"));

            if (string.IsNullOrEmpty(dados))
                return null;
            
            return dados.Split('|');
        }
    }
}
