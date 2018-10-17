// <copyright file="LeituraDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Producao.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de uma leitura da peça.
    /// </summary>
    [DataContract(Name = "Leitura")]
    public class LeituraDto : DataFuncionarioDto
    {
        /// <summary>
        /// Obtém ou define os dados do setor da leitura.
        /// </summary>
        [DataMember]
        [JsonProperty("setor")]
        public SetorDto Setor { get; set; }

        /// <summary>
        /// Obtém ou define os dados da chapa de corte.
        /// </summary>
        [DataMember]
        [JsonProperty("chapa")]
        public ChapaVidroDto Chapa { get; set; }
    }
}
