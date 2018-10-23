// <copyright file="CfopDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.Model;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Cfops.V1.Filtro
{
    /// <summary>
    /// Classe que encapsula os dados básicos de CFOP para os controles de filtro.
    /// </summary>
    [DataContract(Name = "Cfop")]
    public class CfopDto : IdCodigoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="CfopDto"/>.
        /// </summary>
        /// <param name="sessao">A transação atual com o banco de dados.</param>
        /// <param name="cfop">A model de CFOP's.</param>
        public CfopDto(GDASession sessao, Cfop cfop)
        {
            this.Id = cfop.IdCfop;
            this.Codigo = cfop.CodInterno;
            this.Descricao = cfop.Descricao;
        }

        /// <summary>
        /// Obtém ou define a descrição do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("descricao")]
        public string Descricao { get; set; }
    }
}
