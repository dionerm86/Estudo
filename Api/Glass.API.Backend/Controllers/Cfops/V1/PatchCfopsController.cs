// <copyright file="PatchCfopsController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Cfops;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Cfops.V1.CadastroAtualizacao;
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
        /// Atualiza um CFOP.
        /// </summary>
        /// <param name="id">O identificador do CFOP que será alterado.</param>
        /// <param name="dadosParaAlteracao">Os novos dados que serão alterados no CFOP indicado.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPatch]
        [Route("{id:int}")]
        [SwaggerResponse(202, "CFOP alterado.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "CFOP não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AtualizarCfop(int id, [FromBody] CadastroAtualizacaoDto dadosParaAlteracao)
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

                    var cfopAtual = fluxo.ObtemCfop(id);

                    cfopAtual = new ConverterCadastroAtualizacaoParaCfop(dadosParaAlteracao, cfopAtual)
                        .ConverterParaCfop();

                    var resultado = fluxo.SalvarCfop(cfopAtual);

                    if (!resultado)
                    {
                        sessao.Rollback();
                        return this.ErroValidacao($"Falha ao atualizar o CFOP. {resultado.Message.Format()}");
                    }

                    sessao.Commit();
                    return this.Aceito($"CFOP atualizado com sucesso!");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao atualizar o CFOP.", ex);
                }
            }
        }
    }
}
