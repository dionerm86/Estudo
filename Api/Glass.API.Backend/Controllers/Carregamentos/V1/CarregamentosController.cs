// <copyright file="CarregamentosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Carregamentos.V1
{
    /// <summary>
    /// Controller de carregamentos.
    /// </summary>
    [RoutePrefix("api/v1/carregamentos")]
    public partial class CarregamentosController : BaseController
    {
        private IHttpActionResult ValidarIdCarregamento(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador do carregamento deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdCarregamento(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdCarregamento(id);

            if (validacao == null && !CarregamentoDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Carregamento não encontrado.");
            }

            return null;
        }
    }
}
