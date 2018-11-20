// <copyright file="ContabilistasController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Contabilistas.V1
{
    /// <summary>
    /// Controller de contabilistas.
    /// </summary>
    [RoutePrefix("api/v1/contabilistas")]
    public partial class ContabilistasController : BaseController
    {
        private IHttpActionResult ValidarIdContabilista(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador do contabilista deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdContabilista(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdContabilista(id);

            if (validacao == null && !ContabilistaDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Contabilista não encontrado.");
            }

            return null;
        }
    }
}
