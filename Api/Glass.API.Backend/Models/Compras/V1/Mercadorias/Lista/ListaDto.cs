// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.Model;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Compras.V1.Mercadorias.Lista
{
    /// <summary>
    /// Classe que encapsula um item para a lista de compras de mercadorias.
    /// </summary>
    [DataContract (Name = "Lista")]
    public class ListaDto : IdDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="compraMercadoria">A compra de mercadoria que será retornada.</param>
        public ListaDto(Compra compraMercadoria)
        {
            this.Id = (int)compraMercadoria.IdCompra;
            this.IdPedido = (int)(compraMercadoria.IdPedidoEspelho ?? 0);
            this.Fornecedor = compraMercadoria.IdNomeFornec;
            this.Loja = compraMercadoria.NomeLoja;
            this.UsuarioCadastro = compraMercadoria.DescrUsuCad;
            this.TipoCompra = compraMercadoria.DescrTipoCompra;
            this.Total = compraMercadoria.Total;
            this.DataCadastro = compraMercadoria.DataCad;
            this.Situacao = compraMercadoria.DescrSituacao;
            this.Contabil = compraMercadoria.Contabil;
            this.EstoqueCreditado = compraMercadoria.EstoqueBaixado;

            this.Permissoes = new PermissoesDto
            {
                Editar = compraMercadoria.EditVisible,
                Cancelar = compraMercadoria.CancelVisible,
                GerenciarFotos = compraMercadoria.FotosVisible,
                Reabrir = compraMercadoria.ReabrirVisible,
                GerarNotaFiscal = compraMercadoria.GerarNFeVisible,
                ExibirProdutoChegou = compraMercadoria.ProdutoChegouVisible,
            };
        }

        /// <summary>
        /// Obtém ou define o identificador do pedido para a lista de compras de mercadorias.
        /// </summary>
        [DataMember]
        [JsonProperty("idPedido")]
        public int? IdPedido { get; set; }

        /// <summary>
        /// Obtém ou define o fornecedor para a lista de compras de mercadorias.
        /// </summary>
        [DataMember]
        [JsonProperty("fornecedor")]
        public string Fornecedor { get; set; }

        /// <summary>
        /// Obtém ou define a loja para a lista de compras de mercadorias.
        /// </summary>
        [DataMember]
        [JsonProperty("loja")]
        public string Loja { get; set; }

        /// <summary>
        /// Obtém ou define o usuário de cadastro para a lista de compras de mercadorias.
        /// </summary>
        [DataMember]
        [JsonProperty("usuarioCadastro")]
        public string UsuarioCadastro { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de compras de mercadoria.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoCompra")]
        public string TipoCompra { get; set; }

        /// <summary>
        /// Obtém ou define o total da compra de mercadoria.
        /// </summary>
        [DataMember]
        [JsonProperty("total")]
        public decimal Total { get; set; }

        /// <summary>
        /// Obtém ou define a data do cadastro da compra de mercadoria.
        /// </summary>
        [DataMember]
        [JsonProperty("dataCadastro")]
        public DateTime? DataCadastro { get; set; }

        /// <summary>
        /// Obtém ou define a situação da compra de mercadoria.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public string Situacao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a compra de mercadotia é contábil.
        /// </summary>
        [DataMember]
        [JsonProperty("contabil")]
        public bool Contabil { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o estoque da compra de mercadoria foi creditado.
        /// </summary>
        [DataMember]
        [JsonProperty("estoqueCreditado")]
        public bool EstoqueCreditado { get; set; }

        /// <summary>
        /// Obtém ou define as permissões para a tela de listagem de compras de mercadorias.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}
