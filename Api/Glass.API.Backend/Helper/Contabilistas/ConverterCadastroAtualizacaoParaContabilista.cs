// <copyright file="ConverterCadastroAtualizacaoParaContabilista.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Contabilistas.V1.CadastroAtualizacao;
using Glass.Data.Model;
using System;

namespace Glass.API.Backend.Helper.Contabilistas
{
    /// <summary>
    /// Classe que realiza a tradução entre o DTO e a model para cadastro ou atualização de contabilistas.
    /// </summary>
    internal class ConverterCadastroAtualizacaoParaContabilista
    {
        private readonly CadastroAtualizacaoDto cadastro;
        private readonly Lazy<Contabilista> contabilista;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConverterCadastroAtualizacaoParaContabilista"/>.
        /// </summary>
        /// <param name="cadastro">O DTO de cadastro, enviado para o endpoint.</param>
        /// <param name="atual">O contabilista atual (opcional), para que sejam aproveitados os valores, se necessário.</param>
        public ConverterCadastroAtualizacaoParaContabilista(
            CadastroAtualizacaoDto cadastro,
            Contabilista atual = null)
        {
            this.cadastro = cadastro;
            this.contabilista = new Lazy<Contabilista>(() =>
            {
                var destino = atual ?? new Contabilista();
                this.ConverterDtoParaModelo(destino);

                return destino;
            });
        }

        /// <summary>
        /// Realiza a conversão para a model.
        /// </summary>
        /// <returns>A model de conta bancária preenchida.</returns>
        public Contabilista ConverterParaContabilista()
        {
            return this.contabilista.Value;
        }

        private void ConverterDtoParaModelo(Contabilista destino)
        {
            destino.Nome = this.cadastro.ObterValorNormalizado(c => c.Nome, destino.Nome);
            destino.TipoPessoa = this.cadastro.ObterValorNormalizado(c => c.TipoPessoa, destino.TipoPessoa == "F" ? TipoPessoa.Fisica : TipoPessoa.Juridica) == TipoPessoa.Fisica ? "F" : "J";
            destino.CpfCnpj = this.cadastro.ObterValorNormalizado(c => c.CpfCnpj, destino.CpfCnpj);
            destino.Crc = this.cadastro.ObterValorNormalizado(c => c.Crc, destino.Crc);
            destino.Situacao = (int)this.cadastro.ObterValorNormalizado(c => c.Situacao, (Situacao)destino.Situacao);

            this.ConverterDadosContato(destino);
            this.ConverterDadosEndereco(destino);
        }

        private void ConverterDadosContato(Contabilista destino)
        {
            if (!this.cadastro.VerificarCampoInformado(c => c.DadosContato))
            {
                return;
            }

            destino.TelCont = this.cadastro.DadosContato.ObterValorNormalizado(c => c.Telefone, destino.TelCont);
            destino.Fax = this.cadastro.DadosContato.ObterValorNormalizado(c => c.Fax, destino.Fax);
            destino.Email = this.cadastro.DadosContato.ObterValorNormalizado(c => c.Email, destino.Email);
        }

        private void ConverterDadosEndereco(Contabilista destino)
        {
            if (!this.cadastro.VerificarCampoInformado(c => c.Endereco))
            {
                return;
            }

            destino.Endereco = this.cadastro.Endereco.ObterValorNormalizado(c => c.Logradouro, destino.Endereco);
            destino.Numero = this.cadastro.Endereco.ObterValorNormalizado(c => c.Numero, destino.Numero);
            destino.Compl = this.cadastro.Endereco.ObterValorNormalizado(c => c.Complemento, destino.Compl);
            destino.Bairro = this.cadastro.Endereco.ObterValorNormalizado(c => c.Bairro, destino.Bairro);
            destino.Cep = this.cadastro.Endereco.ObterValorNormalizado(c => c.Cep, destino.Cep);

            if (this.cadastro.Endereco.VerificarCampoInformado(c => c.Cidade))
            {
                destino.IdCidade = (uint)this.cadastro.Endereco.Cidade.ObterValorNormalizado(c => c.Id, (int)destino.IdCidade);
            }
        }
    }
}
