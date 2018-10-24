// <copyright file="DeleteRoteirosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Producao.V1.Roteiros
{
    /// <summary>
    /// Controller de roteiros.
    /// </summary>
    public partial class RoteirosController : BaseController
    {
        /// <summary>
        /// Exclui um roteiro.
        /// </summary>
        /// <param name="id">O identificador do roteiro que será excluído.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Roteiro excluído.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Roteiro não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirRoteiro(int id)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    sessao.BeginTransaction();

                    var validacao = this.ValidarExistenciaIdRoteiro(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var roteiro = RoteiroProducaoDAO.Instance.ObtemElemento(sessao, id);

                    RoteiroProducaoDAO.Instance.Delete(sessao, roteiro);
                    RoteiroProducaoSetorDAO.Instance.ApagarPorRoteiroProducao(sessao, roteiro.IdRoteiroProducao);

                    sessao.Commit();

                    return this.Aceito($"Roteiro excluído.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao excluir roteiro.", ex);
                }
            }
        }
    }
}
