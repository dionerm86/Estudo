// <copyright file="ConverterCadastroAtualizacaoParaGrupoProjeto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Projetos.V1.GruposProjeto.CadastroAtualizacao;
using Glass.Data.Model;
using System;

namespace Glass.API.Backend.Helper.Projetos.GruposProjeto
{
    /// <summary>
    /// Classe que realiza a tradução entre o DTO e a model para cadastro ou atualização de grupos de projeto.
    /// </summary>
    internal class ConverterCadastroAtualizacaoParaGrupoProjeto
    {
        private readonly CadastroAtualizacaoDto cadastro;
        private readonly Lazy<GrupoModelo> grupoProjeto;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConverterCadastroAtualizacaoParaGrupoProjeto"/>.
        /// </summary>
        /// <param name="cadastro">O DTO de cadastro, enviado para o endpoint.</param>
        /// <param name="atual">O grupo de projeto atual (opcional), para que sejam aproveitados os valores, se necessário.</param>
        public ConverterCadastroAtualizacaoParaGrupoProjeto(
            CadastroAtualizacaoDto cadastro,
            GrupoModelo atual = null)
        {
            this.cadastro = cadastro;
            this.grupoProjeto = new Lazy<GrupoModelo>(() =>
            {
                var destino = atual ?? new GrupoModelo();
                this.ConverterDtoParaModelo(destino);

                return destino;
            });
        }

        /// <summary>
        /// Realiza a conversão para a model.
        /// </summary>
        /// <returns>A model de grupo de projeto preenchida.</returns>
        public GrupoModelo ConverterParaGrupoProjeto()
        {
            return this.grupoProjeto.Value;
        }

        private void ConverterDtoParaModelo(GrupoModelo destino)
        {
            destino.Descricao = this.cadastro.ObterValorNormalizado(c => c.Nome, destino.Descricao);
            destino.BoxPadrao = this.cadastro.ObterValorNormalizado(c => c.BoxPadrao, destino.BoxPadrao);
            destino.Esquadria = this.cadastro.ObterValorNormalizado(c => c.Esquadria, destino.Esquadria);
            destino.Situacao = (int)this.cadastro.ObterValorNormalizado(c => c.Situacao, (Situacao)destino.Situacao);
        }
    }
}
