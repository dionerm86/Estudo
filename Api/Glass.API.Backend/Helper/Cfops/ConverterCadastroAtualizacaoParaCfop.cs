// <copyright file="ConverterCadastroAtualizacaoParaCfop.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Cfops.V1.CadastroAtualizacao;
using Glass.Fiscal.Negocios.Entidades;
using System;

namespace Glass.API.Backend.Helper.Cfops
{
    /// <summary>
    /// Classe que realiza a tradução entre o DTO e a model para cadastro ou atualização do CFOP.
    /// </summary>
    internal class ConverterCadastroAtualizacaoParaCfop
    {
        private readonly CadastroAtualizacaoDto cadastro;
        private readonly Lazy<Cfop> cfop;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConverterCadastroAtualizacaoParaCfop"/>.
        /// </summary>
        /// <param name="cadastro">O DTO de cadastro, enviado para o endpoint.</param>
        /// <param name="atual">O CFOP atual (opcional), para que sejam aproveitados os valores, se necessário.</param>
        public ConverterCadastroAtualizacaoParaCfop(
            CadastroAtualizacaoDto cadastro,
            Cfop atual = null)
        {
            this.cadastro = cadastro;
            this.cfop = new Lazy<Cfop>(() =>
            {
                var destino = atual ?? new Cfop();
                this.ConverterDtoParaModelo(destino);

                return destino;
            });
        }

        /// <summary>
        /// Realiza a conversão para a model.
        /// </summary>
        /// <returns>A model de CFOP preenchida.</returns>
        public Cfop ConverterParaCfop()
        {
            return this.cfop.Value;
        }

        private void ConverterDtoParaModelo(Cfop destino)
        {
            destino.CodInterno = this.cadastro.ObterValorNormalizado(c => c.Codigo, destino.CodInterno);
            destino.Descricao = this.cadastro.ObterValorNormalizado(c => c.Nome, destino.Descricao);
            destino.IdTipoCfop = this.cadastro.ObterValorNormalizado(c => c.IdTipoCfop, destino.IdTipoCfop);
            destino.TipoMercadoria = this.cadastro.ObterValorNormalizado(c => c.TipoMercadoria, destino.TipoMercadoria);
            destino.AlterarEstoqueTerceiros = this.cadastro.ObterValorNormalizado(c => c.AlterarEstoqueTerceiros, destino.AlterarEstoqueTerceiros);
            destino.AlterarEstoqueCliente = this.cadastro.ObterValorNormalizado(c => c.AlterarEstoqueCliente, destino.AlterarEstoqueCliente);
            destino.Obs = this.cadastro.ObterValorNormalizado(c => c.Observacao, destino.Obs);
        }
    }
}
