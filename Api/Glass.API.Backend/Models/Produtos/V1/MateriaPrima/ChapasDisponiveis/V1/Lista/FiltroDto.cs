// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Produtos.MateriaPrima.ChapasDisponiveis;
using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;

namespace Glass.API.Backend.Models.Produtos.MateriaPrima.ChapasDisponiveis.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os itens de filtro para a lista de chapas disponíveis.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaChapasDisponiveis(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o identificador do fornecedor.
        /// </summary>
        [JsonProperty("idFornecedor")]
        public int? IdFornecedor { get; set; }

        /// <summary>
        /// Obtém ou define o nome do fornecedor.
        /// </summary>
        [JsonProperty("nomeFornecedor")]
        public string NomeFornecedor { get; set; }

        /// <summary>
        /// Obtém ou define o código do produto.
        /// </summary>
        [JsonProperty("codigoProduto")]
        public string CodigoProduto { get; set; }

        /// <summary>
        /// Obtém ou define a descrição do produto.
        /// </summary>
        [JsonProperty("descricaoProduto")]
        public string DescricaoProduto { get; set; }

        /// <summary>
        /// Obtém ou define o número da nota fiscal.
        /// </summary>
        [JsonProperty("numeroNotaFiscal")]
        public int? NumeroNotaFiscal { get; set; }

        /// <summary>
        /// Obtém ou define o lote da chapa.
        /// </summary>
        [JsonProperty("lote")]
        public string Lote { get; set; }

        /// <summary>
        /// Obtém ou define a altura da chapa.
        /// </summary>
        [JsonProperty("altura")]
        public int? Altura { get; set; }

        /// <summary>
        /// Obtém ou define a largura da chapa.
        /// </summary>
        [JsonProperty("largura")]
        public int? Largura { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da cor do vidro.
        /// </summary>
        [JsonProperty("idsCorVidro")]
        public string IdsCorVidro { get; set; }

        /// <summary>
        /// Obtém ou define a espessura da chapa.
        /// </summary>
        [JsonProperty("espessura")]
        public int? Espessura { get; set; }

        /// <summary>
        /// Obtém ou define o código da etiqueta da chapa.
        /// </summary>
        [JsonProperty("codigoEtiqueta")]
        public string CodigoEtiqueta { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da loja.
        /// </summary>
        [JsonProperty("idLoja")]
        public int? IdLoja { get; set; }
    }
}
