// <copyright file="ConfiguracaoIntegrador.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

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
        public IEnumerable<string> NomesParametro => this.parametros.Keys;

        /// <summary>
        /// Obtém ou define o parâmetro associado com o nome informado.
        /// </summary>
        /// <param name="nome">Nome do parâmetro.</param>
        /// <returns>Valor do parâmetro.</returns>
        public object this[string nome]
        {
            get
            {
                return this.parametros[nome];
            }

            set
            {
                this.parametros[nome] = value;
            }
        }

        /// <summary>
        /// Verfica se a configuração é somente leitura.
        /// </summary>
        /// <param name="nome">Nome da configuração.</param>
        /// <returns>True se a configuração for somente leitura.</returns>
        public virtual bool VerificarSomenteLeitura(string nome)
        {
            return false;
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
