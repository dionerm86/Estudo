// <copyright file="RoteirosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Producao.V1.Roteiros
{
    /// <summary>
    /// Controller de roteiros.
    /// </summary>
    [RoutePrefix("api/v1/producao/roteiros")]
    public partial class RoteirosController : BaseController
    {
        private IHttpActionResult ValidarIdRoteiro(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador do roteiro deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdRoteiro(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdRoteiro(id);

            if (validacao == null && !RoteiroProducaoDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Roteiro não encontrado.");
            }

            return null;
        }
    }
}
