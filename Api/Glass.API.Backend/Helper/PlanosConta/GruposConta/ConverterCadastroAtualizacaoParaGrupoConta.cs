// <copyright file="ConverterCadastroAtualizacaoParaGrupoConta.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.PlanosConta.V1.GruposConta.CadastroAtualizacao;
using Glass.Financeiro.Negocios.Entidades;
using System;

namespace Glass.API.Backend.Helper.PlanosConta.GruposConta
{
    /// <summary>
    /// Classe que realiza a tradução entre o DTO e a model para cadastro ou atualização de grupos de conta.
    /// </summary>
    internal class ConverterCadastroAtualizacaoParaGrupoConta
    {
        private readonly CadastroAtualizacaoDto cadastro;
        private readonly Lazy<GrupoConta> grupoConta;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConverterCadastroAtualizacaoParaGrupoConta"/>.
        /// </summary>
        /// <param name="cadastro">O DTO de cadastro, enviado para o endpoint.</param>
        /// <param name="atual">O grupo de conta atual (opcional), para que sejam aproveitados os valores, se necessário.</param>
        public ConverterCadastroAtualizacaoParaGrupoConta(
            CadastroAtualizacaoDto cadastro,
            GrupoConta atual = null)
        {
            this.cadastro = cadastro;
            this.grupoConta = new Lazy<GrupoConta>(() =>
            {
                var destino = atual ?? new GrupoConta();
                this.ConverterDtoParaModelo(destino);

                return destino;
            });
        }

        /// <summary>
        /// Realiza a conversão para a model.
        /// </summary>
        /// <returns>A model de grupo de conta preenchida.</returns>
        public GrupoConta ConverterParaGrupoConta()
        {
            return this.grupoConta.Value;
        }

        private void ConverterDtoParaModelo(GrupoConta destino)
        {
            destino.Descricao = this.cadastro.ObterValorNormalizado(c => c.Nome, destino.Descricao);
            destino.PontoEquilibrio = this.cadastro.ObterValorNormalizado(c => c.ExibirPontoEquilibrio, destino.PontoEquilibrio);
            destino.IdCategoriaConta = this.cadastro.ObterValorNormalizado(c => c.IdCategoriaConta, destino.IdCategoriaConta);
            destino.Situacao = this.cadastro.ObterValorNormalizado(c => c.Situacao, destino.Situacao);
        }
    }
}
