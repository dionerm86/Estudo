using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Practices.ServiceLocation;
using Glass.Api.Seguranca;
using System.Web.Security;
using System.Web;

namespace Glass.Api.Host.Controllers.Pedido
{
    public class PedidoController : ApiController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idProjeto"></param>
        /// <returns></returns>
        [Route("Pedido/GerarPedido/{idProjeto}")]
        [HttpPost]
        public IHttpActionResult GerarPedido([FromUri]int idProjeto)
        {
            return Ok(ServiceLocator.Current.GetInstance<Api.Pedido.IPedidoFluxo>().GerarPedido(idProjeto));
        }

        /// <summary>
        /// Obtem resultado da pesquisa por pedido.
        /// </summary>
        /// <param name="pesquisaPedido"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult ObterMeusPedidos([FromBody] Glass.Api.Implementacao.Pedido.PesquisaPedido pesquisaPedido)
        {
            return Ok(Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Glass.Api.Pedido.IPedidoFluxo>().ObterPedidos(pesquisaPedido.IdPedido,
                pesquisaPedido.CodCliente, pesquisaPedido.DtIni != DateTime.MinValue ? pesquisaPedido.DtIni.ToString("dd-MM-yyyy") : string.Empty,
                pesquisaPedido.DtFim != DateTime.MinValue ? pesquisaPedido.DtFim.ToString("dd-MM-yyyy") : string.Empty,
                pesquisaPedido.ApenasAbertos, string.Empty, pesquisaPedido.StartRow, pesquisaPedido.PageSize));
        }


        [HttpGet]
        public IHttpActionResult EnviarEmail()
        {
            return null;
        }

        /// <summary>
        /// Salva a foto do pedido.
        /// </summary>
        /// <param name="fotoPedido"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult SalvarFotoPedido([FromBody] Glass.Api.Implementacao.Pedido.FotoPedido fotoPedido)
        {
            fotoPedido.Imagem = Convert.FromBase64String(fotoPedido.ImgData);

            var fotoDescritor = ServiceLocator.Current.GetInstance<Glass.Api.Pedido.IPedidoFluxo>().SalvarFotoPedido(fotoPedido);
            if (fotoDescritor == null)
                return BadRequest(string.Format("Ocorreu um erro e não foi possível salvar a foto para o pedido {0}", fotoPedido.IdPedido));

            return Ok(fotoDescritor);
        }

        /// <summary>
        /// Recupera as fotos do pedido.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        [Route("Pedido/ObterFotosPedido/{idPedido}")]
        [HttpGet]
        public IHttpActionResult ObterFotosPedido([FromUri] int idPedido)
        {
            return Ok(ServiceLocator.Current.GetInstance<Glass.Api.Pedido.IPedidoFluxo>().ObterFotosPedido(idPedido));
        }

        /// <summary>
        /// Apaga uma foto pedido.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        [Route("Pedido/ApagarFotoPedido/{idFoto}")]
        [HttpPost]
        public IHttpActionResult ApagarFotoPedido([FromUri] int idFoto)
        {
            ServiceLocator.Current.GetInstance<Glass.Api.Pedido.IPedidoFluxo>().ApagarFotoPedido(idFoto);
            return Ok();
        }

        /// <summary>
        /// Apagar foto pedido
        /// </summary>
        /// <param name="fotoPedido"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult AtualizarFotoPedidoDescricao([FromBody] Glass.Api.Implementacao.Pedido.FotoPedido fotoPedido)
        {
            ServiceLocator.Current.GetInstance<Glass.Api.Pedido.IPedidoFluxo>().AtualizarFotoPedidoDescricao(fotoPedido.IdFoto, fotoPedido.Descricao);
            return Ok();
        }
    }
}
