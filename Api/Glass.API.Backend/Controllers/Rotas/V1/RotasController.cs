// <copyright file="RotasController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Rotas.V1
{
    /// <summary>
    /// Controller de rotas.
    /// </summary>
    [RoutePrefix("api/v1/rotas")]
    public partial class RotasController : BaseController
    {
        private IHttpActionResult ValidarIdRota(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador da rota deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdRota(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdRota(id);

            if (validacao == null && !RotaDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Rota não encontrada.");
            }

            return null;
        }
    }
}
