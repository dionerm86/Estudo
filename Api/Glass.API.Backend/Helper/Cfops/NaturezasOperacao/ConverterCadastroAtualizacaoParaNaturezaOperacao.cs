// <copyright file="ConverterCadastroAtualizacaoParaNaturezaOperacao.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Cfops.V1.NaturezasOperacao.CadastroAtualizacao;
using Glass.Fiscal.Negocios.Entidades;
using System;

namespace Glass.API.Backend.Helper.Cfops.NaturezasOperacao
{
    /// <summary>
    /// Classe que realiza a tradução entre o DTO e a model para cadastro ou atualização da natureza de operação.
    /// </summary>
    internal class ConverterCadastroAtualizacaoParaNaturezaOperacao
    {
        private readonly CadastroAtualizacaoDto cadastro;
        private readonly Lazy<NaturezaOperacao> naturezaOperacao;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConverterCadastroAtualizacaoParaNaturezaOperacao"/>.
        /// </summary>
        /// <param name="cadastro">O DTO de cadastro, enviado para o endpoint.</param>
        /// <param name="atual">A natureza de operação atual (opcional), para que sejam aproveitados os valores, se necessário.</param>
        public ConverterCadastroAtualizacaoParaNaturezaOperacao(
            CadastroAtualizacaoDto cadastro,
            NaturezaOperacao atual = null)
        {
            this.cadastro = cadastro;
            this.naturezaOperacao = new Lazy<NaturezaOperacao>(() =>
            {
                var destino = atual ?? new NaturezaOperacao();
                this.ConverterDtoParaModelo(destino);

                return destino;
            });
        }

        /// <summary>
        /// Realiza a conversão para a model.
        /// </summary>
        /// <returns>A model de natureza operação preenchida.</returns>
        public NaturezaOperacao ConverterParaNaturezaOperacao()
        {
            return this.naturezaOperacao.Value;
        }

        private void ConverterDtoParaModelo(NaturezaOperacao destino)
        {
            destino.CodInterno = this.cadastro.ObterValorNormalizado(c => c.Codigo, destino.CodInterno);
            destino.Mensagem = this.cadastro.ObterValorNormalizado(c => c.Mensagem, destino.Mensagem);
            destino.AlterarEstoqueFiscal = this.cadastro.ObterValorNormalizado(c => c.AlterarEstoqueFiscal, destino.AlterarEstoqueFiscal);
            destino.CalcEnergiaEletrica = this.cadastro.ObterValorNormalizado(c => c.CalculoDeEnergiaEletrica, destino.CalcEnergiaEletrica);
            destino.Ncm = this.cadastro.ObterValorNormalizado(c => c.Ncm, destino.Ncm);

            this.ConverterDadosIcms(destino);
            this.ConverterDadosIpi(destino);
            this.ConverterDadosPisCofins(destino);
        }

        private void ConverterDadosIcms(NaturezaOperacao destino)
        {
            if (!this.cadastro.VerificarCampoInformado(c => c.DadosIcms))
            {
                return;
            }

            destino.CstIcms = this.cadastro.DadosIcms.ObterValorNormalizado(c => c.CstIcms, destino.CstIcms);
            destino.Csosn = this.cadastro.DadosIcms.ObterValorNormalizado(c => c.Csosn, destino.Csosn);
            destino.CalcIcms = this.cadastro.DadosIcms.ObterValorNormalizado(c => c.CalcularIcms, destino.CalcIcms);
            destino.CalcIcmsSt = this.cadastro.DadosIcms.ObterValorNormalizado(c => c.CalcularIcmsSt, destino.CalcIcmsSt);
            destino.IpiIntegraBcIcms = this.cadastro.DadosIcms.ObterValorNormalizado(c => c.IpiIntegraBcIcms, destino.IpiIntegraBcIcms);
            destino.DebitarIcmsDesonTotalNf = this.cadastro.DadosIcms.ObterValorNormalizado(c => c.DebitarIcmsDesoneradoTotalNf, destino.DebitarIcmsDesonTotalNf);
            destino.PercReducaoBcIcms = (float)this.cadastro.DadosIcms.ObterValorNormalizado(c => c.PercentualReducaoBcIcms, (decimal)destino.PercReducaoBcIcms);
            destino.PercDiferimento = (float)this.cadastro.DadosIcms.ObterValorNormalizado(c => c.PercentualDiferimento, (decimal)destino.PercDiferimento);
            destino.CalcularDifal = this.cadastro.DadosIcms.ObterValorNormalizado(c => c.CalcularDifal, destino.CalcularDifal);
        }

        private void ConverterDadosIpi(NaturezaOperacao destino)
        {
            if (!this.cadastro.VerificarCampoInformado(c => c.DadosIpi))
            {
                return;
            }

            destino.CstIpi = this.cadastro.DadosIpi.ObterValorNormalizado(c => c.CstIpi, destino.CstIpi);
            destino.CalcIpi = this.cadastro.DadosIpi.ObterValorNormalizado(c => c.CalcularIpi, destino.CalcIpi);
            destino.FreteIntegraBcIpi = this.cadastro.DadosIpi.ObterValorNormalizado(c => c.FreteIntegraBcIpi, destino.FreteIntegraBcIpi);
            destino.CodEnqIpi = this.cadastro.DadosIpi.ObterValorNormalizado(c => c.CodigoEnquadramentoIpi, destino.CodEnqIpi);
        }

        private void ConverterDadosPisCofins(NaturezaOperacao destino)
        {
            if (!this.cadastro.VerificarCampoInformado(c => c.DadosPisCofins))
            {
                return;
            }

            destino.CstPisCofins = (int?)this.cadastro.DadosPisCofins.ObterValorNormalizado(c => c.CstPisCofins, (Data.EFD.DataSourcesEFD.CstPisCofinsEnum?)destino.CstPisCofins);
            destino.CalcPis = this.cadastro.DadosPisCofins.ObterValorNormalizado(c => c.CalcularPis, destino.CalcPis);
            destino.CalcCofins = this.cadastro.DadosPisCofins.ObterValorNormalizado(c => c.CalcularCofins, destino.CalcCofins);
        }
    }
}
