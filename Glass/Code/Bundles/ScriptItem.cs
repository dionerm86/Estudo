// <copyright file="ScriptItem.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System.Web.Optimization;

namespace Glass.UI.Web.Bundles
{
    /// <summary>
    /// Classe que encapsula os dados para um script.
    /// </summary>
    public class ScriptItem
    {
        protected readonly Bundle bundle;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ScriptItem"/>.
        /// </summary>
        /// <param name="bundle">O bundle.</param>
        public ScriptItem(Bundle bundle)
        {
            this.bundle = bundle;
            this.Path = bundle.Path;
        }

        /// <summary>
        /// Obtém o camimnho do script no bundle.
        /// </summary>
        public string Path { get; private set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return System.Web.Optimization.Scripts.Render(this.Path).ToHtmlString();
        }
    }
}
