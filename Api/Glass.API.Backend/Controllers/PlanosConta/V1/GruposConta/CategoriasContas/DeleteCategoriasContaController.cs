// <copyright file="DeleteCategoriasContaController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.PlanosConta.V1.GruposConta.CategoriasConta
{
    /// <summary>
    /// Controller de categorias de conta.
    /// </summary>
    public partial class CategoriasContaController : BaseController
    {
        /// <summary>
        /// Exclui uma categoria de conta.
        /// </summary>
        /// <param name="id">O identificador da categoria de conta que será excluído.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Categoria de conta excluída.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Categoria de conta não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirCategoriaConta(int id)
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

                    var categoriaConta = fluxo.ObtemCategoriaConta(id);

                    var resultado = fluxo.ApagarCategoriaConta(categoriaConta);

                    if (!resultado)
                    {
                        return this.ErroValidacao($"Falha ao excluir categoria de conta. {resultado.Message.Format()}");
                    }

                    return this.Aceito($"Categoria de conta excluída.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao excluir categoria de conta.", ex);
                }
            }
        }
    }
}
