// <copyright file="PostComprasMercadoriasController.cs" company="Sync Softwares">
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
        /// Reabre uma compra de mercadoria.
        /// </summary>
        /// <param name="id">O identificador da compra de mercadoria que será reaberta.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPost]
        [Route("reabrir/{id:int}")]
        [SwaggerResponse(202, "Compra de mercadoria reaberta.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Compra de mercadoria não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ReabrirCompraMercadoria(int id)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    sessao.BeginTransaction();

                    var validacao = this.ValidarExistenciaIdCompraMercadorias(sessao, id);

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

        /// <summary>
        /// Gera uma nota fiscal para uma compra de mercadoria.
        /// </summary>
        /// <param name="id">O identificador da compra de mercadoria que será gerada a nota fiscal.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPost]
        [Route("gerarNotaFiscal/{id:int}")]
        [SwaggerResponse(201, "Nota fiscal gerada.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Compra não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult GerarNFeCompraMercadoria(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdCompraMercadorias(sessao, id);

                if (validacao != null)
                {
                    return validacao;
                }

                try
                {
                    sessao.BeginTransaction();

                    var idNf = NotaFiscalDAO.Instance.GerarNfCompraComTransacao((uint)id, null, ProdutosCompraDAO.Instance.GetByCompra(sessao, (uint)id));
                    sessao.Commit();

                    return this.Criado("Nota fiscal gerada.", (int)idNf);
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao("Falha ao gerar nota.", ex);
                }
            }
        }
    }
}
