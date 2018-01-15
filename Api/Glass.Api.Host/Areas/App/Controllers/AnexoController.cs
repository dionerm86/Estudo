using Glass.Data.Model;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Glass.Api.Host.Areas.App.Controllers
{
    /// <summary>
    /// Controlador de anexos
    /// </summary>
    /// [Authorize]
    public class AnexoController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage AnexarArquivo()
        {
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                var root = HttpContext.Current.Server.MapPath("~/App_Data");
                var provider = new MultipartFormDataStreamProvider(root);

                var result = Task.Run(() => Request.Content.ReadAsMultipartAsync(provider)).Result;

                if (provider.FileData.Count != 1)
                    throw new Exception("Número invalido de arquivos enviados.");

                var file = provider.FileData[0];


                using (var zip = Ionic.Zip.ZipFile.Read(file.LocalFileName))
                {
                    foreach (var ze in zip)
                    {
                        using (var ms = new MemoryStream())
                        {
                            ze.Extract(ms);

                            var tipo = (IFoto.TipoFoto)Enum.Parse(typeof(IFoto.TipoFoto), provider.FormData["tipo"], true);
                            var idParent = provider.FormData["idParent"].StrParaUint();
                            var descricao = provider.FormData[ze.FileName];

                            ServiceLocator.Current.GetInstance<IAnexoFluxo>().AnexarArquivo(tipo, idParent, ms.ToArray(), ze.FileName, descricao);
                        }
                    }


                }

                if (File.Exists(file.LocalFileName))
                    File.Delete(file.LocalFileName);

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}
