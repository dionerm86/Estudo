using Glass.API.Backend.Helper.Estoques;
using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Glass.API.Backend.Models.Estoques.V1.TrocasDevolucoes.Lista
{
    /// <summary>
    /// Classe com os itens para o filtro de lista de Troca/Devolução.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaTrocasDevolucoes(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o identificador do cliente.
        /// </summary>
        [JsonProperty("id")]
        public int? Id { get; set; }

        /// <summary>
        /// Obtém ou define o nome do cliente.
        /// </summary>
        [JsonProperty("idCliente")]
        public int? Idcliente { get; set; }

        /// <summary>
        /// Obtém ou define o nome do cliente.
        /// </summary>
        [JsonProperty("idPedido")]
        public int? IdPedido { get; set; }

        /// <summary>
        /// Obtém ou define o nome do cliente.
        /// </summary>
        [JsonProperty("idsFuncionario")]
        public int?[] IdsFuncionario { get; set; }

        /// <summary>
        /// Obtém ou define o nome do cliente.
        /// </summary>
        [JsonProperty("idsFuncionarioAssociadoCliente")]
        public int?[] IdsFuncionarioAssociadoCliente { get; set; }

        /// <summary>
        /// Obtém ou define o nome do cliente.
        /// </summary>
        [JsonProperty("nomeCliente")]
        public string NomeCliente { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da loja do cliente.
        /// </summary>
        [JsonProperty("idLoja")]
        public int? IdLoja { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da loja do cliente.
        /// </summary>
        [JsonProperty("idProduto")]
        public int? IdProduto { get; set; }

        /// <summary>
        /// Obtém ou define o tipo do cliente.
        /// </summary>
        [JsonProperty("tipo")]
        public int? Tipo { get; set; }

        /// <summary>
        /// Obtém ou define a situação do cliente.
        /// </summary>
        [JsonProperty("situacao")]
        public Data.Model.TrocaDevolucao.SituacaoTrocaDev? Situacao { get; set; }

        /// <summary>
        /// Obtém ou define a data inicial de cadastro do cliente.
        /// </summary>
        [JsonProperty("dataInicio")]
        public DateTime? DataInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data final de cadastro do cliente.
        /// </summary>
        [JsonProperty("dataFim")]
        public DateTime? DataFim { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica qual rota será alterada para os clientes filtrados.
        /// </summary>
        [JsonProperty("idRotaNova")]
        public int? IdRotaNova { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica qual vendedor será alterado para os clientes filtrados.
        /// </summary>
        [JsonProperty("idVendedorNovo")]
        public int? IdVendedorNovo { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica qual vendedor será alterado para os clientes filtrados.
        /// </summary>
        [JsonProperty("alturaMinima")]
        public int? AlturaMinima { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica qual vendedor será alterado para os clientes filtrados.
        /// </summary>
        [JsonProperty("alturaMaxima")]
        public int? AlturaMaxima { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica qual vendedor será alterado para os clientes filtrados.
        /// </summary>
        [JsonProperty("larguraMinima")]
        public int? LarguraMinima { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica qual vendedor será alterado para os clientes filtrados.
        /// </summary>
        [JsonProperty("larguraMaxima")]
        public int? LarguraMaxima { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica qual vendedor será alterado para os clientes filtrados.
        /// </summary>
        [JsonProperty("idOrigemTrocaDevolucao")]
        public int? IdOrigemTrocaDevolucao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica qual vendedor será alterado para os clientes filtrados.
        /// </summary>
        [JsonProperty("idTipoPerda")]
        public int? IdTipoPerda { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica qual vendedor será alterado para os clientes filtrados.
        /// </summary>
        [JsonProperty("idSetor")]
        public int? IdSetor { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica qual vendedor será alterado para os clientes filtrados.
        /// </summary>
        [JsonProperty("tipoPedido")]
        public string TipoPedido { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica qual vendedor será alterado para os clientes filtrados.
        /// </summary>
        [JsonProperty("idGrupoProduto")]
        public int IdGrupoProduto { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica qual vendedor será alterado para os clientes filtrados.
        /// </summary>
        [JsonProperty("idSubgrupoProduto")]
        public int? IdSubgrupoProduto { get; set; }
    }
}
