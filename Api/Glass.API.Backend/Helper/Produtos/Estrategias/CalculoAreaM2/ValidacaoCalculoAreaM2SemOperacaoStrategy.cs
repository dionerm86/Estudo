// <copyright file="ValidacaoCalculoAreaM2SemOperacaoStrategy.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Produtos.CalculoAreaM2;
using System.Web.Http;

namespace Glass.API.Backend.Helper.Produtos.Estrategias.CalculoAreaM2
{
    /// <summary>
    /// Classe para ignorar a validação do cálculo de área em m².
    /// </summary>
    internal class ValidacaoCalculoAreaM2SemOperacaoStrategy : IValidacaoCalculoAreaM2
    {
        /// <inheritdoc/>
        public IHttpActionResult ValidarAntesCalculo(GDASession sessao, string dadosAdicionaisValidacao)
        {
            return null;
        }

        /// <inheritdoc/>
        public IHttpActionResult ValidarDepoisCalculo(GDASession sessao, AreaCalculadaDto areaCalculada, string dadosAdicionaisValidacao)
        {
            return null;
        }
    }
}
