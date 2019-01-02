// <copyright file="ComprasController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Compras.V1
{
    /// <summary>
    /// Controller de compras.
    /// </summary>
    [RoutePrefix("api/v1/compras")]
    public partial class ComprasController : BaseController
    {
        private IHttpActionResult ValidarIdCompra(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador da compra deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdCompra(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdCompra(id);

            if (validacao == null && !CompraDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Compra não encontrada.");
            }

            return validacao;
        }
    }
}