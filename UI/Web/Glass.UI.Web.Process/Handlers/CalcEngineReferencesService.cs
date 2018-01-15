using System.Collections.Generic;
using System.Linq;

namespace Glass.UI.Web.Process.Handlers
{
    public class CalcEngineReferencesService : System.Web.IHttpHandler
    {
        #region Propriedades

        /// <summary>
        /// Identifica se o conteúdo do handle pode ser reutilizável.
        /// </summary>
        public bool IsReusable
        {
            get { return false; }
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Processa a requisição.
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(System.Web.HttpContext context)
        {
            try
            {
                // Verifica se o endereço de requisição está correto, e se é para retornar os valores de referência
                if (!string.IsNullOrEmpty(context.Request.PathInfo) && context.Request.PathInfo.ToLower() == "/api/referencescontextrepository/getcontext/webglass")
                {
                    context.Response.ContentType = "application/json";

                    var referencesService = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Projeto.Negocios.ICalcEngineReferencesService>();

                    var contexto = referencesService.GetContext("webglass");

                    // Serializa e retorna o JSON ao solicitante.
                    var resultado = Newtonsoft.Json.JsonConvert.SerializeObject(contexto);
                    context.Response.Write(resultado);
                    context.Response.Flush();
                }
                // Verifica se o endereço de requisição está correto, e se é para retornar todos os Contextos sem valores de referência
                else if (!string.IsNullOrEmpty(context.Request.PathInfo) && context.Request.PathInfo.ToLower() == "/api/referencescontextrepository/getcontexts")
                {
                    context.Response.ContentType = "application/json";

                    var referencesService = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Projeto.Negocios.ICalcEngineReferencesService>();

                    var contextos = referencesService.GetContexts();

                    // Serializa e retorna o JSON ao solicitante.
                    var resultado = Newtonsoft.Json.JsonConvert.SerializeObject(contextos.ToArray());
                    context.Response.Write(resultado);
                    context.Response.Flush();
                }
            }
            catch { }
        }

        #endregion
    }
}
