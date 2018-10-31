// <copyright file="Esquema.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System.Collections.Generic;

namespace Glass.Integracao.Historico
{
    /// <summary>
    /// Representa um esquema de histório.
    /// </summary>
    public class Esquema
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="Esquema"/>.
        /// </summary>
        /// <param name="id">Identificador do esquema.</param>
        /// <param name="nome">Nome do esquema.</param>
        /// <param name="descricao">Descrição do esquema.</param>
        /// <param name="itens">Itens associados.</param>
        public Esquema(int id, string nome, string descricao, IEnumerable<ItemEsquema> itens)
        {
            this.Id = id;
            this.Nome = nome;
            this.Descricao = descricao;
            this.Itens = itens ?? new ItemEsquema[0];

            foreach (var item in this.Itens)
            {
                item.Esquema = this;
            }
        }

        /// <summary>
        /// Obtém o identificador do esquema.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Obtém o nome do esquema.
        /// </summary>
        public string Nome { get; }

        /// <summary>
        /// Obtém a descrição do esquema.
        /// </summary>
        public string Descricao { get; }

        /// <summary>
        /// Obtém os itens do esquema do histórico.
        /// </summary>
        public IEnumerable<ItemEsquema> Itens { get; }
    }
}
