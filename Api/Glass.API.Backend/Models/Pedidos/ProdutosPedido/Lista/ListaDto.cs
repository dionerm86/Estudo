// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper;
using Glass.API.Backend.Models.Genericas;
using Glass.API.Backend.Models.Genericas.Venda;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.ProdutosPedido.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de um produto de pedido.
    /// </summary>
    [DataContract(Name = "Produto")]
    public class ListaDto : ItemDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="produtoPedido">A model de produto de pedido.</param>
        internal ListaDto(Data.Model.ProdutosPedido produtoPedido)
        {
            this.Id = (int)produtoPedido.IdProdPed;
            this.IdMaterialProjeto = (int?)produtoPedido.IdMaterItemProj;
            this.Produto = new ProdutoDto
            {
                Id = (int)produtoPedido.IdProd,
                Codigo = produtoPedido.CodInterno,
                Descricao = produtoPedido.DescrProduto,
                DescricaoComBeneficiamentos = produtoPedido.DescricaoProdutoComBenef,
                Espessura = produtoPedido.Espessura,
                Vidro = produtoPedido.IsVidro == "true",
            };

            this.Quantidade = produtoPedido.Qtde;
            this.QuantidadeAmbiente = produtoPedido.QtdeAmbiente;
            this.DescontoPorQuantidade = new DescontoQuantidadeDto
            {
                Percentual = produtoPedido.PercDescontoQtde,
                Valor = produtoPedido.ValorDescontoQtde,
            };

            this.Largura = produtoPedido.Largura;
            this.Altura = new AlturaDto
            {
                Real = produtoPedido.AlturaReal,
                ParaCalculo = produtoPedido.Altura,
                ParaExibirNaLista = produtoPedido.AlturaLista,
            };

            this.AreaEmM2 = new AreaDto
            {
                Real = produtoPedido.TotM,
                ParaCalculo = produtoPedido.TotM2Calc,
                ParaCalculoSemChapa = produtoPedido.TotalM2CalcSemChapaString,
            };

            this.ValorUnitario = produtoPedido.ValorVendido;
            this.Processo = this.ObterIdCodigo((int?)produtoPedido.IdProcesso, produtoPedido.CodProcesso);
            this.Aplicacao = this.ObterIdCodigo((int?)produtoPedido.IdAplicacao, produtoPedido.CodAplicacao);
            this.CodigoPedidoCliente = produtoPedido.PedCli;
            this.Total = produtoPedido.Total;
            this.Beneficiamentos = new BeneficiamentosDto
            {
                Valor = produtoPedido.ValorBenef,
                Altura = produtoPedido.AlturaBenef,
                Largura = produtoPedido.LarguraBenef,
                Espessura = produtoPedido.EspessuraBenef,
                Redondo = produtoPedido.Redondo,
                Itens = produtoPedido.Beneficiamentos?.ObterListaBeneficiamentos(),
            };

            this.Observacao = produtoPedido.Obs;
            this.PercentualComissao = (double)produtoPedido.PercComissao;
            this.PossuiFilhos = produtoPedido.IsProdLamComposicao;
            this.Permissoes = new PermissoesDto
            {
                Editar = produtoPedido.PodeEditar,
                Excluir = produtoPedido.DeleteVisible,
            };
        }

        /// <summary>
        /// Obtém ou define a observação do produto do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("observacao")]
        public string Observacao { get; set; }

        /// <summary>
        /// Obtém ou define as permissões do produto do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}
