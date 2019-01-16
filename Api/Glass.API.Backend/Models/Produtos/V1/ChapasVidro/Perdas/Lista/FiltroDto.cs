// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Produtos.ChapasVidro.Perdas;
using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.V1.ChapasVidro.Perdas.Lista
{
    /// <summary>
    /// Classe que encapsula os filtros para a tela de perdas de chapa de vidro.
    /// </summary>
    [DataContract(Name = "Filtro")]
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaPerdasChapasVidro(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o identificador da perda de chapa de vidro.
        /// </summary>
        [JsonProperty("id")]
        public int? Id { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do tipo da perda.
        /// </summary>
        [JsonProperty("idTipoPerda")]
        public int? IdTipoPerda { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do subtipo da perda.
        /// </summary>
        [JsonProperty("idSubtipoPerda")]
        public int? IdSubtipoPerda { get; set; }

        /// <summary>
        /// Obtém ou define o inicio do período de cadastro da perda.
        /// </summary>
        [JsonProperty("periodoCadastroInicio")]
        public DateTime? PeriodoCadastroInicio { get; set; }

        /// <summary>
        /// Obtém ou define o fim do período de cadastro da perda.
        /// </summary>
        [JsonProperty("periodoCadastroFim")]
        public DateTime? PeriodoCadastroFim { get; set; }

        /// <summary>
        /// Obtém ou define o código da etiqueta referente a perda.
        /// </summary>
        [JsonProperty("codigoEtiqueta")]
        public string CodigoEtiqueta { get; set; }
    }
}