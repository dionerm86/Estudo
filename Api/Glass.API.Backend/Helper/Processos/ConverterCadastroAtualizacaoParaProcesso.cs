// <copyright file="ConverterCadastroAtualizacaoParaProcesso.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Processos.CadastroAtualizacao;
using System;
using System.Linq;

namespace Glass.API.Backend.Helper.Processos
{
    /// <summary>
    /// Classe que realiza a tradução entre o DTO e a model para cadastro ou atualização de processo.
    /// </summary>
    internal class ConverterCadastroAtualizacaoParaProcesso
    {
        private readonly CadastroAtualizacaoDto cadastro;
        private readonly Lazy<Data.Model.EtiquetaProcesso> processo;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConverterCadastroAtualizacaoParaProcesso"/>.
        /// </summary>
        /// <param name="cadastro">O DTO de cadastro, enviado para o endpoint.</param>
        /// <param name="atual">O processo de etiqueta atual (opcional), para que sejam aproveitados os valores, se necessário.</param>
        public ConverterCadastroAtualizacaoParaProcesso(
            CadastroAtualizacaoDto cadastro,
            Data.Model.EtiquetaProcesso atual = null)
        {
            this.cadastro = cadastro;
            this.processo = new Lazy<Data.Model.EtiquetaProcesso>(() =>
            {
                var destino = atual ?? new Data.Model.EtiquetaProcesso();
                this.ConverterDtoParaModelo(destino);

                return destino;
            });
        }

        /// <summary>
        /// Realiza a conversão para a model.
        /// </summary>
        /// <returns>A model de processo de etiqueta preenchida.</returns>
        public Data.Model.EtiquetaProcesso ConverterParaProcesso()
        {
            return this.processo.Value;
        }

        private void ConverterDtoParaModelo(Data.Model.EtiquetaProcesso destino)
        {
            destino.CodInterno = this.cadastro.Codigo ?? destino.CodInterno;
            destino.Descricao = this.cadastro.Descricao ?? destino.Descricao;
            destino.IdAplicacao = this.cadastro.IdAplicacao ?? destino.IdAplicacao;
            destino.DestacarEtiqueta = this.cadastro.DestacarNaEtiqueta ?? destino.DestacarEtiqueta;
            destino.GerarFormaInexistente = this.cadastro.GerarFormaInexistente ?? destino.GerarFormaInexistente;
            destino.GerarArquivoDeMesa = this.cadastro.GerarArquivoDeMesa ?? destino.GerarArquivoDeMesa;
            destino.NumeroDiasUteisDataEntrega = this.cadastro.NumeroDiasUteisDataEntrega ?? destino.NumeroDiasUteisDataEntrega;
            destino.TipoProcesso = this.cadastro.TipoProcesso ?? destino.TipoProcesso;
            destino.Situacao = this.cadastro.Situacao ?? destino.Situacao;

            this.ConverterTiposPedidos(destino);
        }

        private void ConverterTiposPedidos(Data.Model.EtiquetaProcesso destino)
        {
            if (this.cadastro.TiposPedidos == null)
            {
                return;
            }

            destino.TipoPedido = string.Join(
                ",",
                this.cadastro.TiposPedidos
                    .Select(tipoPedido => (int)tipoPedido));
        }
    }
}
