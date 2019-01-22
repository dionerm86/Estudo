// <copyright file="DeleteComprasMercadoriasController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Compras.V1.Mercadorias
{
    /// <summary>
    /// Controller de compras de mercadorias.
    /// </summary>
    public partial class ComprasMercadoriasController : BaseController
    {
        /// <summary>
        /// Cancela uma compra e registra o motivo do cancelamento.
        /// </summary>
        /// <param name="id">O identificador da compra que será cancelada.</param>
        /// <param name="motivo">O motivo do cancelamento da compra.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Compra de mercadoria cancelada.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Compra de mercadoria não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult CancelarCompraMercadoria(int id, [FromBody]string motivo)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    sessao.BeginTransaction();

                    var validacao = this.ValidarExistenciaIdCompra(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var compraMercadoria = CompraDAO.Instance.GetElementByPrimaryKey(sessao, id);

                    motivo = "Motivo do cancelamento: " + motivo;
                    compraMercadoria.Obs = !string.IsNullOrEmpty(compraMercadoria.Obs) ? compraMercadoria.Obs + " " + motivo : motivo;

                    compraMercadoria.Obs = compraMercadoria.Obs.Length > 300 ? compraMercadoria.Obs.Substring(0, 300) : compraMercadoria.Obs;

                    CompraDAO.Instance.CancelarCompra(sessao, compraMercadoria.IdCompra, compraMercadoria.Obs);

                    sessao.Commit();
                    sessao.Close();

                    return this.Aceito($"Compra cancelada.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao cancelar a compra.", ex);
                }
            }
        }
    }
}
