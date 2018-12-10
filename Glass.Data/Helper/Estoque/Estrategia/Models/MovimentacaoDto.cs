// <copyright file="MovimentacaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Data.Model;
using System;

namespace Glass.Data.Helper.Estoque.Estrategia.Models
{
    /// <summary>
    /// Classe que encapsula comunicação com as estratégias de estoque.
    /// </summary>
    public class MovimentacaoDto
    {
        /// <summary>
        /// Obtém ou define o identificador do produto.
        /// </summary>
        public uint IdProd { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da loja.
        /// </summary>
        public uint IdLoja { get; set; }

        /// <summary>
        /// Obtém ou define o tipo da movimentação.
        /// </summary>
        public MovEstoque.TipoMovEnum TipoMov { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do pedido.
        /// </summary>
        public uint? IdPedido { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da compra.
        /// </summary>
        public uint? IdCompra { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da liberação de pedido.
        /// </summary>
        public uint? IdLiberarPedido { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do produto de produção.
        /// </summary>
        public uint? IdProdPedProducao { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da troca/devolução.
        /// </summary>
        public uint? IdTrocaDevolucao { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da nota fiscal.
        /// </summary>
        public uint? IdNf { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do pedido interno.
        /// </summary>
        public uint? IdPedidoInterno { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do produto do pedido.
        /// </summary>
        public uint? IdProdPed { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do produto de compra.
        /// </summary>
        public uint? IdProdCompra { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do item da liberação de pedido.
        /// </summary>
        public uint? IdProdLiberarPedido { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do produto devolvido na troca/devolução.
        /// </summary>
        public uint? IdProdTrocaDev { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do produto trocado.
        /// </summary>
        public uint? IdProdTrocado { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do produto da nota fiscal.
        /// </summary>
        public uint? IdProdNf { get; set; }

        /// <summary>
        /// Obtém ou define o produto do pedido interno.
        /// </summary>
        public uint? IdProdPedInterno { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do retalho.
        /// </summary>
        public uint? IdRetalhoProducao { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da perda da chapa de vidro.
        /// </summary>
        public uint? IdPerdaChapaVidro { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do carregamento.
        /// </summary>
        public uint? IdCarregamento { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do volume.
        /// </summary>
        public uint? IdVolume { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do inventário de estoque.
        /// </summary>
        public uint? IdInventarioEstoque { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do produto de impressão (chapa).
        /// </summary>
        public uint? IdProdImpressaoChapa { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o lançamento de estoque é manual.
        /// </summary>
        public bool LancManual { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade de movimentação que será feita no estoque.
        /// </summary>
        public decimal QtdeMov { get; set; }

        /// <summary>
        /// Obtém ou define o valor total da movimentação.
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será alterada a matéria prima do produto ao invés dele mesmo.
        /// </summary>
        public bool AlterarMateriaPrima { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se deverá ser baixado o próprio produto caso o mesmo não tenha matéria prima.
        /// </summary>
        public bool BaixarMesmoProdutoSemMateriaPrima { get; set; }

        /// <summary>
        /// Obtém ou define a data da movimentação no estoque.
        /// </summary>
        public DateTime DataMov { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se deverá alterar também o estoque do produto base do produto informado.
        /// </summary>
        public bool AlterarProdBase { get; set; }

        /// <summary>
        /// Obtém ou define o usuário que está realizando a movimentação no estoque.
        /// </summary>
        public LoginUsuario Usuario { get; set; }

        /// <summary>
        /// Obtém ou define a observação da movimentação.
        /// </summary>
        public string Observacao { get; set; }
    }
}
