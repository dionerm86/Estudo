// <copyright file="NotasFiscaisController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.NotasFiscais.V1
{
    /// <summary>
    /// Controller de notas fiscais.
    /// </summary>
    [RoutePrefix("api/v1/notasFiscais")]
    public partial class NotasFiscaisController : BaseController
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
