// <copyright file="TransportadoresController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Transportadores.V1
{
    /// <summary>
    /// Controller de transportadores.
    /// </summary>
    [RoutePrefix("api/v1/transportadores")]
    public partial class TransportadoresController : BaseController
    {
        private IHttpActionResult ValidarIdTransportador(int idTransportador)
        {
            if (idTransportador <= 0)
            {
                return this.ErroValidacao("Identificador do transportador deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdTransportador(GDASession sessao, int idTransportador)
        {
            var validacao = this.ValidarIdTransportador(idTransportador);

            if (validacao == null && !TransportadorDAO.Instance.Exists(sessao, idTransportador))
            {
                return this.NaoEncontrado("Transportador não encontrado.");
            }

            return null;
        }
    }
}
