// <copyright file="PatchCarregamentosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Carregamentos;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Carregamentos.V1.CadastroAtualizacao;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;
using WebGlass.Business.OrdemCarga.Fluxo;

namespace Glass.API.Backend.Controllers.Carregamentos.V1
{
    /// <summary>
    /// Controller de carregamento.
    /// </summary>
    public partial class CarregamentosController : BaseController
    {
        /// <summary>
        /// Atualiza um carregamento.
        /// </summary>
        /// <param name="id">O identificador do carregamento que será alterado.</param>
        /// <param name="dadosParaAlteracao">Os novos dados que serão alterados no carregamento indicado.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPatch]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Carregamento alterado.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Carregamento não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AtualizarCarregamento(int id, [FromBody] CadastroAtualizacaoDto dadosParaAlteracao)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    sessao.BeginTransaction();

                    var validacao = this.ValidarExistenciaIdCarregamento(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var carregamentoAtual = CarregamentoDAO.Instance.GetElementByPrimaryKey(sessao, id);

                    carregamentoAtual = new ConverterCadastroAtualizacaoParaCarregamento(dadosParaAlteracao, carregamentoAtual)
                        .ConverterParaCarregamento();

                    CarregamentoFluxo.Instance.Update(sessao, carregamentoAtual);

                    sessao.Commit();

                    return this.Aceito($"Carregamento atualizado com sucesso!");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao atualizar o carregamento.", ex);
                }
            }
        }
    }
}
