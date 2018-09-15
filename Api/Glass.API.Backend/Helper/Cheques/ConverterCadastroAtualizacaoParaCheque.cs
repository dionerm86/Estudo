// <copyright file="ConverterCadastroAtualizacaoParaCheque.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Cheques.CadastroAtualizacao;
using System;

namespace Glass.API.Backend.Helper.Cheques
{
    /// <summary>
    /// Classe que realiza a tradução entre o DTO e a model para cadastro ou atualização de cheque.
    /// </summary>
    internal class ConverterCadastroAtualizacaoParaCheque
    {
        private readonly CadastroAtualizacaoDto cadastro;
        private readonly Lazy<Data.Model.Cheques> cheque;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConverterCadastroAtualizacaoParaCheque"/>.
        /// </summary>
        /// <param name="cadastro">O DTO de cadastro, enviado para o endpoint.</param>
        /// <param name="atual">O cheque atual (opcional), para que sejam aproveitados os valores, se necessário.</param>
        public ConverterCadastroAtualizacaoParaCheque(
            CadastroAtualizacaoDto cadastro,
            Data.Model.Cheques atual = null)
        {
            this.cadastro = cadastro;
            this.cheque = new Lazy<Data.Model.Cheques>(() =>
            {
                var destino = atual ?? new Data.Model.Cheques();
                this.ConverterDtoParaModelo(destino);

                return destino;
            });
        }

        /// <summary>
        /// Realiza a conversão para a model.
        /// </summary>
        /// <returns>A model de cheque preenchida.</returns>
        public Data.Model.Cheques ConverterParaCheque()
        {
            return this.cheque.Value;
        }

        private void ConverterDtoParaModelo(Data.Model.Cheques destino)
        {
            destino.Num = this.cadastro.NumeroCheque;
            destino.DigitoNum = this.cadastro.DigitoNumeroCheque;
            destino.Banco = this.cadastro.Banco;
            destino.Agencia = this.cadastro.Agencia;
            destino.Conta = this.cadastro.Conta;
            destino.Titular = this.cadastro.Titular;
            destino.DataVenc = this.cadastro.DataVencimento;
            destino.Obs = this.cadastro.Observacao;
        }
    }
}