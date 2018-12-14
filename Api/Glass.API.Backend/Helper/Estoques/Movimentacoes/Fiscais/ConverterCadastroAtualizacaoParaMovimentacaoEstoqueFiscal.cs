// <copyright file="ConverterCadastroAtualizacaoParaMovimentacaoEstoqueFiscal.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Estoques.V1.Movimentacoes.Fiscais.CadastroAtualizacao;
using Glass.Data.Helper;
using System;

namespace Glass.API.Backend.Helper.Estoques.Movimentacoes.Fiscais
{
    /// <summary>
    /// Classe que realiza a tradução entre o DTO e a model para cadastro ou atualização de movimentações do estoque fiscal.
    /// </summary>
    internal class ConverterCadastroAtualizacaoParaMovimentacaoEstoqueFiscal
    {
        private readonly CadastroAtualizacaoDto cadastro;
        private readonly Lazy<Glass.Data.Model.MovEstoqueFiscal> movimentacaoEstoque;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConverterCadastroAtualizacaoParaMovimentacaoEstoqueFiscal"/>.
        /// </summary>
        /// <param name="cadastro">O DTO de cadastro, enviado para o endpoint.</param>
        public ConverterCadastroAtualizacaoParaMovimentacaoEstoqueFiscal(
            CadastroAtualizacaoDto cadastro)
        {
            this.cadastro = cadastro;
            this.movimentacaoEstoque = new Lazy<Data.Model.MovEstoqueFiscal>(() =>
            {
                var destino = new Glass.Data.Model.MovEstoqueFiscal();
                this.ConverterDtoParaModelo(destino);

                return destino;
            });
        }

        /// <summary>
        /// Realiza a conversão para a model.
        /// </summary>
        /// <returns>A model de movimentação do estoque fiscal preenchida.</returns>
        public Glass.Data.Model.MovEstoqueFiscal ConverterParaMovimentacaoEstoqueFiscal()
        {
            return this.movimentacaoEstoque.Value;
        }

        private void ConverterDtoParaModelo(Glass.Data.Model.MovEstoqueFiscal destino)
        {
            destino.DataMov = this.cadastro.ObterValorNormalizado(c => c.DataMovimentacao, destino.DataMov);
            destino.QtdeMov = this.cadastro.ObterValorNormalizado(c => c.Quantidade, destino.QtdeMov);
            destino.TipoMov = this.cadastro.ObterValorNormalizado(c => c.TipoMovimentacao, destino.TipoMov);
            destino.ValorMov = this.cadastro.ObterValorNormalizado(c => c.Valor, destino.ValorMov);
            destino.Obs = this.cadastro.ObterValorNormalizado(c => c.Observacao, destino.Obs);
            destino.IdProd = (uint)this.cadastro.ObterValorNormalizado(c => c.IdProduto, (int)destino.IdProd);
            destino.IdLoja = (uint)this.cadastro.ObterValorNormalizado(c => c.IdLoja, (int)destino.IdLoja);
        }
    }
}
