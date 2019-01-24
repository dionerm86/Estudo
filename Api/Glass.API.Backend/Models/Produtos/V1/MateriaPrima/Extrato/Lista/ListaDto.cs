// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Estoque.Negocios.Entidades;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.V1.MateriaPrima.Extrato.Lista
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

            var movimentacoesChapasDetalhadas = this.GerarDadosExtrato(movimentacaoChapa.Chapas);

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
        /// Obtém ou define os dados referentes as quantidades.
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
        /// Obtém ou define os dados referentes ao extrato de movimentação.
        /// </summary>
        [DataMember]
        [JsonProperty("extrato")]
        public IEnumerable<ExtratoDto> Extrato { get; set; }

        private IEnumerable<ExtratoDto> GerarDadosExtrato(List<MovChapaDetalhe> chapas)
        {
            var movimentacoesChapasDetalhadas = new List<ExtratoDto>();

            foreach (var chapa in chapas)
            {
                movimentacoesChapasDetalhadas.Add(new ExtratoDto
                {
                    DataLeitura = chapa.DataLeitura,
                    CodigoEtiquetaChapa = chapa.NumEtiqueta,
                    Produto = chapa.DescricaoProd,
                    MetroQuadrado = new MetroQuadradoDto
                    {
                        Utilizado = chapa.M2Utilizado,
                        Lido = chapa.M2Lido,
                        Sobra = chapa.Sobra,
                    },

                    PlanosCorteVinculados = chapa.PlanosCorte,
                    CodigosEtiquetasVinculadas = chapa.Etiquetas,
                    CorLinha = this.ObterCorLinha(chapa),
                });
            }

            return movimentacoesChapasDetalhadas;
        }

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