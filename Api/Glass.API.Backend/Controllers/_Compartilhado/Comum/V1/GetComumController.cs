// <copyright file="GetComumController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Genericas.V1;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Comum.V1
{
    /// <summary>
    /// Controller comum ao projeto.
    /// </summary>
    public partial class ComumController : BaseController
    {
        /// <summary>
        /// Recupera a lista de situações.
        /// </summary>
        /// <returns>Uma lista JSON com os dados básicos das situações.</returns>
        [HttpGet]
        [Route("situacoes")]
        [SwaggerResponse(200, "Situações encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Situações não encontradas.")]
        public IHttpActionResult ObterSituacoes()
        {
            using (var sessao = new GDATransaction())
            {
                var tipos = new ConversorEnum<Situacao>()
                    .ObterTraducao();

                return this.Lista(tipos);
            }
        }

        /// <summary>
        /// Recupera a lista de tipos de pessoa.
        /// </summary>
        /// <returns>Uma lista JSON com os dados básicos dos tipos de pessoa.</returns>
        [HttpGet]
        [Route("tiposPessoa")]
        [SwaggerResponse(200, "Tipos de pessoa encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Tipos de pessoa não encontrados.")]
        public IHttpActionResult ObterTiposPessoa()
        {
            using (var sessao = new GDATransaction())
            {
                var tipos = new ConversorEnum<Data.Model.TipoPessoa>()
                    .ObterTraducao();

                return this.Lista(tipos);
            }
        }
    }
}
