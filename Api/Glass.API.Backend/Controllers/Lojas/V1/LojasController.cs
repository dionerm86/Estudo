// <copyright file="LojasController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Lojas.V1
{
    /// <summary>
    /// Controller de lojas.
    /// </summary>
    [RoutePrefix("api/v1/lojas")]
    public partial class LojasController : BaseController
    {
        private IHttpActionResult ValidarIdLoja(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador da loja deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdLoja(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdLoja(id);

            if (validacao == null && !LojaDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Loja não encontrada.");
            }

            return null;
        }
    }
}
