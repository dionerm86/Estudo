using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Glass.Api.Host.Controllers.App
{
    /// <summary>
    /// Controlador dos pedidos.
    /// </summary>
    [Authorize]
    public class PedidoController : ApiController
    {
        ///// <summary>
        ///// Consulta os pedidos do cliente.
        ///// </summary>
        ///// <param name="codCliente"></param>
        ///// <param name="dataInicio"></param>
        ///// <param name="dataFim"></param>
        ///// <param name="apenasAbertos"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[Colosoft.Web.Http.MultiPostParameters]
        //public object Consultar(string codCliente, DateTime dataInicio, DateTime dataFim, bool apenasAbertos)
        //{
        //    return Glass.Data.DAL.PedidoDAO.Instance.GetListAcessoExterno(0, codCliente, dataInicio, dataFim, apenasAbertos, null, 0, 0)
        //        .Select(f => new Glass.Api.Implementacao.Pedido.PedidoDescritor(f)).ToList<Glass.Api.Pedido.IPedidoDescritor>();
        //}
    }
}