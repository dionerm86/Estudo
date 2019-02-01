// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.ArquivosRemessa;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.Model;
using Newtonsoft.Json;
using System;

namespace Glass.API.Backend.Models.ArquivosRemessa.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os itens de filtro para a lista de arquivos de remessa.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaArquivosRemessa(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o identificador do arquivo de remessa.
        /// </summary>
        [JsonProperty("id")]
        public int? Id { get; set; }

        /// <summary>
        /// Obtém ou define o número do arquivo de remessa.
        /// </summary>
        [JsonProperty("numeroArquivoRemessa")]
        public int? NumeroArquivoRemessa { get; set; }

        /// <summary>
        /// Obtém ou define o tipo do arquivo de remessa.
        /// </summary>
        [JsonProperty("tipo")]
        public int? Tipo { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da conta bancária do arquivo de remessa.
        /// </summary>
        [JsonProperty("idContaBanco")]
        public int? IdContaBanco { get; set; }

        /// <summary>
        /// Obtém ou define a faixa inicial no período de cadastro do arquivo de remessa.
        /// </summary>
        [JsonProperty("periodoCadastroInicio")]
        public DateTime? PeriodoCadastroInicio { get; set; }

        /// <summary>
        /// Obtém ou define a faixa final no período de cadastro do arquivo de remessa.
        /// </summary>
        [JsonProperty("periodoCadastroFim")]
        public DateTime? PeriodoCadastroFim { get; set; }
    }
}