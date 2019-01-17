// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Estoque.Negocios.Entidades;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.V1.MateriaPrima.Extrato.MovimentacaoChapa.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de um item para a tela de extrato de movimentação de chapa.
    /// </summary>
    [DataContract(Name = "ExtratoMovimentacaoChapa")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="movimentacaoChapa">A model de movimentação de chapa.</param>
        internal ListaDto(MovChapa movimentacaoChapa)
        {
            this.CorVidro = new CorVidroDto
            {
                Id = movimentacaoChapa.IdCorVidro,
                Descricao = movimentacaoChapa.CorVidro,
            };

            this.Espessura = (decimal)movimentacaoChapa.Espessura;
            this.Quantidades = new QuantidadesDto
            {
                Inicial = movimentacaoChapa.Inicial,
                Utilizada = movimentacaoChapa.Utilizado,
                Disponivel = movimentacaoChapa.Disponivel,
            };

            this.MetroQuadradoLido = movimentacaoChapa.M2Lido;
            this.Sobra = movimentacaoChapa.Sobra;

            var movimentacoesChapasDetalhadas = new List<ExtratoDto>();
            for (int i = 0; i < movimentacaoChapa.Chapas.Count; i++)
            {
                movimentacoesChapasDetalhadas.Add(new ExtratoDto
                {
                    DataLeitura = movimentacaoChapa.Chapas[i].DataLeitura,
                    CodigoEtiquetaChapa = movimentacaoChapa.Chapas[i].NumEtiqueta,
                    Produto = movimentacaoChapa.Chapas[i].DescricaoProd,
                    MetroQuadrado = new MetroQuadradoDto
                    {
                        Utilizado = movimentacaoChapa.Chapas[i].M2Utilizado,
                        Lido = movimentacaoChapa.Chapas[i].M2Lido,
                        Sobra = movimentacaoChapa.Chapas[i].Sobra,
                        PlanosCorteVinculados = movimentacaoChapa.Chapas[i].PlanosCorte,
                        CodigosEtiquetasVinculadas = movimentacaoChapa.Chapas[i].Etiquetas,
                    },

                    CorLinha = this.ObterCorLinha(movimentacaoChapa.Chapas[i]),
                });
            }

            this.Extrato = movimentacoesChapasDetalhadas;
        }

        /// <summary>
        /// Obtém ou define a cor do vidro.
        /// </summary>
        [DataMember]
        [JsonProperty("corVidro")]
        public CorVidroDto CorVidro { get; set; }

        /// <summary>
        /// Obtém ou define a espessura do vidro.
        /// </summary>
        [DataMember]
        [JsonProperty("espessura")]
        public decimal? Espessura { get; set; }

        /// <summary>
        /// Obtém ou define os dados referêntes as quantidades.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidades")]
        public QuantidadesDto Quantidades { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade em metros quadrados lida.
        /// </summary>
        [DataMember]
        [JsonProperty("metroQuadradoLido")]
        public decimal? MetroQuadradoLido { get; set; }

        /// <summary>
        /// Obtém ou define a sobra.
        /// </summary>
        [DataMember]
        [JsonProperty("sobra")]
        public decimal? Sobra { get; set; }

        /// <summary>
        /// Obtém ou define os dados referêntes ao extrato de movimentação.
        /// </summary>
        [DataMember]
        [JsonProperty("extrato")]
        public IEnumerable<ExtratoDto> Extrato { get; set; }

        private string ObterCorLinha(MovChapaDetalhe movimentacaoChapaDetalhe)
        {
            if (movimentacaoChapaDetalhe.TemOutrasLeituras)
            {
                return "Red";
            }
            else if (movimentacaoChapaDetalhe.SaidaRevenda)
            {
                return "Blue";
            }
            else
            {
                return string.Empty;
            }
        }
    }
}