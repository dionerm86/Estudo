// <copyright file="IProvedorLimiteCredito.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.Data
{
    /// <summary>
    /// Assinatura do provedor de limite de crédito do cliente.
    /// </summary>
    public interface IProvedorLimiteCredito
    {
        /// <summary>
        /// Obtém um valor que indica se o provedor está ativo.
        /// </summary>
        bool Ativo { get; }

        /// <summary>
        /// Obtém o valor do limite de crédito para o cliente informado.
        /// </summary>
        /// <param name="sessao">Sessão de acesso ao banco de dados.</param>
        /// <param name="cliente">Cliente que será verificado.</param>
        /// <returns>Valor do limite.</returns>
        decimal ObterLimite(GDA.GDASession sessao, Model.Cliente cliente);

        /// <summary>
        /// Obtém o valor do limite de crédito para o cliente informado.
        /// </summary>
        /// <param name="sessao">Sessão de acesso ao banco de dados.</param>
        /// <param name="idCliente">Identificador do cliente que será verificado.</param>
        /// <returns>Valor do limite.</returns>
        decimal ObterLimite(GDA.GDASession sessao, int idCliente);
    }
}
