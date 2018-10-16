// <copyright file="ConverterCadastroAtualizacaoParaAplicacao.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Aplicacoes.CadastroAtualizacao;
using System;
using System.Linq;

namespace Glass.API.Backend.Helper.Aplicacoes
{
    /// <summary>
    /// Classe que realiza a tradução entre o DTO e a model para cadastro ou atualização de aplicação.
    /// </summary>
    internal class ConverterCadastroAtualizacaoParaAplicacao
    {
        private readonly CadastroAtualizacaoDto cadastro;
        private readonly Lazy<Data.Model.EtiquetaAplicacao> aplicacao;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConverterCadastroAtualizacaoParaAplicacao"/>.
        /// </summary>
        /// <param name="cadastro">O DTO de cadastro, enviado para o endpoint.</param>
        /// <param name="atual">A aplicação de etiqueta atual (opcional), para que sejam aproveitados os valores, se necessário.</param>
        public ConverterCadastroAtualizacaoParaAplicacao(
            CadastroAtualizacaoDto cadastro,
            Data.Model.EtiquetaAplicacao atual = null)
        {
            this.cadastro = cadastro;
            this.aplicacao = new Lazy<Data.Model.EtiquetaAplicacao>(() =>
            {
                var destino = atual ?? new Data.Model.EtiquetaAplicacao();
                this.ConverterDtoParaModelo(destino);

                return destino;
            });
        }

        /// <summary>
        /// Realiza a conversão para a model.
        /// </summary>
        /// <returns>A model de aplicação de etiqueta preenchida.</returns>
        public Data.Model.EtiquetaAplicacao ConverterParaAplicacao()
        {
            return this.aplicacao.Value;
        }

        private void ConverterDtoParaModelo(Data.Model.EtiquetaAplicacao destino)
        {
            destino.CodInterno = this.cadastro.ObterValorNormalizado(c => c.Codigo, destino.CodInterno);
            destino.Descricao = this.cadastro.ObterValorNormalizado(c => c.Descricao, destino.Descricao);
            destino.DestacarEtiqueta = this.cadastro.ObterValorNormalizado(c => c.DestacarNaEtiqueta, destino.DestacarEtiqueta);
            destino.GerarFormaInexistente = this.cadastro.ObterValorNormalizado(c => c.GerarFormaInexistente, destino.GerarFormaInexistente);
            destino.NaoPermitirFastDelivery = this.cadastro.ObterValorNormalizado(c => c.NaoPermitirFastDelivery, destino.NaoPermitirFastDelivery);
            destino.DiasMinimos = (int)this.cadastro.ObterValorNormalizado(c => c.NumeroDiasUteisDataEntrega, destino.DiasMinimos);
            destino.Situacao = this.cadastro.ObterValorNormalizado(c => c.Situacao, destino.Situacao);

            this.ConverterTiposPedidos(destino);
        }

        private void ConverterTiposPedidos(Data.Model.EtiquetaAplicacao destino)
        {
            var valorDestino = destino.TipoPedido?.Split(',')
                .Select(tipoPedido => tipoPedido.StrParaInt())
                .Select(tipoPedido => (Data.Model.Pedido.TipoPedidoEnum)tipoPedido);

            var valorNormalizado = this.cadastro.ObterValorNormalizado(c => c.TiposPedidos, valorDestino);

            destino.TipoPedido = valorNormalizado != null
                ? string.Join(",", valorNormalizado.Select(tipoPedido => (int)tipoPedido))
                : null;
        }
    }
}
