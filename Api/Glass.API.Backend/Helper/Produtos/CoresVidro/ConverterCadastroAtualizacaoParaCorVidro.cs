// <copyright file="ConverterCadastroAtualizacaoParaCorVidro.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Produtos.CoresVidro.CadastroAtualizacao;
using System;
using System.Linq;

namespace Glass.API.Backend.Helper.Produtos.CoresVidro
{
    /// <summary>
    /// Classe que realiza a tradução entre o DTO e a model para cadastro ou atualização de cor de vidro.
    /// </summary>
    internal class ConverterCadastroAtualizacaoParaCorVidro
    {
        private readonly CadastroAtualizacaoDto cadastro;
        private readonly Lazy<Data.Model.CorVidro> corVidro;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConverterCadastroAtualizacaoParaCorVidro"/>.
        /// </summary>
        /// <param name="cadastro">O DTO de cadastro, enviado para o endpoint.</param>
        /// <param name="atual">A cor de vidro atual (opcional), para que sejam aproveitados os valores, se necessário.</param>
        public ConverterCadastroAtualizacaoParaCorVidro(
            CadastroAtualizacaoDto cadastro,
            Data.Model.CorVidro atual = null)
        {
            this.cadastro = cadastro;
            this.corVidro = new Lazy<Data.Model.CorVidro>(() =>
            {
                var destino = atual ?? new Data.Model.CorVidro();
                this.ConverterDtoParaModelo(destino);

                return destino;
            });
        }

        /// <summary>
        /// Realiza a conversão para a model.
        /// </summary>
        /// <returns>A model de cor de vidro preenchida.</returns>
        public Data.Model.CorVidro ConverterParaCorVidro()
        {
            return this.corVidro.Value;
        }

        private void ConverterDtoParaModelo(Data.Model.CorVidro destino)
        {
            destino.Sigla = this.cadastro.ObterValorNormalizado(c => c.Sigla, destino.Sigla);
            destino.Descricao = this.cadastro.ObterValorNormalizado(c => c.Descricao, destino.Descricao);
        }
    }
}
