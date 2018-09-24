// <copyright file="BoletosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Boletos.V1
{
    /// <summary>
    /// Controller para a exibição de boletos.
    /// </summary>
    [RoutePrefix("api/v1/boletos")]
    public partial class BoletosController : BaseController
    {
        private IHttpActionResult ValidarIdNotaFiscal(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador da nota fiscal deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdNotaFiscal(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdNotaFiscal(id);

            if (validacao == null && !NotaFiscalDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Nota fiscal não encontrada.");
            }

            return null;
        }
    }
}
