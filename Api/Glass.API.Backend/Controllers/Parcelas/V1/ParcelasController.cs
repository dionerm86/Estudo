// <copyright file="ParcelasController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Parcelas.V1
{
    /// <summary>
    /// Controller de parcelas.
    /// </summary>
    [RoutePrefix("api/v1/parcelas")]
    public partial class ParcelasController : BaseController
    {
        private IHttpActionResult ValidarIdParcela(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador da parcela deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdParcela(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdParcela(id);

            if (validacao == null && !ParcelasDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Parcela não encontrada.");
            }

            return null;
        }
    }
}
