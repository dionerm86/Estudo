// <copyright file="DeleteCarregamentosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Carregamentos.V1
{
    /// <summary>
    /// Controller de carregamentos.
    /// </summary>
    public partial class CarregamentosController : BaseController
    {
        /// <summary>
        /// Exclui um carregamento.
        /// </summary>
        /// <param name="id">O identificador do carregamento que será excluído.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Carregamento excluído.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Carregamento não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirCarregamento(int id)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaIdCarregamento(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var carregamento = CarregamentoDAO.Instance.GetElementByPrimaryKey(id);

                    WebGlass.Business.OrdemCarga.Fluxo.CarregamentoFluxo.Instance.Delete(carregamento);

                    return this.Aceito($"Carregamento excluído.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao excluir carregamento.", ex);
                }
            }
        }
    }
}
