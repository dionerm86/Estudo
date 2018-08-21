// <copyright file="ScriptItem.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.UI.Web.Bundles
{
    /// <summary>
    /// Classe que encapsula os dados para um script.
    /// </summary>
    public class ScriptItem
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ScriptItem"/>.
        /// </summary>
        /// <param name="path">O caminho do script no bundle.</param>
        public ScriptItem(string path)
        {
            this.Path = path;
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
