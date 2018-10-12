// <copyright file="DeleteGruposProdutoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Produtos.V1.GruposProduto
{
    /// <summary>
    /// Controller de grupos de produto.
    /// </summary>
    public partial class GruposProdutoController : BaseController
    {
        /// <summary>
        /// Exclui um grupo de produto.
        /// </summary>
        /// <param name="id">O identificador do grupo de produto que será excluído.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id}")]
        [SwaggerResponse(202, "Grupo de produto excluído.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Grupo de produto não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirGrupo(int id)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaIdGrupoProduto(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Global.Negocios.IGrupoProdutoFluxo>();

                    var grupo = fluxo.ObtemGrupoProduto(id);

                    var resultado = fluxo.ApagarGrupoProduto(grupo);

                    if (!resultado)
                    {
                        return this.ErroValidacao($"Falha ao excluir grupo de produto. {resultado.Message.Format()}");
                    }

                    return this.Aceito($"Grupo de produto excluído.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao excluir grupo de produto.", ex);
                }
            }
        }
    }
}
