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
        private readonly Lazy<PlanoContas> turno;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConverterCadastroAtualizacaoParaPlanoConta"/>.
        /// </summary>
        /// <param name="cadastro">O DTO de cadastro, enviado para o endpoint.</param>
        /// <param name="atual">O turno atual (opcional), para que sejam aproveitados os valores, se necessário.</param>
        public ConverterCadastroAtualizacaoParaPlanoConta(
            CadastroAtualizacaoDto cadastro,
            PlanoContas atual = null)
        {
            this.cadastro = cadastro;
            this.turno = new Lazy<PlanoContas>(() =>
            {
                var destino = atual ?? new PlanoContas();
                this.ConverterDtoParaModelo(destino);

                return destino;
            });
        }

        /// <summary>
        /// Realiza a conversão para a model.
        /// </summary>
        /// <returns>A model de turno preenchida.</returns>
        public PlanoContas ConverterParaPlanoConta()
        {
            return this.turno.Value;
        }

        private void ConverterDtoParaModelo(PlanoContas destino)
        {
            destino.Descricao = this.cadastro.ObterValorNormalizado(c => c.Nome, destino.Descricao);
            destino.ExibirDre = this.cadastro.ObterValorNormalizado(c => c.ExibirDre, destino.ExibirDre);
            destino.IdGrupo = this.cadastro.ObterValorNormalizado(c => c.GrupoConta, destino.IdGrupo);
            destino.Situacao = this.cadastro.ObterValorNormalizado(c => c.Situacao, destino.Situacao);
        }
    }
}
