// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Configuracoes;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.V1.SubgruposProduto.Configuracoes
{
    /// <summary>
    /// Classe que encapsula as configurações para a tela de listagem de subgrupos de produtos.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        internal ListaDto()
        {
            this.DescricaoMlal = Global.CalculosFluxo.NOME_MLAL;
            this.UsarControleOrdemDeCarga = OrdemCargaConfig.UsarControleOrdemCarga;
            this.BloquearItensVendaRevendaPedido = PedidoConfig.DadosPedido.BloquearItensTipoPedido;
            this.IdGrupoVidro = (int)Glass.Data.Model.NomeGrupoProd.Vidro;
            this.TipoCalculoQuantidade = Data.Model.TipoCalculoGrupoProd.Qtd;
            this.TipoCalculoMetroQuadrado = Data.Model.TipoCalculoGrupoProd.M2;
            this.SituacaoAtiva = Situacao.Ativo;
            this.SituacaoInativa = Situacao.Inativo;
        }

        /// <summary>
        /// Obtém ou define a descrição do MLAL.
        /// </summary>
        [DataMember]
        [JsonProperty("descricaoMlal")]
        public string DescricaoMlal { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a empresa trabalha com ordem de carga.
        /// </summary>
        [DataMember]
        [JsonProperty("usarControleOrdemDeCarga")]
        public bool UsarControleOrdemDeCarga { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a empresa trabalha com separação de pedidos de venda/revenda.
        /// </summary>
        [DataMember]
        [JsonProperty("bloquearItensVendaRevendaPedido")]
        public bool BloquearItensVendaRevendaPedido { get; set; }

        /// <summary>
        /// Obtém ou define o valor do identificador do grupo vidro.
        /// </summary>
        [DataMember]
        [JsonProperty("idGrupoVidro")]
        public int IdGrupoVidro { get; set; }

        /// <summary>
        /// Obtém ou define o valor do tipo cálculo "quantidade".
        /// </summary>
        [DataMember]
        [JsonProperty("tipoCalculoQuantidade")]
        public Data.Model.TipoCalculoGrupoProd TipoCalculoQuantidade { get; set; }

        /// <summary>
        /// Obtém ou define o valor do tipo cálculo "metro quadrado".
        /// </summary>
        [DataMember]
        [JsonProperty("tipoCalculoMetroQuadrado")]
        public Data.Model.TipoCalculoGrupoProd TipoCalculoMetroQuadrado { get; set; }

        /// <summary>
        /// Obtém ou define o valor da situação "ativa" no sistema.
        /// </summary>
        [DataMember]
        [JsonProperty("situacaoAtiva")]
        public Situacao SituacaoAtiva { get; set; }

        /// <summary>
        /// Obtém ou define o valor da situação "inativa" no sistema.
        /// </summary>
        [DataMember]
        [JsonProperty("situacaoInativa")]
        public Situacao SituacaoInativa { get; set; }
    }
}
