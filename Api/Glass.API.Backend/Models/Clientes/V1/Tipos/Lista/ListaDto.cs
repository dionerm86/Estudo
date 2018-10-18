// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Glass.Global.Negocios.Entidades;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Clientes.V1.Tipos.Lista
{
    /// <summary>
    /// Classe que encapsula um item para a lista de tipos de cliente.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto : IdDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="tipoCliente">O tipo de cliente que será retornado.</param>
        public ListaDto(TipoCliente tipoCliente)
        {
            this.Id = tipoCliente.IdTipoCliente;
            this.Descricao = tipoCliente.Descricao;
            this.CobrarAreaMinima = tipoCliente.CobrarAreaMinima;

            this.Permissoes = new PermissoesDto
            {
                LogAlteracoes = LogAlteracaoDAO.Instance.TemRegistro(Data.Model.LogAlteracao.TabelaAlteracao.TipoCartao, (uint)tipoCliente.IdTipoCliente, null),
            };
        }

        /// <summary>
        /// Obtém ou define a descrição do tipo de cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("descricao")]
        public string Descricao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se esse tipo de cliente cobra área mínima.
        /// </summary>
        [DataMember]
        [JsonProperty("cobrarAreaMinima")]
        public bool CobrarAreaMinima { get; set; }

        /// <summary>
        /// Obtém ou define a lista de permissões concedidas no tipo de cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}
