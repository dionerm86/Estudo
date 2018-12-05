// <copyright file="PostCarregamentosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
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
        /// Fatura um carregamento.
        /// </summary>
        /// <param name="id">O identificador do carregamento que será faturado.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPost]
        [Route("{id:int}/faturar")]
        [SwaggerResponse(202, "Carregamento faturado.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Carregamento não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult FaturarCarregamento(int id)
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

                    var resultado = WebGlass.Business.OrdemCarga.Fluxo.CarregamentoFluxo.Instance.EfetuarFaturamento(id);

                    if (resultado.StartsWith("erro"))
                    {
                        return this.ErroValidacao(resultado.Split('|')[2]);
                    }

                    return this.Aceito($"Carregamento faturado.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao faturar carregamento.", ex);
                }
            }
        }
    }
}
