// <copyright file="SugestaoClienteController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.SugestoesCliente.V1
{
    /// <summary>
    /// Controller de sugestão de clientes.
    /// </summary>
    [RoutePrefix("api/v1/sugestaoCliente")]
    public partial class SugestaoClienteController : BaseController
    {
        private IHttpActionResult ValidarIdSugestao(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador da sugestão de cliente deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdSugestao(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdSugestao(id);

            if (validacao == null && !SugestaoClienteDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Sugestão de cliente não encontrada.");
            }

            return null;
        }
    }
}
