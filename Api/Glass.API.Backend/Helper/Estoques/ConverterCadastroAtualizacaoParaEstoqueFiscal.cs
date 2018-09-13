// <copyright file="ConverterCadastroAtualizacaoParaEstoqueFiscal.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Estoques.CadastroAtualizacao;
using System;

namespace Glass.API.Backend.Helper.Estoques
{
    /// <summary>
    /// Classe que realiza a tradução entre o DTO e a model para cadastro ou atualização de estoque fiscal.
    /// </summary>
    internal class ConverterCadastroAtualizacaoParaEstoqueFiscal
    {
        private readonly CadastroAtualizacaoFiscalDto cadastro;
        private readonly Lazy<Data.Model.ProdutoLoja> estoque;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConverterCadastroAtualizacaoParaEstoqueFiscal"/>.
        /// </summary>
        /// <param name="cadastro">O DTO de cadastro, enviado para o endpoint.</param>
        /// <param name="atual">O estoque do produto atual (opcional), para que sejam aproveitados os valores, se necessário.</param>
        public ConverterCadastroAtualizacaoParaEstoqueFiscal(
            CadastroAtualizacaoFiscalDto cadastro,
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
            destino.EstoqueFiscal = (double)this.cadastro.QuantidadeEstoqueFiscal;
            destino.QtdePosseTerceiros = (double)this.cadastro.QuantidadePosseTerceiros;

            this.ConverterParticipante(destino);
        }

        private void ConverterParticipante(Data.Model.ProdutoLoja destino)
        {
            if (this.cadastro.IdParticipante == null && this.cadastro.TipoParticipante == null)
            {
                return;
            }

            destino.IdCliente = null;
            destino.IdFornec = null;
            destino.IdLojaTerc = null;
            destino.IdTransportador = null;
            destino.IdAdminCartao = null;

            switch (this.cadastro.TipoParticipante.Value)
            {
                case Data.EFD.DataSourcesEFD.TipoPartEnum.Cliente:
                    destino.IdCliente = this.cadastro.IdParticipante;
                    break;

                case Data.EFD.DataSourcesEFD.TipoPartEnum.Fornecedor:
                    destino.IdFornec = this.cadastro.IdParticipante;
                    break;

                case Data.EFD.DataSourcesEFD.TipoPartEnum.Loja:
                    destino.IdLojaTerc = this.cadastro.IdParticipante;
                    break;

                case Data.EFD.DataSourcesEFD.TipoPartEnum.Transportador:
                    destino.IdTransportador = this.cadastro.IdParticipante;
                    break;

                case Data.EFD.DataSourcesEFD.TipoPartEnum.AdministradoraCartao:
                    destino.IdAdminCartao = this.cadastro.IdParticipante;
                    break;
            }
        }
    }
}
