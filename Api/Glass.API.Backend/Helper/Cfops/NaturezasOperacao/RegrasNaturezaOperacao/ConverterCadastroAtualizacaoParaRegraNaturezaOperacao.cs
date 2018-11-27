// <copyright file="ConverterCadastroAtualizacaoParaRegraNaturezaOperacao.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Cfops.V1.NaturezasOperacao.RegrasNaturezaOperacao.CadastroAtualizacao;
using Glass.Fiscal.Negocios.Entidades;
using System;

namespace Glass.API.Backend.Helper.Cfops.NaturezasOperacao.RegrasNaturezaOperacao
{
    /// <summary>
    /// Classe que realiza a tradução entre o DTO e a model para cadastro ou atualização da regra.
    /// </summary>
    internal class ConverterCadastroAtualizacaoParaRegraNaturezaOperacao
    {
        private readonly CadastroAtualizacaoDto cadastro;
        private readonly Lazy<RegraNaturezaOperacao> regra;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConverterCadastroAtualizacaoParaRegraNaturezaOperacao"/>.
        /// </summary>
        /// <param name="cadastro">O DTO de cadastro, enviado para o endpoint.</param>
        /// <param name="atual">A regra atual (opcional), para que sejam aproveitados os valores, se necessário.</param>
        public ConverterCadastroAtualizacaoParaRegraNaturezaOperacao(
            CadastroAtualizacaoDto cadastro,
            RegraNaturezaOperacao atual = null)
        {
            this.cadastro = cadastro;
            this.regra = new Lazy<RegraNaturezaOperacao>(() =>
            {
                var destino = atual ?? new RegraNaturezaOperacao();
                this.ConverterDtoParaModelo(destino);

                return destino;
            });
        }

        /// <summary>
        /// Realiza a conversão para a model.
        /// </summary>
        /// <returns>A model de regra preenchida.</returns>
        public RegraNaturezaOperacao ConverterParaRegraNaturezaOperacao()
        {
            return this.regra.Value;
        }

        private void ConverterDtoParaModelo(RegraNaturezaOperacao destino)
        {
            destino.IdLoja = this.cadastro.ObterValorNormalizado(c => c.IdLoja, destino.IdLoja);
            destino.IdTipoCliente = this.cadastro.ObterValorNormalizado(c => c.IdTipoCliente, destino.IdTipoCliente);
            destino.UfDest = string.Join(",", this.cadastro.ObterValorNormalizado(c => c.UfsDestino, destino.UfDest?.Split(',') ?? null) ?? new string[0]);

            this.ConverterDadosProduto(destino);
            this.ConverterDadosNaturezaOperacaoProducao(destino);
            this.ConverterDadosNaturezaOperacaoRevenda(destino);
        }

        private void ConverterDadosProduto(RegraNaturezaOperacao destino)
        {
            if (!this.cadastro.VerificarCampoInformado(c => c.Produto))
            {
                return;
            }

            destino.IdGrupoProd = this.cadastro.Produto.ObterValorNormalizado(c => c.IdGrupoProduto, destino.IdGrupoProd);
            destino.IdSubgrupoProd = this.cadastro.Produto.ObterValorNormalizado(c => c.IdSubgrupoProduto, destino.IdSubgrupoProd);
            destino.Espessura = this.cadastro.Produto.ObterValorNormalizado(c => c.Espessura, destino.Espessura);

            this.ConverterDadosCores(destino);
        }

        private void ConverterDadosCores(RegraNaturezaOperacao destino)
        {
            if (!this.cadastro.Produto.VerificarCampoInformado(c => c.Cores))
            {
                return;
            }

            destino.IdCorVidro = this.cadastro.Produto.Cores.ObterValorNormalizado(c => c.Vidro, destino.IdCorVidro);
            destino.IdCorFerragem = this.cadastro.Produto.Cores.ObterValorNormalizado(c => c.Ferragem, destino.IdCorFerragem);
            destino.IdCorAluminio = this.cadastro.Produto.Cores.ObterValorNormalizado(c => c.Aluminio, destino.IdCorAluminio);
        }

        private void ConverterDadosNaturezaOperacaoProducao(RegraNaturezaOperacao destino)
        {
            if (!this.cadastro.VerificarCampoInformado(c => c.NaturezaOperacaoProducao))
            {
                return;
            }

            destino.IdNaturezaOperacaoProdInter = this.cadastro.NaturezaOperacaoProducao.ObterValorNormalizado(c => c.Interestadual, destino.IdNaturezaOperacaoProdInter);
            destino.IdNaturezaOperacaoProdIntra = this.cadastro.NaturezaOperacaoProducao.ObterValorNormalizado(c => c.Intraestadual, destino.IdNaturezaOperacaoProdIntra);
            destino.IdNaturezaOperacaoProdStInter = this.cadastro.NaturezaOperacaoProducao.ObterValorNormalizado(c => c.InterestadualComSt, destino.IdNaturezaOperacaoProdStInter);
            destino.IdNaturezaOperacaoProdStIntra = this.cadastro.NaturezaOperacaoProducao.ObterValorNormalizado(c => c.IntraestadualComSt, destino.IdNaturezaOperacaoProdStIntra);
        }

        private void ConverterDadosNaturezaOperacaoRevenda(RegraNaturezaOperacao destino)
        {
            if (!this.cadastro.VerificarCampoInformado(c => c.NaturezaOperacaoRevenda))
            {
                return;
            }

            destino.IdNaturezaOperacaoRevInter = this.cadastro.NaturezaOperacaoRevenda.ObterValorNormalizado(c => c.Interestadual, destino.IdNaturezaOperacaoRevInter);
            destino.IdNaturezaOperacaoRevIntra = this.cadastro.NaturezaOperacaoRevenda.ObterValorNormalizado(c => c.Intraestadual, destino.IdNaturezaOperacaoRevIntra);
            destino.IdNaturezaOperacaoRevStInter = this.cadastro.NaturezaOperacaoRevenda.ObterValorNormalizado(c => c.InterestadualComSt, destino.IdNaturezaOperacaoRevStInter);
            destino.IdNaturezaOperacaoRevStIntra = this.cadastro.NaturezaOperacaoRevenda.ObterValorNormalizado(c => c.IntraestadualComSt, destino.IdNaturezaOperacaoRevStIntra);
        }
    }
}
