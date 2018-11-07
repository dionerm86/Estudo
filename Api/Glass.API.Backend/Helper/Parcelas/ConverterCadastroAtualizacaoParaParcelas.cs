// <copyright file="ConverterCadastroAtualizacaoParaParcelas.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Parcelas.V1.CadastroAtualizacao;
using System;

namespace Glass.API.Backend.Helper.Parcelas
{
    /// <summary>
    /// Classe que realiza a tradução entre o DTO e a model para cadastro ou atualização de parcelas.
    /// </summary>
    internal class ConverterCadastroAtualizacaoParaParcelas
    {
        private readonly CadastroAtualizacaoDto cadastro;
        private readonly Lazy<Glass.Financeiro.Negocios.Entidades.Parcelas> parcela;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConverterCadastroAtualizacaoParaParcelas"/>.
        /// </summary>
        /// <param name="cadastro">O DTO de cadastro, enviado para o endpoint.</param>
        /// <param name="atual">A parcela atual (opcional), para que sejam aproveitados os valores, se necessário.</param>
        public ConverterCadastroAtualizacaoParaParcelas(
            CadastroAtualizacaoDto cadastro,
            Glass.Financeiro.Negocios.Entidades.Parcelas atual = null)
        {
            this.cadastro = cadastro;
            this.parcela = new Lazy<Financeiro.Negocios.Entidades.Parcelas>(() =>
            {
                var destino = atual ?? new Glass.Financeiro.Negocios.Entidades.Parcelas();
                this.ConverterDtoParaModelo(destino);

                return destino;
            });
        }

        /// <summary>
        /// Realiza a conversão para a model.
        /// </summary>
        /// <returns>A model de parcela preenchida.</returns>
        public Financeiro.Negocios.Entidades.Parcelas ConverterParaParcela()
        {
            return this.parcela.Value;
        }

        private void ConverterDtoParaModelo(Financeiro.Negocios.Entidades.Parcelas destino)
        {
            destino.Descricao = this.cadastro.ObterValorNormalizado(c => c.Descricao, destino.Descricao);
            destino.Dias = this.cadastro.ObterValorNormalizado(c => c.Dias, destino.Dias);
            destino.ParcelaAVista = this.cadastro.ObterValorNormalizado(c => c.ParcelaAVista, destino.ParcelaAVista);
            destino.Situacao = this.cadastro.ObterValorNormalizado(c => c.Situacao, destino.Situacao);
            destino.Desconto = this.cadastro.ObterValorNormalizado(c => c.Desconto, destino.Desconto);
            destino.ParcelaPadrao = this.cadastro.ObterValorNormalizado(c => c.ParcelaPadrao, destino.ParcelaPadrao);
            destino.NumParcelas = this.cadastro.ObterValorNormalizado(c => c.NumeroParcelas, destino.NumParcelas);
        }
    }
}
