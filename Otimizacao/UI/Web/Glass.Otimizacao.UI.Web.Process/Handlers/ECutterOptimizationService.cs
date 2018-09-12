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
        bool IHttpHandler.IsReusable => false;

        private Negocios.IOtimizacaoFluxo OtimizacaoFluxo =>
            Microsoft.Practices.ServiceLocation.ServiceLocator.Current
                    .GetInstance<Otimizacao.Negocios.IOtimizacaoFluxo>();

        private eCutter.IAutenticadorProtocolo Autenticator =>
            Microsoft.Practices.ServiceLocation.ServiceLocator.Current
                    .GetInstance<eCutter.IAutenticadorProtocolo>();

        private IRepositorioSolucaoOtimizacao RepositorioSolucaoOtimizacao =>
            Microsoft.Practices.ServiceLocation.ServiceLocator.Current
                    .GetInstance<IRepositorioSolucaoOtimizacao>();

        private static string ObterCaminhoArquivoPlanoOtimizacao(string id)
        {
            var nome = "Exp_" + id.PadLeft(10, '0');

            var arquivo = System.IO.Path.Combine(Data.Helper.Utils.GetArquivoOtimizacaoPath, string.Format("{0}.zip", nome));

            if (!System.IO.File.Exists(arquivo))
            {
                arquivo = System.IO.Path.Combine(Data.Helper.Utils.GetArquivoOtimizacaoPath, string.Format("{0}.asc", nome));
            }

            return arquivo;
        }

        private void NaoEncontrado(HttpContext context)
        {
            context.Response.StatusCode = 404;
            context.Response.StatusDescription = "Not Found";
            context.Response.Flush();
            context.Response.End();
        }

        private void NaoAutorizado(HttpContext context)
        {
            context.Response.StatusCode = 401;
            context.Response.StatusDescription = "Unauthorized";
            context.Response.Flush();
            context.Response.End();
        }

        private eCutter.ResultadoSalvarTransacao Importar(HttpContext context, string token, IEnumerable<IArquivoSolucaoOtimizacao> arquivos)
        {
            if (!arquivos.Any())
            {
                return new eCutter.ResultadoSalvarTransacao(
                    false,
                    null,
                    new[]
                    {
                        new eCutter.MensagemTransacao("Falha", "Não foi encontrado o arquivo de importação.", eCutter.TipoMensagemTransacao.Erro),
                    });
            }

            var idArquivoOtimizacao = 0;
            if (!int.TryParse(context.Request["id"], out idArquivoOtimizacao))
            {
                return new eCutter.ResultadoSalvarTransacao(
                    false,
                    null,
                    new[]
                    {
                        new eCutter.MensagemTransacao("Falha", "Não foi encontrado o identificador do arquivo de otimização.", eCutter.TipoMensagemTransacao.Erro),
                    });
            }

            try
            {
                var importacao = this.OtimizacaoFluxo.Importar(idArquivoOtimizacao, arquivos);

                if (importacao == null)
                {
                    return new eCutter.ResultadoSalvarTransacao(
                        true,
                        null,
                        new[]
                        {
                            new eCutter.MensagemTransacao("Sucesso", $"Otimização salva com sucesso.", eCutter.TipoMensagemTransacao.Informacao),
                        });
                }

                // Verifica se nenhum plano de corte foi impotado
                if (importacao.Solucao != null && !importacao.Solucao.PlanosOtimizacao.Any(f => f.PlanosCorte.Any()))
                {
                    return new eCutter.ResultadoSalvarTransacao(
                        true,
                        null,
                        new[]
                        {
                            new eCutter.MensagemTransacao("Otimização incompleta", $"Nenhum plano de corte foi otimizado.", eCutter.TipoMensagemTransacao.Erro),
                        });
                }

                var redirectUrl = string.Empty;
                var url = context.Request.Url.AbsoluteUri;

                if (importacao.Solucao != null)
                {
                    redirectUrl = "../Listas/LstEtiquetaImprimir.aspx?idsolucaootimizacao=" + importacao.Solucao.IdSolucaoOtimizacao;
                }
                else
                {
                    redirectUrl = "../Listas/LstEtiquetaImprimir.aspx?idarquivootimizacao=" + importacao.IdArquivoOtimizacao;
                }

                if (!string.IsNullOrEmpty(context.Request.QueryString["token"]))
                {
                    // Adiciona no resultado o token correto
                    url = url.Replace($"token={context.Request.QueryString["Token"]}", $"token={token}");
                }
                else
                {
                    url += $"token={token}";
                }

                url += $"&confirm=true&redirect={HttpUtility.UrlEncode(redirectUrl)}";

                return new eCutter.ResultadoSalvarTransacao(
                    true,
                    new Uri(url),
                    new[]
                    {
                        new eCutter.MensagemTransacao("Sucesso", $"Otimização salva com sucesso.", eCutter.TipoMensagemTransacao.Informacao),
                    });
            }
            catch (Exception ex)
            {
                return new eCutter.ResultadoSalvarTransacao(
                    false,
                    null,
                    new[]
                    {
                        new eCutter.MensagemTransacao(
                            "Falha",
                            $"Ocorreu uma falha na importação do resultado da otimização pelo WebGlass. {ex.Message}",
                            eCutter.TipoMensagemTransacao.Erro),
                    });
            }
        }

        private void Autenticar(HttpContext context, string usuario, string senha, out string token)
        {
            eCutter.AutenticacaoProtocolo autenticacao;

            try
            {
                autenticacao = this.Autenticator.Autenticar(usuario, senha);
            }
            catch (Exception ex)
            {
                autenticacao = new eCutter.AutenticacaoProtocolo
                {
                    Sucesso = false,
                    Mensagem = ex.Message,
                };
            }

            token = null;
            if (autenticacao.Sucesso)
            {
                var ticket = new System.Web.Security.FormsAuthenticationTicket(autenticacao.Usuario, true, 10);
                token = System.Web.Security.FormsAuthentication.Encrypt(ticket);
            }

            var writer = XmlWriter.Create(
                context.Response.OutputStream,
                new XmlWriterSettings
                {
                    CloseOutput = false,
                });

            writer.WriteStartElement("Authentication");
            Otimizacao.eCutter.Serializador.Serializar(writer, autenticacao, token);
            writer.WriteEndElement();
            writer.Flush();
            context.Response.Flush();
        }

        private bool VerificarAutenticacao(HttpContext context, out string token)
        {
            var requestType = context.Request.RequestType?.ToLower();

            // Verifica se está sendo solicitada uma autenticação
            if (requestType == "post" && context.Request.Form["operation"] == "authentication")
            {
                var usuario = context.Request.Form["username"];
                var senha = context.Request.Form["password"];

                this.Autenticar(context, usuario, senha, out token);
                return false;
            }
            else
            {
                token = context.Request.Headers["x-token"];

                if (string.IsNullOrEmpty(token))
                {
                    token = context.Request["token"];
                }

                // Verifica se o token de segurança foi informado
                if (string.IsNullOrEmpty(token))
                {
                    this.NaoAutorizado(context);
                    return false;
                }

                try
                {
                    // Recupera o ticket de autenticação
                    var ticket = System.Web.Security.FormsAuthentication.Decrypt(token);

                    // Verifica se o ticket expirou
                    if (ticket.Expired)
                    {
                        this.NaoAutorizado(context);
                        return false;
                    }
                    else if (context.Request.Form["operation"] == "check-token")
                    {
                        return false;
                    }

                    HttpContext.Current.User = new Colosoft.Security.Principal.DefaultPrincipal(new Colosoft.Security.Principal.DefaultIdentity(ticket.Name, null, !ticket.Expired));
                }
                catch
                {
                    this.NaoAutorizado(context);
                    return false;
                }
            }

            return true;
        }

        void IHttpHandler.ProcessRequest(HttpContext context)
        {
            var requestType = context.Request.RequestType?.ToLower();

            var id = context.Request["id"];

            // Verifica foi informado o identificador do plano de otimização
            if (string.IsNullOrEmpty(id))
            {
                this.NaoEncontrado(context);
                return;
            }

            string token;

            if (!this.VerificarAutenticacao(context, out token))
            {
                return;
            }

            if (requestType == "post")
            {
                eCutter.ResultadoSalvarTransacao resultado = null;

                // Verifica se é a operação de cancelamento
                if (context.Request.QueryString["cancel"] == "true")
                {
                    resultado = new eCutter.ResultadoSalvarTransacao(true, null, null);
                }
                else
                {
                    var arquivos = new List<IArquivoSolucaoOtimizacao>();

                    for (var i = 0; i < context.Request.Files.Count; i++)
                    {
                        arquivos.Add(new ConteudoArquivoOtimizacao(context.Request.Files[i]));
                    }

                    resultado = this.Importar(context, token, arquivos);
                }

                var writer = XmlWriter.Create(
                    context.Response.OutputStream,
                    new XmlWriterSettings
                    {
                        CloseOutput = false,
                    });

                writer.WriteStartElement("ProtocolTransactionOperationResult");
                eCutter.Serializador.Serializar(writer, resultado);
                writer.WriteEndElement();
                writer.Flush();

                context.Response.Flush();
                context.Response.End();
            }

            // Verifica se é uma requisição para recuperar o estoque de chapas
            else if (!string.IsNullOrEmpty(context.Request["sheetstock"]))
            {
                var sessaoOtimizacao = this.OtimizacaoFluxo.ObterSessaoOtimizacao(int.Parse(id));

                var writer = System.Xml.XmlWriter.Create(
                    context.Response.OutputStream,
                    new System.Xml.XmlWriterSettings
                    {
                        CloseOutput = false,
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
                var sessaoOtimizacao = this.OtimizacaoFluxo.ObterSessaoOtimizacao(int.Parse(id));

                var writer = System.Xml.XmlWriter.Create(
                    context.Response.OutputStream,
                    new System.Xml.XmlWriterSettings
                    {
                        CloseOutput = false,
                    });

                eCutter.Serializador.Serializar(writer, sessaoOtimizacao.ObterPecasPadrao());
                writer.Flush();
                context.Response.Flush();
            }

            // Verifica se foi informado o arquivo para download
            else if (!string.IsNullOrEmpty(context.Request["optimizationplan"]))
            {
                var idArquivoOtimizacao = int.Parse(id);
                var solucaoOtimizacao = this.OtimizacaoFluxo.ObterSolucaoOtimizacaoPelaArquivoOtimizacao(idArquivoOtimizacao);

                // O nome da solução será gerado com base no identificador do arquivo de otimização
                // codificado sobre a base 32.
                var nomeSolucaoOtimizacao = Data.Helper.Utils.BytesToBase32(System.BitConverter.GetBytes(idArquivoOtimizacao).Take(3).ToArray());

                if (solucaoOtimizacao != null)
                {
                    var arquivo = this.RepositorioSolucaoOtimizacao.ObterArquivos(solucaoOtimizacao)
                        .FirstOrDefault(f => StringComparer.InvariantCultureIgnoreCase.Equals(System.IO.Path.GetExtension(f.Nome), ".optsln"));

                    if (arquivo != null)
                    {
                        using (var stream = arquivo.Abrir())
                        {
                            context.Response.ContentType = "application/ecutter-optimization";
                            context.Response.AddHeader("Content-Disposition", $"attachment; filename={nomeSolucaoOtimizacao}.optsln");
                            context.Response.AddHeader("Content-Length", stream.Length.ToString());

                            context.Response.Flush();

                            var buffer = new byte[1024];
                            var read = 0;

                            var outputStream = context.Response.OutputStream;
                            while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                outputStream.Write(buffer, 0, read);
                            }

                            outputStream.Flush();
                        }
                    }
                    else
                    {
                        this.NaoEncontrado(context);
                    }
                }
                else
                {
                    string arquivo = ObterCaminhoArquivoPlanoOtimizacao(id);

                    if (System.IO.File.Exists(arquivo))
                    {
                        context.Response.ContentType = "application/ecutter-optimization";
                        context.Response.AddHeader("Content-Disposition", $"attachment; filename={nomeSolucaoOtimizacao}.optsln");
                        context.Response.WriteFile(arquivo);
                    }
                    else
                    {
                        this.NaoEncontrado(context);
                    }
                }
            }
            else if (!string.IsNullOrEmpty(context.Request["confirm"]))
            {
                var redirect = context.Request["redirect"];
                context.Response.Redirect(redirect);
            }
            else
            {
                var possuiSolucaoOtimizacao = this.OtimizacaoFluxo.PossuiSolucaoOtimizacao(int.Parse(id));

                string formato = null;

                if (possuiSolucaoOtimizacao)
                {
                    formato = "eCutter";
                }
                else
                {
                    switch (System.IO.Path.GetExtension(ObterCaminhoArquivoPlanoOtimizacao(id))?.ToLower())
                    {
                        case ".zip":
                            formato = "OptyWay Package ASCII Importer";
                            break;
                        case ".asc":
                            formato = "Optway ASCII Import File";
                            break;
                    }
                }

                var mergeSheetStock =
                    Configuracoes.OtimizacaoConfig.TipoEstoqueChapas == Data.Helper.DataSources.TipoEstoqueChapasOtimizacaoEnum.Externo &&
                    !possuiSolucaoOtimizacao;

                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(eCutter.ProtocolConfiguration));
                serializer.Serialize(
                    context.Response.OutputStream,
                    new eCutter.ProtocolConfiguration(context.Request.Url, id, formato)
                    {
                        IsReadOnly = possuiSolucaoOtimizacao,
                        MergeSheetStock = mergeSheetStock,
                        ConsolidateSheetStock = mergeSheetStock,
                    });

                context.Response.Flush();
            }
        }

        /// <summary>
        /// Implementação que encapsula o arquivo postado com um conteúdo
        /// do arquivo de otimização.
        /// </summary>
        private class ConteudoArquivoOtimizacao : IArquivoSolucaoOtimizacao
        {
            private readonly HttpPostedFile arquivo;

            /// <summary>
            /// Obtém o nome do arquivo.
            /// </summary>
            public string Nome => arquivo.FileName;

            /// <summary>
            /// Initializes a new instance of the <see cref="ConteudoArquivoOtimizacao"/> class.
            /// </summary>
            /// <param name="arquivo">Arquivo postado.</param>
            public ConteudoArquivoOtimizacao(HttpPostedFile arquivo)
            {
                this.arquivo = arquivo;
            }

            /// <summary>
            /// Abre o conteúdo do arquivo.
            /// </summary>
            /// <returns>Stream do contéudo do arquivo.</returns>
            public System.IO.Stream Abrir() => this.arquivo.InputStream;
        }
    }
}
