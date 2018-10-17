// <copyright file="DeleteProducaoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Producao.V1
{
    /// <summary>
    /// Controller de produção.
    /// </summary>
    public partial class ProducaoController : BaseController
    {
        /// <summary>
        /// Desfaz a última leitura de produção da peça informada.
        /// </summary>
        /// <param name="id">O identificador da peça de produção que terá a última leitura desfeita.</param>
        /// <returns>Um JSON com o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id:int}/leituras")]
        [SwaggerResponse(202, "Leitura do último setor da peça desfeita.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação ao desfazer a marcação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Peça de produção não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult DesfazerUltimaLeituraPeca(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdProdutoProducao(sessao, id);

                if (validacao != null)
                {
                    return validacao;
                }

                try
                {
                    sessao.BeginTransaction();

                    ProdutoPedidoProducaoDAO.Instance.VoltarPeca(sessao, (uint)id, null, true);

                    sessao.Commit();
                    return this.Aceito("Leitura da peça desfeita com sucesso.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao("Falha ao desfazer última leitura da peça.", ex);
                }
            }
        }
    }
}
