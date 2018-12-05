// <copyright file="ProvedorLimiteCreditoPadrao.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.Model;
using System;

namespace Glass.Data
{
    /// <summary>
    /// Representa o provedor do limite de crédito dos clientes padrão.
    /// </summary>
    internal class ProvedorLimiteCreditoPadrao : IProvedorLimiteCredito
    {
        /// <inheritdoc />
        public bool Ativo => true;

        /// <inheritdoc />
        public decimal ObterLimite(GDASession sessao, int idCliente)
        {
            return Data.DAL.ClienteDAO.Instance.ObtemLimite(sessao, (uint)idCliente);
        }

        /// <inheritdoc />
        public decimal ObterLimite(GDASession sessao, Cliente cliente)
        {
            if (cliente == null)
            {
                throw new ArgumentNullException(nameof(cliente));
            }

            return cliente.Limite;
        }
    }
}
