// <copyright file="VolumesController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Volumes.V1
{
    /// <summary>
    /// Controller de volumes.
    /// </summary>
    [RoutePrefix("api/v1/volumes")]
    public partial class VolumesController : BaseController
    {
        private IHttpActionResult ValidarIdVolume(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador do volume deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdVolume(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdVolume(id);

            if (validacao == null && !VolumeDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Volume não encontrado.");
            }

            return validacao;
        }
    }
}
