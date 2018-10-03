// <copyright file="ComissionadosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Comissionados.V1
{
    /// <summary>
    /// Controller de comissionados.
    /// </summary>
    [RoutePrefix("api/v1/comissionados")]
    public partial class ComissionadosController : BaseController
    {
        private IHttpActionResult ValidarIdComissionado(int idComissionado)
        {
            if (idComissionado <= 0)
            {
                return this.ErroValidacao("Identificador do comissionado deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdComissionado(GDASession sessao, int idComissionado)
        {
            var validacao = this.ValidarIdComissionado(idComissionado);

            if (validacao == null && !ComissionadoDAO.Instance.Exists(sessao, idComissionado))
            {
                return this.NaoEncontrado("Comissionado não encontrado.");
            }

            return null;
        }
    }
}
