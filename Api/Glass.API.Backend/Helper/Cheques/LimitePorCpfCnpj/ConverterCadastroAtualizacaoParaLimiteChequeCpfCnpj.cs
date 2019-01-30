// <copyright file="ConverterCadastroAtualizacaoParaLimiteCheque.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Cheques.V1.LimitePorCpfCnpj.CadastroAtualizacao;
using Glass.Data.Model;
using System;

namespace Glass.API.Backend.Helper.Cheques.LimitePorCpfCnpj
{
    /// <summary>
    /// Classe que realiza a tradução entre o DTO e a model para cadastro ou atualização de limite de cheques.
    /// </summary>
    public class ConverterCadastroAtualizacaoParaLimiteChequeCpfCnpj
    {
        private readonly CadastroAtualizacaoDto cadastro;
        private readonly Lazy<LimiteChequeCpfCnpj> limiteCheque;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConverterCadastroAtualizacaoParaLimiteChequeCpfCnpj"/>.
        /// </summary>
        /// <param name="cadastro">O DTO de cadastro, enviado para o endpoint.</param>
        /// <param name="atual">O limite de cheque atual (opcional), para que sejam aproveitados os valores, se necessário.</param>
        public ConverterCadastroAtualizacaoParaLimiteChequeCpfCnpj(
            CadastroAtualizacaoDto cadastro,
            LimiteChequeCpfCnpj atual = null)
        {
            this.cadastro = cadastro;
            this.limiteCheque = new Lazy<LimiteChequeCpfCnpj>(() =>
            {
                var destino = atual ?? new LimiteChequeCpfCnpj();
                this.ConverterDtoParaModelo(destino);

                return destino;
            });
        }

        /// <summary>
        /// Realiza a conversão para a model.
        /// </summary>
        /// <returns>A model de limite de cheque preenchida.</returns>
        public LimiteChequeCpfCnpj ConverterParaLimiteCheque()
        {
            return this.limiteCheque.Value;
        }

        private void ConverterDtoParaModelo(LimiteChequeCpfCnpj destino)
        {
            destino.CpfCnpj = this.cadastro.ObterValorNormalizado(c => c.CpfCnpj, destino.CpfCnpj);
            destino.Limite = this.cadastro.ObterValorNormalizado(c => c.Limite, destino.Limite);
            destino.Observacao = this.cadastro.ObterValorNormalizado(c => c.Observacao, destino.Observacao);
        }
    }
}