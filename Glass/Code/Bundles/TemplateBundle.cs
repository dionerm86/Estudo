// <copyright file="TemplateBundle.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System.Linq;
using System.Web.Optimization;

namespace Glass.UI.Web.Bundles
{
    /// <summary>
    /// Classe que contém informações sobre o bundle de templates Vue.
    /// </summary>
    internal class TemplateBundle : Bundle
    {
        private readonly string path;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TemplateBundle"/>.
        /// </summary>
        /// <param name="path">O caminho do bundle.</param>
        public TemplateBundle(string path)
            : base(ObterNomeBundle(path), new TemplateMinify())
        {
            this.path = path;
            this.Include(path);
        }

        private static string ObterNomeBundle(string path)
        {
            const string NOME_BUNDLE = "~/Scripts/Templates/{0}";
            return string.Format(NOME_BUNDLE, ObterNomeArquivo(path));
        }

        private static string ObterNomeArquivo(string path)
        {
            var partes = path.Split('/');
            var nome = partes.Last();
            return nome.Substring(0, nome.LastIndexOf('.'));
        }
    }
}
