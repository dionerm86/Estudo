// <copyright file="ConverterCadastroAtualizacaoParaCondutor.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Condutores.CadastroAtualizacao;
using System;

namespace Glass.API.Backend.Helper.Condutores
{
    /// <summary>
    /// Classe que realiza a tradulçai entre o DTO e a model para cadastro ou atualização de condutor.
    /// </summary>
    internal class ConverterCadastroAtualizacaoParaCondutor
    {
        private readonly CadastroAtualizacaoDto cadastro;
        private readonly Lazy<Data.Model.Condutores> condutor;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConverterCadastroAtualizacaoParaCondutor"/>.
        /// </summary>
        /// <param name="cadastro">O DTO de cadastro, enviado para o endpoint.</param>
        /// <param name="atual">O condutor atual (opcional), para que sejam aproveitados os valores, se necessário.</param>
        public ConverterCadastroAtualizacaoParaCondutor(
            CadastroAtualizacaoDto cadastro,
            Data.Model.Condutores atual = null)
        {
            this.cadastro = cadastro;
            this.condutor = new Lazy<Data.Model.Condutores>(() =>
            {
                var destino = atual ?? new Data.Model.Condutores();
                this.ConverterDtoParaModelo(destino);

                return destino;
            });
        }

        /// <summary>
        /// Realiza a conversão para a model.
        /// </summary>
        /// <returns>A model de condutor preenchida.</returns>
        public Data.Model.Condutores ConverterParaCondutor()
        {
            return this.condutor.Value;
        }

        private void ConverterDtoParaModelo(Data.Model.Condutores destino)
        {
            destino.Nome = this.cadastro.ObterValorNormalizado(c => c.Nome, destino.Nome);
            destino.CpfCnpj = this.cadastro.ObterValorNormalizado(c => c.CpfCnpj, destino.CpfCnpj);
        }
    }
}
