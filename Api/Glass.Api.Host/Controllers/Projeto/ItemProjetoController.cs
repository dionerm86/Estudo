using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Glass.Api.Host.Controllers.Projeto
{
    public class ItemProjetoController : ApiController
    {
        /// <summary>
        /// Recupera o item projeto.
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <returns></returns>
        [Route("ItemProjeto/obteritemprojeto/{idItemProjeto}")]
        [HttpGet, Authorize]
        public IHttpActionResult ObterItemProjeto([FromUri]int idItemProjeto)
        {
            try
            {
                return Ok(ServiceLocator.Current.GetInstance<Glass.Api.Projeto.ICalculaProjetoFluxo>().ObterItemProjeto(idItemProjeto));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        ///  Cria um item projeto.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost, Authorize]
        public IHttpActionResult CriarItemProjeto([FromBody]Glass.Api.Implementacao.Projeto.ItemProjeto item)
        {
            try
            {
                return Ok(ServiceLocator.Current.GetInstance<Glass.Api.Projeto.ICalculaProjetoFluxo>().CriarItemProjeto(item.IdProjeto,
                    item.IdProjetoModelo, item.EspessuraVidro,
                    item.IdCorVidro, 1, 1, true, item.MedidaExata));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Remove um item de projeto.
        /// </summary>
        /// <param name="idItemProjeto"></param>
        /// <returns></returns>
        [Route("ItemProjeto/DeletarItemProjeto/{idItemProjeto}")]
        [HttpPost, Authorize]
        public IHttpActionResult DeletarItemProjeto(int idItemProjeto)
        {
            try
            {
                return Ok(ServiceLocator.Current.GetInstance<Glass.Api.Projeto.ICalculaProjetoFluxo>().DeletarItemProjeto(idItemProjeto));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
