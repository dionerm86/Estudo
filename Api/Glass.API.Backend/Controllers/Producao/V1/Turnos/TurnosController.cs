// <copyright file="TurnosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Producao.V1.Turnos
{
    /// <summary>
    /// Controller de turnos.
    /// </summary>
    [RoutePrefix("api/v1/producao/turnos")]
    public partial class TurnosController : BaseController
    {
        private IHttpActionResult ValidarIdTurno(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador do turno deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdTurno(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdTurno(id);

            if (validacao == null && !TurnoDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Turno não encontrado.");
            }

            return null;
        }
    }
}
