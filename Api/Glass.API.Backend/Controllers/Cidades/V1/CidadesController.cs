// <copyright file="CidadesController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System.Web.Http;

namespace Glass.API.Backend.Controllers.Cidades.V1
{
    /// <summary>
    /// Controller de cidades.
    /// </summary>
    [RoutePrefix("api/v1/cidades")]
    public partial class CidadesController : BaseController
    {
        private IHttpActionResult ValidarId(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Código da cidade é obrigatório.");
            }

            return null;
        }
    }
}
