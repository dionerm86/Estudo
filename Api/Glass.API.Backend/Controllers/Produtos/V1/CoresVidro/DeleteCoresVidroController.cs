// <copyright file="DeleteCoresVidroController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Produtos.V1.CoresVidro
{
    /// <summary>
    /// Controller de cores de vidro.
    /// </summary>
    public partial class CoresVidroController : BaseController
    {
        /// <summary>
        /// Exclui uma cor de vidro.
        /// </summary>
        /// <param name="id">O identificador da cor de vidro que será excluída.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id}")]
        [SwaggerResponse(202, "Cor de vidro excluída.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Cor de vidro não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirCorVidro(int id)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaIdCorVidro(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    sessao.BeginTransaction();

                    CorVidroDAO.Instance.DeleteByPrimaryKey(sessao, id);
                    sessao.Commit();

                    return this.Aceito($"Cor de vidro excluída.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao excluir cor de vidro.", ex);
                }
            }
        }
    }
}
