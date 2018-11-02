// <copyright file="GruposProjetoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Projetos.V1.GruposProjeto
{
    /// <summary>
    /// Controller de grupos de projeto.
    /// </summary>
    [RoutePrefix("api/v1/projetos/grupos")]
    public partial class GruposProjetoController : BaseController
    {
        private IHttpActionResult ValidarIdGrupoProjeto(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador do grupo de projeto deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdGrupoProjeto(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdGrupoProjeto(id);

            if (validacao == null && !GrupoModeloDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Grupo de projeto não encontrado.");
            }

            return null;
        }
    }
}
