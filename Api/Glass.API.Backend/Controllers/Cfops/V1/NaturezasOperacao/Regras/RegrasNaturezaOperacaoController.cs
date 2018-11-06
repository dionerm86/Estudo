// <copyright file="RegrasNaturezaOperacaoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Cfops.V1.NaturezasOperacao.RegrasNaturezaOperacao
{
    /// <summary>
    /// Controller de regras de natureza de operação.
    /// </summary>
    [RoutePrefix("api/v1/cfops/naturezasOperacao/regras")]
    public partial class RegrasNaturezaOperacaoController : BaseController
    {
        private IHttpActionResult ValidarIdRegraNaturezaOperacao(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador da regra de natureza de operação deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdRegraNaturezaOperacao(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdRegraNaturezaOperacao(id);

            if (validacao == null && !RegraNaturezaOperacaoDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Regra de natureza de operação não encontrada.");
            }

            return null;
        }
    }
}
