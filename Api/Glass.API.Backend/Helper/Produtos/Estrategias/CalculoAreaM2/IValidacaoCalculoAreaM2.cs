// <copyright file="IValidacaoCalculoAreaM2.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Produtos.V1.CalculoAreaM2;
using System.Web.Http;

namespace Glass.API.Backend.Helper.Produtos.Estrategias.CalculoAreaM2
{
    /// <summary>
    /// Interface com os métodos para validação do cálculo de área em m².
    /// </summary>
    internal interface IValidacaoCalculoAreaM2
    {
        /// <summary>
        /// Validação executada antes do cálculo.
        /// </summary>
        /// <param name="sessao">A transação do banco de dados que será utilizada.</param>
        /// <param name="dadosAdicionaisValidacao">Os dados adicionais para a validação do produto, se necessário.</param>
        /// <returns>A resposta HTTP, caso ocorra algum erro de validação.</returns>
        IHttpActionResult ValidarAntesCalculo(GDASession sessao, string dadosAdicionaisValidacao);

        /// <summary>
        /// Validação executada depois do cálculo.
        /// </summary>
        /// <param name="sessao">A transação do banco de dados que será utilizada.</param>
        /// <param name="areaCalculada">A área calculada pelo serviço.</param>
        /// <param name="dadosAdicionaisValidacao">Os dados adicionais para a validação do produto, se necessário.</param>
        /// <returns>A resposta HTTP, caso ocorra algum erro de validação.</returns>
        IHttpActionResult ValidarDepoisCalculo(GDASession sessao, AreaCalculadaDto areaCalculada, string dadosAdicionaisValidacao);
    }
}
