// <copyright file="IdentificadorItemEsquema.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System;

namespace Glass.Integracao.Historico
{
    /// <summary>
    /// Representa o identificador do item do esquema.
    /// </summary>
    public class IdentificadorItemEsquema
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="IdentificadorItemEsquema"/>.
        /// </summary>
        /// <param name="nome">Nome do identificador.</param>
        /// <param name="tipo">Tipo do identificador.</param>
        public IdentificadorItemEsquema(string nome, Type tipo)
        {
            this.Nome = nome;
            this.Tipo = tipo;
        }

        /// <summary>
        /// Obtém o nome do identificador.
        /// </summary>
        public string Nome { get; }

        /// <summary>
        /// Obtém o tipo do identificador.
        /// </summary>
        public Type Tipo { get; }
    }
}
