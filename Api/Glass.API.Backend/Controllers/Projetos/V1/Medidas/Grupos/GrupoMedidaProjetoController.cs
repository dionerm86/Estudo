// <copyright file="GrupoMedidaProjetoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Projetos.V1.Medidas.Grupos
{
    /// <summary>
    /// Controller de grupos de medida de projeto.
    /// </summary>
    [RoutePrefix("api/v1/projetos/medidas/grupos")]
    public partial class GrupoMedidaProjetoController : BaseController
    {
        private IHttpActionResult ValidarIdGrupoMedidaProjeto(int idGrupoMedidaProjeto)
        {
            if (idGrupoMedidaProjeto <= 0)
            {
                return this.ErroValidacao("Identificador do grupo de medida de projeto deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdGrupoMedidaProjeto(GDASession sessao, int idProjeto)
        {
            var validacao = this.ValidarIdGrupoMedidaProjeto(idProjeto);

            if (validacao == null && !GrupoMedidaProjetoDAO.Instance.Exists(sessao, idProjeto))
            {
                return this.NaoEncontrado("Grupo de medida de projeto não encontrado.");
            }

            return null;
        }
    }
}