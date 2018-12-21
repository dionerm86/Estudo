// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Carregamentos.V1.OrdensCarga.Lista.Pedidos
{
    /// <summary>
    /// Classe que encapsula os dados de um pedido para a tela de ordens de carga.
    /// </summary>
    [DataContract(Name = "PedidoOrdemCarga")]
    public class ListaDto : IdDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="pedido">O pedido que será retornado.</param>
        internal ListaDto(Glass.Data.Model.Pedido pedido)
        {
            this.Id = (int)pedido.IdPedido;
            this.IdPedidoExterno = (int?)pedido.IdPedidoExterno;
            this.Importado = pedido.Importado;
            this.CodigoPedidoCliente = pedido.IdPedidoCodCliente;
            this.ClienteExterno = ((int?)pedido.IdClienteExterno).HasValue
                ? null
                : new IdNomeDto
                {
                    Id = (int)pedido.IdClienteExterno,
                    Nome = pedido.ClienteExterno,
                };

            this.RotaExterna = pedido.RotaExterna;
            this.TipoPedido = pedido.DescricaoTipoPedido;
            this.DataEntrega = pedido.DataEntrega;
            this.Peso = (decimal)pedido.Peso;
            this.QuantidadePecasPendentes = (int)pedido.QtdePecaPendenteProducao;
            this.QuantidadeVolumesPendentes = (int)pedido.QtdePecasPendenteVolume;
            this.ObservacaoLiberacao = pedido.ObservacaoLiberacaoClientePedido;
            this.CorLinha = this.ObterCorLinha();
        }

        /// <summary>
        /// Obtém ou define o identificador do pedido externo.
        /// </summary>
        [DataMember]
        [JsonProperty("idPedidoExterno")]
        public int? IdPedidoExterno { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o pedido é importado.
        /// </summary>
        [DataMember]
        [JsonProperty("importado")]
        public bool? Importado { get; set; }

        /// <summary>
        /// Obtém ou define o código do pedido do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("codigoPedidoCliente")]
        public string CodigoPedidoCliente { get; set; }

        /// <summary>
        /// Obtém ou define o cliente externo do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("clienteExterno")]
        public IdNomeDto ClienteExterno { get; set; }

        /// <summary>
        /// Obtém ou define a rota externa do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("rotaExterna")]
        public string RotaExterna { get; set; }

        /// <summary>
        /// Obtém ou define o tipo do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoPedido")]
        public string TipoPedido { get; set; }

        /// <summary>
        /// Obtém ou define a data de entrega do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("dataEntrega")]
        public DateTime? DataEntrega { get; set; }

        /// <summary>
        /// Obtém ou define o peso total do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("peso")]
        public decimal? Peso { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade de peças pendentes.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidadePecasPendentes")]
        public int? QuantidadePecasPendentes { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade de volumes pendentes.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidadeVolumesPendentes")]
        public int? QuantidadeVolumesPendentes { get; set; }

        /// <summary>
        /// Obtém ou define a observação da liberação do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("observacaoLiberacao")]
        public string ObservacaoLiberacao { get; set; }

        /// <summary>
        /// Obtém ou define a cor da linha.
        /// </summary>
        [DataMember]
        [JsonProperty("corLinha")]
        public string CorLinha { get; set; }

        private string ObterCorLinha()
        {
            return this.QuantidadePecasPendentes > 0 ? "red" : string.Empty;
        }
    }
}