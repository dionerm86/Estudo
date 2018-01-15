using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Glass.Api.Host.Controllers.Projeto
{
    public class ProjetoModeloController : ApiController
    {
        /// <summary>
        /// Recupera os grupos de modelo.
        /// </summary>
        /// <returns></returns>
        [HttpGet, Authorize]
        public IHttpActionResult ObterGruposModelo()
        {
            try
            {
                return Ok(ServiceLocator.Current.GetInstance<Api.Projeto.IGrupoModeloFluxo>().ObterGruposModelo());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Recupera os projetos de modelo mais usados.
        /// </summary>
        /// <returns></returns>
        [Route("ProjetoModelo/ObterProjetosModeloMaisUsados")]
        [HttpGet, Authorize]
        public IHttpActionResult ObterProjetosMaisUsados()
        {
            try
            {
                return Ok(ServiceLocator.Current.GetInstance<Api.Projeto.IProjetoModeloFluxo>().ObterMaisUsados());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Recupera os projetos filtrados por grupo e/ou código.
        /// CalculaProjeto/ObterProjetos/Names?idGrupoModelo=15447&codigo=CAR33A
        /// </summary>
        /// <param name="idGruposModelo"></param>
        /// <param name="codigo"></param>
        /// <returns></returns>
        [Route("ProjetoModelo/ObterProjetosModelo/{idGrupoModelo?}/{codigo?}")]
        [HttpGet, Authorize]
        public IHttpActionResult ObterProjetosModelo(int idGrupoModelo = 0, string codigo = "")
        {
            try
            {
                return Ok(ServiceLocator.Current.GetInstance<Api.Projeto.IProjetoModeloFluxo>().ObterProjetoModelo(idGrupoModelo, codigo));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
