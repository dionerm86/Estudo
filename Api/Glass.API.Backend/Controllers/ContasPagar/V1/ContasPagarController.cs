// <copyright file="ContasPagarController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.ContasPagar.V1
{
    /// <summary>
    /// Controller de contas a pagar.
    /// </summary>
    [RoutePrefix("api/v1/contasPagar")]
    public partial class ContasPagarController : BaseController
    {
        private IHttpActionResult ValidarIdContasPagar(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador da conta a pagar deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdContasPagar(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdContasPagar(id);

            if (validacao == null && !ContasPagarDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Conta a pagar não encontrada.");
            }

            return validacao;
        }
    }
}