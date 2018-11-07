// <copyright file="OperacaoIntegracao.cs" company="Sync Softwares">
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
    /// Representa uma operação de integração.
    /// </summary>
    public class OperacaoIntegracao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="OperacaoIntegracao"/>.
        /// </summary>
        /// <param name="nome">Nome da operação.</param>
        /// <param name="descricao">Descrição da operação.</param>
        /// <param name="parametros">Parâmetros da operação.</param>
        public OperacaoIntegracao(string nome, string descricao, params ParametroOperacaoIntegracao[] parametros)
        {
            this.Nome = nome;
            this.Descricao = descricao;
            this.Parametros = parametros;
        }

        /// <summary>
        /// Obtém o nome da operação.
        /// </summary>
        public string Nome { get; }

        /// <summary>
        /// Obtém a descrição da operação.
        /// </summary>
        public string Descricao { get; }

        /// <summary>
        /// Obtém os parâmetros da operação.
        /// </summary>
        public IEnumerable<ParametroOperacaoIntegracao> Parametros { get; }
    }
}
