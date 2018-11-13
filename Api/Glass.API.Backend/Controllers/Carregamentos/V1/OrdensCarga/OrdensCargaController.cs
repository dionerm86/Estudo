// <copyright file="OrdensCargaController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Carregamentos.V1.OrdensCarga
{
    /// <summary>
    /// Controller de ordens de carga.
    /// </summary>
    [RoutePrefix("api/v1/carregamentos/ordensCarga")]
    public partial class OrdensCargaController : BaseController
    {
        private IHttpActionResult ValidarIdOrdemCarga(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador da ordem de carga deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdOrdemCarga(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdOrdemCarga(id);

            if (validacao == null && !OrdemCargaDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Ordem de carga não encontrada.");
            }

            return validacao;
        }
    }
}
