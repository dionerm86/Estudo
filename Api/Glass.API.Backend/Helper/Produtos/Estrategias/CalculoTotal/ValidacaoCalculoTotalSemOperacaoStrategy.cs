// <copyright file="ValidacaoCalculoAreaM2SemOperacaoStrategy.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Produtos.V1.CalculoTotal;
using System.Web.Http;

namespace Glass.API.Backend.Helper.Produtos.Estrategias.CalculoTotal
{
    /// <summary>
    /// Classe para ignorar a validação do cálculo de área em m².
    /// </summary>
    internal class ValidacaoCalculoTotalSemOperacaoStrategy : IValidacaoCalculoTotal
    {
        /// <inheritdoc/>
        public IHttpActionResult ValidarAntesCalculo(GDASession sessao, string dadosAdicionaisValidacao)
        {
            return null;
        }

        /// <inheritdoc/>
        public IHttpActionResult ValidarDepoisCalculo(GDASession sessao, TotalCalculadoDto totalCalculado, string dadosAdicionaisValidacao)
        {
            return null;
        }

        /// <inheritdoc/>
        public decimal ObterPercentualDescontoQuantidade(string dadosAdicionaisValidacao)
        {
            return 0;
        }
    }
}
