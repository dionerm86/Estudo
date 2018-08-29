using Glass.Data.Helper;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

namespace Glass.Relatorios.UI.Web
{
    /// <summary>
    /// Implementação básica da página de relatórios.
    /// </summary>
    public abstract class ReportPage : System.Web.UI.Page
    {
        #region Classes de Suporte

        protected class JavaScriptData
        {
            public delegate string GetJavaScriptError(Exception error);

            public bool BackgroundLoading { get; private set; }
            public string IsLoading { get; private set; }
            public GetJavaScriptError JavaScriptError { get; private set; }

            public JavaScriptData(bool backgroundLoading, string isLoading)
                : this(backgroundLoading, isLoading, null)
            {
            }

            public JavaScriptData(bool backgroundLoading, string isLoading, GetJavaScriptError javaScriptError)
            {
                BackgroundLoading = backgroundLoading;
                IsLoading = isLoading;
                JavaScriptError = javaScriptError;
            }
        }

        public class ReportException : Exception
        {
            public string JavaScript { get; private set; }

            public ReportException(Exception innerException)
                : this(innerException, null)
            {
            }

            public ReportException(Exception innerException, string javaScript) :
                base("Erro ao processar relatório.", innerException)
            {
                JavaScript = javaScript;
            }
        }

        #endregion

        private string _mimeType;
        private Stream _stream;
        private static Dictionary<ChaveDicionario, DadosDicionario> _threads;
        private IDictionary<string, IEnumerable<System.Reflection.PropertyInfo>> _propriedadesSubreport;

        static ReportPage()
        {
            _threads = new Dictionary<ChaveDicionario, DadosDicionario>(new Comparador());
        }

        public ReportPage()
        {
            _mimeType = null;
            _propriedadesSubreport = new Dictionary<string, IEnumerable<System.Reflection.PropertyInfo>>();

            Page.Load += new EventHandler(Page_Load);
        }

        private Aleatorio _aleatorio
        {
            get
            {
                if (ViewState["aleatorio"] == null)
                    ViewState["aleatorio"] = new Aleatorio(Request, TemPostData(false));

                return ViewState["aleatorio"] as Aleatorio;
            }
        }

