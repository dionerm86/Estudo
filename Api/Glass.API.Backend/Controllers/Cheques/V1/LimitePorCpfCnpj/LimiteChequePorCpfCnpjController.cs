// <copyright file="LimiteChequePorCpfCnpjController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Cheques.V1.LimitePorCpfCnpj
{
    /// <summary>
    /// Controller de limite de cheques.
    /// </summary>
    [RoutePrefix("api/v1/cheques/limitePorCpfCnpj")]
    public partial class LimiteChequePorCpfCnpjController : BaseController
    {
        private IHttpActionResult ValidarIdLimiteCheque(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador do limite de cheque deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdLimiteCheque(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdLimiteCheque(id);

            if (validacao == null && !LimiteChequeCpfCnpjDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Limite de cheque não encontrado.");
            }

            return validacao;
        }
    }
}