// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.AcertosCheques;
using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.AcertosCheques.V1.Lista
{
    /// <summary>
    /// Classe com os itens para o filtro de lista de acertos de cheques.
    /// </summary>
    [DataContract(Name = "Filtro")]
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaAcertosCheques(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o identificador do acerto de cheque.
        /// </summary>
        [JsonProperty("id")]
        public int? Id { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do funcionário.
        /// </summary>
        [JsonProperty("idFuncionario")]
        public int? IdFuncionario { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do cliente.
        /// </summary>
        [JsonProperty("idCliente")]
        public int? IdCliente { get; set; }

        /// <summary>
        /// Obtém ou define o nome do cliente.
        /// </summary>
        [JsonProperty("nomeCliente")]
        public string NomeCliente { get; set; }

        /// <summary>
        /// Obtém ou define o período inicial do cadastro do acerto de cheque.
        /// </summary>
        [JsonProperty("periodoCadastroInicio")]
        public DateTime? PeriodoCadastroInicio { get; set; }

        /// <summary>
        /// Obtém ou define o período final do cadastro do acerto de cheque.
        /// </summary>
        [JsonProperty("periodoCadastroFim")]
        public DateTime? PeriodoCadastroFim { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se existem acertos de cheques próprios.
        /// </summary>
        [JsonProperty("buscarAcertosChequesProprios")]
        public string BuscarAcertosChequesProprios { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se existem acertos de cheques do caixa diário.
        /// </summary>
        [JsonProperty("buscarAcertosCaixaDiario")]
        public bool BuscarAcertosCaixaDiario { get; set; }
    }
}