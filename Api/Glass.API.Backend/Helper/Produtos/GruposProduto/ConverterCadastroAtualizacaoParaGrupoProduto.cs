// <copyright file="ConverterCadastroAtualizacaoParaGrupoProduto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Produtos.GruposProduto.CadastroAtualizacao;
using Glass.Global.Negocios.Entidades;
using System;
using System.Linq;

namespace Glass.API.Backend.Helper.Produtos.GruposProduto
{
    /// <summary>
    /// Classe que realiza a tradução entre o DTO e a model para cadastro ou atualização de cor de vidro.
    /// </summary>
    internal class ConverterCadastroAtualizacaoParaGrupoProduto
    {
        private readonly CadastroAtualizacaoDto cadastro;
        private readonly Lazy<GrupoProd> grupoProd;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConverterCadastroAtualizacaoParaGrupoProduto"/>.
        /// </summary>
        /// <param name="cadastro">O DTO de cadastro, enviado para o endpoint.</param>
        /// <param name="atual">O grupo de produto atual (opcional), para que sejam aproveitados os valores, se necessário.</param>
        public ConverterCadastroAtualizacaoParaGrupoProduto(
            CadastroAtualizacaoDto cadastro,
            GrupoProd atual = null)
        {
            this.cadastro = cadastro;
            this.grupoProd = new Lazy<GrupoProd>(() =>
            {
                var destino = atual ?? new GrupoProd();
                this.ConverterDtoParaModelo(destino);

                return destino;
            });
        }

        /// <summary>
        /// Realiza a conversão para a model.
        /// </summary>
        /// <returns>A model de grupo de produto preenchida.</returns>
        public GrupoProd ConverterParaGrupoProduto()
        {
            return this.grupoProd.Value;
        }

        private void ConverterDtoParaModelo(GrupoProd destino)
        {
            destino.Descricao = this.cadastro.ObterValorNormalizado(c => c.Nome, destino.Descricao);
            destino.TipoGrupo = this.cadastro.ObterValorNormalizado(c => c.Tipo, destino.TipoGrupo);
            destino.TipoCalculo = this.cadastro.ObterValorNormalizado(c => c.TipoCalculoPedido, destino.TipoCalculo);
            destino.TipoCalculoNf = this.cadastro.ObterValorNormalizado(c => c.TipoCalculoNotaFiscal, destino.TipoCalculoNf);
            destino.BloquearEstoque = this.cadastro.ObterValorNormalizado(c => c.BloquearEstoque, destino.BloquearEstoque);
            destino.AlterarEstoque = this.cadastro.ObterValorNormalizado(c => c.AlterarEstoque, destino.AlterarEstoque);
            destino.AlterarEstoqueFiscal = this.cadastro.ObterValorNormalizado(c => c.AlterarEstoqueFiscal, destino.AlterarEstoqueFiscal);
            destino.ExibirMensagemEstoque = this.cadastro.ObterValorNormalizado(c => c.ExibirMensagemEstoque, destino.ExibirMensagemEstoque);
            destino.GeraVolume = this.cadastro.ObterValorNormalizado(c => c.GeraVolume, destino.GeraVolume);
        }
    }
}
