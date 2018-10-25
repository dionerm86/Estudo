using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Integracao
{
    /// <summary>
    /// Representa a configuração do integrador.
    /// </summary>
    public class ConfiguracaoIntegrador
    {
        private readonly Dictionary<string, object> parametros = new Dictionary<string, object>();

        /// <summary>
        /// Evento acianado quando ocorrer uma solicitação para salvar os dados da configuração.
        /// </summary>
        public event EventHandler SolicitacaoSalvar;

        /// <summary>
        /// Obtém os nomes do parâmetros da confgiuração.
        /// </summary>
        public IEnumerable<string> ParameterNames => this.parametros.Keys;

        /// <summary>
        /// Obtém ou define o parâmetro associado com o nome informado.
        /// </summary>
        /// <param name="name">Nome do parâmetro.</param>
        /// <returns>Valor do parâmetro.</returns>
        public object this[string name]
        {
            get
            {
                return this.parametros[name];
            }

            set
            {
                this.parametros[name] = value;
            }
        }

        /// <summary>
        /// Salva os dados da configuração.
        /// </summary>
        public void Salvar()
        {
            this.SolicitacaoSalvar?.Invoke(this, EventArgs.Empty);
        }
    }
}
