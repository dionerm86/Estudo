// <copyright file="ContasPagasController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.ContasPagar.V1.Pagas
{
    /// <summary>
    /// Controller de contas pagas.
    /// </summary>
    [RoutePrefix("api/v1/contasPagar/pagas")]
    public partial class ContasPagasController : BaseController
    {
        private IHttpActionResult ValidarIdContaPaga(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador da conta paga deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdContaPaga(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdContaPaga(id);

            if (validacao == null && !ContasPagarDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Conta paga não encontrada.");
            }

            return validacao;
        }
    }
}