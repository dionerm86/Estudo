// <copyright file="TiposController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Clientes.V1.Tipos
{
    /// <summary>
    /// Controller de tipos de cliente.
    /// </summary>
    [RoutePrefix("api/v1/tiposCliente")]
    public partial class TiposClienteController : BaseController
    {
        private IHttpActionResult ValidarIdTipoCliente(int idTipoCliente)
        {
            if (idTipoCliente <= 0)
            {
                return this.ErroValidacao("Identificador do tipo de cliente deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdTipoCliente(GDASession sessao, int idTipoCliente)
        {
            var validacao = this.ValidarIdTipoCliente(idTipoCliente);

            if (validacao == null && !TipoClienteDAO.Instance.Exists(sessao, idTipoCliente))
            {
                return this.NaoEncontrado("Tipo de cliente não encontrado.");
            }

            return null;
        }
    }
}
