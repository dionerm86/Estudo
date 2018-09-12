// <copyright file="ConverterCadastroAtualizacaoParaEstoque.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Estoques.CadastroAtualizacao;
using System;

namespace Glass.API.Backend.Helper.Estoques
{
    /// <summary>
    /// Classe que realiza a tradução entre o DTO e a model para cadastro ou atualização de conta recebida.
    /// </summary>
    internal class ConverterCadastroAtualizacaoParaEstoque
    {
        private readonly CadastroAtualizacaoDto cadastro;
        private readonly Lazy<Data.Model.ProdutoLoja> estoque;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConverterCadastroAtualizacaoParaEstoque"/>.
        /// </summary>
        /// <param name="cadastro">O DTO de cadastro, enviado para o endpoint.</param>
        /// <param name="atual">O estoque do produto atual (opcional), para que sejam aproveitados os valores, se necessário.</param>
        public ConverterCadastroAtualizacaoParaEstoque(
            CadastroAtualizacaoDto cadastro,
            Data.Model.ProdutoLoja atual = null)
        {
            this.cadastro = cadastro;
            this.estoque = new Lazy<Data.Model.ProdutoLoja>(() =>
            {
                var destino = atual ?? new Data.Model.ProdutoLoja();
                this.ConverterDtoParaModelo(destino);

                return destino;
            });
        }

        /// <summary>
        /// Realiza a conversão para a model.
        /// </summary>
        /// <returns>A model de estoque de produto preenchida.</returns>
        public Data.Model.ProdutoLoja ConverterParaEstoque()
        {
            return this.estoque.Value;
        }

        private void ConverterDtoParaModelo(Data.Model.ProdutoLoja destino)
        {
            destino.EstMinimo = (double)this.cadastro.EstoqueMinimo;
            destino.M2 = (double)this.cadastro.EstoqueM2;
            destino.EstMinimo = (double)this.cadastro.QuantidadeEstoque;
            destino.EstoqueFiscal = (double)this.cadastro.QuantidadeEstoqueFiscal;
            destino.Defeito = (double)this.cadastro.QuantidadeDefeito;
            destino.QtdePosseTerceiros = (double)this.cadastro.QuantidadePosseTerceiros;
            destino.IdCliente = this.cadastro.IdCliente;
            destino.IdFornec = this.cadastro.IdFornecedor;
            destino.IdLojaTerc = this.cadastro.IdLojaTerceiros;
            destino.IdTransportador = this.cadastro.IdTransportador;
            destino.IdAdminCartao = this.cadastro.IdAdministradoraCartao;
        }
    }
}