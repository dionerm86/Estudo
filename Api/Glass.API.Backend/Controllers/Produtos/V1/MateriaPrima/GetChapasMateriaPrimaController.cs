// <copyright file="GetChapasMateriaPrimaController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Produtos.V1.MateriaPrima.Lista;
using Glass.Data.RelDAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Produtos.V1.MateriaPrima
{
    /// <summary>
    /// Controller de chapas de matéria prima.
    /// </summary>
    public partial class ChapasMateriaPrimaController : BaseController
    {
        /// <summary>
        /// Recupera a lista de chapas de matéria prima.
        /// </summary>
        /// <param name="filtro">Os filtros para busca das chapas de matéria prima.</param>
        /// <returns>Uma lista JSON com os dados das chapas de matéria prima.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Chapas de matéria prima encontradas.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Chapas de matéria prima não encontradas.")]
        public IHttpActionResult ObterListaChapasMateriaPrima([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new FiltroDto();

                var validacao = this.ValidarExistenciaEspessuraECorVidro(
                    sessao,
                    filtro.IdCorVidro ?? 0,
                    filtro.Espessura ?? 0);

                if (validacao != null)
                {
                    return validacao;
                }

                var chapasMateriaPrima = PosicaoMateriaPrimaChapaDAO.Instance.GetChapaByCorEsp(
                    (uint)(filtro.IdCorVidro ?? 0),
                    (float)(filtro.Espessura ?? 0),
                    null,
                    null);

                return this.Lista(
                    chapasMateriaPrima
                        .Select(cmp => new ListaDto(cmp)));
            }
        }
    }
}
