// <copyright file="IncluirTemplateTela.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.UI.Web.Bundles;
using System.Linq;
using System.Web.Optimization;

namespace Glass.UI.Web
{
    /// <summary>
    /// Classe com métodos para inclusão de scripts de templates nas telas.
    /// </summary>
    public static class IncluirTemplateTela
    {
        /// <summary>
        /// Inclui um ou mais scripts de template na tela.
        /// </summary>
        /// <param name="caminhoRelativo">O caminho relativo de um ou mais scripts.</param>
        /// <returns>Uma string com a inclusão dos templates de tela.</returns>
        public static string Script(params string[] caminhoRelativo)
        {
            return string.Join(
                string.Empty,
                caminhoRelativo.Select(url => ProcessarScript(url)));
        }

        private static string ProcessarScript(string caminhoRelativo)
        {
            var bundle = new TemplateBundle(caminhoRelativo);

            try
            {
                BundleTable.Bundles.Add(bundle);

                var template = new TemplateItem(bundle.Path);
                return template.ToString();
            }
            finally
            {
                BundleTable.Bundles.Remove(bundle);
            }
        }
    }
}
