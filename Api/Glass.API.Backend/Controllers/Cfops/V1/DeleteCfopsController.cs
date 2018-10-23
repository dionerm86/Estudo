// <copyright file="DeleteCfopsController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Cfops.V1
{
    /// <summary>
    /// Controller de CFOP.
    /// </summary>
    public partial class CfopsController : BaseController
    {
        /// <summary>
        /// Exclui um CFOP.
        /// </summary>
        /// <param name="id">O identificador do CFOP que será excluído.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id:int}")]
        [SwaggerResponse(202, "CFOP excluído.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "CFOP não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirCfop(int id)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    sessao.BeginTransaction();

                    var validacao = this.ValidarExistenciaIdCfop(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Fiscal.Negocios.ICfopFluxo>();

                    var cfop = fluxo.ObtemCfop(id);

                    var resultado = fluxo.ApagarCfop(cfop);

                    if (!resultado)
                    {
                        sessao.Rollback();
                        return this.ErroValidacao($"Falha ao excluir CFOP. {resultado.Message.Format()}");
                    }

                    sessao.Commit();
                    return this.Aceito($"CFOP excluído.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao excluir CFOP.", ex);
                }
            }
        }
    }
}
