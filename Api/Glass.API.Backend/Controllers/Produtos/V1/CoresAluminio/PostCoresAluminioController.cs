// <copyright file="PostCoresAluminioController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Produtos.CoresAluminio;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Produtos.V1.CoresAluminio.CadastroAtualizacao;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Produtos.V1.CoresAluminio
{
    /// <summary>
    /// Controller de cores de alumínio.
    /// </summary>
    public partial class CoresAluminioController : BaseController
    {
        /// <summary>
        /// Cadastra uma cor de alumínio.
        /// </summary>
        /// <param name="dadosParaCadastro">Objeto contendo dados para inserção de uma cor de alumínio.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPost]
        [Route("")]
        [SwaggerResponse(201, "Cor de alumínio cadastrada.", Type = typeof(CriadoDto<int>))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        public IHttpActionResult CadastrarCorAluminio([FromBody] CadastroAtualizacaoDto dadosParaCadastro)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var corAluminio = new ConverterCadastroAtualizacaoParaCorAluminio(dadosParaCadastro)
                        .ConverterParaCorAluminio();

                    var idCorAluminio = CorAluminioDAO.Instance.Insert(sessao, corAluminio);
                    sessao.Commit();

                    return this.Criado("Cor de alumínio inserida com sucesso!", idCorAluminio);
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao inserir cor de alumínio.", ex);
                }
            }
        }
    }
}
