// <copyright file="ConverterCadastroAtualizacaoParaFabricanteFerragem.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Projetos.V1.Ferragens.FabricantesFerragem.CadastroAtualizacao;
using Glass.Projeto.Negocios.Entidades;
using System;

namespace Glass.API.Backend.Helper.Projetos.Ferragens.FabricantesFerragem
{
    /// <summary>
    /// Classe que realiza a tradução entre o DTO e a model para cadastro ou atualização de fabricantes de ferragens.
    /// </summary>
    internal class ConverterCadastroAtualizacaoParaFabricanteFerragem
    {
        private readonly CadastroAtualizacaoDto cadastro;
        private readonly Lazy<FabricanteFerragem> grupoProjeto;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConverterCadastroAtualizacaoParaFabricanteFerragem"/>.
        /// </summary>
        /// <param name="cadastro">O DTO de cadastro, enviado para o endpoint.</param>
        /// <param name="atual">O fabricante de ferragem atual (opcional), para que sejam aproveitados os valores, se necessário.</param>
        public ConverterCadastroAtualizacaoParaFabricanteFerragem(
            CadastroAtualizacaoDto cadastro,
            FabricanteFerragem atual = null)
        {
            this.cadastro = cadastro;
            this.grupoProjeto = new Lazy<FabricanteFerragem>(() =>
            {
                var destino = atual ?? new FabricanteFerragem();
                this.ConverterDtoParaModelo(destino);

                return destino;
            });
        }

        /// <summary>
        /// Realiza a conversão para a model.
        /// </summary>
        /// <returns>A model de fabricante de ferragem preenchida.</returns>
        public FabricanteFerragem ConverterParaFabricanteFerragem()
        {
            return this.grupoProjeto.Value;
        }

        private void ConverterDtoParaModelo(FabricanteFerragem destino)
        {
            destino.Nome = this.cadastro.ObterValorNormalizado(c => c.Nome, destino.Nome);
            destino.Sitio = this.cadastro.ObterValorNormalizado(c => c.Site, destino.Sitio);
        }
    }
}
