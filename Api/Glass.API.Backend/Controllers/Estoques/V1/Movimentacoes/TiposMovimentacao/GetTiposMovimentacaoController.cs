// <copyright file="GetMovimentacaoEstoqueRealController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Data.Model;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Estoques.V1.Movimentacoes.TiposMovimentacao
{
    /// <summary>
    /// Controller de tipos de movimentação.
    /// </summary>
    public class GetTiposMovimentacaoController : BaseController
    {
        /// <summary>
        /// Recupera a lista de movimentações do estoque real.
        /// </summary>
        /// <returns>Uma lista JSON com os dados dos estoques.</returns>
        [HttpGet]
        [Route("api/v1/estoques/movimentacoes/tiposMovimentacao")]
        [SwaggerResponse(200, "Tipos de Movimentação encontrados.", Type = typeof(IEnumerable<Models.Estoques.V1.Movimentacoes.TiposMovimentacao.TipoMovimentação>))]
        public IHttpActionResult ObterTiposMovimentacao()
        {
            var tiposMovimentacao = new Helper.ConversorEnum<Models.Estoques.V1.Movimentacoes.TiposMovimentacao.TipoMovimentação>()
                .ObterTraducao();

            return this.Lista(tiposMovimentacao);
        }
    }
}