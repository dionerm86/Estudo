using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Glass.Api.Host.Controllers.Projeto
{
    public class ProjetoController : ApiController
    {
        /// <summary>
        ///  Recupera os projetos do usuário.
        /// </summary>
        /// <param name="pesquisaProjeto"></param>
        /// <param name="">idUsuario</param>
        /// <returns></returns>
        [Route("Projeto/ObterMeusProjetos/{idUsuario}")]
        [HttpPost, Authorize]
        public IHttpActionResult ObterMeusProjetos([FromBody] Glass.Api.Implementacao.Projeto.PesquisaProjeto pesquisaProjeto, [FromUri] int idUsuario)
        {
            try
            {
                if (idUsuario < 1) return BadRequest("Usuário não informado.");

                return Ok(ServiceLocator.Current.GetInstance<Api.Projeto.ICalculaProjetoFluxo>().ObterProjetos(pesquisaProjeto.IdProjeto, idUsuario,
                    pesquisaProjeto.DtIni != DateTime.MinValue ? pesquisaProjeto.DtIni.ToString("dd-MM-yyyy") : string.Empty,
                    pesquisaProjeto.DtFim != DateTime.MinValue ? pesquisaProjeto.DtFim.ToString("dd-MM-yyyy") : string.Empty,
                    pesquisaProjeto.StartRow, pesquisaProjeto.PageSize));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Cria novo projeto.        
        /// </summary>
        /// <param name="projeto"></param>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        [Route("Projeto/CriarProjeto/{idUsuario}")]
        [HttpPost, Authorize]
        public IHttpActionResult CriarProjeto([FromBody] Glass.Api.Implementacao.Projeto.Projeto projeto, [FromUri] int idUsuario)
        {
            try
            {
                if (idUsuario < 1) return BadRequest("Usuário não informado.");

                return Ok(ServiceLocator.Current.GetInstance<Api.Projeto.ICalculaProjetoFluxo>().CriarProjeto(projeto.IdTipoEntrega, idUsuario, projeto.CodPedido));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Atualiza o código do pedido.
        /// </summary>
        /// <param name="projeto"></param>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        [Route("Projeto/AtualizarCodPedido/{idUsuario}")]
        [HttpPost, Authorize]
        public IHttpActionResult AtualizarCodPedido([FromBody] Glass.Api.Implementacao.Projeto.Projeto projeto, [FromUri] int idUsuario)
        {
            try
            {
                if (idUsuario < 1) return BadRequest("Usuário não informado.");

                ServiceLocator.Current.GetInstance<Api.Projeto.ICalculaProjetoFluxo>().AtualizarCodPedido(projeto.IdProjeto, idUsuario, projeto.CodPedido);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Recupera um projeto baseado no id.
        /// </summary>
        /// <param name="idProjeto"></param>
        /// <returns></returns>
        [Route("Projeto/ObterProjeto/{idProjeto}")]
        [HttpGet, Authorize]
        public IHttpActionResult ObterProjeto(int idProjeto)
        {
            try
            {
                return Ok(ServiceLocator.Current.GetInstance<Api.Projeto.ICalculaProjetoFluxo>().ObterProjeto(idProjeto));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }      

        /// <summary>
        /// Recupera o resumo do projeto.
        /// </summary>
        /// <param name="idProjeto"></param>
        [Route("Projeto/obterprojetoresumo/{idProjeto}")]
        [HttpGet, Authorize]
        public IHttpActionResult ObterProjetoResumo([FromUri]int idProjeto)
        {
            try
            {
                return Ok(ServiceLocator.Current.GetInstance<Api.Projeto.ICalculaProjetoFluxo>().ObterProjetoResumo(idProjeto));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Deletar projeto.
        /// </summary>
        /// <param name="idProjeto"></param>
        /// <returns></returns>
        [Route("Projeto/DeletarProjeto/{idProjeto}")]
        [HttpPost, Authorize]
        public IHttpActionResult DeletarProjeto(int idProjeto)
        {
            try
            {
                return Ok(ServiceLocator.Current.GetInstance<Glass.Api.Projeto.ICalculaProjetoFluxo>().DeletarProjeto(idProjeto));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
