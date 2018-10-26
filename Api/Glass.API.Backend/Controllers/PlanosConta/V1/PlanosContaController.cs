// <copyright file="PlanosContaController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.PlanosConta.V1
{
    /// <summary>
    /// Controller de planos de conta.
    /// </summary>
    [RoutePrefix("api/v1/planosConta")]
    public partial class PlanosContaController : BaseController
    {
        private IHttpActionResult ValidarIdPlanoConta(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador do plano de conta deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdPlanoConta(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdPlanoConta(id);

            if (validacao == null && !PlanoContasDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Plano de conta não encontrado.");
            }

            return null;
        }
    }
}
