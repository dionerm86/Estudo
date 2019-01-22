// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

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
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="mercadoria">A mercadoria que será retornada.</param>
        public ListaDto(Compra mercadoria)
        {
            this.Id = (int)mercadoria.IdCompra;
            this.IdPedido = (int?)mercadoria.IdPedidoEspelho;
            this.Fornecedor = mercadoria.IdNomeFornec;
            this.Loja = mercadoria.NomeLoja;
            this.UsuarioCadastro = mercadoria.DescrUsuCad;
            this.TipoCompra = mercadoria.DescrTipoCompra;
            this.Total = mercadoria.Total;
            this.DataCadastro = mercadoria.DataCad;
            this.Situacao = mercadoria.DescrSituacao;
            this.Contabil = mercadoria.Contabil;
            this.EstoqueCreditado = mercadoria.EstoqueBaixado;

            this.Permissoes = new PermissoesDto
            {
                Editar = mercadoria.EditVisible,
                Cancelar = mercadoria.CancelVisible,
                GerenciarFotos = mercadoria.FotosVisible,
                Reabrir = mercadoria.ReabrirVisible,
                GerarNotaFiscal = mercadoria.GerarNFeVisible,
                ExibirProdutoChegou = mercadoria.ProdutoChegouVisible,
            };
        }

        /// <summary>
        /// Obtém ou define o identificador da compra.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("idPedido")]
        public int? IdPedido { get; set; }

        /// <summary>
        /// Obtém ou define o fornecedor.
        /// </summary>
        [DataMember]
        [JsonProperty("fornecedor")]
        public string Fornecedor { get; set; }

        /// <summary>
        /// Obtém ou define a loja.
        /// </summary>
        [DataMember]
        [JsonProperty("loja")]
        public string Loja { get; set; }

        /// <summary>
        /// Obtém ou define o usuário de cadastro.
        /// </summary>
        [DataMember]
        [JsonProperty("usuarioCadastro")]
        public string UsuarioCadastro { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de compra.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoCompra")]
        public string TipoCompra { get; set; }

        /// <summary>
        /// Obtém ou define o total.
        /// </summary>
        [DataMember]
        [JsonProperty("total")]
        public decimal Total { get; set; }

        /// <summary>
        /// Obtém ou define a data do cadastro.
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
        /// Obtém ou define um valor que indica se é contábil.
        /// </summary>
        [DataMember]
        [JsonProperty("contabil")]
        public bool Contabil { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o estoque pode ser creditado.
        /// </summary>
        [DataMember]
        [JsonProperty("estoqueCreditado")]
        public bool EstoqueCreditado { get; set; }

        /// <summary>
        /// Obtém ou define as permissões.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}
