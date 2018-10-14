// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas;
using Glass.Global.Negocios.Entidades;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.TabelasDescontoAcrescimoCliente.Lista
{
    /// <summary>
    /// Classe que encapsula um item para a lista de tabelas de desconto/acréscimo cliente.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto : IdNomeDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="tabela">A tabela de desconto/acréscimo que será retornada.</param>
        public ListaDto(TabelaDescontoAcrescimoCliente tabela)
        {
            this.Id = tabela.IdTabelaDesconto;
            this.Nome = tabela.Descricao;
        }
    }
}
