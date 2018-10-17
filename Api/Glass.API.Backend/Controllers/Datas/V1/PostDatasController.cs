// <copyright file="PostDatasController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Datas.V1.Validacao;
using Glass.Configuracoes;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Datas.V1
{
    /// <summary>
    /// Controller de datas.
    /// </summary>
    public partial class DatasController : BaseController
    {
        /// <summary>
        /// Valida se uma data é "dia útil".
        /// </summary>
        /// <param name="dadosEntrada">Dados de entrada do método.</param>
        /// <returns>Uma resposta HTTP com o resultado da validação.</returns>
        [HttpPost]
        [Route("validar")]
        [SwaggerResponse(200, "Data válida.")]
        [SwaggerResponse(400, "Data não é válida.", Type = typeof(MensagemDto))]
        public IHttpActionResult ValidarDiaUtil([FromBody] DadosEntradaDto dadosEntrada)
        {
            if (dadosEntrada == null)
            {
                return this.ErroValidacao("Dados de entrada do método são obrigatórios.");
            }

            if (!dadosEntrada.PermitirFimDeSemana)
            {
                var dataValida = (dadosEntrada.Data.DayOfWeek != DayOfWeek.Saturday || Geral.ConsiderarSabadoDiaUtil)
                    && (dadosEntrada.Data.DayOfWeek != DayOfWeek.Sunday || Geral.ConsiderarDomingoDiaUtil);

                if (!dataValida)
                {
                    return this.ErroValidacao("Data não pode ser selecionada em um fim de semana.");
                }
            }

            if (!dadosEntrada.PermitirFeriado)
            {
                var dataValida = !FeriadoDAO.Instance.IsFeriado(dadosEntrada.Data);

                if (!dataValida)
                {
                    return this.ErroValidacao("Data não pode ser selecionada em um feriado.");
                }
            }

            return this.Ok();
        }
    }
}
