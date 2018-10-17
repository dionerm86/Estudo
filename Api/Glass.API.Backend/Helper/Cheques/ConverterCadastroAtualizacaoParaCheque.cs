// <copyright file="ConverterCadastroAtualizacaoParaCheque.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Cheques.V1.CadastroAtualizacao;
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
            destino.Num = this.cadastro.ObterValorNormalizado(c => c.NumeroCheque, destino.Num);
            destino.DigitoNum = this.cadastro.ObterValorNormalizado(c => c.DigitoNumeroCheque, destino.DigitoNum);
            destino.Banco = this.cadastro.ObterValorNormalizado(c => c.Banco, destino.Banco);
            destino.Agencia = this.cadastro.ObterValorNormalizado(c => c.Agencia, destino.Agencia);
            destino.Conta = this.cadastro.ObterValorNormalizado(c => c.Conta, destino.Conta);
            destino.Titular = this.cadastro.ObterValorNormalizado(c => c.Titular, destino.Titular);
            destino.DataVenc = this.cadastro.ObterValorNormalizado(c => c.DataVencimento, destino.DataVenc);
            destino.Obs = this.cadastro.ObterValorNormalizado(c => c.Observacao, destino.Obs);
        }
    }
}