using Glass.Api.Seguranca;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Web.Http;
using System.Web.Security;

namespace Glass.Api.Host.Controllers.Projeto
{
    public class CalculaProjetoController : ApiController
    {    

        /// <summary>
        /// Recupera o modelo.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult ObterModelo([FromBody]Glass.Api.Implementacao.Projeto.ItemProjeto item)
        {
            try
            { 
                return Ok(ServiceLocator.Current.GetInstance<Api.Projeto.ICalculaProjetoFluxo>().MontaModelo(item.IdProjeto, item.IdItemProjeto, item.IdProjetoModelo, item.MedidaExata));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("CalculaProjeto/ObterModelo02/{IdProjeto}/{IdItemProjeto}/{IdProjetoModelo}")]
        [HttpGet]
        public IHttpActionResult ObterModelo2(int IdProjeto, int IdItemProjeto, int IdProjetoModelo)
        {
            try
            {
                return Ok(ServiceLocator.Current.GetInstance<Api.Projeto.ICalculaProjetoFluxo>().MontaModelo(IdProjeto, IdItemProjeto, IdProjetoModelo, false));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        public IHttpActionResult ConfirmaModelo([FromBody] Glass.Api.Implementacao.Projeto.CalcModelo modelo)
        {
            try
            {
                return Ok(ServiceLocator.Current.GetInstance<Api.Projeto.ICalculaProjetoFluxo>().ConfirmarItemProjeto(modelo.IdProjeto, modelo.IdItemProjeto, modelo.IdProjetoModelo,
                    modelo.MedidaExata, modelo.Ambiente, modelo.EspessuraVidro, modelo.IdCorVidro, modelo));
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #region Configurações

        /// <summary>
        /// Recupera as configurações.
        /// </summary>
        /// <returns></returns>
        [HttpGet, Authorize]
        public IHttpActionResult ObterConfig()
        {
            try
            {
                var retorno = new Glass.Api.Implementacao.Global.CalcProjetoOptions(
                    ServiceLocator.Current.GetInstance<Api.Global.IVidroFluxo>().ObterCores(),
                    ServiceLocator.Current.GetInstance<Api.Global.IVidroFluxo>().ObterEspessuras(),
                    ServiceLocator.Current.GetInstance<Api.Global.IAluminioFluxo>().ObterCores(),
                    ServiceLocator.Current.GetInstance<Api.Global.IFerragemFluxo>().ObterCores(),
                    ServiceLocator.Current.GetInstance<Api.Projeto.IGrupoModeloFluxo>().ObterGruposModelo(),
                    ServiceLocator.Current.GetInstance<Api.Global.ITipoEntregaFluxo>().ObterTiposEntregaParceiros(),
                    ServiceLocator.Current.GetInstance<Api.Projeto.IProjetoModeloFluxo>().ObterMaisUsados(),
                    ServiceLocator.Current.GetInstance<Api.Global.ITipoEntregaFluxo>().ObterTipoEntregaPadrao()
                    );

                return Ok(retorno);
            }
            catch(Exception ex)
            {
                return BadRequest(string.Format("Erro ao carregas as configurações. {0}", ex.Message));
            }
        }
        
        /// <summary>
        /// Recupera as cores do vidro.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult ObterCoresVidro()
        {
            try
            {
                return Ok(ServiceLocator.Current.GetInstance<Api.Global.IVidroFluxo>().ObterCores());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult ObterEspessuras()
        {
            return null;
        }

        /// <summary>
        /// Recupera os dados do tipo de entrega.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult ObterTiposEntrega()
        {
            try
            {
                return Ok(ServiceLocator.Current.GetInstance<Api.Global.ITipoEntregaFluxo>().ObterTiposEntregaParceiros());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Recupera as medidas do projeto modelo por código do modelo e item do projeto (caso já tenha sido cadastrado).
        /// </summary>
        /// <param name="idProjetoModelo"></param>
        /// <param name="idItemProjeto"></param>
        /// <returns></returns>
        [Route("CalculaProjeto/ObterMediasProjetoModelo/{idProjetoModelo}/{idItemProjeto?}")]
        [HttpGet]
        public IHttpActionResult ObterMediasProjetoModelo(int idProjetoModelo, int idItemProjeto = 0)
        {
            try
            {
                return Ok(ServiceLocator.Current.GetInstance<Api.Projeto.IMediaProjetoModeloFluxo>().ObterMedidasProjetoModelo((uint)idProjetoModelo, (uint)idItemProjeto));
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

    }
}
