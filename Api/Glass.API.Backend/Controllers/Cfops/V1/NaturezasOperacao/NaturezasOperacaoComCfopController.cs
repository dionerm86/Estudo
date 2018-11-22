// <copyright file="NaturezasOperacaoComCfopController.cs" company="Sync Softwares">
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
    [RoutePrefix("api/v1/cfops/{idCfop:int}/naturezasOperacao")]
    public partial class NaturezasOperacaoComCfopController : BaseController
    {
        private IHttpActionResult ValidarIdCfop(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador do CFOP deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdCfop(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdCfop(id);

            if (validacao == null && !CfopDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("CFOP não encontrado.");
            }

            return null;
        }
    }
}
