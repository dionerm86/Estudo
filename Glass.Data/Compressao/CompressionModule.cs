using System;
using System.IO.Compression;
using System.Web;
using System.IO;

namespace Glass.Data.Compressao
{
    //O módulo, por padrão, implementa a interface IHttpModule, que possui
    //os métodos Dispose e Init.
    public class CompressionModule : IHttpModule
    {
        #region Métodos de Suporte

        private static bool AcceptEncoding(HttpContext context)
        {
            return AcceptGzipEncoding(context) || AcceptDeflateEncoding(context);
        }

        private static bool AcceptGzipEncoding(HttpContext context)
        {
            //Verifica se na requisição existe um cabeçalho dizendo "Sim, aceito codificação"
            string acceptEncoding = context.Request.Headers["Accept-Encoding"];

            //Caso aceite codificação, verifica se nas codificações aceitas existe gzip
            return !String.IsNullOrEmpty(acceptEncoding) && acceptEncoding.Contains("gzip");
        }

        private static bool AcceptDeflateEncoding(HttpContext context)
        {
            //Verifica se na requisição existe um cabeçalho dizendo "Sim, aceito codificação"
            string acceptEncoding = context.Request.Headers["Accept-Encoding"];

            //Caso aceite codificação, verifica se nas codificações aceitas existe deflate
            return !String.IsNullOrEmpty(acceptEncoding) && acceptEncoding.Contains("deflate");
        }

        private static void CompressGzip(HttpContext context, ref Stream streamToCompress)
        {
            streamToCompress = new GZipStream(streamToCompress, CompressionMode.Compress);
            context.Response.AppendHeader("Content-Encoding", "gzip");
        }

        private static void CompressDeflate(HttpContext context, ref Stream streamToCompress)
        {
            streamToCompress = new DeflateStream(streamToCompress, CompressionMode.Compress);
            context.Response.AppendHeader("Content-Encoding", "deflate");
        }

        #endregion

        #region Métodos de Compressão

        //Esse método vai receber o contexto da requisição e comprimir o conteúdo se possível
        internal static void CompressResponse(HttpContext context)
        {
            Stream stream = context.Response.Filter;
            CompressResponse(context, ref stream);
            context.Response.Filter = stream;
        }

        //Esse método vai receber o contexto da requisição e comprimir o conteúdo se possível
        internal static void CompressResponse(HttpContext context, ref Stream streamToCompress)
        {
            //Caso aceite codificação, verifica se nas codificações aceitas existe gzip ou deflate
            if (AcceptEncoding(context))
            {
                //Se tiver gzip, configura a reposta para comprimir o conteúdo e coloca no
                //cabeçalho que o conteúdo está comprimido com gzip
                if (AcceptGzipEncoding(context))
                    CompressGzip(context, ref streamToCompress);

                //Se tiver deflate, configura a reposta para comprimir o conteúdo e coloca no
                //cabeçalho que o conteúdo está comprimido com deflate
                else
                    CompressDeflate(context, ref streamToCompress);
            }
        }

        #endregion

        //Como não precisamos fazer nada no método Dispose para este módulo,
        //vamos deixar em branco
        public void Dispose() { }

        public void Init(HttpApplication context)
        {
            context.PostReleaseRequestState += new EventHandler(context_PostReleaseRequestState);
        }

        //Aqui tratamos o evento de compressão
        void context_PostReleaseRequestState(object sender, EventArgs e)
        {
            //Convertemos o sender para o tipo HttpApplication
            HttpApplication app = sender as HttpApplication;

            //Se ele for do tipo System.Web.UI.Page (aspx), css ou javascript, então
            //podemos fazer a compressão
            if ((app.Request.Path.Contains(".aspx") && !app.Request.Path.Contains(".ashx")) || app.Request.Path.Contains(".css") || (app.Request.Path.Contains(".js") && !app.Request.Path.Contains("resource")))
                CompressResponse(app.Context);

            // Tenta aplicar o filtro de JavaScript/CSS
            if (app.Request.Path.Contains(".aspx") && !app.Request.Path.Contains(".ashx") && !app.Request.Path.Contains(".axd"))
                app.Response.Filter = new RandomFilter(app.Context, app.Response.Filter);
        }
    }
}