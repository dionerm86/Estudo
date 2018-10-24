// <copyright file="PosicaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Producao.V1.Setores.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de alteração da posição de um setor.
    /// </summary>
    [DataContract(Name = "Posicao")]
    public class PosicaoDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se a posição do setor será alterada para cima.
        /// </summary>
        [DataMember]
        [JsonProperty("acima")]
        public bool Acima { get; set; }
    }
}
