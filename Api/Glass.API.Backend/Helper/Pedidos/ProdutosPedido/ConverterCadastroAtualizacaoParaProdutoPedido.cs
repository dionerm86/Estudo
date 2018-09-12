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
            destino.IdAmbientePedido = (uint?)this.cadastro.ObterValorNormalizado(c => c.IdAmbiente, (int?)destino.IdAmbientePedido);
            destino.IdProd = this.cadastro.Produto != null ? (uint)this.cadastro.Produto.Id : destino.IdProd;
            destino.Espessura = this.cadastro.Produto != null ? (float)this.cadastro.Produto.Espessura : destino.Espessura;
            destino.Qtde = (float)this.cadastro.ObterValorNormalizado(c => c.Quantidade, (double)destino.Qtde);
            destino.PercDescontoQtde = this.cadastro.DescontoPorQuantidade != null ? (float)this.cadastro.DescontoPorQuantidade.Percentual.GetValueOrDefault() : destino.PercDescontoQtde;
            destino.ValorDescontoQtde = this.cadastro.DescontoPorQuantidade != null ? this.cadastro.DescontoPorQuantidade.Valor.GetValueOrDefault() : destino.ValorDescontoQtde;
            destino.Largura = this.cadastro.ObterValorNormalizado(c => c.Largura, destino.Largura);
            destino.Altura = (float?)this.cadastro.Altura?.ParaCalculo ?? destino.Altura;
            destino.AlturaReal = (float?)this.cadastro.Altura?.Real ?? destino.Altura;
            destino.TotM = (float?)this.cadastro.AreaEmM2?.Real ?? destino.TotM;
            destino.TotM2Calc = (float?)this.cadastro.AreaEmM2?.ParaCalculo ?? destino.TotM2Calc;
            destino.ValorVendido = this.cadastro.ObterValorNormalizado(c => c.ValorUnitario, destino.ValorVendido);
            destino.IdProcesso = (uint?)this.cadastro.ObterValorNormalizado(c => c.IdProcesso, (int?)destino.IdProcesso);
            destino.IdAplicacao = (uint?)this.cadastro.ObterValorNormalizado(c => c.IdAplicacao, (int?)destino.IdAplicacao);
            destino.CodPedCliente = this.cadastro.ObterValorNormalizado(c => c.CodigoPedidoCliente, destino.CodPedCliente);
            destino.Total = this.cadastro.ObterValorNormalizado(c => c.Total, destino.Total);
            destino.Obs = this.cadastro.ObterValorNormalizado(c => c.Observacao, destino.Obs);
            destino.AplicarBenefComposicao = this.cadastro.Composicao != null ? this.cadastro.Composicao.AplicarBeneficiamentosProdutosFilhos.GetValueOrDefault() : destino.AplicarBenefComposicao;

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
