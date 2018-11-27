// <copyright file="NaturezasOperacaoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Cfops.V1.NaturezasOperacao
{
    /// <summary>
    /// Controller de naturezas de operação.
    /// </summary>
    [RoutePrefix("api/v1/cfops/naturezasOperacao")]
    public partial class NaturezasOperacaoController : BaseController
    {
        private IHttpActionResult ValidarIdNaturezaOperacao(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador da natureza de operação deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdNaturezaOperacao(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdNaturezaOperacao(id);

            if (validacao == null && !NaturezaOperacaoDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Natureza de operação não encontrada.");
            }

            return null;
        }
    }
}
