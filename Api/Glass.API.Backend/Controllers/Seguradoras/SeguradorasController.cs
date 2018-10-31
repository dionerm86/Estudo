// <copyright file="SeguradorasController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL.CTe;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Seguradoras.V1
{
    /// <summary>
    /// Controller de seguradoras.
    /// </summary>
    [RoutePrefix("api/v1/seguradoras")]
    public partial class SeguradorasController : BaseController
    {
        private IHttpActionResult ValidarIdSeguradora(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador da seguradora deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdSeguradora(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdSeguradora(id);

            if (validacao == null && !SeguradoraDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Seguradora não encontrada.");
            }

            return null;
        }
    }
}
