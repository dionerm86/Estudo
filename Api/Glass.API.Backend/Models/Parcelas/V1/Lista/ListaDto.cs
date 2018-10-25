// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Parcelas.V1.Lista
{
    /// <summary>
    /// Classe que encapsula um item para a lista de parcelas.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto : IdNomeDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="parcelas">A parcela que será retornada.</param>
        public ListaDto(Data.Model.Parcelas parcelas)
        {
            this.Id = parcelas.IdParcela;
            this.Nome = parcelas.Descricao;
            this.Dias = parcelas.Dias;
            this.NumParcelas = parcelas.NumParcelas;
            this.ParcelaPadrao = parcelas.ParcelaPadrao;
            this.Desconto = parcelas.Desconto;
            this.ParcelaAVista = parcelas.ParcelaAVista;
            this.Situacao = Colosoft.Translator.Translate(parcelas.Situacao).Format();
        }

        /// <summary>
        /// Obtém dias da parcela.
        /// </summary>
        [DataMember]
        [JsonProperty("dias")]
        public string Dias { get; set; }

        /// <summary>
        /// Obtém o numero de parcelas.
        /// </summary>
        [DataMember]
        [JsonProperty("numParcelas")]
        public int NumParcelas { get; set; }

        /// <summary>
        /// Obtém a situação da parcela.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public string Situacao { get; set; }

        /// <summary>
        /// Obtém se a parcela é padrão.
        /// </summary>
        [DataMember]
        [JsonProperty("parcelaPadrao")]
        public bool ParcelaPadrao { get; set; }

        /// <summary>
        /// Obtém se a parcela é padrão.
        /// </summary>
        [DataMember]
        [JsonProperty("parcelaAvista")]
        public bool ParcelaAVista { get; set; }

        /// <summary>
        /// Obtém se a parcela é padrão.
        /// </summary>
        [DataMember]
        [JsonProperty("desconto")]
        public decimal Desconto { get; set; }
    }
}
