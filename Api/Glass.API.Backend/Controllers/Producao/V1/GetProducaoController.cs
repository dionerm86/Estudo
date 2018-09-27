// <copyright file="GetProducaoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Genericas;
using Glass.Data.Helper;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Producao.V1
{
    /// <summary>
    /// Controller de produção.
    /// </summary>
    public partial class ProducaoController : BaseController
    {
        /// <summary>
        /// Recupera a lista de situações de produção possíveis.
        /// </summary>
        /// <returns>Uma lista JSON com os dados básicos das situações de produção.</returns>
        [HttpGet]
        [Route("situacoes")]
        [SwaggerResponse(200, "Situações de produção encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Situações de produção não encontradas.")]
        public IHttpActionResult ObterSituacoes()
        {
            using (var sessao = new GDATransaction())
            {
                var situacoes = DataSources.Instance.GetSituacaoProducao()
                    .Select(s => new IdNomeDto
                    {
                        Id = (int)(s.Id ?? 0),
                        Nome = s.Descr,
                    });

                return this.Lista(situacoes);
            }
        }

        /// <summary>
        /// Recupera a lista de setores de produção.
        /// </summary>
        /// <returns>Uma lista JSON com os dados básicos das setores de produção.</returns>
        [HttpGet]
        [Route("setores")]
        [SwaggerResponse(200, "Setores de produção encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Setores de produção não encontrados.")]
        public IHttpActionResult ObterSetores()
        {
            using (var sessao = new GDATransaction())
            {
                var situacoes = Glass.Data.DAL.SetorDAO.Instance.GetAll()
                    .Select(s => new IdNomeDto
                    {
                        Id = s.IdSetor,
                        Nome = s.Descricao,
                    });

                return this.Lista(situacoes);
            }
        }
    }
}
