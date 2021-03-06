﻿// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.API.Backend.Models.Genericas.V1.Venda;
using Glass.Data.DAL;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.V1.ProdutosPedido.Lista
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
        /// <param name="sessao">A transação com o banco de dados.</param>
        /// <param name="produtoPedido">A model de produto de pedido.</param>
        internal ListaDto(GDASession sessao, Data.Model.ProdutosPedido produtoPedido)
        {
            this.Id = (int)produtoPedido.IdProdPed;
            this.IdMaterialProjeto = (int?)produtoPedido.IdMaterItemProj;
            this.Produto = new ProdutoDto
            {
                Id = (int)produtoPedido.IdProd,
                Codigo = produtoPedido.CodInterno,
                Descricao = produtoPedido.DescrProduto,
                DescricaoComBeneficiamentos = produtoPedido.DescricaoProdutoComBenef,
                Espessura = (decimal)produtoPedido.Espessura,
                Vidro = produtoPedido.IsVidro == "true",
            };

            this.Quantidade = (decimal)produtoPedido.Qtde;
            this.QuantidadeAmbiente = produtoPedido.QtdeAmbiente;
            this.DescontoPorQuantidade = new DescontoQuantidadeDto
            {
                Percentual = (decimal)produtoPedido.PercDescontoQtde,
                Valor = produtoPedido.ValorDescontoQtde,
            };

            this.Largura = produtoPedido.Largura;
            this.Altura = new AlturaDto
            {
                Real = (decimal)produtoPedido.AlturaReal,
                ParaCalculo = (decimal)produtoPedido.Altura,
                ParaExibirNaLista = produtoPedido.AlturaLista,
            };

            this.AreaEmM2 = new AreaDto
            {
                Real = (decimal)produtoPedido.TotM,
                ParaCalculo = (decimal)produtoPedido.TotM2Calc,
                ParaCalculoSemChapa = produtoPedido.TotalM2CalcSemChapaString,
            };

            this.ValorUnitario = produtoPedido.ValorVendido;
            this.Processo = IdCodigoDto.TentarConverter((int?)produtoPedido.IdProcesso, produtoPedido.CodProcesso);
            this.Aplicacao = IdCodigoDto.TentarConverter((int?)produtoPedido.IdAplicacao, produtoPedido.CodAplicacao);
            this.CodigoPedidoCliente = produtoPedido.PedCli;
            this.Total = produtoPedido.Total;
            this.Beneficiamentos = new BeneficiamentosDto
            {
                Valor = produtoPedido.ValorBenef,
                Altura = produtoPedido.AlturaBenef,
                Largura = produtoPedido.LarguraBenef,
                Espessura = (decimal?)produtoPedido.EspessuraBenef,
                Redondo = produtoPedido.Redondo,
                Itens = produtoPedido.Beneficiamentos?.ObterListaBeneficiamentos(),
            };

            this.Observacao = produtoPedido.Obs;
            this.PercentualComissao = (decimal)produtoPedido.PercComissao;
            this.Composicao = new ComposicaoDto
            {
                PossuiFilhos = produtoPedido.IsProdLamComposicao,
                PermitirInserirFilhos = produtoPedido.IsProdLamComposicao
                    && !ProdutosPedidoDAO.Instance.IsProdLaminado(produtoPedido.IdProdPed),
            };

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
