// <copyright file="PatchCategoriasContaController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.PlanosConta.CategoriasConta;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.PlanosConta.V1.CategoriasConta.CadastroAtualizacao;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.PlanosConta.V1.CategoriasConta
{
    /// <summary>
    /// Controller de categorias de conta.
    /// </summary>
    public partial class CategoriasContaController : BaseController
    {
        /// <summary>
        /// Atualiza uma categoria de conta.
        /// </summary>
        /// <param name="id">O identificador da categoria de conta que será alterado.</param>
        /// <param name="dadosParaAlteracao">Os novos dados que serão alterados na categoria de conta indicada.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPatch]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Categoria de conta alterada.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Categoria de conta não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AtualizarCategoriaConta(int id, [FromBody] CadastroAtualizacaoDto dadosParaAlteracao)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaIdCategoriaConta(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Financeiro.Negocios.IPlanoContasFluxo>();

                    var categoriaContaAtual = fluxo.ObtemCategoriaConta(id);

                    categoriaContaAtual = new ConverterCadastroAtualizacaoParaCategoriaConta(dadosParaAlteracao, categoriaContaAtual)
                        .ConverterParaCategoriaConta();

                    var resultado = fluxo.SalvarCategoriaConta(categoriaContaAtual);

                    if (!resultado)
                    {
                        return this.ErroValidacao($"Falha ao atualizar categoria de conta. {resultado.Message.Format()}");
                    }

                    return this.Aceito($"Categoria de conta atualizada com sucesso!");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao atualizar categoria de conta.", ex);
                }
            }
        }

        /// <summary>
        /// Atualiza a posição de uma categoria de conta.
        /// </summary>
        /// <param name="id">O identificador da categoria de conta que será alterado.</param>
        /// <param name="posicao">Define se a categoria de conta será movimentada para cima.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPatch]
        [Route("{id:int}/posicao")]
        [SwaggerResponse(202, "Posição da categoria de conta alterada.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Categoria de conta não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AlterarPosicaoCategoria(int id, Models.Producao.V1.Setores.CadastroAtualizacao.PosicaoDto posicao)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaIdCategoriaConta(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Financeiro.Negocios.IPlanoContasFluxo>();

                    var resultado = fluxo.AlterarPosicaoCategoriaConta(id, posicao.Acima);

                    if (!resultado)
                    {
                        return this.ErroValidacao($"Falha ao alterar a posição da categoria de conta. {resultado.Message}");
                    }

                    return this.Aceito($"Posição da categoria de conta alterada com sucesso!");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao atualizar categoria de conta.", ex);
                }
            }
        }
    }
}
