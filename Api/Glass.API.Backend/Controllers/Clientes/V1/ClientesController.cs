// <copyright file="ClientesController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Clientes.V1
{
    /// <summary>
    /// Controller de clientes.
    /// </summary>
    [RoutePrefix("api/v1/clientes")]
    public partial class ClientesController : BaseController
    {
        private IHttpActionResult ValidarIdCliente(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador do cliente deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdCliente(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdCliente(id);

            if (validacao == null && !ClienteDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Pedido não encontrado.");
            }

            return null;
        }
    }
}
