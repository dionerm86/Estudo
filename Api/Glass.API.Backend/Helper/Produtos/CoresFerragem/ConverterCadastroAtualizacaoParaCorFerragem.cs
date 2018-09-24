// <copyright file="ConverterCadastroAtualizacaoParaCorFerragem.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Produtos.CoresFerragem.CadastroAtualizacao;
using System;
using System.Linq;

namespace Glass.API.Backend.Helper.Produtos.CoresFerragem
{
    /// <summary>
    /// Classe que realiza a tradução entre o DTO e a model para cadastro ou atualização de cor de ferragem.
    /// </summary>
    internal class ConverterCadastroAtualizacaoParaCorFerragem
    {
        private readonly CadastroAtualizacaoDto cadastro;
        private readonly Lazy<Data.Model.CorFerragem> corFerragem;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConverterCadastroAtualizacaoParaCorFerragem"/>.
        /// </summary>
        /// <param name="cadastro">O DTO de cadastro, enviado para o endpoint.</param>
        /// <param name="atual">A cor de ferragem atual (opcional), para que sejam aproveitados os valores, se necessário.</param>
        public ConverterCadastroAtualizacaoParaCorFerragem(
            CadastroAtualizacaoDto cadastro,
            Data.Model.CorFerragem atual = null)
        {
            this.cadastro = cadastro;
            this.corFerragem = new Lazy<Data.Model.CorFerragem>(() =>
            {
                var destino = atual ?? new Data.Model.CorFerragem();
                this.ConverterDtoParaModelo(destino);

                return destino;
            });
        }

        /// <summary>
        /// Realiza a conversão para a model.
        /// </summary>
        /// <returns>A model de cor de ferragem preenchida.</returns>
        public Data.Model.CorFerragem ConverterParaCorFerragem()
        {
            return this.corFerragem.Value;
        }

        private void ConverterDtoParaModelo(Data.Model.CorFerragem destino)
        {
            destino.Sigla = this.cadastro.ObterValorNormalizado(c => c.Sigla, destino.Sigla);
            destino.Descricao = this.cadastro.ObterValorNormalizado(c => c.Descricao, destino.Descricao);
        }
    }
}
