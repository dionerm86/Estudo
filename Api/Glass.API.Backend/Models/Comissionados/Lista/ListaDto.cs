// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas;
using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Global.Negocios.Entidades;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Comissionados.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de um comissionado para a tela de listagem.
    /// </summary>
    [DataContract(Name = "Comissionado")]
    public class ListaDto : IdNomeDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="comissionado">A model de comissionado.</param>
        internal ListaDto(Global.Negocios.Entidades.Comissionado comissionado)
        {
            this.Id = comissionado.IdComissionado;
            this.Nome = comissionado.Nome;
            this.CpfCnpj = comissionado.CpfCnpj;
            this.RgInscricaoEstadual = comissionado.RgInscEst;
            this.Percentual = (decimal)comissionado.Percentual;

            this.Telefones = new TelefonesDto
            {
                Residencial = comissionado.TelRes,
                Celular = comissionado.TelCel,
            };
        }

        /// <summary>
        /// Obtém ou define o CFP/CNPJ do comissionado.
        /// </summary>
        [DataMember]
        [JsonProperty("cpfCnpj")]
        public string CpfCnpj { get; set; }

        /// <summary>
        /// Obtém ou define o RG/Inscrição estadual do comissionado.
        /// </summary>
        [DataMember]
        [JsonProperty("rgInscricaoEstadual")]
        public string RgInscricaoEstadual { get; set; }

        /// <summary>
        /// Obtém ou define o percentual de comissão do comissionado.
        /// </summary>
        [DataMember]
        [JsonProperty("percentual")]
        public decimal Percentual { get; set; }

        /// <summary>
        /// Obtém ou define os telefones do comissionado.
        /// </summary>
        [DataMember]
        [JsonProperty("telefones")]
        public TelefonesDto Telefones { get; set; }
    }
}
