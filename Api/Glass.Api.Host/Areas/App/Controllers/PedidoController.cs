using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Glass.Api.Host.Areas.App.Controllers
{
    /// <summary>
    /// Controlador dos pedidos.
    /// </summary>
    [Authorize]
    public class PedidoController : ApiController
    {
        /// <summary>
        /// Consulta os pedidos do cliente.
        /// </summary>
        /// <param name="codCliente">Código do cliente para filtrar os pedidos.</param>
        /// <param name="dataInicio"></param>
        /// <param name="dataFim"></param>
        /// <param name="apenasAbertos"></param>
        /// <returns></returns>
        [HttpPost]
        [Colosoft.Web.Http.MultiPostParameters]
        public object Consultar(string codCliente, DateTime? dataInicio, DateTime? dataFim, bool apenasAbertos)
        {
            return Glass.Data.DAL.PedidoDAO.Instance.GetListAcessoExterno(0, codCliente, dataInicio, dataFim, apenasAbertos, null, 0, 0)
                .Select(f =>
                {
                    var situacaoProducao = f.DescrSituacaoProducao;

                    if (situacaoProducao == "Etiqueta não impressa") situacaoProducao = "Pendente";

                    return new
                    {
                        IdPedido = f.IdPedido,
                        PedidoCli = f.CodCliente,
                        NomeLoja = f.NomeLoja,
                        DataCad = f.DataCad,
                        DataEntrega = f.DataEntrega,
                        Situacao = f.Situacao,
                        SituacaoProducao = situacaoProducao,
                        TotM = f.TotM,
                        Peso = f.Peso,
                        Total = f.Total
                    };
                });
        }
    }
}