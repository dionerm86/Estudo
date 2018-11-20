// <copyright file="ConverterCadastroAtualizacaoParaCarregamento.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Carregamentos.V1.CadastroAtualizacao;
using Glass.Data.Model;
using System;

namespace Glass.API.Backend.Helper.Carregamentos
{
    /// <summary>
    /// Classe que realiza a tradução entre o DTO e a model para cadastro ou atualização do carregamento.
    /// </summary>
    internal class ConverterCadastroAtualizacaoParaCarregamento
    {
        private readonly CadastroAtualizacaoDto cadastro;
        private readonly Lazy<Carregamento> carregamento;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConverterCadastroAtualizacaoParaCarregamento"/>.
        /// </summary>
        /// <param name="cadastro">O DTO de cadastro, enviado para o endpoint.</param>
        /// <param name="atual">O carregamento atual (opcional), para que sejam aproveitados os valores, se necessário.</param>
        public ConverterCadastroAtualizacaoParaCarregamento(
            CadastroAtualizacaoDto cadastro,
            Carregamento atual = null)
        {
            this.cadastro = cadastro;
            this.carregamento = new Lazy<Carregamento>(() =>
            {
                var destino = atual ?? new Carregamento();
                this.ConverterDtoParaModelo(destino);

                return destino;
            });
        }

        /// <summary>
        /// Realiza a conversão para a model.
        /// </summary>
        /// <returns>A model de carregamento preenchida.</returns>
        public Carregamento ConverterParaCarregamento()
        {
            return this.carregamento.Value;
        }

        private void ConverterDtoParaModelo(Carregamento destino)
        {
            destino.IdMotorista = (uint)this.cadastro.ObterValorNormalizado(c => c.IdMotorista, (int)destino.IdMotorista);
            destino.Placa = this.cadastro.ObterValorNormalizado(c => c.Placa, destino.Placa);
            destino.DataPrevistaSaida = this.cadastro.ObterValorNormalizado(c => c.DataPrevisaoSaida, destino.DataPrevistaSaida);
        }
    }
}
