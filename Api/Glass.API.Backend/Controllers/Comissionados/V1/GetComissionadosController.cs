// <copyright file="GetComissionadosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Genericas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Comissionados.V1
{
    /// <summary>
    /// Controller de clientes.
    /// </summary>
    public partial class ComissionadosController : BaseController
    {
        /// <summary>
        /// Recupera os comissionados para o controle de pesquisa.
        /// </summary>
        /// <param name="id">O identificador do comissionado para pesquisa.</param>
        /// <param name="nome">O nome do comissionado para pesquisa.</param>
        /// <returns>Uma lista JSON com os dados dos comissionados encontrados.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Comissionados encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Comissionados não encontrados.")]
        [SwaggerResponse(400, "Filtros não informados.", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterFiltro(int? id = null, string nome = null)
        {
            using (var sessao = new GDATransaction())
            {
                var comissionados = ComissionadoDAO.Instance.ObterAtivos(
                    sessao,
                    id.GetValueOrDefault(),
                    nome)
                    .Select(c => new IdNomeDto()
                    {
                        Id = c.IdComissionado,
                        Nome = c.Nome,
                    });

                return this.Lista(comissionados);
            }
        }
    }
}
