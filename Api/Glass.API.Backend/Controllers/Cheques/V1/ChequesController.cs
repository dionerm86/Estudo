// <copyright file="ChequesController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Cheques.V1
{
    /// <summary>
    /// Controller de cheques.
    /// </summary>
    [RoutePrefix("api/v1/cheques")]
    public partial class ChequesController : BaseController
    {
        private IHttpActionResult ValidarIdProdutoIdCheque(int idCheque)
        {
            if (idCheque <= 0)
            {
                return this.ErroValidacao("Identificador do cheque deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdCheque(GDASession sessao, int idCheque)
        {
            var validacao = this.ValidarIdProdutoIdCheque(idCheque);

            if (validacao == null && !ChequesDAO.Instance.Exists(sessao, idCheque))
            {
                return this.NaoEncontrado("Cheque não encontrado.");
            }

            return null;
        }
    }
}
