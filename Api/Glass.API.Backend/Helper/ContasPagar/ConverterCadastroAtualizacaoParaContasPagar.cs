// <copyright file="ConverterCadastroAtualizacaoParaContasPagar.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.ContasPagar;
using Glass.API.Backend.Models.ContasPagar.V1.CadastroAtualizacao;
using System;

namespace Glass.API.Backend.Helper.ContasPagar
{
    /// <summary>
    /// Classe que realiza a tradução entre o DTO e a model para cadastro ou atualização de contas a pagar.
    /// </summary>
    public class ConverterCadastroAtualizacaoParaContasPagar
    {
        private readonly CadastroAtualizacaoDto cadastro;
        private readonly Lazy<Data.Model.ContasPagar> contaAPagar;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConverterCadastroAtualizacaoParaContasPagar"/>.
        /// </summary>
        /// <param name="cadastro">O DTO de cadastro, enviado para o endpoint.</param>
        /// <param name="atual">A conta a pagar atual (opcional), para que sejam aproveitados os valores, se necessário.</param>
        public ConverterCadastroAtualizacaoParaContasPagar(
            CadastroAtualizacaoDto cadastro,
            Data.Model.ContasPagar atual = null)
        {
            this.cadastro = cadastro;
            this.contaAPagar = new Lazy<Data.Model.ContasPagar>(() =>
            {
                var destino = atual ?? new Data.Model.ContasPagar();
                this.ConverterDtoParaModelo(destino);

                return destino;
            });
        }

        /// <summary>
        /// Realiza a conversão para a model.
        /// </summary>
        /// <returns>A model de conta a pagar preenchida.</returns>
        public Data.Model.ContasPagar ConverterParaContasPagar()
        {
            return this.contaAPagar.Value;
        }

        private void ConverterDtoParaModelo(Data.Model.ContasPagar destino)
        {
            destino.IdConta = (uint?)this.cadastro.ObterValorNormalizado(c => c.IdPlanoConta, (int?)destino.IdConta);
            destino.IdFormaPagto = (uint?)this.cadastro.ObterValorNormalizado(c => c.IdFormaPagamento, (int?)destino.IdFormaPagto);
            destino.DataVenc = (DateTime)this.cadastro.ObterValorNormalizado(c => c.DataVencimento, (DateTime?)destino.DataVenc);
            destino.Obs = this.cadastro.ObterValorNormalizado(c => c.Observacao, destino.Obs);
        }
    }
}