// <copyright file="ConverterCadastroAtualizacaoParaEstoqueFiscal.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.NotasFiscais;
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
            destino.EstoqueFiscal = this.cadastro.ObterValorNormalizado(c => c.QuantidadeEstoqueFiscal, destino.EstoqueFiscal);
            destino.QtdePosseTerceiros = this.cadastro.ObterValorNormalizado(c => c.QuantidadePosseTerceiros, destino.QtdePosseTerceiros);

            this.ConverterParticipante(destino);
        }

        private void ConverterParticipante(Data.Model.ProdutoLoja destino)
        {
            var participanteInformado = this.cadastro.VerificarCampoInformado(c => c.IdParticipante);
            var tipoParticipanteInformado = this.cadastro.VerificarCampoInformado(c => c.TipoParticipante);

            if (!participanteInformado && !tipoParticipanteInformado)
            {
                return;
            }

            var conversor = new ConversorParticipanteDtoParaModelo(
                participanteInformado,
                this.cadastro.IdParticipante,
                tipoParticipanteInformado,
                this.cadastro.TipoParticipante,
                destino);

            destino.IdCliente = conversor.IdCliente;
            destino.IdFornec = conversor.IdFornecedor;
            destino.IdLojaTerc = conversor.IdLoja;
            destino.IdTransportador = conversor.IdTransportador;
            destino.IdAdminCartao = conversor.IdAdministradoraCartao;
        }
    }
}
