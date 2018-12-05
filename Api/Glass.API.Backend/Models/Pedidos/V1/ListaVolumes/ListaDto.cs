// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.V1.ListaVolumes
{
    /// <summary>
    /// Classe que encapsula os dados de um pedido para a tela de listagem.
    /// </summary>
    [DataContract(Name = "Pedido")]
    public class ListaDto : IdDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="pedido">A model de pedidos.</param>
        internal ListaDto(Data.Model.Pedido pedido)
        {
            this.Id = (int)pedido.IdPedido;
            this.Cliente = new IdNomeDto
            {
                Id = (int)pedido.IdCli,
                Nome = pedido.NomeCli,
            };

            this.Importado = pedido.Importado;
            this.Loja = pedido.NomeLoja;
            this.Funcionario = pedido.NomeFunc;
            this.DataEntrega = pedido.DataEntrega;
            this.DataEntregaOriginal = pedido.DataEntregaOriginal;
            this.Rota = pedido.CodRota;
            this.QuantidadePecasPedido = (int)pedido.QtdePecas;
            this.PedidoExterno = new PedidoExternoDto
            {
                Id = (int?)pedido.IdPedidoExterno,
                Rota = pedido.RotaExterna,
                Cliente = new IdNomeDto
                {
                    Id = (int)pedido.IdClienteExterno,
                    Nome = pedido.ClienteExterno,
                },
            };

            this.DadosVolume = new DadosVolumeDto
            {
                QuantidadePecas = (decimal)pedido.QtdePecasVolume,
                QuantidadePecasPendentes = (decimal)pedido.QtdePecasPendenteVolume,
                MetroQuadrado = (decimal)pedido.TotMVolume,
                Peso = (decimal)pedido.PesoVolume,
                Situacao = Colosoft.Translator.Translate(pedido.SituacaoVolume).Format(),
            };

            this.CorLinha = this.ObterCorLinha(pedido.SituacaoVolume);
            this.Permissoes = new PermissoesDto
            {
                GerarVolume = pedido.GerarVolumeVisible,
                ExibirRelatorioVolume = pedido.RelatorioVolumeVisible,
            };
        }

        /// <summary>
        /// Obtém ou define o cliente do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("cliente")]
        public IdNomeDto Cliente { get; set; }

        /// <summary>
        /// Obtém ou define a loja do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("loja")]
        public string Loja { get; set; }

        /// <summary>
        /// Obtém ou define o funcionário do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("funcionario")]
        public string Funcionario { get; set; }

        /// <summary>
        /// Obtém ou define a data de entrega do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("dataEntrega")]
        public DateTime? DataEntrega { get; set; }

        /// <summary>
        /// Obtém ou define a data de entrega original do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("dataEntregaOriginal")]
        public DateTime? DataEntregaOriginal { get; set; }

        /// <summary>
        /// Obtém ou define a rota do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("rota")]
        public string Rota { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade de peças do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidadePecasPedido")]
        public int QuantidadePecasPedido { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o pedido é de importação.
        /// </summary>
        [DataMember]
        [JsonProperty("importado")]
        public bool Importado { get; set; }

        /// <summary>
        /// Obtém ou define dados do pedido externo.
        /// </summary>
        [DataMember]
        [JsonProperty("pedidoExterno")]
        public PedidoExternoDto PedidoExterno { get; set; }

        /// <summary>
        /// Obtém ou define dados do volume.
        /// </summary>
        [DataMember]
        [JsonProperty("dadosVolume")]
        public DadosVolumeDto DadosVolume { get; set; }

        /// <summary>
        /// Obtém ou define a cor da linha do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("corLinha")]
        public string CorLinha { get; set; }

        /// <summary>
        /// Obtém ou define permissões do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }

        private string ObterCorLinha(Data.Model.Pedido.SituacaoVolumeEnum situacao)
        {
            switch (situacao)
            {
                case Data.Model.Pedido.SituacaoVolumeEnum.SemVolume:
                    return "Red";

                case Data.Model.Pedido.SituacaoVolumeEnum.Pendente:
                    return "Blue";

                default:
                    return "Black";
            }
        }
    }
}
