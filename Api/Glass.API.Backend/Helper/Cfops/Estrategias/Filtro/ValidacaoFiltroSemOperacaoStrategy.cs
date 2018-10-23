// <copyright file="ValidacaoFiltroSemOperacaoStrategy.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Cfops.V1.Filtro;
using Glass.Data.Model;
using System.Web.Http;

namespace Glass.API.Backend.Helper.Cfops.Estrategias.Filtro
{
    /// <summary>
    /// Classe para ignorar a validação do filtro de CFOP's.
    /// </summary>
    internal class ValidacaoFiltroSemOperacaoStrategy : IValidacaoFiltro
    {
        /// <inheritdoc/>
        public IHttpActionResult ValidarAntesBusca(GDASession sessao, string codigoInterno, string dadosAdicionaisValidacao)
        {
            return null;
        }

        /// <inheritdoc/>
        public IHttpActionResult ValidarDepoisBusca(GDASession sessao, string codigoInterno, string dadosAdicionaisValidacao, Cfop cfop)
        {
            return null;
        }

        /// <summary>
        /// Cria o CFOP, com base no tipo de validação.
        /// </summary>
        /// <param name="sessao">A transação do banco de dados que será utilizada.</param>
        /// <param name="dadosAdicionaisValidacao">Os dados adicionais para a validação do CFOP, se necessário.</param>
        /// <param name="cfop">O CFOP encontrado.</param>
        /// <returns>O DTO com o CFOP para o tipo de validação.</returns>
        public CfopDto ObterCfop(GDASession sessao, string dadosAdicionaisValidacao, Cfop cfop)
        {
            return new CfopDto(sessao, cfop);
        }
    }
}
