// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Exportacao.Lista.V1
{
    /// <summary>
    /// Classe que encapsula os dados da listagem de exportações de pedido.
    /// </summary>
    [DataContract(Name = "ExportacaoPedidos")]
    public class ListaDto : IdDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="exportacao">A Moldel da Exportacao de Pedidos.</param>
        public ListaDto(Data.Model.Exportacao exportacao)
        {
            this.Id = (int)exportacao.IdExportacao;
            this.Fornecedor = exportacao.NomeFornec;
            this.Funcionario = exportacao.NomeFunc;
            this.DataExportacao = exportacao.DataExportacao;
        }

        /// <summary>
        /// Obtém ou define o identificador da listagem de exportações de pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Obtém ou define o fornecedor da listagem de exportações de pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("fornecedor")]
        public string Fornecedor { get; set; }

        /// <summary>
        /// Obtém ou define o funcionário da listagem de exportações de pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("funcionario")]
        public string Funcionario { get; set; }

        /// <summary>
        /// Obtém ou define a data das exportações de pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("dataExportacao")]
        public DateTime? DataExportacao { get; set; }
    }
}
