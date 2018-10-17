// <copyright file="ConverterCadastroAtualizacaoParaContaRecebida.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.ContasReceber.V1.CadastroAtualizacao;
using System;

namespace Glass.API.Backend.Helper.ContasReceber
{
    /// <summary>
    /// Classe que realiza a tradução entre o DTO e a model para cadastro ou atualização de conta recebida.
    /// </summary>
    internal class ConverterCadastroAtualizacaoParaContaRecebida
    {
        private readonly CadastroAtualizacaoRecebidaDto cadastro;
        private readonly Lazy<Data.Model.ContasReceber> contaRecebida;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConverterCadastroAtualizacaoParaContaRecebida"/>.
        /// </summary>
        /// <param name="cadastro">O DTO de cadastro, enviado para o endpoint.</param>
        /// <param name="atual">A conta recebida atual (opcional), para que sejam aproveitados os valores, se necessário.</param>
        public ConverterCadastroAtualizacaoParaContaRecebida(
            CadastroAtualizacaoRecebidaDto cadastro,
            Data.Model.ContasReceber atual = null)
        {
            this.cadastro = cadastro;
            this.contaRecebida = new Lazy<Data.Model.ContasReceber>(() =>
            {
                var destino = atual ?? new Data.Model.ContasReceber();
                this.ConverterDtoParaModelo(destino);

                return destino;
            });
        }

        /// <summary>
        /// Realiza a conversão para a model.
        /// </summary>
        /// <returns>A model de conta recebida preenchida.</returns>
        public Data.Model.ContasReceber ConverterParaContaRecebida()
        {
            return this.contaRecebida.Value;
        }

        private void ConverterDtoParaModelo(Data.Model.ContasReceber destino)
        {
            destino.Obs = this.cadastro.Observacao;
        }
    }
}