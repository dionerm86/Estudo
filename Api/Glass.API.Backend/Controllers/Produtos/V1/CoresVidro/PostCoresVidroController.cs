// <copyright file="PostCoresVidroController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Produtos.CoresVidro;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Produtos.V1.CoresVidro.CadastroAtualizacao;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Produtos.V1.CoresVidro
{
    /// <summary>
    /// Controller de cores de Vidro.
    /// </summary>
    public partial class CoresVidroController : BaseController
    {
        /// <summary>
        /// Cadastra uma cor de Vidro.
        /// </summary>
        /// <param name="dadosParaCadastro">Objeto contendo dados para inserção de uma cor de Vidro.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPost]
        [Route("")]
        [SwaggerResponse(201, "Cor de Vidro cadastrada.", Type = typeof(CriadoDto<int>))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        public IHttpActionResult CadastrarCorVidro([FromBody] CadastroAtualizacaoDto dadosParaCadastro)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var corVidro = new ConverterCadastroAtualizacaoParaCorVidro(dadosParaCadastro)
                        .ConverterParaCorVidro();

                    var idCorVidro = CorVidroDAO.Instance.Insert(sessao, corVidro);
                    sessao.Commit();

                    return this.Criado("Cor de Vidro inserida com sucesso!", idCorVidro);
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao inserir cor de Vidro.", ex);
                }
            }
        }
    }
}
