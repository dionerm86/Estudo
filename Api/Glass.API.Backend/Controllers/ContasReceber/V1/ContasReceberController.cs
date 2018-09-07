// <copyright file="ContasReceberController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.ContasReceber.V1
{
    /// <summary>
    /// Controller de contas a receber/recebidas.
    /// </summary>
    [RoutePrefix("api/v1/contasReceber")]
    public partial class ContasReceberController : BaseController
    {
        private IHttpActionResult ValidarIdContaReceber(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador da conta a receber/recebida deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdContaReceber(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdContaReceber(id);

            if (validacao == null && !LiberarPedidoDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Conta a receber/recebida não encontrada.");
            }

            return null;
        }
    }
}
