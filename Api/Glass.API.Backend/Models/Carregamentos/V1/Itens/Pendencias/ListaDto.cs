// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Configuracoes;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Data.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Carregamentos.V1.Itens.Pendencias
{
    /// <summary>
    /// Classe que encapsula os dados das pendencias de carregamentos
    /// </summary>
    [DataContract(Name = "PendenciaCarregamento")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto" />
        /// </summary>
        /// <param name="pendenciaCarregamento">A model de pendencias de carregamento.</param>
        internal ListaDto(WebGlass.Business.OrdemCarga.Entidade.ListagemPendenciaCarregamento pendenciaCarregamento)
        {
            this.Id = (int)pendenciaCarregamento.IdCarregamento;
            this.Cliente = new IdNomeDto
            {
                Id = (int)pendenciaCarregamento.IdCliente,
                Nome = pendenciaCarregamento.NomeCliente,
            };

            this.ClienteExterno = new IdNomeDto
            {
                Id = (int)pendenciaCarregamento.IdClienteExterno,
                Nome = pendenciaCarregamento.ClienteExterno,
            };

            this.Peso = pendenciaCarregamento.PesoTotal;
        }

        /// <summary>
        /// Obtém ou define o identificador da pendencia.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do cliente associado ao carregamento pendente.
        /// </summary>
        [DataMember]
        [JsonProperty("cliente")]
        public IdNomeDto Cliente { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do cliente externo associado ao carregamento pendente.
        /// </summary>
        [DataMember]
        [JsonProperty("clienteExterno")]
        public IdNomeDto ClienteExterno { get; set; }

        /// <summary>
        /// Obtém ou define o peso pendente para o carregamento.
        /// </summary>
        [DataMember]
        [JsonProperty("peso")]
        public decimal Peso { get; set; }
    }
}
