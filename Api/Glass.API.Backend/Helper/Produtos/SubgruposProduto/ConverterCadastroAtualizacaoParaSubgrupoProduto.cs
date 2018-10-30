// <copyright file="ConverterCadastroAtualizacaoParaSubgrupoProduto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Produtos.V1.SubgruposProduto.CadastroAtualizacao;
using Glass.Global.Negocios.Entidades;
using System;
using System.Linq;

namespace Glass.API.Backend.Helper.Produtos.SubgruposProduto
{
    /// <summary>
    /// Classe que realiza a tradução entre o DTO e a model para cadastro ou atualização de cor de vidro.
    /// </summary>
    internal class ConverterCadastroAtualizacaoParaSubgrupoProduto
    {
        private readonly CadastroAtualizacaoDto cadastro;
        private readonly Lazy<SubgrupoProd> subgrupoProd;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConverterCadastroAtualizacaoParaSubgrupoProduto"/>.
        /// </summary>
        /// <param name="cadastro">O DTO de cadastro, enviado para o endpoint.</param>
        /// <param name="atual">O subgrupo de produto atual (opcional), para que sejam aproveitados os valores, se necessário.</param>
        public ConverterCadastroAtualizacaoParaSubgrupoProduto(
            CadastroAtualizacaoDto cadastro,
            SubgrupoProd atual = null)
        {
            this.cadastro = cadastro;
            this.subgrupoProd = new Lazy<SubgrupoProd>(() =>
            {
                var destino = atual ?? new SubgrupoProd();
                this.ConverterDtoParaModelo(destino);

                return destino;
            });
        }

        /// <summary>
        /// Realiza a conversão para a model.
        /// </summary>
        /// <returns>A model de subgrupo de produto preenchida.</returns>
        public SubgrupoProd ConverterParaSubgrupoProduto()
        {
            return this.subgrupoProd.Value;
        }

        private void ConverterDtoParaModelo(SubgrupoProd destino)
        {
            destino.IdGrupoProd = this.cadastro.ObterValorNormalizado(c => c.IdGrupoProduto, destino.IdGrupoProd);
            destino.IdCli = this.cadastro.ObterValorNormalizado(c => c.IdCliente, destino.IdCli);
            destino.Descricao = this.cadastro.ObterValorNormalizado(c => c.Nome, destino.Descricao);
            destino.TipoSubgrupo = this.cadastro.ObterValorNormalizado(c => c.Tipo, destino.TipoSubgrupo);
            destino.TipoCalculo = this.cadastro.ObterValorNormalizado(c => c.TipoCalculoPedido, destino.TipoCalculo);
            destino.TipoCalculoNf = this.cadastro.ObterValorNormalizado(c => c.TipoCalculoNotaFiscal, destino.TipoCalculoNf);
            destino.ProdutosEstoque = this.cadastro.ObterValorNormalizado(c => c.ProdutoParaEstoque, destino.ProdutosEstoque);
            destino.IsVidroTemperado = this.cadastro.ObterValorNormalizado(c => c.VidroTemperado, destino.IsVidroTemperado);
            destino.BloquearEstoque = this.cadastro.ObterValorNormalizado(c => c.BloquearEstoque, destino.BloquearEstoque);
            destino.AlterarEstoque = this.cadastro.ObterValorNormalizado(c => c.AlterarEstoque, destino.AlterarEstoque);
            destino.AlterarEstoqueFiscal = this.cadastro.ObterValorNormalizado(c => c.AlterarEstoqueFiscal, destino.AlterarEstoqueFiscal);
            destino.ExibirMensagemEstoque = this.cadastro.ObterValorNormalizado(c => c.ExibirMensagemEstoque, destino.ExibirMensagemEstoque);
            destino.GeraVolume = this.cadastro.ObterValorNormalizado(c => c.GeraVolume, destino.GeraVolume);
            destino.BloquearEcommerce = this.cadastro.ObterValorNormalizado(c => c.BloquearVendaECommerce, destino.BloquearEcommerce);
            destino.NumeroDiasMinimoEntrega = this.cadastro.ObterValorNormalizado(c => c.DiasMinimoEntrega, destino.NumeroDiasMinimoEntrega);
            destino.DiaSemanaEntrega = this.cadastro.ObterValorNormalizado(c => c.DiaSemanaEntrega, destino.DiaSemanaEntrega);
            destino.LiberarPendenteProducao = this.cadastro.ObterValorNormalizado(c => c.LiberarPendenteProducao, destino.LiberarPendenteProducao);
            destino.PermitirItemRevendaNaVenda = this.cadastro.ObterValorNormalizado(c => c.PermitirItemRevendaNaVenda, destino.PermitirItemRevendaNaVenda);

            var idsLojaAssociacao = this.cadastro.ObterValorNormalizado(c => c.IdsLojasAssociadas, destino.IdsLojaAssociacao);
            destino.IdsLojaAssociacao = idsLojaAssociacao != null ? idsLojaAssociacao.ToArray() : null;
        }
    }
}
