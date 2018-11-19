// <copyright file="PostGruposProjetoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Projetos.GruposProjeto;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Projetos.V1.GruposProjeto.CadastroAtualizacao;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Projetos.V1.GruposProjeto
{
    /// <summary>
    /// Controller de grupos de projeto.
    /// </summary>
    public partial class GruposProjetoController : BaseController
    {
        /// <summary>
        /// Cadastra um grupo de projeto.
        /// </summary>
        /// <param name="dadosParaCadastro">Objeto contendo dados para inserção de um grupo de projeto.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPost]
        [Route("")]
        [SwaggerResponse(201, "Grupo de projeto cadastrado.", Type = typeof(CriadoDto<int>))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        public IHttpActionResult CadastrarGrupoProjeto([FromBody] CadastroAtualizacaoDto dadosParaCadastro)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    sessao.BeginTransaction();

                    var grupoProjeto = new ConverterCadastroAtualizacaoParaGrupoProjeto(dadosParaCadastro)
                        .ConverterParaGrupoProjeto();

                    var id = GrupoModeloDAO.Instance.Insert(sessao, grupoProjeto);

                    sessao.Commit();

                    return this.Criado("Grupo de projeto cadastrado com sucesso!", id);
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao cadastrar grupo de projeto.", ex);
                }
            }
        }
    }
}
