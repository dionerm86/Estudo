// <copyright file="OrcamentosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Orcamentos.V1
{
    /// <summary>
    /// Controller de orçamentos.
    /// </summary>
    [RoutePrefix("api/v1/orcamentos")]
    public partial class OrcamentosController : BaseController
    {
        private IHttpActionResult ValidarIdOrcamento(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador do orçamento deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdOrcamento(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdOrcamento(id);

            if (validacao == null && !OrcamentoDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Orçamento não encontrado.");
            }

            return null;
        }
    }
}
