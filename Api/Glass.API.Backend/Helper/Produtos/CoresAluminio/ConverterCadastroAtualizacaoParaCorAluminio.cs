// <copyright file="ConverterCadastroAtualizacaoParaCorAluminio.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Produtos.V1.CoresAluminio.CadastroAtualizacao;
using System;
using System.Linq;

namespace Glass.API.Backend.Helper.Produtos.CoresAluminio
{
    /// <summary>
    /// Classe que realiza a tradução entre o DTO e a model para cadastro ou atualização de cor de alumínio.
    /// </summary>
    internal class ConverterCadastroAtualizacaoParaCorAluminio
    {
        private readonly CadastroAtualizacaoDto cadastro;
        private readonly Lazy<Data.Model.CorAluminio> corAluminio;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConverterCadastroAtualizacaoParaCorAluminio"/>.
        /// </summary>
        /// <param name="cadastro">O DTO de cadastro, enviado para o endpoint.</param>
        /// <param name="atual">A cor de alumínio atual (opcional), para que sejam aproveitados os valores, se necessário.</param>
        public ConverterCadastroAtualizacaoParaCorAluminio(
            CadastroAtualizacaoDto cadastro,
            Data.Model.CorAluminio atual = null)
        {
            this.cadastro = cadastro;
            this.corAluminio = new Lazy<Data.Model.CorAluminio>(() =>
            {
                var destino = atual ?? new Data.Model.CorAluminio();
                this.ConverterDtoParaModelo(destino);

                return destino;
            });
        }

        /// <summary>
        /// Realiza a conversão para a model.
        /// </summary>
        /// <returns>A model de cor de alumínio preenchida.</returns>
        public Data.Model.CorAluminio ConverterParaCorAluminio()
        {
            return this.corAluminio.Value;
        }

        private void ConverterDtoParaModelo(Data.Model.CorAluminio destino)
        {
            destino.Sigla = this.cadastro.ObterValorNormalizado(c => c.Sigla, destino.Sigla);
            destino.Descricao = this.cadastro.ObterValorNormalizado(c => c.Descricao, destino.Descricao);
        }
    }
}
