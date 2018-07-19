// <copyright file="ConverterCadastroAtualizacaoParaProdutoPedido.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Pedidos.ProdutosPedido.CadastroAtualizacao;
using System;

namespace Glass.API.Backend.Helper.Pedidos.ProdutosPedido
{
    /// <summary>
    /// Classe que realiza a tradução entre o DTO e a model para cadastro ou atualização de produto de pedido.
    /// </summary>
    internal class ConverterCadastroAtualizacaoParaProdutoPedido
    {
        private readonly CadastroAtualizacaoDto cadastro;
        private readonly Lazy<Data.Model.ProdutosPedido> produtoPedido;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConverterCadastroAtualizacaoParaProdutoPedido"/>.
        /// </summary>
        /// <param name="cadastro">O DTO de cadastro, enviado para o endpoint.</param>
        /// <param name="atual">O produto de pedido atual (opcional), para que sejam aproveitados os valores, se necessário.</param>
        public ConverterCadastroAtualizacaoParaProdutoPedido(CadastroAtualizacaoDto cadastro, Data.Model.ProdutosPedido atual = null)
        {
            this.cadastro = cadastro;
            this.produtoPedido = new Lazy<Data.Model.ProdutosPedido>(() =>
            {
                var destino = atual ?? new Data.Model.ProdutosPedido();
                this.ConverterDtoParaModelo(destino);

                return destino;
            });
        }

        /// <summary>
        /// Realiza a conversão para a model.
        /// </summary>
        /// <returns>A model de produto de pedido preenchida.</returns>
        public Data.Model.ProdutosPedido ConverterParaProdutoPedido()
        {
            return this.produtoPedido.Value;
        }

        private void ConverterDtoParaModelo(Data.Model.ProdutosPedido destino)
        {
            destino.IdProd = (uint?)this.cadastro.Produto?.Id ?? destino.IdProd;
            destino.Espessura = (float?)this.cadastro.Produto?.Espessura ?? destino.Espessura;
            destino.Qtde = (float?)this.cadastro.Quantidade ?? destino.Qtde;
            destino.PercDescontoQtde = (float?)this.cadastro.DescontoPorQuantidade?.Percentual ?? destino.PercDescontoQtde;
            destino.ValorDescontoQtde = this.cadastro.DescontoPorQuantidade?.Valor ?? destino.ValorDescontoQtde;
            destino.Largura = this.cadastro.Largura ?? destino.Largura;
            destino.Altura = (float?)this.cadastro.Altura?.ParaCalculo ?? destino.Altura;
            destino.AlturaReal = (float?)this.cadastro.Altura?.Real ?? destino.Altura;
            destino.TotM = (float?)this.cadastro.AreaEmM2?.Real ?? destino.TotM;
            destino.TotM2Calc = (float?)this.cadastro.AreaEmM2?.ParaCalculo ?? destino.TotM2Calc;
            destino.ValorVendido = this.cadastro.ValorUnitario ?? destino.ValorVendido;
            destino.IdProcesso = this.cadastro.Processo != null
                ? (uint?)this.cadastro.Processo.Id
                : destino.IdProcesso;

            destino.IdAplicacao = this.cadastro.Aplicacao != null
                ? (uint?)this.cadastro.Aplicacao.Id
                : destino.IdAplicacao;

            destino.CodPedCliente = this.cadastro.CodigoPedidoCliente ?? destino.CodPedCliente;
            destino.Total = this.cadastro.Total ?? destino.Total;
            destino.Obs = this.cadastro.Observacao ?? destino.Obs;

            this.ConverterBeneficiamentos(destino);
        }

        private void ConverterBeneficiamentos(Data.Model.ProdutosPedido destino)
        {
            var itens = this.cadastro.Beneficiamentos?.Itens
                ?? new Models.Genericas.ItemBeneficiamentoDto[0];

            destino.ValorBenef = this.cadastro.Beneficiamentos?.Valor ?? destino.ValorBenef;
            destino.AlturaBenef = this.cadastro.Beneficiamentos?.Altura ?? destino.AlturaBenef;
            destino.LarguraBenef = this.cadastro.Beneficiamentos?.Largura ?? destino.LarguraBenef;
            destino.Redondo = this.cadastro.Beneficiamentos?.Redondo ?? destino.Redondo;
            destino.Beneficiamentos = itens.ObterListaBeneficiamentos();
        }
    }
}
