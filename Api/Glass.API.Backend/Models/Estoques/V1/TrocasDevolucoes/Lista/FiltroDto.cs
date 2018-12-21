using Glass.API.Backend.Helper.Estoques;
using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System;

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
        /// Obtém ou define o identificador do pedido.
        /// </summary>
        [JsonProperty("idPedido")]
        public int? IdPedido { get; set; }

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
        /// Obtém ou define os identificadores dos funcionários associados a troca/devolução.
        /// </summary>
        [JsonProperty("idsFuncionario")]
        public int[] IdsFuncionario { get; set; }

        /// <summary>
        /// Obtém ou define os identificadores dos funcionários/vendedores associados ao cliente da troca/devolução.
        /// </summary>
        [JsonProperty("idsFuncionarioAssociadoCliente")]
        public int[] IdsFuncionarioAssociadoCliente { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do produto.
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
        /// Obtém ou define a data inicial de troca/devolução.
        /// </summary>
        [JsonProperty("periodoTrocaInicio")]
        public DateTime? PeriodoTrocaInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data final de troca/devolução.
        /// </summary>
        [JsonProperty("periodoTrocaFim")]
        public DateTime? PeriodoTrocaFim { get; set; }

        /// <summary>
        /// Obtém ou define a altura mínima do produto.
        /// </summary>
        [JsonProperty("alturaMinima")]
        public int? AlturaMinima { get; set; }

        /// <summary>
        /// Obtém ou define a altura máxima do produto.
        /// </summary>
        [JsonProperty("alturaMaxima")]
        public int? AlturaMaxima { get; set; }

        /// <summary>
        /// Obtém ou define a largura mínima do produto.
        /// </summary>
        [JsonProperty("larguraMinima")]
        public int? LarguraMinima { get; set; }

        /// <summary>
        /// Obtém ou define a largura máxima do produto.
        /// </summary>
        [JsonProperty("larguraMaxima")]
        public int? LarguraMaxima { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da origem de troca/devolução.
        /// </summary>
        [JsonProperty("idOrigem")]
        public int? IdOrigem { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do tipo de perda.
        /// </summary>
        [JsonProperty("idTipoPerda")]
        public int? IdTipoPerda { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do setor.
        /// </summary>
        [JsonProperty("idSetor")]
        public int? IdSetor { get; set; }

        /// <summary>
        /// Obtém ou define os tipos de pedido que geraram as trocas/devoluções.
        /// </summary>
        [JsonProperty("tipoPedido")]
        public int[] TipoPedido { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do gurpo de produto.
        /// </summary>
        [JsonProperty("idGrupoProduto")]
        public int IdGrupoProduto { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do subgrupo de produto.
        /// </summary>
        [JsonProperty("idSubgrupoProduto")]
        public int? IdSubgrupoProduto { get; set; }
    }
}
