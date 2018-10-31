// <copyright file="ContasBancariasController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.ContasBancarias.V1
{
    /// <summary>
    /// Controller de contas bancárias.
    /// </summary>
    [RoutePrefix("api/v1/contasBancarias")]
    public partial class ContasBancariasController : BaseController
    {
        private IHttpActionResult ValidarIdContaBancaria(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador da conta bancária deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdContaBancaria(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdContaBancaria(id);

            if (validacao == null && !ContaBancoDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Conta bancária não encontrada.");
            }

            return null;
        }
    }
}
