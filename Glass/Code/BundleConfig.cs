// <copyright file="BundleConfig.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.UI.Web.Bundles;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Web;
using System.Web.Optimization;

namespace Glass.UI.Web
{
    /// <summary>
    /// Classe de configuração dos bundles.
    /// </summary>
    public static class BundleConfig
    {
        private static List<ScriptItem> scripts = new List<ScriptItem>();
        private static List<string> estilos = new List<string>();

        /// <summary>
        /// Inicia a configuração de bundles.
        /// </summary>
        /// <param name="bundles">A coleção de bundles do site.</param>
        public static void RegisterBundles(BundleCollection bundles)
        {
            Adicionar(bundles, ScriptBundle());
            Adicionar(bundles, CssBundle());
            Adicionar(bundles, VueBundle());

            foreach (var bundle in VueTemplatesBundle())
            {
                Adicionar(bundles, bundle);
            }

            Otimizar();
        }

        /// <summary>
        /// Retorna a lista de scripts aplicados.
        /// </summary>
        /// <returns>A lista de scripts.</returns>
        public static IEnumerable<ScriptItem> Scripts()
        {
            return new ReadOnlyCollection<ScriptItem>(scripts);
        }

        /// <summary>
        /// Retorna a lista de estilos aplicados.
        /// </summary>
        /// <returns>A lista de estilos.</returns>
        public static IEnumerable<string> Estilos()
        {
            return new ReadOnlyCollection<string>(estilos);
        }

        private static void Adicionar(BundleCollection bundles, Bundle bundle)
        {
            bundles.Add(bundle);

            if (bundle is StyleBundle)
            {
                estilos.Add(bundle.Path);
            }
            else
            {
                ScriptItem item;

                if (bundle is TemplateBundle)
                {
                    item = new TemplateItem(bundle.Path);
                }
                else
                {
                    item = new ScriptItem(bundle.Path);
                }

                scripts.Add(item);
            }
        }

        private static Bundle ScriptBundle()
        {
            return new ScriptBundle("~/Scripts/app")
                .Include("~/Scripts/jquery/jquery-2.0.0.js")
                .Include("~/Scripts/jquery/jlinq/jlinq.js")
                .Include("~/Scripts/jquery/jquery.utils.js")
                .Include("~/Scripts/Utils.js")
                .Include("~/Scripts/dhtmlgoodies_calendar.js");
        }

        private static Bundle CssBundle()
        {
            return new StyleBundle("~/Style/app")
                .Include("~/Style/Geral.css")
                .Include("~/Style/GridView.css")
                .Include("~/Style/dhtmlgoodies_calendar.css");
        }

        private static Bundle VueBundle()
        {
            return new ScriptBundle("~/Scripts/Vue")
                .IncludeDirectory("~/Vue/_Compartilhado/Filtros", "*.js")
                .IncludeDirectory("~/Vue/_Compartilhado/Mixins", "*.js")
                .IncludeDirectory("~/Vue/_Compartilhado/Diretivas", "*.js")
                .IncludeDirectory("~/Vue/_Compartilhado/Servicos", "*.js")
                .IncludeDirectory("~/Vue/_Compartilhado/Componentes/Base", "*.js")
                .IncludeDirectory("~/Vue/_Compartilhado/Componentes", "*.js");
        }

        private static IEnumerable<Bundle> VueTemplatesBundle()
        {
            const string PATH = "~/Vue/_Compartilhado/Templates/";
            var caminhos = new string[] { "Base", "." };

            var bundles = new List<Bundle>();

            foreach (var caminho in caminhos)
            {
                var caminhoFinal = $"{PATH}{caminho}".TrimEnd('.', '/');
                var caminhoReal = HttpContext.Current.Server.MapPath(caminhoFinal);
                var diretorio = new DirectoryInfo(caminhoReal);

                foreach (var info in diretorio.GetFiles())
                {
                    bundles.Add(new TemplateBundle($"{caminhoFinal}/{info.Name}"));
                }
            }

            return bundles;
        }

        private static void Otimizar()
        {
#if DEBUG
            BundleTable.EnableOptimizations = false;
#else
            BundleTable.EnableOptimizations = true;
#endif
        }
    }
}
