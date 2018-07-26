// <copyright file="ObservacoesFinanceiroController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System.Web.Http;

namespace Glass.API.Backend.Controllers.Pedidos.V1.ObservacoesFinanceiro
{
    /// <summary>
    /// Controller de observações financeiras de pedido.
    /// </summary>
    [RoutePrefix("api/v1/pedidos/{idPedido}/observacoesFinanceiro")]
    public partial class ObservacoesFinanceiroController : ApiController
    {
        private IHttpActionResult ValidarIdPedido(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador do pedido deve ser um número maior que zero.");
            }

            return null;
        }
    }
}
