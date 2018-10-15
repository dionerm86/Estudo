using Colosoft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Integracao
{
    /// <summary>
    /// Representa o gerenciador dos integradores.
    /// </summary>
    public sealed class GerenciadorIntegradores : IDisposable
    {
        private readonly IProvedorIntegradores provedor;
        private readonly List<IIntegrador> integradores = new List<IIntegrador>();

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="GerenciadorIntegradores"/>.
        /// </summary>
        /// <param name="provedor">Provedor que será usado na inicialização.</param>
        public GerenciadorIntegradores(IProvedorIntegradores provedor)
        {
            if (provedor == null)
            {
                throw new ArgumentNullException(nameof(provedor));
            }

            this.provedor = provedor;
        }

        /// <summary>
        /// Finaliza uma instância da classe <see cref="GerenciadorIntegradores"/>.
        /// </summary>
        ~GerenciadorIntegradores()
        {
            this.Dispose();
        }

        /// <summary>
        /// Inicializa os integradores.
        /// </summary>
        /// <param name="logger">Logger que será usado na operação.</param>
        /// <returns>True se a inicialização foi bem sucedida.</returns>
        public async Task<bool> Inicializar(Colosoft.Logging.ILogger logger)
        {
            IEnumerable<IIntegrador> integradoresDisponiveis;

            try
            {
                integradoresDisponiveis = await this.provedor.ObterIntegradoresDisponiveis();
            }
            catch (Exception ex)
            {
                logger.Error("Não foi possível carrega os integradores disponíveis".GetFormatter(), ex);
                return false;
            }

            var inicializacaoSucedida = true;

            foreach (var integrador in integradoresDisponiveis)
            {
                if (integrador.Ativo)
                {
                    try
                    {
                        await integrador.Setup();
                        this.integradores.Add(integrador);
                    }
                    catch (Exception ex)
                    {
                        logger.Error($"Ocorreu um erro no setup do integrador {integrador.Nome}".GetFormatter(), ex);
                        inicializacaoSucedida = false;
                        integrador.Dispose();
                    }
                }
            }

            return inicializacaoSucedida;
        }

        /// <summary>
        /// Libera a instância.
        /// </summary>
        public void Dispose()
        {
            foreach (var integrador in this.integradores)
            {
                integrador.Dispose();
            }

            this.integradores.Clear();
            GC.SuppressFinalize(this);
        }
    }
}
