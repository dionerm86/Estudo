// <copyright file="ConverterCadastroAtualizacaoParaTabelaDescontoAcrescimoCliente.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.TabelasDescontoAcrescimoCliente.V1.CadastroAtualizacao;
using Glass.Global.Negocios.Entidades;
using System;

namespace Glass.API.Backend.Helper.TabelasDescontoAcrescimoCliente
{
    /// <summary>
    /// Classe que realiza a tradução entre o DTO e a model para cadastro ou atualização de tabela de desconto/acréscimo de cliente.
    /// </summary>
    internal class ConverterCadastroAtualizacaoParaTabelaDescontoAcrescimoCliente
    {
        private readonly CadastroAtualizacaoDto cadastro;
        private readonly Lazy<TabelaDescontoAcrescimoCliente> tabela;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConverterCadastroAtualizacaoParaTabelaDescontoAcrescimoCliente"/>.
        /// </summary>
        /// <param name="cadastro">O DTO de cadastro, enviado para o endpoint.</param>
        /// <param name="atual">A tabela de desconto/acréscimo atual (opcional), para que sejam aproveitados os valores, se necessário.</param>
        public ConverterCadastroAtualizacaoParaTabelaDescontoAcrescimoCliente(
            CadastroAtualizacaoDto cadastro,
            TabelaDescontoAcrescimoCliente atual = null)
        {
            this.cadastro = cadastro;
            this.tabela = new Lazy<TabelaDescontoAcrescimoCliente>(() =>
            {
                var destino = atual ?? new TabelaDescontoAcrescimoCliente();
                this.ConverterDtoParaModelo(destino);

                return destino;
            });
        }

        /// <summary>
        /// Realiza a conversão para a model.
        /// </summary>
        /// <returns>A model de tabela de desconto/acréscimo de cliente preenchida.</returns>
        public TabelaDescontoAcrescimoCliente ConverterParaTabelaDescontoAcrescimoCliente()
        {
            return this.tabela.Value;
        }

        private void ConverterDtoParaModelo(TabelaDescontoAcrescimoCliente destino)
        {
            destino.Descricao = this.cadastro.ObterValorNormalizado(c => c.Nome, destino.Descricao);
        }
    }
}
