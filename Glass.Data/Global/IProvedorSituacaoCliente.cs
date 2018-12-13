// <copyright file="IProvedorSituacaoCliente.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System.Collections.Generic;

namespace Glass.Data
{
    /// <summary>
    /// Assinatura do provedor que gerência a situação do cliente.
    /// </summary>
    public interface IProvedorSituacaoCliente
    {
        /// <summary>
        /// Obtém um valor que indica se provedor está ativo.
        /// </summary>
        bool Ativo { get; }

        /// <summary>
        /// Verifica se o cliente possui algum bloqueio.
        /// </summary>
        /// <param name="sessao">Sessão do banco de dados.</param>
        /// <param name="cliente">Instância com os dados do cliente que será analizado.</param>
        /// <param name="motivos">Motivos do bloqueio.</param>
        /// <returns>True caso o cliente possua algum bloqueio ativo.</returns>
        bool VerificarBloqueio(GDA.GDASession sessao, Model.Cliente cliente, out IEnumerable<string> motivos);
    }
}
