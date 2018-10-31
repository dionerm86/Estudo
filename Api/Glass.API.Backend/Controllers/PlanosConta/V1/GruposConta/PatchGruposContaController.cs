// <copyright file="PatchGruposContaController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.PlanosConta.GruposConta;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.PlanosConta.V1.GruposConta.CadastroAtualizacao;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.PlanosConta.V1.GruposConta
{
    /// <summary>
    /// Controller de grupos de conta.
    /// </summary>
    public partial class GruposContaController : BaseController
    {
        /// <summary>
        /// Atualiza um grupo de conta.
        /// </summary>
        /// <param name="id">O identificador do grupo de conta que será alterado.</param>
        /// <param name="dadosParaAlteracao">Os novos dados que serão alterados no grupo de conta indicado.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPatch]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Grupo de conta alterado.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Grupo de conta não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AtualizarGrupoConta(int id, [FromBody] CadastroAtualizacaoDto dadosParaAlteracao)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaIdGrupoConta(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Financeiro.Negocios.IPlanoContasFluxo>();

                    var grupoContaAtual = fluxo.ObtemGrupoConta(id);

                    grupoContaAtual = new ConverterCadastroAtualizacaoParaGrupoConta(dadosParaAlteracao, grupoContaAtual)
                        .ConverterParaGrupoConta();

                    var resultado = fluxo.SalvarGrupoConta(grupoContaAtual);

                    if (!resultado)
                    {
                        return this.ErroValidacao($"Falha ao atualizar grupo de conta. {resultado.Message.Format()}");
                    }

                    return this.Aceito($"Grupo de conta atualizado com sucesso!");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao atualizar grupo de conta.", ex);
                }
            }
        }

        /// <summary>
        /// Atualiza a posição de um grupo de conta.
        /// </summary>
        /// <param name="id">O identificador do grupo de conta que será alterado.</param>
        /// <param name="posicao">Define se o grupo de conta será movimentado para cima.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPatch]
        [Route("{id:int}/posicao")]
        [SwaggerResponse(202, "Posição do grupo de conta alterada.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Grupo de conta não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AlterarPosicaoSetor(int id, Models.Producao.V1.Setores.CadastroAtualizacao.PosicaoDto posicao)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaIdGrupoConta(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Financeiro.Negocios.IPlanoContasFluxo>();

                    var resultado = fluxo.AlterarPosicaoGrupoConta(id, posicao.Acima);

                    if (!resultado)
                    {
                        return this.ErroValidacao($"Falha ao alterar a posição do grupo de conta. {resultado.Message}");
                    }

                    return this.Aceito($"Posição do grupo de conta alterada com sucesso!");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao atualizar grupo de conta.", ex);
                }
            }
        }
    }
}
