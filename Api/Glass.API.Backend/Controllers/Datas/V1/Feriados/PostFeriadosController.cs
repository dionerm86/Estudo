// <copyright file="PostFeriadosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Datas.Feriados;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Datas.V1.Feriados.CadastroAtualizacao;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Datas.V1.Feriados
{
    /// <summary>
    /// Controller de feriados.
    /// </summary>
    public partial class FeriadosController : BaseController
    {
        /// <summary>
        /// Cadastra um feriado.
        /// </summary>
        /// <param name="dadosParaCadastro">Objeto contendo dados para inserção de um feriado.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPost]
        [Route("")]
        [SwaggerResponse(201, "Feriado cadastrado.", Type = typeof(CriadoDto<int>))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        public IHttpActionResult CadastrarFeriado([FromBody] CadastroAtualizacaoDto dadosParaCadastro)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var feriado = new ConverterCadastroAtualizacaoParaFeriado(dadosParaCadastro)
                        .ConverterParaFeriado();

                    var idFeriado = FeriadoDAO.Instance.Insert(sessao, feriado);
                    sessao.Commit();

                    return this.Criado("Feriado inserido com sucesso!", idFeriado);
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao inserir feriado.", ex);
                }
            }
        }
    }
}
