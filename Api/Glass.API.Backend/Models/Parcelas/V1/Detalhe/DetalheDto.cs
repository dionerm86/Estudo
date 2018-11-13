// <copyright file="DetalheDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Parcelas.V1.Detalhe
{
    /// <summary>
    /// Classe que encapsula os dados de cadastro ou atualização de uma parcela.
    /// </summary>
    [DataContract(Name = "Parcela")]
    public class DetalheDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="DetalheDto"/>.
        /// </summary>
        /// <param name="parcela">Objeto Parcela. </param>
        internal DetalheDto(Glass.Financeiro.Negocios.Entidades.Parcelas parcela)
        {
            this.Id = parcela.IdParcela;
            this.Descricao = parcela.Descricao;
            this.ParcelaPadrao = parcela.ParcelaPadrao;
            this.Situacao = new IdNomeDto
            {
                Id = (int)parcela.Situacao,
                Nome = Colosoft.Translator.Translate(parcela.Situacao).Format(),
            };

            this.Dias = !string.IsNullOrWhiteSpace(parcela.Dias) ? Array.ConvertAll(parcela.Dias.Split(','), i => Conversoes.StrParaInt(i)) : new int[0];
            this.ParcelaAVista = parcela.ParcelaAVista;
            this.NumeroParcelas = parcela.NumParcelas;
        }

        /// <summary>
        /// Obtém ou define o identificador da parcela.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Obtém ou define a descrição da parcela.
        /// </summary>
        [DataMember]
        [JsonProperty("descricao")]
        public string Descricao { get; set; }

        /// <summary>
        /// Obtém ou define a situacao da parcela.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public IdNomeDto Situacao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a parcela é a vista.
        /// </summary>
        [DataMember]
        [JsonProperty("parcelaAvista")]
        public bool ParcelaAVista { get; set; }

        /// <summary>
        /// Obtém ou define os dias da parcela.
        /// </summary>
        [DataMember]
        [JsonProperty("dias")]
        public IEnumerable<int> Dias { get; set; }

        /// <summary>
        /// Obtém ou define o desconto da parcela.
        /// </summary>
        [DataMember]
        [JsonProperty("desconto")]
        public decimal Desconto { get; set; }

        /// <summary>
        /// Obtém ou define o numero de parcelas.
        /// </summary>
        [DataMember]
        [JsonProperty("numeroParcelas")]
        public int NumeroParcelas { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a parcela é padrão.
        /// </summary>
        [DataMember]
        [JsonProperty("parcelaPadrao")]
        public bool ParcelaPadrao { get; set; }
    }
}
