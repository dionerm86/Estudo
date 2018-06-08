using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

namespace Glass.Otimizacao.UI.Web.Process.Handlers
{
    /// <summary>
    /// Serviço de integração com o otimizador do eCutter.
    /// </summary>
    public class ECutterOptimizationService : IHttpHandler
    {
        #region Propriedades

        /// <summary>
        /// Identifica se a requisição pode ser reutilizada.
        /// </summary>
        public bool IsReusable
        {
            get { return false; }
        }

        /// <summary>
        /// Fluxo de negócio da otimização.
        /// </summary>
        private Otimizacao.Negocios.IOtimizacaoFluxo OtimizacaoFluxo =>
            Microsoft.Practices.ServiceLocation.ServiceLocator.Current
                    .GetInstance<Otimizacao.Negocios.IOtimizacaoFluxo>();

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Envia para o cliente a reposta de página não encontrada.
        /// </summary>
        /// <param name="context"></param>
        private void NaoEncontrado(HttpContext context)
        {
            context.Response.StatusCode = 404;
            context.Response.StatusDescription = "Not Found";
            context.Response.Flush();
        }

        /// <summary>
        /// Envia para o cliente a resposta de requisição não autorizada.
        /// </summary>
        /// <param name="context"></param>
        private void NaoAutorizado(HttpContext context)
        {
            context.Response.StatusCode = 401;
            context.Response.StatusDescription = "Unauthorized";
            context.Response.Flush();
        }

        /// <summary>
        /// Recupera o nome do arquivo do plano de otimização.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static string ObterCaminhoArquivoPlanoOtimizacao(string id)
        {
            var nome = "Exp_" + id.PadLeft(10, '0');

            var arquivo = System.IO.Path.Combine(Data.Helper.Utils.GetArquivoOtimizacaoPath, string.Format("{0}.zip", nome));

            if (!System.IO.File.Exists(arquivo))
                arquivo = System.IO.Path.Combine(Data.Helper.Utils.GetArquivoOtimizacaoPath, string.Format("{0}.asc", nome));

            return arquivo;
        }

