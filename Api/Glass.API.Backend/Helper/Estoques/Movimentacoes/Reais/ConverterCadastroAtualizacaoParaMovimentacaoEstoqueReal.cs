// <copyright file="ConverterCadastroAtualizacaoParaMovimentacaoEstoqueReal.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Estoques.V1.Movimentacoes.Reais.CadastroAtualizacao;
using System;

namespace Glass.API.Backend.Helper.Estoques.Movimentacoes.Reais
{
    /// <summary>
    /// Classe que realiza a tradução entre o DTO e a model para cadastro ou atualização de movimentações do estoque real.
    /// </summary>
    internal class ConverterCadastroAtualizacaoParaMovimentacaoEstoqueReal
    {
        private readonly CadastroAtualizacaoDto cadastro;
        private readonly Lazy<Glass.Data.Model.MovEstoque> movimentacaoEstoque;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConverterCadastroAtualizacaoParaMovimentacaoEstoqueReal"/>.
        /// </summary>
        /// <param name="cadastro">O DTO de cadastro, enviado para o endpoint.</param>
        public ConverterCadastroAtualizacaoParaMovimentacaoEstoqueReal(
            CadastroAtualizacaoDto cadastro)
        {
            this.movimentacaoEstoque = new Lazy<Data.Model.MovEstoque>(() =>
            {
                var destino = new Glass.Data.Model.MovEstoque();
                this.ConverterDtoParaModelo(destino);

                return destino;
            });
        }

        /// <summary>
        /// Realiza a conversão para a model.
        /// </summary>
        /// <returns>A model de movimentação do estoque real preenchida.</returns>
        public Glass.Data.Model.MovEstoque ConverterParaMovimentacaoEstoqueReal()
        {
            return this.movimentacaoEstoque.Value;
        }

        private void ConverterDtoParaModelo(Glass.Data.Model.MovEstoque destino)
        {
            destino.DataMov = this.cadastro.ObterValorNormalizado(c => c.DataMovimentacao, destino.DataMov);
            destino.QtdeMov = this.cadastro.ObterValorNormalizado(c => c.Quantidade, destino.QtdeMov);
            destino.TipoMov = this.cadastro.ObterValorNormalizado(c => c.TipoMovimentacao, destino.TipoMov);
            destino.ValorMov = this.cadastro.ObterValorNormalizado(c => c.ValorTotal, destino.ValorMov);
            destino.Obs = this.cadastro.ObterValorNormalizado(c => c.Observacao, destino.Obs);
        }
    }
}
