// <copyright file="PatchSetoresController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Producao.Setores;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Producao.V1.Setores.CadastroAtualizacao;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Producao.V1.Setores
{
    /// <summary>
    /// Controller de setores.
    /// </summary>
    public partial class SetoresController : BaseController
    {
        /// <summary>
        /// Atualiza um setor.
        /// </summary>
        /// <param name="id">O identificador do setor que será alterado.</param>
        /// <param name="dadosParaAlteracao">Os novos dados que serão alterados no setor indicado.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPatch]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Setor alterado.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Setor não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AtualizarSetor(int id, [FromBody] CadastroAtualizacaoDto dadosParaAlteracao)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaIdSetor(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<PCP.Negocios.ISetorFluxo>();

                    var setorAtual = fluxo.ObtemSetor(id);

                    setorAtual = new ConverterCadastroAtualizacaoParaSetor(dadosParaAlteracao, setorAtual)
                        .ConverterParaSetor();

                    var resultado = fluxo.SalvarSetor(setorAtual);

                    if (!resultado)
                    {
                        return this.ErroValidacao($"Falha ao atualizar setor. {resultado.Message.Format()}");
                    }

                    return this.Aceito($"Setor atualizado com sucesso!");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao atualizar setor.", ex);
                }
            }
        }

        /// <summary>
        /// Atualiza a posição de um setor.
        /// </summary>
        /// <param name="id">O identificador do setor que será alterado.</param>
        /// <param name="acima">Define se o setor será movimentado para cima.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPatch]
        [Route("{id:int}/posicao")]
        [SwaggerResponse(202, "Posição do setor alterada.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Setor não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AlterarPosicaoSetor(int id, bool acima)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaIdSetor(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<PCP.Negocios.ISetorFluxo>();

                    var resultado = fluxo.AlterarPosicao(id, acima);

                    if (!resultado)
                    {
                        return this.ErroValidacao($"Falha ao alterar a posição do setor. {resultado.Message}");
                    }

                    return this.Aceito($"Posição do setor alterada com sucesso!");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao atualizar setor.", ex);
                }
            }
        }
    }
}
