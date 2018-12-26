// <copyright file="PagamentosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Pagamentos.V1
{
    /// <summary>
    /// Controller de pagamentos.
    /// </summary>
    [RoutePrefix("api/v1/pagamentos")]
    public partial class PagamentosController : BaseController
    {
        private IHttpActionResult ValidarIdPagamento(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador do pagamento deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdPagamento(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdPagamento(id);

            if (validacao == null && !PagtoDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Pagamento não encontrado.");
            }

            return null;
        }
    }
}