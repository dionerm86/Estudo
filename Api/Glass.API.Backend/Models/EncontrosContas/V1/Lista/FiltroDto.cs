
using Glass.API.Backend.Helper.EncontrosContas;
using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System;

namespace Glass.API.Backend.Models.EncontrosContas.V1.Lista
{
    /// <summary>
    /// 
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// 
        /// </summary>
        internal FiltroDto()
            : base(item => new TraducaoOrdenacaoListaEncontrosContas(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o identificador da loja.
        /// </summary>
        [JsonProperty("id")]
        public int? Id { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da loja.
        /// </summary>
        [JsonProperty("idCliente")]
        public int? IdCliente { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da loja.
        /// </summary>
        [JsonProperty("nomeCliente")]
        public string NomeCliente { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da loja.
        /// </summary>
        [JsonProperty("idFornecedor")]
        public int? IdFornecedor { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da loja.
        /// </summary>
        [JsonProperty("nomeFornecedor")]
        public string NomeFornecedor { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da loja.
        /// </summary>
        [JsonProperty("observacao")]
        public string Observacao { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da loja.
        /// </summary>
        [JsonProperty("periodoCadastroInicio")]
        public DateTime? PeriodoCadastroInicio { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da loja.
        /// </summary>
        [JsonProperty("periodoCadastroFim")]
        public DateTime? PeriodoCadastroFim { get; set; }
    }
}