// <copyright file="DeleteNotasFiscaisController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.NotasFiscais.V1
{
    /// <summary>
    /// Controller de notas fiscais.
    /// </summary>
    public partial class NotasFiscaisController : BaseController
    {
        /// <summary>
        /// Exclui uma nota fiscal, se possível.
        /// </summary>
        /// <param name="id">O identificador da nota fiscal que será excluída.</param>
        /// <returns>Um status HTTP indicando se a nota fiscal foi excluída.</returns>
        [HttpDelete]
        [Route("{id}")]
        [SwaggerResponse(202, "Nota fiscal excluída.")]
        [SwaggerResponse(400, "Erro de valor ou formato do campo id ou de validação na exclusão da nota fiscal.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Notas fiscal não encontrada.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirNotaFiscal(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdNotaFiscal(sessao, id);

                if (validacao != null)
                {
                    return validacao;
                }

                try
                {
                    sessao.BeginTransaction();

                    NotaFiscalDAO.Instance.DeleteNotaFiscal(sessao, (uint)id);
                    sessao.Commit();

                    return this.Aceito($"Nota fiscal {id} excluída.");
                }
                catch (Exception e)
                {
                    sessao.Rollback();
                    return this.ErroValidacao("Erro ao excluir a nota fiscal.", e);
                }
            }
        }
    }
}
