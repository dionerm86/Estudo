// <copyright file="GerenciadorSituacaoCliente.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System.Collections.Generic;
using System.Linq;

namespace Glass.Data
{
    /// <summary>
    /// Representa o gerenciador da situação dos clientes.
    /// </summary>
    public sealed class GerenciadorSituacaoCliente
    {
        private static GerenciadorSituacaoCliente gerenciador;
        private readonly IEnumerable<IProvedorSituacaoCliente> provedores;

        private GerenciadorSituacaoCliente(IEnumerable<IProvedorSituacaoCliente> provedores)
        {
            this.provedores = provedores;
        }

        /// <summary>
        /// Obtém a instância única do gerenciador da situação dos clientes.
        /// </summary>
        public static GerenciadorSituacaoCliente Gerenciador
        {
            get
            {
                if (gerenciador == null)
                {
                    var provedoresSituacaoCliente = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetAllInstances<IProvedorSituacaoCliente>()
                        .ToList();

                    gerenciador = new GerenciadorSituacaoCliente(provedoresSituacaoCliente);
                }

                return gerenciador;
            }
        }

        /// <summary>
        /// Verifica se o cliente possui algum bloqueio.
        /// </summary>
        /// <param name="sessao">Sessão do banco de dados.</param>
        /// <param name="cliente">Instância com os dados do cliente que será analizado.</param>
        /// <param name="motivos">Motivos do bloqueio.</param>
        /// <returns>True caso o cliente possua algum bloqueio ativo.</returns>
        public bool VerificarBloqueio(GDA.GDASession sessao, Model.Cliente cliente, out IEnumerable<string> motivos)
        {
            var motivosResultado = new List<string>();
            var bloqueado = false;

            foreach (var provedor in this.provedores)
            {
                if (provedor.Ativo)
                {
                    try
                    {
                        IEnumerable<string> motivosBloqueio;
                        if (provedor.VerificarBloqueio(sessao, cliente, out motivosBloqueio))
                        {
                            motivosResultado.AddRange(motivosBloqueio);
                            bloqueado = true;
                        }
                    }
                    catch (System.Exception ex)
                    {
                        bloqueado = true;
                        motivosResultado.Add(ex.Message);
                    }
                }
            }

            motivos = motivosResultado;
            return bloqueado;
        }
    }
}
