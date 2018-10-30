// <copyright file="GetNaturezasOperacaoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Cfops.V1.NaturezasOperacao
{
    /// <summary>
    /// Controller de naturezas de operação.
    /// </summary>
    public partial class NaturezasOperacaoController : BaseController
    {
        /// <summary>
        /// Recupera as naturezas de operação para o controle de pesquisa.
        /// </summary>
        /// <returns>Uma lista JSON com os dados dos itens encontrados.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Naturezas de operação encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Naturezas de operação não encontradas.")]
        public IHttpActionResult ObterFiltro()
        {
            using (var sessao = new GDATransaction())
            {
                var naturezas = NaturezaOperacaoDAO.Instance.ObtemTodosOrdenados(sessao)
                    .Select(c => new IdNomeDto()
                    {
                        Id = c.IdNaturezaOperacao,
                        Nome = c.CodInterno,
                    });

                return this.Lista(naturezas);
            }
        }
    }
}
