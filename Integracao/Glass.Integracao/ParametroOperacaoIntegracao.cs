// <copyright file="ParametroOperacaoIntegracao.cs" company="Sync Softwares">
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
    /// Representa o parâmetro da operação de integração.
    /// </summary>
    public class ParametroOperacaoIntegracao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ParametroOperacaoIntegracao"/>.
        /// </summary>
        /// <param name="nome">Nome do parâmetro.</param>
        /// <param name="tipo">Tipo do parâmetro.</param>
        /// <param name="descricao">Descrição do parâmetro.</param>
        /// <param name="valorPadrao">Valor padrão.</param>
        public ParametroOperacaoIntegracao(string nome, Type tipo, string descricao, object valorPadrao)
        {
            this.Nome = nome;
            this.Tipo = tipo;
            this.Descricao = descricao;
            this.ValorPadrao = valorPadrao;
        }

        /// <summary>
        /// Obtém o nome do parâmetro.
        /// </summary>
        public string Nome { get; }

        /// <summary>
        /// Obtém a descrição do parâmetro.
        /// </summary>
        public string Descricao { get; }

        /// <summary>
        /// Obtém o tipo do parâmetro.
        /// </summary>
        public Type Tipo { get; }

        /// <summary>
        /// Obtém o valor padrão do parâmetro.
        /// </summary>
        public object ValorPadrao { get; }
    }
}
