// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Fornecedores;
using Glass.API.Backend.Models.Genericas;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Fornecedores.Lista
{
    /// <summary>
    /// Classe com os itens para o filtro de lista de fornecedores.
    /// </summary>
    [DataContract(Name = "Filtro")]
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaFornecedores(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o identificador do fornecedor.
        /// </summary>
        [JsonProperty("id")]
        public int? Id { get; set; }

        /// <summary>
        /// Obtém ou define o nome do fornecedor.
        /// </summary>
        [JsonProperty("nome")]
        public string Nome { get; set; }

        /// <summary>
        /// Obtém ou define a situação do fornecedor.
        /// </summary>
        [JsonProperty("situacao")]
        public Data.Model.SituacaoFornecedor? Situacao { get; set; }

        /// <summary>
        /// Obtém ou define o CPF/CNPJ do fornecedor.
        /// </summary>
        [JsonProperty("cpfCnpj")]
        public string CpfCnpj { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se é para buscar somente fornecedores com crédito.
        /// </summary>
        [JsonProperty("comCredito")]
        public bool ComCredito { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do plano de conta do fornecedor.
        /// </summary>
        [JsonProperty("idPlanoConta")]
        public int? IdPlanoConta { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da parcela do fornecedor.
        /// </summary>
        [JsonProperty("idParcela")]
        public int? IdParcela { get; set; }

        /// <summary>
        /// Obtém ou define o endereço do fornecedor.
        /// </summary>
        [JsonProperty("endereco")]
        public string Endereco { get; set; }

        /// <summary>
        /// Obtém ou define o vendedor do fornecedor.
        /// </summary>
        [JsonProperty("vendedor")]
        public string Vendedor { get; set; }
    }
}
