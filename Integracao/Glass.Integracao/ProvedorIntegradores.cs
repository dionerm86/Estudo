// <copyright file="ProvedorIntegradores.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Integracao
{
    /// <summary>
    /// Representa o provedor dos integradores.
    /// </summary>
    public class ProvedorIntegradores : IProvedorIntegradores
    {
        private readonly Microsoft.Practices.ServiceLocation.IServiceLocator serviceLocator;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ProvedorIntegradores"/>.
        /// </summary>
        /// <param name="serviceLocator">Localizador de serviços.</param>
        public ProvedorIntegradores(Microsoft.Practices.ServiceLocation.IServiceLocator serviceLocator)
        {
            this.serviceLocator = serviceLocator;
        }

        /// <summary>
        /// Obtém os integradores disponíveis.
        /// </summary>
        /// <returns>Coleção dos integradores.</returns>
        public Task<IEnumerable<IIntegrador>> ObterIntegradoresDisponiveis()
        {
            return Task.FromResult(this.serviceLocator.GetAllInstances<IIntegrador>());
        }
    }
}
