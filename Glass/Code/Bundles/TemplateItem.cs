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
        /// <param name="bundle">O bundle.</param>
        /// <param name="type">O tipo de script no bundle.</param>
        public TemplateItem(Bundle bundle, string type = TIPO_TEMPLATE_VUE)
            : base(bundle)
        {
            this.Id = this.Path.Split('/')
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

            var contextoHttp = new HttpContextWrapper(HttpContext.Current);
            var contexto = new BundleContext(contextoHttp, BundleTable.Bundles, string.Empty);

            var conteudo = this.bundle.GenerateBundleResponse(contexto);
            var separador = conteudo.Content.IndexOf('\n') > -1
                ? Environment.NewLine
                : string.Empty;

            var script = string.Format(
                SCRIPT,
                this.Type,
                this.Id,
                conteudo.Content,
                separador);

            return new HtmlString(script)
                .ToHtmlString();
        }
    }
}
