// <copyright file="MedidasProjetoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Projetos.V1.MedidasProjeto
{
    /// <summary>
    /// Controller de medidas de projeto.
    /// </summary>
    [RoutePrefix("api/v1/projetos/medidas")]
    public partial class MedidasProjetoController : BaseController
    {
        private IHttpActionResult ValidarIdMedidaProjeto(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador da medida de projeto deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdMedidaProjeto(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdMedidaProjeto(id);

            if (validacao == null && !MedidaProjetoDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Medida de projeto não encontrada.");
            }

            return null;
        }
    }
}
