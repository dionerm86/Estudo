// <copyright file="ConverterCadastroAtualizacaoParaCategoriaConta.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.PlanosConta.V1.CategoriasConta.CadastroAtualizacao;
using Glass.Financeiro.Negocios.Entidades;
using System;

namespace Glass.API.Backend.Helper.PlanosConta.CategoriasConta
{
    /// <summary>
    /// Classe que realiza a tradução entre o DTO e a model para cadastro ou atualização de categorias de conta.
    /// </summary>
    internal class ConverterCadastroAtualizacaoParaCategoriaConta
    {
        private readonly CadastroAtualizacaoDto cadastro;
        private readonly Lazy<CategoriaConta> categoriaConta;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConverterCadastroAtualizacaoParaCategoriaConta"/>.
        /// </summary>
        /// <param name="cadastro">O DTO de cadastro, enviado para o endpoint.</param>
        /// <param name="atual">A categoria de conta atual (opcional), para que sejam aproveitados os valores, se necessário.</param>
        public ConverterCadastroAtualizacaoParaCategoriaConta(
            CadastroAtualizacaoDto cadastro,
            CategoriaConta atual = null)
        {
            this.cadastro = cadastro;
            this.categoriaConta = new Lazy<CategoriaConta>(() =>
            {
                var destino = atual ?? new CategoriaConta();
                this.ConverterDtoParaModelo(destino);

                return destino;
            });
        }

        /// <summary>
        /// Realiza a conversão para a model.
        /// </summary>
        /// <returns>A model de categoria de conta preenchida.</returns>
        public CategoriaConta ConverterParaCategoriaConta()
        {
            return this.categoriaConta.Value;
        }

        private void ConverterDtoParaModelo(CategoriaConta destino)
        {
            destino.Descricao = this.cadastro.ObterValorNormalizado(c => c.Nome, destino.Descricao);
            destino.Tipo = this.cadastro.ObterValorNormalizado(c => c.Tipo, destino.Tipo);
            destino.Situacao = this.cadastro.ObterValorNormalizado(c => c.Situacao, destino.Situacao);
        }
    }
}
