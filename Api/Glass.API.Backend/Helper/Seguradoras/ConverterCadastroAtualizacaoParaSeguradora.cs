// <copyright file="ConverterCadastroAtualizacaoParaSeguradora.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Seguradoras.V1.CadastroAtualizacao;
using Glass.Fiscal.Negocios.Entidades;
using System;

namespace Glass.API.Backend.Helper.Seguradoras
{
    /// <summary>
    /// Classe que realiza a tradução entre o DTO e a model para cadastro ou atualização de seguradoras.
    /// </summary>
    internal class ConverterCadastroAtualizacaoParaSeguradora
    {
        private readonly CadastroAtualizacaoDto cadastro;
        private readonly Lazy<Seguradora> seguradora;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConverterCadastroAtualizacaoParaSeguradora"/>.
        /// </summary>
        /// <param name="cadastro">O DTO de cadastro, enviado para o endpoint.</param>
        /// <param name="atual">A seguradora atual atual (opcional), para que sejam aproveitados os valores, se necessário.</param>
        public ConverterCadastroAtualizacaoParaSeguradora(
            CadastroAtualizacaoDto cadastro,
            Seguradora atual = null)
        {
            this.cadastro = cadastro;
            this.seguradora = new Lazy<Seguradora>(() =>
            {
                var destino = atual ?? new Seguradora();
                this.ConverterDtoParaModelo(destino);

                return destino;
            });
        }

        /// <summary>
        /// Realiza a conversão para a model.
        /// </summary>
        /// <returns>A model de seguradora preenchida.</returns>
        public Seguradora ConverterParaSeguradora()
        {
            return this.seguradora.Value;
        }

        private void ConverterDtoParaModelo(Seguradora destino)
        {
            destino.NomeSeguradora = this.cadastro.ObterValorNormalizado(c => c.Nome, destino.NomeSeguradora);
            destino.CNPJ = this.cadastro.ObterValorNormalizado(c => c.Cnpj, destino.CNPJ);
        }
    }
}
