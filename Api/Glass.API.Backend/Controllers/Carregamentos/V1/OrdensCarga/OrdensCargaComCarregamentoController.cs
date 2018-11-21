// <copyright file="OrdensCargaComCarregamentoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Carregamentos.V1.OrdensCarga
{
    /// <summary>
    /// Controller de ordens de carga com carregamento.
    /// </summary>
    [RoutePrefix("api/v1/carregamentos/{idCarregamento:int}/ordensCarga")]
    public partial class OrdensCargaComCarregamentoController : BaseController
    {
        private IHttpActionResult ValidarIdCarregamento(int idCarregamento)
        {
            if (idCarregamento <= 0)
            {
                return this.ErroValidacao("Identificador do carregamento deve ser um número maior que zero.");
            }

            return null;
        }

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
