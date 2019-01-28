// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Glass.Data.Model;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.V1.Exportacao.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de um pedido para a tela de listagem de pedidos para exportação.
    /// </summary>
    [DataContract(Name = "PedidoExportacao")]
    public class ListaDto : IdDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="pedido">A model de pedidos.</param>
        internal ListaDto(Data.Model.Pedido pedido)
        {
            this.Id = (int)pedido.IdPedido;
            this.IdProjeto = (int)(pedido.IdProjeto ?? 0);
            this.IdOrcamento = (int)(pedido.IdOrcamento ?? 0);
            this.Cliente = new IdNomeDto
            {
                Id = (int)pedido.IdCli,
                Nome = pedido.NomeCli,
            };

            this.Loja = pedido.NomeLoja;
            this.Funcionario = pedido.NomeFunc;
            this.Total = pedido.Total;
            this.TipoVenda = pedido.DescrTipoVenda;
            this.TipoPedido = pedido.DescricaoTipoPedido;
            this.Datas = new DatasDto
            {
                Pedido = pedido.DataPedido,
                Entrega = pedido.DataEntrega,
            };

            this.Situacoes = new SituacoesDto
            {
                Pedido = pedido.DescrSituacaoPedido,
                Exportacao = pedido.DescricaoSituacaoExportacao,
            };

            this.Permissoes = new PermissoesDto
            {
                ConsultarSituacao = pedido.SituacaoExportacao != 0,
            };
        }

        /// <summary>
        /// Obtém ou define o identificador do projeto.
        /// </summary>
        [DataMember]
        [JsonProperty("idProjeto")]
        public int? IdProjeto { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do orçamento.
        /// </summary>
        [DataMember]
        [JsonProperty("idOrcamento")]
        public int? IdOrcamento { get; set; }

        /// <summary>
        /// Obtém ou define o cliente associado ao pedido.
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
        /// Obtém ou define o funcionário associado ao pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("funcionario")]
        public string Funcionario { get; set; }

        /// <summary>
        /// Obtém ou define o valor total do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("total")]
        public decimal Total { get; set; }

        /// <summary>
        /// Obtém ou define o tipo da venda.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoVenda")]
        public string TipoVenda { get; set; }

        /// <summary>
        /// Obtém ou define o tipo do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoPedido")]
        public string TipoPedido { get; set; }

        /// <summary>
        /// Obtém ou define os dados referentes as datas.
        /// </summary>
        [DataMember]
        [JsonProperty("datas")]
        public DatasDto Datas { get; set; }

        /// <summary>
        /// Obtém ou define os dados referentes as situações.
        /// </summary>
        [DataMember]
        [JsonProperty("situacoes")]
        public SituacoesDto Situacoes { get; set; }

        /// <summary>
        /// Obtém ou define as permissões concebidas ao pedido para exportação.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}