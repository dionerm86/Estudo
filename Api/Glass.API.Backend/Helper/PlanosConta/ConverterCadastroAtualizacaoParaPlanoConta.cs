// <copyright file="ConverterCadastroAtualizacaoParaPlanoConta.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.PlanosConta.V1.CadastroAtualizacao;
using Glass.Financeiro.Negocios.Entidades;
using System;

namespace Glass.API.Backend.Helper.PlanosConta
{
    /// <summary>
    /// Classe que realiza a tradução entre o DTO e a model para cadastro ou atualização de planos de conta.
    /// </summary>
    internal class ConverterCadastroAtualizacaoParaPlanoConta
    {
        private readonly CadastroAtualizacaoDto cadastro;
        private readonly Lazy<PlanoContas> planoConta;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConverterCadastroAtualizacaoParaPlanoConta"/>.
        /// </summary>
        /// <param name="cadastro">O DTO de cadastro, enviado para o endpoint.</param>
        /// <param name="atual">O plano de conta atual (opcional), para que sejam aproveitados os valores, se necessário.</param>
        public ConverterCadastroAtualizacaoParaPlanoConta(
            CadastroAtualizacaoDto cadastro,
            PlanoContas atual = null)
        {
            this.cadastro = cadastro;
            this.planoConta = new Lazy<PlanoContas>(() =>
            {
                var destino = atual ?? new PlanoContas();
                this.ConverterDtoParaModelo(destino);

                return destino;
            });
        }

        /// <summary>
        /// Realiza a conversão para a model.
        /// </summary>
        /// <returns>A model de plano de conta preenchida.</returns>
        public PlanoContas ConverterParaPlanoConta()
        {
            return this.planoConta.Value;
        }

        private void ConverterDtoParaModelo(PlanoContas destino)
        {
            destino.Descricao = this.cadastro.ObterValorNormalizado(c => c.Nome, destino.Descricao);
            destino.ExibirDre = this.cadastro.ObterValorNormalizado(c => c.ExibirDre, destino.ExibirDre);
            destino.IdGrupo = this.cadastro.ObterValorNormalizado(c => c.IdGrupoConta, destino.IdGrupo);
            destino.Situacao = this.cadastro.ObterValorNormalizado(c => c.Situacao, destino.Situacao);
        }
    }
}