        private void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(ReportPage));
            RegistraJavaScript();
        }

        protected abstract object[] Parametros { get; }
        protected abstract JavaScriptData DadosJavaScript { get; }

        protected virtual bool UsarThread
        {
            // Chamado 57786: Config necessária para clientes que não estava aparecendo sempre a logo nos relatórios
            get { return System.Configuration.ConfigurationSettings.AppSettings["UsarThreadRelatorio"] != "false"; }
        }

        protected virtual bool PermitirFecharTelaDuranteAguarde
        {
            get { return false; }
        }

        #region Itens para o controle dos Threads da página

        private class ChaveDicionario
        {
            public uint IdFunc;
            public int Aleatorio;
            public string Url;

            public ChaveDicionario(uint idFunc, string url, int aleatorio)
            {
                IdFunc = idFunc;
                Url = url;
                Aleatorio = aleatorio;
            }
        }

        private class DadosDicionario
        {
            public Thread Thread = null;
            public byte[] Dados = null;
            public bool Executando = false;
            public Exception Erro = null;
            public bool ExportarExcel = false;
            public Warning[] Warnings = null;
            public string[] StreamIds = null;
            public string MimeType = null;
            public string Encoding = null;
            public string Extension = null;
            public object[] Parametros = null;
            public List<ReportParameter> ParametrosRelatorio = null;
            public string BasePath = null;
            public MemoryStream Stream = null;
            public LocalReport Report = null;
            public StateBag ViewState = null;
            public string Status = "Iniciando";
            public string Url;
        }

        private class Comparador : IEqualityComparer<ChaveDicionario>
        {
            public bool Equals(ChaveDicionario x, ChaveDicionario y)
            {
                return x.Aleatorio == y.Aleatorio &&
                    x.IdFunc == y.IdFunc &&
                    x.Url == y.Url;
            }

            public int GetHashCode(ChaveDicionario obj)
            {
                return 1;
            }
        }

        [Serializable]
        protected class Aleatorio
        {
            private string _url, _userHostAddress;
            private NameValueCollection _request;
            private bool _temPostData;
            private int _aleatorio;

            public Aleatorio(HttpRequest request, bool temPostData)
            {
                _url = request.Url.ToString();
                _userHostAddress = request.UserHostAddress;
                _temPostData = temPostData;
                _request = new NameValueCollection();

                foreach (string key in request.QueryString)
                    _request.Add(key, request[key]);

                foreach (string key in request.Form)
                    _request.Add(key, request[key]);

                _aleatorio = _request["aleatorio"] != null ? Glass.Conversoes.StrParaInt(_request["aleatorio"]) :
                    new Random(DateTime.Now.Millisecond).Next();
            }

            public string GetUrlSemAleatorio()
            {
                string url = _url;

                if (_temPostData)
                    url = _url.Split('?')[0] + "?rel=" + _request["rel"];

                while (url.Contains("&aleatorio="))
                {
                    int indexFinal = url.IndexOf("&", url.IndexOf("&aleatorio=") + 3);
                    url = url.Substring(0, url.IndexOf("&aleatorio=")) + (indexFinal > -1 ? url.Substring(indexFinal) : "");
                }

                while (url.Contains("?aleatorio="))
                {
                    int indexFinal = url.IndexOf("?", url.IndexOf("?aleatorio=") + 3);
                    url = url.Substring(0, url.IndexOf("?aleatorio=")) + (indexFinal > -1 ? url.Substring(indexFinal) : "");
                }

                return url;
            }

            public int Get()
            {
                return _aleatorio;
            }
        }

        #endregion

        #region Processamento do relatório (thread)

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
        /// Faz o processamento do relatório.
        /// </summary>
        protected void ProcessaReport(Control aguardar)
        {
            if (TemPostData(true)) return;

            DadosDicionario dados = new DadosDicionario();

            dados.ExportarExcel = Request["ExportarExcel"] == "true";
            dados.Parametros = Parametros;
            dados.BasePath = Server.MapPath("~/");
            dados.Report = new LocalReport();
            dados.ViewState = this.ViewState;
            dados.Url = Request.Url.ToString();

            dados.Report.ShowDetailedSubreportMessages = true;

            dados.Report.SetBasePermissionsForSandboxAppDomain
                (new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted));

            AddAssembly(dados.Report, typeof(Glass.Data.Helper.Config).Assembly);

            dados.ParametrosRelatorio = new List<ReportParameter>();
            dados.Report.EnableExternalImages = true;
            dados.Report.SubreportProcessing += new SubreportProcessingEventHandler(report_SubreportProcessing);

            Warning[] Warnings = null;
            string[] StreamIds = null;
            string Encoding = null;
            string Extension = null;
            byte[] bytes = null;

            var login = UserInfo.GetUserInfo;
            bool naoUtilizarThread = Request["semThread"] == "true" || !UsarThread;
            var diretorioLogotipos = Server.MapPath("~/images");
            var nomeFuncionario = login != null ? login.Nome : string.Empty;
            
            if (Request.Browser.IsMobileDevice)
            {
                LoadReport(null, ref dados.Report, ref dados.ParametrosRelatorio, HttpContext.Current.Request, dados.Parametros, login, diretorioLogotipos);

                VerificarParametros(dados.Report, ref dados.ParametrosRelatorio);
                dados.Report.SetParameters(dados.ParametrosRelatorio);
                byte[] dadosMobile = null;

                if (!dados.ExportarExcel)
                    dadosMobile = dados.Report.Render("PDF", null, out dados.MimeType, out dados.Encoding, out dados.Extension, out dados.StreamIds, out dados.Warnings);
                else
                    dadosMobile = dados.Report.Render("Excel", null, out dados.MimeType, out dados.Encoding, out dados.Extension, out dados.StreamIds, out dados.Warnings);

                //Utilizado para baixar arquivo
                Response.ContentType = _mimeType;
                Response.AddHeader("Content-Length", dadosMobile.Length.ToString());
                Response.OutputStream.Write(dadosMobile, 0, dadosMobile.Length);

                Page.ClientScript.RegisterClientScriptBlock(GetType(), "callbackPronto",
                    string.Format("window.opener.{0}", Request["callbackPronto"]), true);

                var nomeRelatorio = string.Empty;

                if (Request.Path.ToLower().Contains("relpedido.aspx"))
                {
                    nomeRelatorio = string.Format("; filename=PD{0}{1}", Request["idPedido"],
                        (Request["ExportarExcel"] == "true" ? ".xls" : ".pdf"));
                }
                else if (Request["rel"] != null && Request["rel"].ToLower() == "imagemprojeto" && Request["idItemProjeto"] != null)
                {
                    uint idItemProjeto = Request["idItemProjeto"].Split(',')[0].StrParaUint();
                    if (idItemProjeto > 0)
                    {
                        uint? idPedido = Glass.Data.DAL.ItemProjetoDAO.Instance.ObtemIdPedido(idItemProjeto);
                        uint? idPedidoEspelho = Glass.Data.DAL.ItemProjetoDAO.Instance.ObtemIdPedidoEspelho(idItemProjeto);

                        if (idPedido > 0)
                            nomeRelatorio = string.Format("; filename=PJ{0}{1}",
                                idPedido, (Request["ExportarExcel"] == "true" ? ".xls" : ".pdf"));
                        else if (idPedidoEspelho > 0)
                            nomeRelatorio = string.Format("; filename=PJ{0}{1}",
                                idPedidoEspelho, (Request["ExportarExcel"] == "true" ? ".xls" : ".pdf"));
                    }
                }
                else if (Request["rel"] != null)
                    nomeRelatorio = string.Format("; filename={0}.{1}", Request["rel"], dados.Extension);
                else
                    nomeRelatorio = string.Format("; filename=Relatorio.{0}", dados.Extension);

                Response.AddHeader("Content-Disposition", string.Format("attachment{0}", nomeRelatorio));
                Response.End();
            }

            if (naoUtilizarThread)
            {
                try
                {
                    var reportDocument = LoadReport(null, ref dados.Report, ref dados.ParametrosRelatorio, HttpContext.Current.Request, dados.Parametros, login, diretorioLogotipos);

                    if (reportDocument != null)
                    {
                        var viewer = new GlassReportViewer(diretorioLogotipos, nomeFuncionario, null);
                        viewer.Document = reportDocument;
                        Colosoft.Reports.Warning[] warnings2 = null;

                        viewer.Export(
                            Request["ExportarExcel"] != "true" ?
                            Colosoft.Reports.Web.ExportType.Pdf :
                            Colosoft.Reports.Web.ExportType.Excel, CreateStream, out warnings2);
                    }
                    else
                    {
                        VerificarParametros(dados.Report, ref dados.ParametrosRelatorio);
                        dados.Report.SetParameters(dados.ParametrosRelatorio);

                        if (Request["ExportarExcel"] != "true")
                            dados.Report.Render("PDF", null, new CreateStreamCallback(CreateStream), out Warnings);
                        else
                            dados.Report.Render("Excel", null, new CreateStreamCallback(CreateStream), out Warnings);
                    }

                    _stream.Position = 0;
                    bytes = new byte[_stream.Length];
                    _stream.Read(bytes, 0, bytes.Length);

                    if (dados.Report != null)
                    {
                        dados.Report.Dispose();
                        dados.Report = null;
                    }
                }
                catch (Exception ex)
                {
                    throw new ReportException(ex,
                        this.DadosJavaScript.JavaScriptError == null ? null :
                        this.DadosJavaScript.JavaScriptError(ex));
                }
            }
            else
            {
                #region Thread do relatório

                var request = Request;
                var exportType = Request["ExportarExcel"] != "true" ?
                    Colosoft.Reports.Web.ExportType.Pdf :
                    Colosoft.Reports.Web.ExportType.Excel;

                Thread t = new Thread(new System.Threading.ParameterizedThreadStart(
                    delegate(object dadosThread)
                    {
                        DadosDicionario d = (DadosDicionario)dadosThread;

                        d.Executando = true;

                        try
                        {
                            //Chamado 10263, salvar logs da abertura do relatorio.
                            var sw = new Stopwatch();
                            var log = Environment.NewLine;

                            sw.Start();

                            UserInfo.ConfigurarLoginUsuarioGetterThread(() => login);
                            log += "Funcionário: " + login.CodUser + " - " + DateTime.Now + Environment.NewLine;

                            d.Status = "Buscando dados";
                            var reportDocument = LoadReport(d.BasePath, ref d.Report, ref d.ParametrosRelatorio, request, d.Parametros, login, diretorioLogotipos);
                            log += d.Url + Environment.NewLine + "Buscando dados - " + sw.Elapsed.ToString() + Environment.NewLine;

                            if (reportDocument != null)
                            {
                                var viewer = new GlassReportViewer(diretorioLogotipos, nomeFuncionario, x => d.Status = x);
                                viewer.Document = reportDocument;
                                Colosoft.Reports.Warning[] warnings2 = null;

                                using (var stream = viewer.Export(exportType, out d.MimeType, out d.Encoding, out d.Extension, out warnings2))
                                {
                                    if (stream is System.IO.MemoryStream)
                                        d.Dados = ((System.IO.MemoryStream)stream).ToArray();
                                }
                            }
                            else
                            {
                                VerificarParametros(d.Report, ref d.ParametrosRelatorio);
                                d.Status = "Definindo parâmetros";
                                d.Report.SetParameters(d.ParametrosRelatorio);
                                d.Report.EnableExternalImages = true;
                                log += "Definindo parâmetros - " + sw.Elapsed.ToString() + Environment.NewLine;

                                d.Status = "Criando relatório";
                                if (!dados.ExportarExcel)
                                    d.Dados = d.Report.Render("PDF", null, out d.MimeType, out d.Encoding, out d.Extension, out d.StreamIds, out d.Warnings);
                                else
                                    d.Dados = d.Report.Render("Excel", null, out d.MimeType, out d.Encoding, out d.Extension, out d.StreamIds, out d.Warnings);
                                log += "Criando relatório - " + sw.Elapsed.ToString() + Environment.NewLine;
                            }

                            d.Status = "Carregando";

                            sw.Stop();
                            log += "Carregado - " + sw.Elapsed.ToString();

                            if (Glass.Configuracoes.Geral.GravarLogAberturaRelatorio)
                                Glass.Data.Helper.LogArquivo.InsereLogAberturaRelatorio(log);

                            d.Executando = false;
                        }
                        catch (System.Threading.ThreadAbortException)
                        {
                            d.Executando = false;
                        }
                        catch (Exception ex)
                        {
                            d.Erro = ex;
                            d.Executando = false;
                        }

                        if (dados.Report != null)
                        {
                            d.Report.Dispose();
                            d.Report = null;
                        }
                    }
                ));

                #endregion

                ChaveDicionario chave = new ChaveDicionario(UserInfo.GetUserInfo.CodUser,
                    _aleatorio.GetUrlSemAleatorio(), _aleatorio.Get());

                #region Controle do Thread

                if (_threads.ContainsKey(chave) && !_threads[chave].Executando)
                {
                    if (_threads[chave].Erro != null)
                    {
                        Exception ex = _threads[chave].Erro;
                        FecharThread(chave);

                        throw new ReportException(ex,
                            this.DadosJavaScript.JavaScriptError == null ? null :
                            this.DadosJavaScript.JavaScriptError(ex));
                    }
                    else
                    {
                        bytes = _threads[chave].Dados;
                        Warnings = _threads[chave].Warnings;
                        StreamIds = _threads[chave].StreamIds;
                        _mimeType = _threads[chave].MimeType;
                        Encoding = _threads[chave].Encoding;
                        Extension = _threads[chave].Extension;

                        FecharThread(chave);
                    }
                }
                else
                {
                    if (_threads.ContainsKey(chave))
                    {
                        FecharThread(chave);
                        chave.Aleatorio = _aleatorio.Get();
                    }
                    else if (Request["aleatorio"] != null)
                    {
                        Response.Redirect(chave.Url);
                        return;
                    }

                    dados.Thread = t;

                    _threads.Add(chave, dados);
                    dados.Thread.Start(_threads[chave]);
                }

                if (_threads.ContainsKey(chave) && bytes == null && aguardar != null)
                {
                    var item = new System.Web.UI.HtmlControls.HtmlGenericControl();
                    item.InnerHtml = @"
                        <table align='center' style='margin-top: 200px'>
                            <tr>
                                <td align='center'>
                                    <img src='" + this.ResolveClientUrl("~/Images/Load.gif") + @"' />
                                </td>
                            </tr>
                            <tr>
                                <td align='center' style='font-size: xx-large'>
                                    Aguarde
                                </td>
                            </tr>
                            <tr>
                                <td align='center' style='font-size: small; font-style: italic'>
                                    <span id='status'>Iniciando</span>
                                </td>
                            </tr>
                        </table>";

                    if (!PermitirFecharTelaDuranteAguarde)
                        item.InnerHtml += @"
                        <script type='text/javascript'>
                            window.onbeforeunload = function()
                            {
                                return 'As alterações feitas por você ainda estão sendo processadas.\n' +
                                    'Se você sair agora elas podem não ser feitas corretamente.\n' +
                                    'Tem certeza que deseja sair?';
                            }
                        </script>";

                    aguardar.Controls.Add(item);
                    return;
                }

                #endregion
            }

            dados = null;

            try
            {
                #region Monta o nome do arquivo
                
                var nomeRelatorio = string.Empty;
                
                if (!string.IsNullOrEmpty(Request["rel"]))
                    nomeRelatorio = string.Format("; filename={0}.{1}", Request["rel"], Extension);
                else if (Request.Path.ToLower().Contains("relpedido.aspx") && !Request["idPedido"].Contains(","))
                    nomeRelatorio = "; filename=PD" + Request["idPedido"] + (Request["ExportarExcel"] == "true" ? ".xls" : ".pdf");
                else if (Request.Path.ToLower().Contains("relorcamento.aspx"))
                    nomeRelatorio = "; filename=OR" + Request["idOrca"] + (Request["ExportarExcel"] == "true" ? ".xls" : ".pdf");
                else if (Request["rel"] != null && Request["rel"].ToLower() == "imagemprojeto" && Request["idItemProjeto"] != null)
                {
                    uint idItemProjeto = Conversoes.StrParaUint(Request["idItemProjeto"].Split(',')[0]);
                    if (idItemProjeto > 0)
                    {
                        uint? idPedido = Glass.Data.DAL.ItemProjetoDAO.Instance.ObtemIdPedido(idItemProjeto);
                        uint? idPedidoEspelho = Glass.Data.DAL.ItemProjetoDAO.Instance.ObtemIdPedidoEspelho(idItemProjeto);

                        if (idPedido > 0)
                            nomeRelatorio = "; filename=PJ" + idPedido + (Request["ExportarExcel"] == "true" ? ".xls" : ".pdf");
                        else if (idPedidoEspelho > 0)
                            nomeRelatorio = "; filename=PJ" + idPedidoEspelho + (Request["ExportarExcel"] == "true" ? ".xls" : ".pdf");
                    }
                }
                else
                    nomeRelatorio = string.Format("; filename=Relatorio.{0}", Extension);

                #endregion

                Response.ContentType = _mimeType;
                Response.AddHeader("Content-Length", bytes.Length.ToString());
                Response.AddHeader("Content-disposition", "inline" + nomeRelatorio);

                // Utilizado para exibir na tela
                if (bytes != null && bytes.Length > 0)
                {
                    Response.Clear();
                    Response.BinaryWrite(bytes);
                    Response.Flush();
                }
                else if (naoUtilizarThread && Request["callbackPronto"] != null)
                    Page.ClientScript.RegisterClientScriptBlock(GetType(), "callbackPronto", "window.opener." + Request["callbackPronto"], true);

                try
                {
                    Response.End();
                }
                catch (System.Threading.ThreadAbortException)
                {
                    // Ignora
                }
            }
            catch (Exception ex)
            {
                ex.Data["warnings"] = Warnings;
                ex.Data["streamids"] = StreamIds;
                ex.Data["mimetype"] = _mimeType;
                ex.Data["encoding"] = Encoding;
                ex.Data["extension"] = Extension;
                throw ex;
            }
        }

        #endregion

        #region Registra os JavaScripts

        private bool TemPostData(bool semDadosPost)
        {
            return Request["postData"] != null && (semDadosPost ? Request.Form.Count == 0 :
                Request.Form.Count > 0 && Request["rel"] != null);
        }

        private void RegistraJavaScript()
        {
            string script = @"
                var idFunc = " + (UserInfo.GetUserInfo != null ? UserInfo.GetUserInfo.CodUser.ToString() : "999") + @";
                var url = '" + _aleatorio.GetUrlSemAleatorio() + @"';
                var aleatorio = " + _aleatorio.Get() + @";
                var fechando = false;

                window.opener = !!window.opener ? window.opener : window.parent;

                function isLoading()
                {
                    try {
                        return (" + DadosJavaScript.IsLoading + @") == true ? true : false;
                    }
                    catch (err) {
                        return false;
                    }
                }

                function unload()
                {
                    if (aleatorio == -1)
                        return;

                    fechando = true;
                    
                    var resposta;                        
                    if (!isLoading())
                        resposta = " + typeof(ReportPage).Name + @".FecharThread(idFunc, url, aleatorio, 1).value == 'true';
                }

                function esperarCarregar()
                {
                    if (aleatorio == -1 || fechando)
                        return;
                    
                    if (isLoading())
                    {
                        setTimeout(function() { esperarCarregar() }, 250);
                        return;
                    }

                    var status = document.getElementById('status');

                    " + typeof(ReportPage).Name + @".Carregou(idFunc, url, aleatorio, status.innerText, 30, function(resposta) {
                        var excel = " + (Request["exportarExcel"] == "true").ToString().ToLower() + @";
                        
                        if (resposta == null)
                            postBack();

                        resposta = resposta.value.split('|');
                        status.innerText = resposta[1];

                        switch (resposta[0])
                        {
                            case '1':
                                preparaPostBack();

                                " + (Request["callbackPronto"] != null ? "window.opener." + Request["callbackPronto"] + ";" : "// Sem calback") + @"
                                
                                var opener = !excel ? window : window.opener;
                                var concatenar = url.indexOf('?') == -1 ? '?' : '&';
                                opener.redirectUrl(url + concatenar + 'aleatorio=' + aleatorio);
                                
                                if (excel)
                                    setTimeout(function() { closeWindow() }, 1000);
                                
                                break;
                            
                            case '2':
                                setTimeout(function() { esperarCarregar() }, 250);
                                break;
                            
                            default:
                                if (resposta != '0')
                                    alert(resposta);
                                
                                setTimeout(function() { esperarCarregar() }, 250);
                                break;
                        }
                    });
                }

                function preparaPostBack()
                {
                    window.onunload = null;
                    window.onbeforeunload = null;
                    $(window).off('beforeunload');
                }

                function postBack()
                {
                    preparaPostBack();

                    if (!!window['__doPostBack'])
                        __doPostBack();
                    else
                    {
                        triggerEvent(document.forms[0], 'submit');
                        document.forms[0].submit();
                    }
                }

                function getPostData()
                {
                    var data = " + (TemPostData(true) ? "window.opener." + Request["postData"] : "new Object()") + @";
                    data['isPostData'] = '" + (Request["isPostData"] != "true" ? "true" : "false") + @"';

                    for (var key in data)
                    {
                        var campo = document.createElement('input');
                        campo.id = key;
                        campo.name = key;
                        campo.style.display = 'none';
                        campo.value = data[key];
                        
                        document.forms[0].appendChild(campo);
                    }
                    
                    postBack();
                }

                function loadInterno()
                {
                    if (isLoading())
                    {
                        setTimeout(loadInterno, 100);
                        return;
                    }

                    if (" + TemPostData(true).ToString().ToLower() + @")
                        getPostData();
                    else if (" + (!DadosJavaScript.BackgroundLoading).ToString().ToLower() + @")
                    {
                        window.onunload = unload;
                        setTimeout(function() { esperarCarregar(); }, 10);
                    }
                }

                var _loadOriginal = window.onload;

                window.onload = function() {
                    if (!!_loadOriginal)
                        _loadOriginal();

                    loadInterno();
                }";

            Page.ClientScript.RegisterClientScriptBlock(GetType(), "controlJavaScript", script + "\n", true);
        }

        #endregion

        #region Carrega o relatório

        protected abstract Colosoft.Reports.IReportDocument LoadReport(ref LocalReport report, ref List<ReportParameter> lstParam,
            HttpRequest PageRequest, NameValueCollection Request, object[] outrosParametros, LoginUsuario login, string diretorioLogotipos);

        private Colosoft.Reports.IReportDocument LoadReport(string basePath, ref LocalReport report, ref List<ReportParameter> lstParam,
            HttpRequest Request, object[] outrosParametros, LoginUsuario login, string diretorioLogotipos)
        {
            NameValueCollection requestData = new NameValueCollection();

            foreach (string key in Request.QueryString)
                requestData.Add(key, Request[key]);

            foreach (string key in Request.Form)
                requestData.Add(key, Request[key]);

            var reportDocument = LoadReport(ref report, ref lstParam, Request, requestData, outrosParametros, login, diretorioLogotipos);

            // Caso algum parâmetro esteja com valor vazio, preenche com "."
            foreach (var p in lstParam)
                for (int i = 0; i < p.Values.Count; i++)
                    if (String.IsNullOrEmpty(p.Values[i]))
                        p.Values[i] = ".";

            if (!String.IsNullOrEmpty(basePath) && report != null && report.ReportPath != null)
                report.ReportPath = basePath + report.ReportPath.Replace("/", "\\");
            
            return reportDocument;
        }

        /// <summary>
        /// Caso algum parâmetro esteja com valor vazio, preenche com ".".
        /// Caso tenha sido atribuído algum parâmetro que não exista no relatório, remove antes de renderizá-lo.
        /// Verifica também se faltou ser definido algum parâmetro no relatório, neste caso, lança exceção com o parâmetro que faltou.
        /// </summary>
        /// <param name="caminhoRelatorio"></param>
        /// <param name="lstParam"></param>
        private void VerificarParametros(LocalReport report, ref List<ReportParameter> lstParam)
        {
            // Pega os parâmetros do rdlc
            var parametrosRdlc = report.GetParameters();

            if (parametrosRdlc == null)
            {
                report.Refresh();
                parametrosRdlc = report.GetParameters();

                if (parametrosRdlc == null)
                    return;

                return;
            }

            // Caso tenha sido definido algum parâmetro a mais, remove da lista de parâmetros
            for (int i = 0; i < lstParam.Count; i++)
                if (!parametrosRdlc.Select(f => f.Name.ToLower()).Contains(lstParam[i].Name.ToLower()))
                {
                    lstParam.Remove(lstParam[i]);
                    i--;
                }

                // Caso tenha faltado definir algum parâmetro, avisa o usuário
                var parametrosNaoDefinidos = new List<string>();
                foreach (var pRdlc in parametrosRdlc)
                    if (!lstParam.Select(f => f.Name.ToLower()).Contains(pRdlc.Name.ToLower()))
                        parametrosNaoDefinidos.Add(pRdlc.Name);

                if (parametrosNaoDefinidos.Count > 0)
                    throw new Exception(string.Format("Parâmetro{0} não definido{0}: {1}",
                        parametrosNaoDefinidos.Count == 1 ? string.Empty : "s",
                        string.Join(", ", parametrosNaoDefinidos)));
         
        }

        #endregion

        #region SubReport

        /// <summary>
        /// Evento de processamento dos sub-relatórios.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void report_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            // Recupera o relatório pai
            LocalReport report = (LocalReport)sender;

            // Carrega as informações dos DataSources do pai
            if (_propriedadesSubreport.Count == 0)
                lock (_propriedadesSubreport)
                {
                    if (_propriedadesSubreport.Count == 0)
                        foreach (var dataSource in report.DataSources)
                        {
                            // Tenta recuperar o tipo do objeto, se for um enumerador
                            if (typeof(System.Collections.IEnumerable).IsAssignableFrom(dataSource.Value.GetType()))
                            {
                                // Carrega as propriedades para um tipo de DataSource
                                if (!_propriedadesSubreport.ContainsKey(dataSource.Name))
                                {
                                    var enumerador = (dataSource.Value as System.Collections.IEnumerable).GetEnumerator();

                                    if (enumerador.MoveNext() && enumerador.Current != null)
                                    {
                                        // Recupera o tipo do objeto e suas propriedades
                                        var tipoObjeto = enumerador.Current.GetType();
                                        _propriedadesSubreport.Add(dataSource.Name, tipoObjeto.GetProperties());
                                    }
                                }
                            }
                        }
                }

            // Busca por propriedades que possam estar em parâmetros
            var nomesParametros = e.Parameters.Select(x => x.Name).ToList();

            // Carrega no subreport os DataSources definidos no pai
            foreach (string dataSource in e.DataSourceNames)
            {
                var itens = report.DataSources[dataSource].Value;
                
                // Tenta recuperar o tipo do objeto, se for um enumerador
                if (_propriedadesSubreport.ContainsKey(dataSource))
                {
                    var propriedades = _propriedadesSubreport[dataSource]
                        .Where(x => nomesParametros.Contains(x.Name));

                    // Se houver propriedades, aplica como filtros
                    if (propriedades.Any())
                    {
                        // Cria uma lista temporária
                        var novos = (itens as System.Collections.IEnumerable)
                            .Cast<object>();

                        // Filtra pelas propriedades
                        foreach (var p in propriedades)
                        {
                            var parametro = e.Parameters
                                .Where(y => y.Name == p.Name)
                                .FirstOrDefault();

                            // Não filtra se o parâmetro tiver mais de um valor ou se não houver valor
                            if (parametro.MultiValue || string.IsNullOrEmpty(parametro.Values[0]) || parametro.Values[0] == ".")
                                continue;

                            novos = novos
                                .Where(x => String.Equals(
                                    Glass.Conversoes.ConverteValor<string>(p.GetValue(x, null)),
                                    parametro.Values[0]));
                        }

                        // Atribui os itens filtrados à variável que será enviada ao DataSource
                        itens = novos.ToList();
                    }
                }

                e.DataSources.Add(new ReportDataSource(dataSource, itens));
            }
        }

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Cria um Stream.
        /// </summary>
        /// <returns></returns>
        private Stream CreateStream(string name, string extension, Encoding encoding, string mimeType, bool willSeek)
        {
            _mimeType = mimeType;
            if (_stream == null)
                _stream = new MemoryStream();

            return _stream;
        }

        private static void FecharThread(ChaveDicionario chave)
        {
            if (_threads.ContainsKey(chave))
            {
                _threads[chave].Thread.Abort();
                _threads[chave].Thread = null;
                _threads[chave].ViewState.Remove("aleatorio");
                _threads.Remove(chave);
            }
        }

        #endregion

        #region Métodos para AJAX

        [Ajax.AjaxMethod]
        public static string FecharThread(string idFunc, string url, string aleatorio, string duracaoSegundos)
        {
            int contador = 0, duracao = Glass.Conversoes.StrParaInt(duracaoSegundos) * 10;

            ChaveDicionario chave = new ChaveDicionario(Glass.Conversoes.StrParaUint(idFunc), url, Glass.Conversoes.StrParaInt(aleatorio));

            if (!_threads.ContainsKey(chave))
                return "false";

            while (contador++ < duracao)
            {
                try
                {
                    if (_threads.ContainsKey(chave))
                    {
                        FecharThread(chave);
                        break;
                    }
                }
                catch { }

                Thread.Sleep(100);
            }

            return (!_threads.ContainsKey(chave)).ToString().ToLower();
        }

        [Ajax.AjaxMethod]
        public static string Carregou(string idFunc, string url, string aleatorio, string statusAtual, string duracaoSegundos)
        {
            int contador = 0, duracao = Glass.Conversoes.StrParaInt(duracaoSegundos) * 10;

            ChaveDicionario chave = new ChaveDicionario(Glass.Conversoes.StrParaUint(idFunc), url, Glass.Conversoes.StrParaInt(aleatorio));

            if (!_threads.ContainsKey(chave))
                return "2|Erro";

            while (contador++ < duracao)
            {
                try
                {
                    if (!_threads[chave].Executando)
                        return "1|" + _threads[chave].Status;
                    else if (!String.Equals(_threads[chave].Status, statusAtual, StringComparison.InvariantCultureIgnoreCase))
                        return "2|" + _threads[chave].Status;
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }

                Thread.Sleep(100);
            }

            return "2|" + _threads[chave].Status;
        }

        #endregion
    }
}
