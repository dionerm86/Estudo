using System;
using System.Collections.Generic;
using System.Web.Configuration;
using System.Web.UI;

namespace Glass
{
    public static class FuncoesGerais
    {
        /// <summary>
        /// Redirenciona a página
        /// </summary>
        /// <param name="url"></param>
        /// <param name="page"></param>
        public static void RedirecionaPagina(string url, System.Web.UI.Page page)
        {
            page.ClientScript.RegisterStartupScript(typeof(string), Guid.NewGuid().ToString(),
                "redirectUrl('" + url + "');", true);
        }

        /// <summary>
        /// Retorna o tamanho máximo da requisição ASP.NET, em MB.
        /// </summary>
        /// <returns></returns>
        public static float GetTamanhoMaximoUpload()
        {
            HttpRuntimeSection runtime = WebConfigurationManager.GetWebApplicationSection("system.web/httpRuntime") as HttpRuntimeSection;
            return (runtime.MaxRequestLength / 1024f);
        }

        public static bool IsChrome(Page page)
        {
            return page.Request.UserAgent != null && page.Request.UserAgent.IndexOf("Safari", StringComparison.CurrentCultureIgnoreCase) != -1;
        }

        public static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;

            return char.ToUpper(s[0]) + s.Substring(1);
        }
    }

    public class AlphaNumericComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            int firstNumber, secondNumber;
            bool firstIsNumber = int.TryParse(x, out firstNumber);
            bool secondIsNumber = int.TryParse(y, out secondNumber);

            if (firstIsNumber)
                return secondIsNumber ? firstNumber.CompareTo(secondNumber) : -1;

            return secondIsNumber ? 1 : firstNumber.CompareTo(secondNumber);
        }
    }
}
