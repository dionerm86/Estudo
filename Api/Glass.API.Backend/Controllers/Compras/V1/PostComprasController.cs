// <copyright file="PostComprasController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Compras.V1
{
    /// <summary>
    /// Controller de compras.
    /// </summary>
    public partial class ComprasController : BaseController
    {
        /// <summary>
        /// Finaliza uma compra.
        /// </summary>
        /// <param name="id">O identificador da compra que será finalizada.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPost]
        [Route("{id:int}/finalizar")]
        [SwaggerResponse(202, "Compra finalizada.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Compra não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult FinalizarCompra(int id)
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

                    CompraDAO.Instance.FinalizarCompra(sessao, (uint)id);

                    sessao.Commit();
                    sessao.Close();
                    return this.Aceito($"Compra finalizada.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao finalizar compra.", ex);
                }
            }
        }

        /// <summary>
        /// Reabre uma compra.
        /// </summary>
        /// <param name="id">O identificador da compra que será reaberta.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPost]
        [Route("{id:int}/reabrir")]
        [SwaggerResponse(202, "Compra reaberta.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Compra não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ReabrirCompra(int id)
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

                    CompraDAO.Instance.ReabrirCompra(sessao, (uint)id);

                    sessao.Commit();
                    sessao.Close();

                    return this.Aceito($"Compra reaberta.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    sessao.Close();
                    return this.ErroValidacao($"Erro ao reabrir compra.", ex);
                }
            }
        }
    }
}