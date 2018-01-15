using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Xml.Linq;
using Colosoft;

namespace Glass.Relatorios.UI.Web
{
    /// <summary>
    /// Implementação do visualizador de relatórios.
    /// </summary>
    public class GlassReportViewer : Colosoft.Reports.Web.ReportViewer
    {
        #region Variáveis Locais

        private static List<System.Security.Policy.StrongName> _fullTrustModules = new List<System.Security.Policy.StrongName>();
        private Action<string> _alteraStatus;

        #endregion

        #region Propriedades

        /// <summary>
        /// Caminho do diretório de logotipos.
        /// </summary>
        public string DiretorioLogotipos { get; private set; }

        /// <summary>
        /// Nome do funcionário responsável pelo relatório.
        /// </summary>
        public string NomeFuncionario { get; private set; }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="diretorioLogotipos"></param>
        /// <param name="nomeFuncionario"></param>
        public GlassReportViewer(string diretorioLogotipos, string nomeFuncionario, Action<string> alteraStatus)
        {
            this.DiretorioLogotipos = diretorioLogotipos;
            this.NomeFuncionario = nomeFuncionario;
            _alteraStatus = alteraStatus;
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Adiciona um assembly confiável para o dominio SandBox dos relatórios.
        /// </summary>
        /// <param name="assembly"></param>
        public static void AddFullTrustModuleInSandboxAppDomain(System.Reflection.Assembly assembly)
        {
            assembly.Require("assembly").NotNull();
            var assemblyName = assembly.GetName();
            AddFullTrustModuleInSandboxAppDomain(
                new System.Security.Policy.StrongName(
                    new System.Security.Permissions.StrongNamePublicKeyBlob(assemblyName.GetPublicKey()),
                    assemblyName.Name, assemblyName.Version));
        }

        /// <summary>
        /// Adiciona um modulo confiável para o dominio SandBox dos relatórios.
        /// </summary>
        /// <param name="strongName"></param>
        public static void AddFullTrustModuleInSandboxAppDomain(System.Security.Policy.StrongName strongName)
        {
            strongName.Require("strongName").NotNull();
            _fullTrustModules.Add(strongName);
        }

        /// <summary>
        /// Traduz os valor do enum.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="enumTypeName"></param>
        /// <returns></returns>
        public static string Translate(object value, string enumTypeName)
        {
            Type enumType = System.Web.Compilation.BuildManager.GetType(enumTypeName, false, true);

            if (enumType != null)
            {
                var tipo = Enum.GetUnderlyingType(enumType);
                var values = Enum.GetValues(enumType);

                var valorComp = value is ValueType ? 
                    Glass.Conversoes.ConverteValor(tipo, value) : 
                    null;

                for (var i = 0; i < values.Length; i++)
                {
                    if (valorComp != null &&
                        object.Equals(valorComp, Glass.Conversoes.ConverteValor(tipo, values.GetValue(i))))
                        return Colosoft.Translator.Translate(values.GetValue(i)).Format();
                }
            }

            return "";
        }

        /// <summary>
        /// Traduz os valor do enum.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="enumTypeName"></param>
        /// <param name="groupKey">Chave do grupo.</param>
        /// <returns></returns>
        public static string Translate2(object value, string enumTypeName, object groupKey)
        {
            Type enumType = System.Web.Compilation.BuildManager.GetType(enumTypeName, false, true);

            if (enumType != null)
            {
                var tipo = Enum.GetUnderlyingType(enumType);
                var values = Enum.GetValues(enumType);

                var valorComp = value is ValueType ?
                    Glass.Conversoes.ConverteValor(tipo, value) :
                    null;

                for (var i = 0; i < values.Length; i++)
                {
                    if (valorComp != null &&
                        object.Equals(valorComp, Glass.Conversoes.ConverteValor(tipo, values.GetValue(i))))
                        return Colosoft.Translator.Translate(values.GetValue(i), groupKey).Format();
                }
            }

            return "";
        }

        #endregion

        #region Métodos Protegidos

        /// <summary>
        /// Método usado para criar o relatório local.
        /// </summary>
        /// <returns></returns>
        protected override Microsoft.Reporting.WebForms.LocalReport CreateLocalReport()
        {
            //if (_alteraStatus != null)
            //    _alteraStatus("Iniciando relatório");

            var localReport = base.CreateLocalReport();
            localReport.EnableExternalImages = true;
            localReport.SetBasePermissionsForSandboxAppDomain
                (new PermissionSet(System.Security.Permissions.PermissionState.Unrestricted));

            AddAssembly(localReport, typeof(Colosoft.Translator).Assembly);
            AddAssembly(localReport, typeof(Colosoft.Reports.IReportDocument).Assembly);
            AddAssembly(localReport, typeof(Colosoft.Reports.Web.ReportViewer).Assembly);
            AddAssembly(localReport, typeof(GlassReportViewer).Assembly);

            foreach (var i in _fullTrustModules)
                localReport.AddFullTrustModuleInSandboxAppDomain(i);

            return localReport;
        }

        /// <summary>
        /// Adiciona o assembly para o relatório
        /// </summary>
        /// <param name="localReport"></param>
        /// <param name="assembly"></param>
        private static void AddAssembly(Microsoft.Reporting.WebForms.LocalReport localReport, System.Reflection.Assembly assembly)
        {
            var assemblyName = assembly.GetName();
            localReport.AddFullTrustModuleInSandboxAppDomain(
                new System.Security.Policy.StrongName(
                    new System.Security.Permissions.StrongNamePublicKeyBlob(assemblyName.GetPublicKey()),
                    assemblyName.Name, assemblyName.Version));
        }

        /// <summary>
        /// Carrega a definição do relatório.
        /// </summary>
        /// <param name="localReport"></param>
        protected override void LoadDefinition(Microsoft.Reporting.WebForms.LocalReport localReport)
        {
            if (Document is Colosoft.Reports.IReportDefinitionContainer)
            {
                //if (_alteraStatus != null)
                //    _alteraStatus("Carregando definição");

                System.Xml.Linq.XElement root = null;

                using (var stream = ((Colosoft.Reports.IReportDefinitionContainer)Document).GetDefinition())
                    root = System.Xml.Linq.XElement.Load(stream, System.Xml.Linq.LoadOptions.None);

                const string reportDefinition = "http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition";

                var codeElement = root.Elements(System.Xml.Linq.XName.Get("Code", reportDefinition)).FirstOrDefault();

                const string translateMethodHeader = "Public Function Translate";
                const string endMethod = "End Function";
                int index = 0;

                if (codeElement != null &&
                    !codeElement.IsEmpty &&
                    !string.IsNullOrEmpty(codeElement.Value) &&
                    (index = codeElement.Value.IndexOf(translateMethodHeader, StringComparison.InvariantCultureIgnoreCase)) >= 0)
                {
                    var endIndex = codeElement.Value.IndexOf(endMethod, index, StringComparison.InvariantCultureIgnoreCase);

                    // Alterar o método de tradução
                    codeElement.Value = codeElement.Value.Substring(0, index) +
                        @"Public Function Translate(ByVal instance As Object, ByVal enumTypeName As String) As String
                            return Glass.Relatorios.UI.Web.GlassReportViewer.Translate(instance, enumTypeName)
                          End Function" + codeElement.Value.Substring(endIndex + endMethod.Length);
                }

                const string translate2MethodHeader = "Public Function Translate2";
                index = 0;

                if (codeElement != null &&
                    !codeElement.IsEmpty &&
                    !string.IsNullOrEmpty(codeElement.Value) &&
                    (index = codeElement.Value.IndexOf(translate2MethodHeader, StringComparison.InvariantCultureIgnoreCase)) >= 0)
                {
                    var endIndex = codeElement.Value.IndexOf(endMethod, index, StringComparison.InvariantCultureIgnoreCase);

                    // Alterar o método de tradução
                    codeElement.Value = codeElement.Value.Substring(0, index) +
                        @"Public Function Translate2(ByVal instance As Object, ByVal enumTypeName As String, ByVal groupKey As String) As String
                            return Glass.Relatorios.UI.Web.GlassReportViewer.Translate2(instance, enumTypeName, groupKey)
                          End Function" + codeElement.Value.Substring(endIndex + endMethod.Length);
                }

                var codeModules =
                     root.Elements(System.Xml.Linq.XName.Get("CodeModules", reportDefinition))
                         .FirstOrDefault();

                if (codeModules == null)
                {
                    codeModules = new XElement(XName.Get("CodeModules", reportDefinition));
                    root.Add(codeModules);
                }

                codeModules.Add(new XElement(XName.Get("CodeModule", reportDefinition))
                {
                    Value = "Colosoft.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=d3b3c440aed9b980"
                });

                codeModules.Add(new XElement(XName.Get("CodeModule", reportDefinition))
                {
                    Value = "Colosoft.Reports, Version=1.0.0.0, Culture=neutral, PublicKeyToken=ec8331ec4228d300"
                });

                codeModules.Add(new XElement(XName.Get("CodeModule", reportDefinition))
                {
                    Value = "Colosoft.Reports.Web, Version=1.0.0.0, Culture=neutral, PublicKeyToken=b0156e5c4c7a6003"
                });

                codeModules.Add(new XElement(XName.Get("CodeModule", reportDefinition))
                {
                    Value = "Glass.Relatorios, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
                });

                codeModules.Add(new XElement(XName.Get("CodeModule", reportDefinition))
                {
                    Value = "Glass.Relatorios.UI.Web, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
                });

                using (var stream = new System.IO.MemoryStream())
                {
                    root.Save(stream);
                    stream.Seek(0, System.IO.SeekOrigin.Begin);
                    localReport.LoadReportDefinition(stream);
                }
            }
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Exporta o relatório.
        /// </summary>
        /// <param name="exportType"></param>
        /// <param name="mimeType"></param>
        /// <param name="encoding"></param>
        /// <param name="fileNameExtension"></param>
        /// <param name="warnings"></param>
        /// <returns></returns>
        public override System.IO.Stream Export(Colosoft.Reports.Web.ExportType exportType, out string mimeType, out string encoding, out string fileNameExtension, out Colosoft.Reports.Warning[] warnings)
        {
            if (_alteraStatus != null)
                _alteraStatus("Criando relatório");

            var userInfo = Glass.Data.Helper.UserInfo.GetUserInfo;

            if (!Document.Parameters.ContainsKey("Logotipo"))
            {
                int? idLoja = null;

                if (Document is Glass.Relatorios.IRelatorioLoja)
                    idLoja = ((Glass.Relatorios.IRelatorioLoja)Document).IdLoja;

                // Recupera o caminho físico do arquivo do logotipo
                var caminhoLogotipo = Configuracoes.Logotipo.GetReportLogo(DiretorioLogotipos, (uint?)idLoja);

                Document.Parameters.Add("Logotipo", caminhoLogotipo);
            }

            if (!Document.Parameters.ContainsKey("TextoRodape"))
                Document.Parameters.Add("TextoRodape", ConfiguracaoRelatorio.TextoRodapeRelatorio(NomeFuncionario));

            if (!Document.Parameters.ContainsKey("CorRodape"))
                Document.Parameters.Add("CorRodape", "DimGray");

            return base.Export(exportType, out mimeType, out encoding, out fileNameExtension, out warnings);
        }

        #endregion
    }
}
