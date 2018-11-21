// <copyright file="DeleteVolumesController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Volumes.V1
{
    /// <summary>
    /// Controller de volumes.
    /// </summary>
    public partial class VolumesController : BaseController
    {
        /// <summary>
        /// Exclui um volume.
        /// </summary>
        /// <param name="id">O identificador do volume que será excluído.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id}")]
        [SwaggerResponse(202, "Volume excluído.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Volume não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirVolume(int id)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    sessao.BeginTransaction();

                    var validacao = this.ValidarExistenciaIdVolume(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var volume = VolumeDAO.Instance.GetElementByPrimaryKey(sessao, id);

                    WebGlass.Business.OrdemCarga.Fluxo.VolumeFluxo.Instance.Delete(sessao, volume);

                    sessao.Commit();

                    return this.Aceito($"Volume excluído.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao excluir volume.", ex);
                }
            }
        }
    }
}
