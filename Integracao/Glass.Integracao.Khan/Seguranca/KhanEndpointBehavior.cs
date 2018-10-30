// <copyright file="KhanEndpointBehavior.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Integracao.Khan.Seguranca
{
    /// <summary>
    /// Representa o comportamento padrão da Khan para o endpoints de serviço.
    /// </summary>
    internal class KhanEndpointBehavior : IEndpointBehavior
    {
        private readonly ConfiguracaoKhan configuracao;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="KhanEndpointBehavior"/>.
        /// </summary>
        /// <param name="configuracao">Configuração.</param>
        public KhanEndpointBehavior(ConfiguracaoKhan configuracao)
        {
            this.configuracao = configuracao;
        }

        /// <summary>
        /// Método usado para criar a inspetor de mensagens que será usado.
        /// </summary>
        /// <returns>Inspetor de mensagem do token de segurança.</returns>
        protected virtual IClientMessageInspector CreateMessageInspector()
        {
            return new KhanClientMessageInspector(this.configuracao);
        }

        /// <summary>
        /// Adiciona parametros do binding do EndPoint.
        /// </summary>
        /// <param name="endpoint">Ponto final do serviço.</param>
        /// <param name="bindingParameters">Parâmetros de vinculação.</param>
        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
            // Ignora
        }

        /// <summary>
        /// Aplica o comportamento do cliente.
        /// </summary>
        /// <param name="endpoint">Ponto final do serviço.</param>
        /// <param name="clientRuntime">Runtime do cliente.</param>
        public void ApplyClientBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(this.CreateMessageInspector());
        }

        /// <summary>
        /// Aplica o comportamento do despachante das mensagens.
        /// </summary>
        /// <param name="endpoint">Ponto final do serviço.</param>
        /// <param name="endpointDispatcher">Despachando do endpoint.</param>
        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
        {
            // Ignora
        }

        /// <summary>
        /// Valida o ponto do serviço.
        /// </summary>
        /// <param name="endpoint">Ponto final do serviço.</param>
        public void Validate(ServiceEndpoint endpoint)
        {
            // Ignora
        }
    }
}
