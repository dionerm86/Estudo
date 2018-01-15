using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class EtiquetaConfig
    {
        /// <summary>
        /// Classe com as configurações do relatório de etiqueta.
        /// </summary>
        public static class RelatorioEtiqueta
        {
            /// <summary>
            /// Nome do arquivo do relatório.
            /// </summary>
            public static string NomeArquivoRelatorio(int idLoja, bool notaFiscal, bool retalho)
            {
                var caminhoRelatorio = string.Empty;

                // Nota Fiscal
                if (notaFiscal)
                {
                    if (ModeloEtiquetaPorLoja && idLoja > 0)
                    {
                        var caminhoRelatorioLoja = string.Format("Relatorios/ModeloEtiqueta/rptEtiqueta{0}NF{1}.rdlc", ControleSistema.GetSite().ToString(), idLoja);

                        if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath(string.Format("~/{0}", caminhoRelatorioLoja))))
                            return caminhoRelatorioLoja;
                    }

                    caminhoRelatorio = string.Format("Relatorios/ModeloEtiqueta/rptEtiqueta{0}NF.rdlc", ControleSistema.GetSite().ToString());

                    if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath(string.Format("~/{0}", caminhoRelatorio))))
                        return caminhoRelatorio;             
                }

                // Retalho
                if (retalho)
                {
                    if (ModeloEtiquetaPorLoja && idLoja > 0)
                    {
                        var caminhoRelatorioLoja = string.Format("Relatorios/ModeloEtiqueta/rptEtiqueta{0}Ret{1}.rdlc", ControleSistema.GetSite().ToString(), idLoja);

                        if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath(string.Format("~/{0}", caminhoRelatorioLoja))))
                            return caminhoRelatorioLoja;
                    }

                    caminhoRelatorio = string.Format("Relatorios/ModeloEtiqueta/rptEtiqueta{0}Ret.rdlc", ControleSistema.GetSite().ToString());

                    if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath(string.Format("~/{0}", caminhoRelatorio))))
                        return caminhoRelatorio;
                }

                // Padrão
                if (ModeloEtiquetaPorLoja && idLoja > 0)
                {
                    var caminhoRelatorioLoja = string.Format("Relatorios/ModeloEtiqueta/rptEtiqueta{0}{1}.rdlc", ControleSistema.GetSite().ToString(), idLoja);

                    if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath(string.Format("~/{0}", caminhoRelatorioLoja))))
                        return caminhoRelatorioLoja;
                }

                caminhoRelatorio = string.Format("Relatorios/ModeloEtiqueta/rptEtiqueta{0}.rdlc", ControleSistema.GetSite().ToString());

                if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath(string.Format("~/{0}", caminhoRelatorio))))
                    return caminhoRelatorio;

                return PedidoConfig.LiberarPedido ? "Relatorios/ModeloEtiqueta/rptEtiquetaLib.rdlc" :
                    "Relatorios/ModeloEtiqueta/rptEtiqueta.rdlc";
            }

            /// <summary>
            /// Permitir leitura de etiqueta de pedidos de produção mesmo que não tenha sido impressa
            /// </summary>
            public static bool UsarThreadRelatorioEtiqueta
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarThreadRelatorioEtiqueta); }
            }

            /// <summary>
            /// Define que a descrição do grupo do projeto deverá ser carregada para ser exibida na etiqueta
            /// </summary>
            public static bool CarregarDescricaoGrupoProjeto
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.CarregarDescricaoGrupoProjeto); }
            }

            /// <summary>
            /// Define que será usado número sequencial na impressão da etiqueta
            /// </summary>
            public static bool UsarNumeroSequencial
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarNumeroSequencial); }
            }

            /// <summary>
            /// Define que o layout da etiqueta da empresa é definido por loja.
            /// </summary>
            public static bool ModeloEtiquetaPorLoja
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ModeloEtiquetaPorLoja); }
            }
        }
    }
}
