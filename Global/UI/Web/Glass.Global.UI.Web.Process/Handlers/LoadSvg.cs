using System;
using System.Web;
using System.Drawing;
using System.IO;
using Glass.Configuracoes;

namespace Glass.Global.UI.Web.Process.Handlers
{
    public class LoadSvg : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                context.Response.ContentType = "text/html";

                var idProdPedEsp = context.Request["idProdPedEsp"].StrParaIntNullable();

                if (idProdPedEsp.GetValueOrDefault(0) > 0)
                {
                    var caminho = PCPConfig.CaminhoSalvarCadProject(true) + idProdPedEsp.Value + ".svg";

                    if (!File.Exists(caminho))
                        throw new Exception("O arquivo não foi encontrado.");

                    var data = File.ReadAllText(caminho);
                    context.Response.Write(MontaDivSvg(data));
                    
                }
            }
            catch(Exception ex)
            {
                // Devolve o erro
                context.Response.ContentType = "text/html";
                context.Response.Write(GetErrorResponse(ex));
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private string GetErrorResponse(Exception ex)
        {
            bool debug = false;

            string html = debug ? ex.ToString().Replace("\n", "<br>").Replace("\r", "").Replace(" ", "&nbsp;") : @"
            <script type='text/javascript'>
                alert('" + MensagemAlerta.FormatErrorMsg("", ex) + @"');
                window.history.go(-1);
            </script>";

            return @"
            <html>
                <body>
                    " + html + @"
                </body>
            </html>";
        }

        private string MontaDivSvg(string svg)
        {
            //Caminho do template para exibir o SVG
            var caminhoTemplate = HttpContext.Current.Server.MapPath("~/Misc/") + "SvgTemplate.html";

            //Caminho do js responsavel por dar o zoom
            var jsPamZoom = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            jsPamZoom = jsPamZoom + "/Scripts/svg-pan-zoom.js";

            //Carrega o template
            var template = File.ReadAllText(caminhoTemplate);

            //Adiciona o caminho do js
            template = template.Replace("{0}", jsPamZoom);

            //Adiciona o SVG
            template = template.Replace("{1}", svg);

            return template;
        }
    }
}