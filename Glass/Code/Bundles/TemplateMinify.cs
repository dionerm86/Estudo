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
            formatado = this.RemoverComentarios(formatado);
            formatado = this.RemoverEspacosAdicionais(formatado);
            formatado = this.RemoverEspacosDesnecessarios(formatado);

            return formatado;
        }

        private string RemoverQuebrasDeLinha(string html)
        {
            return html.Replace("\r\n", string.Empty)
                .Replace("\r", string.Empty)
                .Replace("\n", string.Empty);
        }

        private string RemoverComentarios(string html)
        {
            return Regex.Replace(html, "<!--.*-->", string.Empty);
        }

        private string RemoverEspacosAdicionais(string html)
        {
            return Regex.Replace(html, " {2,}", " ");
        }

        private string RemoverEspacosDesnecessarios(string html)
        {
            return html.Replace("> ", ">")
                .Replace(" <", "<");
        }
    }
}
