using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Integracao.Khan
{
    /// <summary>
    /// Representa o integrador da Khan.
    /// </summary>
    public class IntegradorKhan : IIntegrador
    {
        /// <summary>
        /// Obtém o nome do integrador.
        /// </summary>
        public string Nome => "Khan";

        /// <summary>
        /// Obtém a configuração do integrador.
        /// </summary>
        public ConfiguracaoIntegrador Configuracao { get; } = new ConfiguracaoIntegrador();

        /// <summary>
        /// Obtém ou define um valor que indica se o integrador está ativo.
        /// </summary>
        public bool Ativo { get; set; }

        /// <summary>
        /// Realiza o setup do integrador.
        /// </summary>
        /// <returns>Tarefa.</returns>
        public Task Setup()
        {
            return Task.CompletedTask;
        }
    }
}
