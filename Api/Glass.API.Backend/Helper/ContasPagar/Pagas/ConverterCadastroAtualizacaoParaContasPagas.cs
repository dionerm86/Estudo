// <copyright file="ConverterCadastroAtualizacaoParaContasPagas.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.ContasPagar.V1.Pagas.CadastroAtualizacao;
using System;

namespace Glass.API.Backend.Helper.ContasPagar.Pagas
{
    /// <summary>
    /// Classe que realiza a tradução entre o DTO e a model para cadastro ou atualização de contas pagas.
    /// </summary>
    public class ConverterCadastroAtualizacaoParaContasPagas
    {
        private readonly CadastroAtualizacaoDto cadastro;
        private readonly Lazy<Data.Model.ContasPagar> contaPaga;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConverterCadastroAtualizacaoParaContasPagas"/>.
        /// </summary>
        /// <param name="cadastro">O DTO de cadastro, enviado para o endpoint.</param>
        /// <param name="atual">A conta paga atual (opcional), para que sejam aproveitados os valores, se necessário.</param>
        public ConverterCadastroAtualizacaoParaContasPagas(
            CadastroAtualizacaoDto cadastro,
            Data.Model.ContasPagar atual = null)
        {
            this.cadastro = cadastro;
            this.contaPaga = new Lazy<Data.Model.ContasPagar>(() =>
            {
                var destino = atual ?? new Data.Model.ContasPagar();
                this.ConverterDtoParaModelo(destino);

                return destino;
            });
        }

        /// <summary>
        /// Realiza a conversão para a model.
        /// </summary>
        /// <returns>A model de conta a pagar preenchida.</returns>
        public Data.Model.ContasPagar ConverterParaContasPagas()
        {
            return this.contaPaga.Value;
        }

        private void ConverterDtoParaModelo(Data.Model.ContasPagar destino)
        {
            destino.IdConta = (uint?)this.cadastro.ObterValorNormalizado(c => c.IdPlanoConta, (int?)destino.IdConta);
            destino.Obs = this.cadastro.ObterValorNormalizado(c => c.Observacao, destino.Obs);
        }
    }
}