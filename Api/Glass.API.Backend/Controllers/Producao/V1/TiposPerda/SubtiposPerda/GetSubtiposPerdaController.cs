// <copyright file="GetSubtiposPerdaController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Genericas.V1;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Producao.V1.TiposPerda.SubtiposPerda
{
    /// <summary>
    /// Controller de subtipos de perda.
    /// </summary>
    public partial class SubtiposPerdaController : BaseController
    {
        /// <summary>
        /// Recupera a lista de subtipos de perda para uso em controles.
        /// </summary>
        /// <param name="idTipoPerda">O identificador do tipo de perda para busca dos subtipos.</param>
        /// <returns>Uma lista JSON com os dados básicos dos tipos de perda.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Subtipos de perda encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Subtipos de perda não encontrados.")]
        [SwaggerResponse(400, "Erro de validação ou de valor do campo id.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Tipo de perda não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterParaFiltro(int idTipoPerda)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdTipoPerda(sessao, idTipoPerda);

                if (validacao != null)
                {
                    return validacao;
                }

                var subtiposPerda = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<PCP.Negocios.IPerdaFluxo>()
                    .PesquisarSubtiposPerda(idTipoPerda)
                    .Select(t => new IdNomeDto
                    {
                        Id = t.IdSubtipoPerda,
                        Nome = t.Descricao,
                    });

                return this.Lista(subtiposPerda);
            }
        }
    }
}
