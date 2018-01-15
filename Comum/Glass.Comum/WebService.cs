using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Glass
{
    public static class WebService
    {
        #region Tipos Aninhados

        /// <remarks/>
        [System.CodeDom.Compiler.GeneratedCodeAttribute("ConsoleApplication1", "1.0.0.0")]
        [System.Diagnostics.DebuggerStepThroughAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Web.Services.WebServiceBindingAttribute(Name = "SyncServiceSoap12", Namespace = "http://webglass.org/")]
        public partial class SyncService : System.Web.Services.Protocols.SoapHttpClientProtocol
        {
            private List<System.Net.Cookie> cookies = new List<System.Net.Cookie>();

            protected override System.Net.WebRequest GetWebRequest(Uri uri)
            {
                var request = (System.Net.HttpWebRequest)base.GetWebRequest(uri);
                request.CookieContainer = this.CookieContainer;
                return request;
            }

            protected override System.Net.WebResponse GetWebResponse(System.Net.WebRequest request)
            {

                if (string.IsNullOrEmpty(request.Headers["Cookie"]) && cookies.Any())
                    request.Headers["Cookie"] = string.Join(";", cookies.Select(f => string.Format("{0}={1}", f.Name, f.Value)));

                var response = (System.Net.HttpWebResponse)base.GetWebResponse(request);

                foreach (System.Net.Cookie i in response.Cookies)
                    cookies.Add(i);

                return response;
            }

            /// <remarks/>
            public SyncService()
            {
                this.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                this.Url = "http://189.3.6.226:8080/service/wsExportacaoPedido2.asmx";
            }

            /// <remarks/>
            [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://webglass.org/Autenticar", RequestNamespace = "http://webglass.org/", ResponseNamespace = "http://webglass.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
            public int Autenticar(string cnpj, int tipoUsuario)
            {
                object[] results = this.Invoke("Autenticar", new object[] {
                    cnpj,
                    tipoUsuario});
                return ((int)(results[0]));
            }

            /// <remarks/>
            [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://webglass.org/Conectar", RequestNamespace = "http://webglass.org/", ResponseNamespace = "http://webglass.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
            public string Conectar()
            {
                object[] results = this.Invoke("Conectar", new object[0]);
                return ((string)(results[0]));
            }

            /// <remarks/>
            [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://webglass.org/Login", RequestNamespace = "http://webglass.org/", ResponseNamespace = "http://webglass.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
            public bool Login(string userName)
            {
                object[] results = this.Invoke("Login", new object[] {
                    userName});
                return ((bool)(results[0]));
            }

            /// <remarks/>
            [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://webglass.org/EnviarPedidosFornecedor", RequestNamespace = "http://webglass.org/", ResponseNamespace = "http://webglass.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
            public string[] EnviarPedidosFornecedor(string cpfCnpj, int tipoUsuario, [System.Xml.Serialization.XmlElementAttribute(DataType = "base64Binary")] byte[] pedido)
            {
                object[] results = this.Invoke("EnviarPedidosFornecedor", new object[] {
                    cpfCnpj,
                    tipoUsuario,
                    pedido});
                return ((string[])(results[0]));
            }

            /// <remarks/>
            [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://webglass.org/CancelarPedido", RequestNamespace = "http://webglass.org/", ResponseNamespace = "http://webglass.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
            public string[] CancelarPedido(string cpfCnpj, int tipoUsuario, int idPedidoCliente)
            {
                object[] results = this.Invoke("CancelarPedido", new object[] {
                    cpfCnpj,
                    tipoUsuario,
                    idPedidoCliente});
                return ((string[])(results[0]));
            }

            /// <remarks/>
            [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://webglass.org/MarcarPedidoPronto", RequestNamespace = "http://webglass.org/", ResponseNamespace = "http://webglass.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
            public string[] MarcarPedidoPronto(string cpfCnpj, int tipoUsuario, int idPedidoCliente)
            {
                object[] results = this.Invoke("MarcarPedidoPronto", new object[] {
                    cpfCnpj,
                    tipoUsuario,
                    idPedidoCliente});
                return ((string[])(results[0]));
            }
        }

        #endregion

        /// <summary>
        /// //Url do webservice
        /// string webserviceUrl  = "http://www.abc.com/lgl/test/webservice/v1_00/security.asmx"
        /// //especifique o nome do serviço
        /// string serviceName = "SecurityAndSessionManagement";
        /// //especifique o nome do método a ser chamado
        /// string methodName = "Session_Start";
        /// //Passe os parâmetros
        /// string[] arArguments = new string[2];
        /// arArguments[0] = "abc";
        /// arArguments[1] = "xxxx";
        /// object sSessionID = ChamarWebService(webserviceUrl, serviceName, methodName, arArguments);
        /// </summary>
        /// <param name="serviceUrl">Url do webService ("http://www.abc.com/lgl/test/webservice/v1_00/security.asmx")</param>
        /// <param name="serviceName">Nome do serviço ("SecurityAndSessionManagement")</param>
        /// <param name="methodName">Nome do método a ser chamado("Session_Start")</param>
        /// <param name="args">Parâmetros</param>
        /// <returns>object</returns>
        public static object ChamarWebService(string serviceUrl, string serviceName, string methodName, object[] args)
        {
            try
            {
                var client = new SyncService();
                client.Url = serviceUrl;
                client.CookieContainer = new CookieContainer();

                client.Conectar();
                var idCliente = client.Autenticar((string)args[0], (int)args[1]);
                client.Login(idCliente.ToString() + "|cliente");

                if (methodName == "CancelarPedido")
                    return client.CancelarPedido((string)args[0], (int)args[1], (int)args[2]);
                else               
                    return client.EnviarPedidosFornecedor((string)args[0], (int)args[1], (byte[])args[2]);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
