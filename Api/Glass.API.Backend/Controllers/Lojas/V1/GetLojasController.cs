// <copyright file="GetLojasController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Genericas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Lojas.V1
{
    /// <summary>
    /// Controller de lojas.
    /// </summary>
    public partial class LojasController : BaseController
    {
        /// <summary>
        /// Recupera as lojas para os controles de filtro das telas.
        /// </summary>
        /// <param name="ativas">Indica se apenas as lojas ativas devem ser retornadas.</param>
        /// <returns>Uma lista JSON com as lojas encontradas.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Lojas encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Lojas não encontradas.")]
        public IHttpActionResult ObterLojasParaFiltro(bool? ativas = null)
        {
            using (var sessao = new GDATransaction())
            {
                Situacao situacaoLojaBuscar = 0;

                if (ativas.HasValue)
                {
                    situacaoLojaBuscar = ativas.Value
                        ? Situacao.Ativo
                        : Situacao.Inativo;
                }

                var situacoes = LojaDAO.Instance.GetAll(sessao)
                    .Where(l => !ativas.HasValue || l.Situacao == situacaoLojaBuscar)
                    .Select(l => new IdNomeDto
                    {
                        Id = l.IdLoja,
                        Nome = l.NomeFantasia,
                    });

                return this.Lista(situacoes);
            }
        }
    }
}
