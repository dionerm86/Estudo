// <copyright file="ConverterCadastroAtualizacaoParaEstoqueReal.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Estoques.CadastroAtualizacao;
using System;

namespace Glass.API.Backend.Helper.Estoques
{
    /// <summary>
    /// Classe que realiza a tradução entre o DTO e a model para cadastro ou atualização de estoque real.
    /// </summary>
    internal class ConverterCadastroAtualizacaoParaEstoqueReal
    {
        private readonly CadastroAtualizacaoRealDto cadastro;
        private readonly Lazy<Data.Model.ProdutoLoja> estoque;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConverterCadastroAtualizacaoParaEstoqueReal"/>.
        /// </summary>
        /// <param name="cadastro">O DTO de cadastro, enviado para o endpoint.</param>
        /// <param name="atual">O estoque do produto atual (opcional), para que sejam aproveitados os valores, se necessário.</param>
        public ConverterCadastroAtualizacaoParaEstoqueReal(
            CadastroAtualizacaoRealDto cadastro,
            Data.Model.ProdutoLoja atual = null)
        {
            this.cadastro = cadastro;
            this.estoque = new Lazy<Data.Model.ProdutoLoja>(() =>
            {
                var destino = atual ?? new Data.Model.ProdutoLoja();
                this.ConverterDtoParaModelo(destino);

                return destino;
            });
        }

        /// <summary>
        /// Realiza a conversão para a model.
        /// </summary>
        /// <returns>A model de estoque de produto preenchida.</returns>
        public Data.Model.ProdutoLoja ConverterParaEstoque()
        {
            return this.estoque.Value;
        }

        private void ConverterDtoParaModelo(Data.Model.ProdutoLoja destino)
        {
            destino.EstMinimo = (double)this.cadastro.EstoqueMinimo;
            destino.M2 = (double)this.cadastro.EstoqueM2;
            destino.QtdEstoque = (double)this.cadastro.QuantidadeEstoque;
            destino.Defeito = (double)this.cadastro.QuantidadeDefeito;
        }
    }
}