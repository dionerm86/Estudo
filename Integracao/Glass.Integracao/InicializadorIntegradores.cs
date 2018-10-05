using Colosoft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Integracao
{
    /// <summary>
    /// Representa o inicializador dos integradores.
    /// </summary>
    public class InicializadorIntegradores
    {
        private readonly IProvedorIntegradores provedor;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="InicializadorIntegradores"/>.
        /// </summary>
        /// <param name="provedor">Provedor que será usado na inicialização.</param>
        public InicializadorIntegradores(IProvedorIntegradores provedor)
        {
            if (provedor == null)
            {
                throw new ArgumentNullException(nameof(provedor));
            }

            this.provedor = provedor;
        }

        /// <summary>
        /// Inicializa os integradores.
        /// </summary>
        /// <param name="logger">Logger que será usado na operação.</param>
        /// <returns>True se a inicialização foi bem sucedida.</returns>
        public async Task<bool> Inicializar(Colosoft.Logging.ILogger logger)
        {
            IEnumerable<IIntegrador> integradores;

            try
            {
                integradores = await this.provedor.ObterIntegradoresDisponiveis();
            }
            catch (Exception ex)
            {
                logger.Error("Não foi possível carrega os integradores disponíveis".GetFormatter(), ex);
                return false;
            }

            var inicializacaoSucedida = true;

            foreach (var integrador in integradores)
            {
                if (integrador.Ativo)
                {
                    try
                    {
                        await integrador.Setup();
                    }
                    catch (Exception ex)
                    {
                        logger.Error($"Ocorreu um erro no setup do integrador {integrador.Nome}".GetFormatter(), ex);
                        inicializacaoSucedida = false;
                    }
                }
            }

            return inicializacaoSucedida;
        }
    }
}
