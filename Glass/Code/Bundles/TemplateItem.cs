// <copyright file="TemplateItem.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace Glass.UI.Web.Bundles
{
    /// <summary>
    /// Classe que encapsula os dados de um script de template.
    /// </summary>
    public class TemplateItem : ScriptItem
    {
        /// <summary>
        /// Constante com o tipo de template utilizado pelo Vue.
        /// </summary>
        internal const string TIPO_TEMPLATE_VUE = "text/x-template";
        private const string SUFIXO_ID = "-template";

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TemplateItem"/>.
        /// </summary>
        /// <param name="path">O caminho do script no bundle.</param>
        /// <param name="type">O tipo de script no bundle.</param>
        public TemplateItem(string path, string type = TIPO_TEMPLATE_VUE)
            : base(path)
        {
            this.Id = path.Split('/')
                .Last()
                .Replace('.', '-')
                .TrimEnd('-')
                + SUFIXO_ID;

            this.Type = type;
        }

        /// <summary>
        /// Obtém o identificador do script no bundle.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Obtém o tipo de script no bundle.
        /// </summary>
        public string Type { get; private set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            const string SCRIPT = @"<script type=""{0}"" id=""{1}"">{3}{2}{3}</script>{3}";

            var httpContext = new HttpContextWrapper(HttpContext.Current);
            var context = new BundleContext(httpContext, BundleTable.Bundles, string.Empty);

            var bundle = BundleTable.Bundles.First(b => b.Path == this.Path);
            var conteudo = bundle.GenerateBundleResponse(context);

            var script = string.Format(
                SCRIPT,
                this.Type,
                this.Id,
                conteudo.Content,
                Environment.NewLine);

            return new HtmlString(script)
                .ToHtmlString();
        }
    }
}
