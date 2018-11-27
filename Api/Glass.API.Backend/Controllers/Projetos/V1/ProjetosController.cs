// <copyright file="ProjetosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Projetos.V1
{
    /// <summary>
    /// Controller de projetos.
    /// </summary>
    [RoutePrefix("api/v1/projetos")]
    public partial class ProjetosController : BaseController
    {
        private IHttpActionResult ValidarIdProjeto(int idProjeto)
        {
            if (idProjeto <= 0)
            {
                return this.ErroValidacao("Identificador do projeto deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdProjeto(GDASession sessao, int idProjeto)
        {
            var validacao = this.ValidarIdProjeto(idProjeto);

            if (validacao == null && !ProjetoDAO.Instance.Exists(sessao, idProjeto))
            {
                return this.NaoEncontrado("Projeto não encontrado.");
            }

            return null;
        }
    }
}
