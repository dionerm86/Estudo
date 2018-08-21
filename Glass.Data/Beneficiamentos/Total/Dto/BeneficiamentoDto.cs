// <copyright file="BeneficiamentoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Glass.Data.Beneficiamentos.Total.Dto
{
    /// <summary>
    /// Classe que encapsula os dados dos beneficiamentos para o controle.
    /// </summary>
    [DataContract(Name = "Beneficiamento")]
    public class BeneficiamentoDto
    {
        /// <summary>
        /// Obtém ou define o identificador do beneficiamento.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Obtém ou define o nome do beneficiamento.
        /// </summary>
        [DataMember]
        [JsonProperty("nome")]
        public string Nome { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o beneficiamento permite cobrança opcional.
        /// </summary>
        [DataMember]
        [JsonProperty("permitirCobrancaOpcional")]
        public bool PermitirCobrancaOpcional { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de controle do beneficiamento.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoControle")]
        public Model.TipoControleBenef TipoControle { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de cálculo do beneficiamento.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoCalculo")]
        public Model.TipoCalculoBenef TipoCalculo { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o cálculo do beneficiamento considera a espessura do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("calculoPorEspessura")]
        public bool CalculoPorEspessura { get; set; }

        /// <summary>
        /// Obtém ou define os preços e custos do beneficiamento.
        /// </summary>
        [DataMember]
        [JsonProperty("precos")]
        public IEnumerable<PrecoBeneficiamentoDto> Precos { get; set; }

        /// <summary>
        /// Obtém ou define s lista de filhos de um beneficiamento (opções que podem ser selecionadas).
        /// </summary>
        [DataMember]
        [JsonProperty("filhos")]
        public IEnumerable<BeneficiamentoDto> Filhos { get; set; }
    }
}
