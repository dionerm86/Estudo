// <copyright file="FornadasController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Producao.V1.Fornadas
{
    /// <summary>
    /// Controller de fornadas.
    /// </summary>
    [RoutePrefix("api/v1/producao/fornadas")]
    public partial class FornadasController : BaseController
    {
        private IHttpActionResult ValidarIdFornada(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador da fornada deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdFornada(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdFornada(id);

            if (validacao == null && !FornadaDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Fornada não encontrada.");
            }

            return validacao;
        }
    }
}