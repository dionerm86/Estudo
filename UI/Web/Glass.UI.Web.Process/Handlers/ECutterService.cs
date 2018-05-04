using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Glass.UI.Web.Process.Handlers
{
    public class ECutterService : IHttpHandler
    {
        #region Propriedades

        /// <summary>
        /// Identifica se a requisição pode ser reutilizada.
        /// </summary>
        public bool IsReusable
        {
            get { return false; }
        }

        #endregion

        #region Métodos Privados

        private void NaoEncontrado(HttpContext context)
        {
            context.Response.StatusCode = 404;
            context.Response.StatusDescription = "Not Found";
            context.Response.Flush();
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Processa a requisição.
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)
        {
            var id = context.Request["id"];

            if (string.IsNullOrEmpty(id))
            {
                NaoEncontrado(context);
                return;
            }

            // Verifica se foi informado o arquivo para download
            if (!string.IsNullOrEmpty(context.Request["download"]))
            {
                var fileName = System.IO.Path.Combine(Data.Helper.Utils.GetArquivoOtimizacaoPath, string.Format("{0}.zip", id));

                if (System.IO.File.Exists(fileName))
                {
                    context.Response.WriteFile(fileName);
                }
                else
                {
                    context.Response.StatusCode = 404;
                    context.Response.StatusDescription = "Not found";
                    context.Response.Flush();
                    context.Response.End();
                }

                return;
            }
            else
            {
                var body = new HttpResponseBody
                {
                    Name = id,
                    GetFormat = ".zip",
                    GetUri = string.Format("{0}&download=true", context.Request.Url),
                    PushUri = string.Format("{0}&save=true", context.Request.Url)
                };

                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(HttpResponseBody));
                serializer.Serialize(context.Response.OutputStream, body);

                context.Response.Flush();
            }

        }

        #endregion

        #region Tipos Aninhados

        /// <summary>
        /// Representa o corpo da resposta sobre Http.
        /// </summary>
        public class HttpResponseBody
        {
            #region Propriedades

            /// <summary>
            /// Nome da solução.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Formato do arquivo na operação de recuperação.
            /// </summary>
            public string GetFormat { get; set; }

            /// <summary>
            /// Uri que será usado para recuperar a solução.
            /// </summary>
            public string GetUri { get; set; }

            /// <summary>
            /// Uri que será usada para salva os dados da solução.
            /// </summary>
            public string PushUri { get; set; }

            #endregion
        }

        #endregion
    }
}
