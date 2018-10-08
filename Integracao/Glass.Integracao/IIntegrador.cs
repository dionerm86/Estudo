using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Integracao
{
    /// <summary>
    /// Assinatura do integrador de um integrador do sistema.
    /// </summary>
    public interface IIntegrador
    {
        /// <summary>
        /// Obtém o nome do integrador.
        /// </summary>
        string Nome { get; }

        /// <summary>
        /// Obtém a configuração do integrador.
        /// </summary>
        ConfiguracaoIntegrador Configuracao { get; }

        /// <summary>
        /// Obtém ou define um valor que indica se o integrador está ativo.
        /// </summary>
        bool Ativo { get; set; }

        /// <summary>
        /// Realiza o setup do integrador.
        /// </summary>
        /// <returns>Tarefa.</returns>
        Task Setup();
    }
}
