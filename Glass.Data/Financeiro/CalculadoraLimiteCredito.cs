// <copyright file="CalculadoraLimiteCredito.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System.Collections.Generic;
using System.Linq;

namespace Glass.Data
{
    /// <summary>
    /// Representa a factory para carregar o provedor dos limite de crédito dos clientes.
    /// </summary>
    public class CalculadoraLimiteCredito
    {
        private static CalculadoraLimiteCredito calculadora;
        private readonly IEnumerable<IProvedorLimiteCredito> provedores;
        private readonly IProvedorLimiteCredito provedorPadrao;

        private CalculadoraLimiteCredito(IEnumerable<IProvedorLimiteCredito> provedores)
        {
            this.provedores = provedores;
            this.provedorPadrao = new ProvedorLimiteCreditoPadrao();
        }

        /// <summary>
        /// Obtém instância única da calculadora dos limites de crédito.
        /// </summary>
        public static CalculadoraLimiteCredito Calculadora
        {
            get
            {
                if (calculadora == null)
                {
                    var provedores1 = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetAllInstances<IProvedorLimiteCredito>()
                        .ToList();

                    calculadora = new CalculadoraLimiteCredito(provedores1);
                }

                return calculadora;
            }
        }

        private IProvedorLimiteCredito Provedor =>
            this.provedores.FirstOrDefault(f => f.Ativo) ?? this.provedorPadrao;

        /// <summary>
        /// Obtém o valor do limite de crédito para o cliente informado.
        /// </summary>
        /// <param name="sessao">Sessão de acesso ao banco de dados.</param>
        /// <param name="cliente">Cliente que será verificado.</param>
        /// <returns>Valor do limite.</returns>
        public decimal ObterLimite(GDA.GDASession sessao, Model.Cliente cliente) =>
            this.Provedor.ObterLimite(sessao, cliente);

        /// <summary>
        /// Obtém o valor do limite de crédito para o cliente informado.
        /// </summary>
        /// <param name="sessao">Sessão de acesso ao banco de dados.</param>
        /// <param name="idCliente">Identificador do cliente que será verificado.</param>
        /// <returns>Valor do limite.</returns>
        public decimal ObterLimite(GDA.GDASession sessao, int idCliente) =>
            this.Provedor.ObterLimite(sessao, idCliente);
    }
}