        /// <summary>
        /// Importa o arquivo do OptyWay.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="arquivo"></param>
        /// <returns></returns>
        private eCutter.ResultadoSalvarTransacao Importar(HttpContext context, IEnumerable<Otimizacao.Negocios.IConteudoArquivoOtimizacao> arquivos)
        {
            if (!arquivos.Any())
                return new eCutter.ResultadoSalvarTransacao(false, null, new[]
                {
                    new eCutter.MensagemTransacao("Falha", "Não foi encontrado o arquivo de importação.", eCutter.TipoMensagemTransacao.Erro)
                });

            try
            {
                var importacao = OtimizacaoFluxo.Importar(arquivos);

                var url = context.Request.Url.AbsoluteUri;
                url = url.Substring(0, url.LastIndexOf("handlers/", StringComparison.InvariantCultureIgnoreCase)) + "Listas/LstEtiquetaImprimir.aspx?idarquivootimizacao=" + importacao.IdArquivoOtimizacao;

                return new eCutter.ResultadoSalvarTransacao(true, new Uri(url), new[]
                {
                    new eCutter.MensagemTransacao("Sucesso", $"Otimização salva com sucesso.", eCutter.TipoMensagemTransacao.Informacao)
                });
            }
            catch (Exception ex)
            {
                return new eCutter.ResultadoSalvarTransacao(false, null, new[]
                {
                    new eCutter.MensagemTransacao("Falha", $"Ocorreu uma falha na importação do resultado da otimização pelo WebGlass. {ex.Message}", eCutter.TipoMensagemTransacao.Erro)
                });
            }
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

            // Verifica foi informado o identificador do plano de otimização
            if (string.IsNullOrEmpty(id))
            {
                NaoEncontrado(context);
                return;
            }

            var token = context.Request["token"];

            // Verifica se o token de segurança foi informado
            if (string.IsNullOrEmpty(token))
            {
                NaoAutorizado(context);
                return;
            }

            try
            {
                // Recupera o ticket de autenticação
                var ticket = System.Web.Security.FormsAuthentication.Decrypt(token);

                // Verifica se o ticket expirou
                if (ticket.Expired)
                {
                    NaoAutorizado(context);
                    return;
                }

                HttpContext.Current.User = new Colosoft.Security.Principal.DefaultPrincipal(new Colosoft.Security.Principal.DefaultIdentity(ticket.Name, null, !ticket.Expired));
            }
            catch
            {
                NaoAutorizado(context);
                return;
            }

            if (StringComparer.InvariantCultureIgnoreCase.Equals(context.Request.RequestType, "POST"))
            {
                var arquivos = new List<Otimizacao.Negocios.IConteudoArquivoOtimizacao>();

                for (var i = 0; i < context.Request.Files.Count; i++)
                    arquivos.Add(new ConteudoArquivoOtimizacao(context.Request.Files[i]));

                var resultado = Importar(context, arquivos);

                var writer = System.Xml.XmlWriter.Create(context.Response.OutputStream,
                   new System.Xml.XmlWriterSettings
                   {
                       CloseOutput = false
                   });

                writer.WriteStartElement("ProtocolTransactionSaveResult");
                Otimizacao.eCutter.Serializador.Serializar(writer, resultado);
                writer.WriteEndElement();
                writer.Flush();

                context.Response.Flush();
            }
            // Verifica se é uma requisição para recuperar o estoque de chapas
            else if (!string.IsNullOrEmpty(context.Request["sheetstock"]))
            {
                var sessaoOtimizacao = OtimizacaoFluxo.ObterSessaoOtimizacao(int.Parse(id));

                var writer = System.Xml.XmlWriter.Create(context.Response.OutputStream,
                    new System.Xml.XmlWriterSettings
                    {
                        CloseOutput = false
                    });

                writer.WriteStartElement("SheetStock");
                Otimizacao.eCutter.Serializador.Serializar(writer, sessaoOtimizacao.ObterEstoqueChapas());
                writer.WriteEndElement();
                writer.Flush();
                context.Response.Flush();
            }
            // Verifica se é uma requisição para recuperar as peças padrão
            else if (!string.IsNullOrEmpty(context.Request["standardpieces"]))
            {
                var sessaoOtimizacao = OtimizacaoFluxo.ObterSessaoOtimizacao(int.Parse(id));

                var writer = System.Xml.XmlWriter.Create(context.Response.OutputStream,
                   new System.Xml.XmlWriterSettings
                   {
                       CloseOutput = false
                   });

                Otimizacao.eCutter.Serializador.Serializar(writer, sessaoOtimizacao.ObterPecasPadrao());
                writer.Flush();
                context.Response.Flush();

            }
            // Verifica se foi informado o arquivo para download
            else if (!string.IsNullOrEmpty(context.Request["optimizationplan"]))
            {
                string arquivo = ObterCaminhoArquivoPlanoOtimizacao(id);

                if (System.IO.File.Exists(arquivo))
                {
                    context.Response.WriteFile(arquivo);
                }
                else
                {
                    NaoEncontrado(context);
                }
            }
            else
            {
                string formato = null;

                switch(System.IO.Path.GetExtension(ObterCaminhoArquivoPlanoOtimizacao(id))?.ToLower())
                {
                    case ".zip":
                        formato = "OptyWay Package ASCII Importer";
                        break;
                    case ".asc":
                        formato = "Optway ASCII Import File";
                        break;
                }

                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(eCutter.ProtocolConfiguration));
                serializer.Serialize(context.Response.OutputStream, 
                    new eCutter.ProtocolConfiguration(context.Request.Url, id, formato));

                context.Response.Flush();
            }

        }

        #endregion

        #region Tipos Aninhados

        /// <summary>
        /// Implementação que encapsula o arquivo postado com um conteúdo
        /// do arquivo de otimização.
        /// </summary>
        class ConteudoArquivoOtimizacao : Otimizacao.Negocios.IConteudoArquivoOtimizacao
        {
            #region Variáveis Locais

            private readonly HttpPostedFile _arquivo;

            #endregion

            #region Propriedades

            /// <summary>
            /// Nome do arquivo.
            /// </summary>
            public string Nome => _arquivo.FileName;

            #endregion

            #region Construtores

            /// <summary>
            /// Construtor padrão.
            /// </summary>
            /// <param name="arquivo"></param>
            public ConteudoArquivoOtimizacao(HttpPostedFile arquivo)
            {
                _arquivo = arquivo;
            }

            #endregion

            #region Métodos Públicos

            /// <summary>
            /// Abre o conteúdo do arquivo.
            /// </summary>
            /// <returns></returns>
            public System.IO.Stream Abrir() => _arquivo.InputStream;

            #endregion
        }

        #endregion
    }
}
