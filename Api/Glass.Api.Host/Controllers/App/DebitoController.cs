using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace Glass.Api.Host.Controllers.App
{
    /// <summary>
    /// Representa o controlador para acessar as informações de débito do aplicativo.
    /// </summary>
    [Authorize]
    public class DebitoController : ApiController
    {
        /// <summary>
        /// Carrega os débitos do cliente.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public object Consultar()
        {
            return Glass.Data.DAL.ContasReceberDAO.Instance.GetDebitosListParceiros
               (0, 0, 0, true, 0, "DataVec Desc", 0, 0)
               .Select(f => new Glass.Api.Implementacao.Debito.DebitoDescritor(f));

        }
    }
}