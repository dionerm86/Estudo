// <copyright file="DeleteMovimentacaoEstoqueRealController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Estoques.V1.Movimentacoes.Reais
{
    /// <summary>
    /// Controller de movimentação de estoque real.
    /// </summary>
    public partial class MovimentacaoEstoqueRealController : BaseController
    {
        /// <summary>
        /// Exclui uma movimentação do estoque.
        /// </summary>
        /// <param name="id">O identificador da movimentação do estoque real que será excluída.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Movimentação excluída.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Movimentação não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirMovimentacao(int id)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaIdMovimentacaoEstoqueReal(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var movimentacao = MovEstoqueDAO.Instance.GetElementByPrimaryKey(id);

                    MovEstoqueDAO.Instance.Delete(movimentacao);

                    return this.Aceito($"Movimentação excluída.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao excluir a movimentação.", ex);
                }
            }
        }
    }
}
