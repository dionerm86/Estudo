// <copyright file="IProvedorIntegradores.cs" company="Sync Softwares">
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
    /// Assinatura do provedor de integradores.
    /// </summary>
    public interface IProvedorIntegradores
    {
        /// <summary>
        /// Obtém os integradores disponíveis.
        /// </summary>
        /// <returns>Coleção dos integradores.</returns>
        Task<IEnumerable<IIntegrador>> ObterIntegradoresDisponiveis();
    }
}
