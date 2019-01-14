// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Data.RelModel;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.V1.MateriaPrima.Posicao.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de uma posilção de matéria prima para a tela de listagem.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="posicaoMateriaPrima">A model de posição de matéria prima.</param>
        internal ListaDto(PosicaoMateriaPrima posicaoMateriaPrima)
        {
            this.CorVidro = new CorVidroDto
            {
                Id = (int)posicaoMateriaPrima.IdCorVidro,
                Descricao = posicaoMateriaPrima.DescrCorVidro,
            };

            this.Espessura = (decimal)posicaoMateriaPrima.Espessura;
            this.MetroQuadrado = new MetroQuadradoDto
            {
                Total = posicaoMateriaPrima.TotM2,
                ComEtiquetaImpressa = posicaoMateriaPrima.TotM2ComEtiqueta,
                SemEtiquetaImpressa = posicaoMateriaPrima.TotM2SemEtiqueta,
                PedidoDeVenda = posicaoMateriaPrima.TotM2Venda,
                PedidoDeProducao = posicaoMateriaPrima.TotM2Producao,
                EmEstoque = posicaoMateriaPrima.TotM2Estoque,
                Disponivel = posicaoMateriaPrima.TotM2DisponivelNovo,
            };

            this.Permissoes = new PermissoesDto
            {
                ExibirChapas = posicaoMateriaPrima.TotM2Estoque > 0,
            };
        }

        /// <summary>
        /// Obtém ou define os dados referentes a cor do vidro.
        /// </summary>
        [DataMember]
        [JsonProperty("corVidro")]
        public CorVidroDto CorVidro { get; set; }

        /// <summary>
        /// Obtém ou define a espessura do vidro.
        /// </summary>
        [DataMember]
        [JsonProperty("espessura")]
        public decimal Espessura { get; set; }

        /// <summary>
        /// Obtém ou define os dados associados ao metro quadrado do vidro.
        /// </summary>
        [DataMember]
        [JsonProperty("metroQuadrado")]
        public MetroQuadradoDto MetroQuadrado { get; set; }

        /// <summary>
        /// Obtém ou define a descrição do grupo do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}
