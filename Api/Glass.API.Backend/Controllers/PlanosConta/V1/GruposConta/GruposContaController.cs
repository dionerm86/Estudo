// <copyright file="GruposContaController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.PlanosConta.V1.GruposConta
{
    /// <summary>
    /// Controller de grupos de conta.
    /// </summary>
    [RoutePrefix("api/v1/planosConta/grupos")]
    public partial class GruposContaController : BaseController
    {
        private IHttpActionResult ValidarIdGrupoConta(int idGrupoConta)
        {
            if (idGrupoConta <= 0)
            {
                return this.ErroValidacao("Identificador do grupo de conta deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdGrupoConta(GDASession sessao, int idGrupoConta)
        {
            var validacao = this.ValidarIdGrupoConta(idGrupoConta);

            if (validacao == null && !GrupoContaDAO.Instance.Exists(sessao, idGrupoConta))
            {
                return this.NaoEncontrado("Grupo de conta não encontrado.");
            }

            return null;
        }
    }
}
