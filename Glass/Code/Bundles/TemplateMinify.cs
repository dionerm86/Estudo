// <copyright file="TemplateMinify.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System.Text.RegularExpressions;
using System.Web.Optimization;

namespace Glass.UI.Web.Bundles
{
    /// <summary>
    /// Classe que minifica os scripts de template.
    /// </summary>
    internal class TemplateMinify : IBundleTransform
    {
        /// <inheritdoc/>
        public void Process(BundleContext context, BundleResponse response)
        {
            if (context.EnableOptimizations)
            {
                response.Content = this.Minificar(response.Content);
            }
        }

        private string Minificar(string html)
        {
            var formatado = html;
            formatado = this.RemoverQuebrasDeLinha(formatado);
            formatado = this.RemoverEspacos(formatado);

            return formatado;
        }

        private string RemoverQuebrasDeLinha(string html)
        {
            return html.Replace("\r\n", string.Empty)
                .Replace("\r", string.Empty)
                .Replace("\n", string.Empty);
        }

        private string RemoverEspacos(string html)
        {
            var formatado = html;
            var regex = new Regex("> *<");

            while (true)
            {
                var match = regex.Match(formatado);

                if (match == null)
                {
                    break;
                }

                formatado = formatado.Replace(match.Value, "><");
            }

            return formatado;
        }
    }
}
