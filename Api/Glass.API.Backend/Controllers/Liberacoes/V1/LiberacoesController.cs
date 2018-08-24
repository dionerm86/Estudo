// <copyright file="LiberacoesController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Liberacoes.V1
{
    /// <summary>
    /// Controller de liberações.
    /// </summary>
    [RoutePrefix("api/v1/liberacoes")]
    public partial class LiberacoesController : BaseController
    {
        private IHttpActionResult ValidarIdLiberacao(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador da liberação deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdLiberacao(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdLiberacao(id);

            if (validacao == null && !LiberarPedidoDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Liberação não encontrada.");
            }

            return null;
        }
    }
}
