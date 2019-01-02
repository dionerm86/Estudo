// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Configuracoes;
using Glass.Data.DAL;
using Glass.Data.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Compras.V1.Lista
{
    /// <summary>
    /// Classe que encapsula um item para a lista de compras.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto : IdDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="compra">A compra que será retornada.</param>
        public ListaDto(Compra compra)
        {
            this.Id = (int)compra.IdCompra;
            this.IdCotacaoCompra = (int)(compra.IdCotacaoCompra ?? 0);
            this.IdsPedido = compra.IdsPedido != null ? compra.IdsPedido.Split(',').Select(int.Parse).ToArray() : null;
            this.IdPedidoEspelho = (int)(compra.IdPedidoEspelho ?? 0);
            this.Fornecedor = new IdNomeDto
            {
                Id = (int)(compra.IdFornec ?? 0),
                Nome = compra.NomeFornec,
            };

            this.Loja = compra.NomeLoja;
            this.UsuarioCadastro = compra.DescrUsuCad;
            this.Total = compra.Total;
            this.Datas = new DatasDto
            {
                Cadastro = compra.DataCad,
                Fabrica = compra.DataFabrica,
            };

            this.Tipo = compra.DescrTipoCompra;
            this.Situacao = compra.DescrSituacao;
            this.Contabil = compra.Contabil;
            this.NumeroNotaFiscal = compra.NumeroNfe;
            this.Observacao = compra.Obs;
            this.EstoqueCreditado = compra.EstoqueBaixado;
            this.CentroCustoCompleto = compra.CentroCustoCompleto;
            this.Permissoes = new PermissoesDto
            {
                Editar = compra.EditVisible,
                Cancelar = compra.CancelVisible,
                GerenciarFotos = compra.FotosVisible,
                GerarNotaFiscal = compra.GerarNFeVisible,
                Reabrir = compra.ReabrirVisible,
                ExibirNotasFiscaisGeradas = compra.ExibirNfeGerada,
                ExibirLinkProdutoChegou = compra.ProdutoChegouVisible,
                ExibirFinalizacaoEntrega = compra.FinalizarAguardandoEntregaVisible,
                ExibirCentroCusto = compra.ExibirCentroCusto,
                LogAlteracoes = LogAlteracaoDAO.Instance.TemRegistro(LogAlteracao.TabelaAlteracao.Compra, compra.IdCompra, null),
            };
        }

        /// <summary>
        /// Obtém ou define o identificador da cotação.
        /// </summary>
        [DataMember]
        [JsonProperty("idCotacaoCompra")]
        public int? IdCotacaoCompra { get; set; }

        /// <summary>
        /// Obtém ou define os identificadores dos pedidos.
        /// </summary>
        [DataMember]
        [JsonProperty("idsPedido")]
        public IEnumerable<int> IdsPedido { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do pedido espelho.
        /// </summary>
        [DataMember]
        [JsonProperty("idPedidoEspelho")]
        public int? IdPedidoEspelho { get; set; }

        /// <summary>
        /// Obtém ou define os dados do fornecedor.
        /// </summary>
        [DataMember]
        [JsonProperty("fornecedor")]
        public IdNomeDto Fornecedor { get; set; }

        /// <summary>
        /// Obtém ou define o nome da loja.
        /// </summary>
        [DataMember]
        [JsonProperty("loja")]
        public string Loja { get; set; }

        /// <summary>
        /// Obtém ou define o nome do usuário que cadastrou a compra.
        /// </summary>
        [DataMember]
        [JsonProperty("usuarioCadastro")]
        public string UsuarioCadastro { get; set; }

        /// <summary>
        /// Obtém ou define o valor total da compra.
        /// </summary>
        [DataMember]
        [JsonProperty("total")]
        public decimal Total { get; set; }

        /// <summary>
        /// Obtém ou define os dados referentes as datas.
        /// </summary>
        [DataMember]
        [JsonProperty("datas")]
        public DatasDto Datas { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de pagamento.
        /// </summary>
        [DataMember]
        [JsonProperty("tipo")]
        public string Tipo { get; set; }

        /// <summary>
        /// Obtém ou define a situação da compra.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public string Situacao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a compra possui algum vinculo contabil.
        /// </summary>
        [DataMember]
        [JsonProperty("contabil")]
        public bool? Contabil { get; set; }

        /// <summary>
        /// Obtém ou define o número da nota fiscal associada a compra.
        /// </summary>
        [DataMember]
        [JsonProperty("numeroNotaFiscal")]
        public string NumeroNotaFiscal { get; set; }

        /// <summary>
        /// Obtém ou define a observação da compra.
        /// </summary>
        [DataMember]
        [JsonProperty("observacao")]
        public string Observacao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o estoque foi creditado pela compra.
        /// </summary>
        [DataMember]
        [JsonProperty("estoqueCreditado")]
        public bool? EstoqueCreditado { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se centro de custo da compra se encontra completo.
        /// </summary>
        [DataMember]
        [JsonProperty("centroCustoCompleto")]
        public bool? CentroCustoCompleto { get; set; }

        /// <summary>
        /// Obtém ou define as permissões utilizadas na tela de listagem de compras.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}