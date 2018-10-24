// <copyright file="KhanClientMessageInspector.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Integracao.Khan.Seguranca
{
    /// <summary>
    /// Representa o inspetor dos mensagens dos clientes de serviço da Khan.
    /// </summary>
    internal class KhanClientMessageInspector : IClientMessageInspector
    {
        /// <summary>
        /// Nome do cabeçalho do token.
        /// </summary>
        public const string TokenHeaderName = "Token";

        /// <summary>
        /// Nome do cabeçalho da empresa.
        /// </summary>
        public const string EmpresaHeaderName = "Codempresa";

        /// <summary>
        /// Namespace do usado nas mensagens da instancia..
        /// </summary>
        public const string Namespace = "";

        /// <summary>
        /// Código que identifica a falha devido ao token inválido.
        /// </summary>
        public const string InvalidTokenFaultReasonCode = "InvalidToken";

        /// <summary>
        /// Código que identifica que o token recebido pelo servidor estava vazio.
        /// </summary>
        public const string EmptyTokenFaultReasonCode = "EmptyToken";

        private readonly ConfiguracaoKhan configuracao;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="KhanClientMessageInspector"/>.
        /// </summary>
        /// <param name="configuracao">Configuração.</param>
        public KhanClientMessageInspector(ConfiguracaoKhan configuracao)
        {
            this.configuracao = configuracao;
        }

        /// <summary>
        /// Processa a respota recebida.
        /// </summary>
        /// <param name="reply">Mensagem da resposta.</param>
        /// <param name="correlationState">Estado de correlação.</param>
        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            if (reply.IsFault)
            {
                var buffer = reply.CreateBufferedCopy(int.MaxValue);
                var copy = buffer.CreateMessage();
                reply = buffer.CreateMessage();

                var messageFault = MessageFault.CreateFault(copy, 0x10000);

                if (messageFault.Code.Name == InvalidTokenFaultReasonCode ||
                     messageFault.Code.Name == EmptyTokenFaultReasonCode)
                {
                    throw new Colosoft.Net.InvalidSecurityTokenException(messageFault.Reason.ToString(), FaultException.CreateFault(messageFault));
                }

                throw FaultException.CreateFault(messageFault);
            }
        }

        /// <summary>
        /// Processa a requisição antes de enviar.
        /// </summary>
        /// <param name="request">Mensagem da requisição.</param>
        /// <param name="channel">Canal do cliente.</param>
        /// <returns>Objecto correlacionado com o estado.</returns>
        public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel)
        {
            object httpRequestMessageObject;

            if (request.Properties.TryGetValue(HttpRequestMessageProperty.Name, out httpRequestMessageObject))
            {
                var httpRequestMessage = httpRequestMessageObject as HttpRequestMessageProperty;
                if (string.IsNullOrWhiteSpace(httpRequestMessage.Headers[TokenHeaderName]))
                {
                    httpRequestMessage.Headers[TokenHeaderName] = this.configuracao.Token;
                    httpRequestMessage.Headers[EmpresaHeaderName] = this.configuracao.Empresa;
                }
            }
            else
            {
                var httpRequestMessage = new HttpRequestMessageProperty();
                httpRequestMessage.Headers.Add(TokenHeaderName, this.configuracao.Token);
                httpRequestMessage.Headers.Add(EmpresaHeaderName, this.configuracao.Empresa);
                request.Properties.Add(HttpRequestMessageProperty.Name, httpRequestMessage);
            }

            request.Headers.Add(MessageHeader.CreateHeader(TokenHeaderName, Namespace, this.configuracao.Token));
            request.Headers.Add(MessageHeader.CreateHeader(EmpresaHeaderName, Namespace, this.configuracao.Empresa));

            return null;
        }
    }
}
