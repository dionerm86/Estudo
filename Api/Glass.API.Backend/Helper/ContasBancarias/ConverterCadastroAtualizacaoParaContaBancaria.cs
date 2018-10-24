// <copyright file="ConverterCadastroAtualizacaoParaContaBancaria.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.ContasBancarias.V1.CadastroAtualizacao;
using Glass.Financeiro.Negocios.Entidades;
using System;

namespace Glass.API.Backend.Helper.ContasBancarias
{
    /// <summary>
    /// Classe que realiza a tradução entre o DTO e a model para cadastro ou atualização de contas bancárias.
    /// </summary>
    internal class ConverterCadastroAtualizacaoParaContaBancaria
    {
        private readonly CadastroAtualizacaoDto cadastro;
        private readonly Lazy<ContaBanco> contaBancaria;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConverterCadastroAtualizacaoParaContaBancaria"/>.
        /// </summary>
        /// <param name="cadastro">O DTO de cadastro, enviado para o endpoint.</param>
        /// <param name="atual">O turno atual (opcional), para que sejam aproveitados os valores, se necessário.</param>
        public ConverterCadastroAtualizacaoParaContaBancaria(
            CadastroAtualizacaoDto cadastro,
            ContaBanco atual = null)
        {
            this.cadastro = cadastro;
            this.contaBancaria = new Lazy<ContaBanco>(() =>
            {
                var destino = atual ?? new ContaBanco();
                this.ConverterDtoParaModelo(destino);

                return destino;
            });
        }

        /// <summary>
        /// Realiza a conversão para a model.
        /// </summary>
        /// <returns>A model de conta bancária preenchida.</returns>
        public ContaBanco ConverterParaContaBancaria()
        {
            return this.contaBancaria.Value;
        }

        private void ConverterDtoParaModelo(ContaBanco destino)
        {
            destino.Nome = this.cadastro.ObterValorNormalizado(c => c.Nome, destino.Nome);
            destino.Situacao = this.cadastro.ObterValorNormalizado(c => c.Situacao, destino.Situacao);
            destino.IdLoja = this.cadastro.ObterValorNormalizado(c => c.IdLoja, (int?)destino.IdLoja).GetValueOrDefault();

            this.ConverterDadosBancarios(destino);
            this.ConverterDadosCnab(destino);
        }

        private void ConverterDadosBancarios(ContaBanco destino)
        {
            if (!this.cadastro.VerificarCampoInformado(c => c.DadosBanco))
            {
                return;
            }

            destino.CodBanco = this.cadastro.DadosBanco.ObterValorNormalizado(c => c.Banco, destino.CodBanco);
            destino.Titular = this.cadastro.DadosBanco.ObterValorNormalizado(c => c.Titular, destino.Titular);
            destino.Agencia = this.cadastro.DadosBanco.ObterValorNormalizado(c => c.Agencia, destino.Agencia);
            destino.Conta = this.cadastro.DadosBanco.ObterValorNormalizado(c => c.Conta, destino.Conta);
            destino.CodConvenio = this.cadastro.DadosBanco.ObterValorNormalizado(c => c.CodigoConvenio, destino.CodConvenio);
        }

        private void ConverterDadosCnab(ContaBanco destino)
        {
            if (!this.cadastro.VerificarCampoInformado(c => c.Cnab))
            {
                return;
            }

            destino.CodCliente = this.cadastro.Cnab.ObterValorNormalizado(c => c.CodigoCliente, destino.CodCliente);
            destino.Posto = this.cadastro.Cnab.ObterValorNormalizado(c => c.Posto, destino.Posto);
        }
    }
}
