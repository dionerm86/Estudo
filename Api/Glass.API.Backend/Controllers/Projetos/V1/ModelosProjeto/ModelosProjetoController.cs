// <copyright file="ModelosProjetoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Projetos.V1.ModelosProjeto
{
    /// <summary>
    /// Controller de modelos de projeto.
    /// </summary>
    [RoutePrefix("api/v1/projetos/modelos")]
    public partial class ModelosProjetoController : BaseController
    {
        private IHttpActionResult ValidarIdModeloProjeto(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador do modelo de projeto deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdModeloProjeto(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdModeloProjeto(id);

            if (validacao == null && !ProjetoDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Modelo de projeto não encontrado.");
            }

            return null;
        }
    }
}
