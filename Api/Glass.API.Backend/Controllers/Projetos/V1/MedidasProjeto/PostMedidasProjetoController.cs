// <copyright file="PostMedidasProjetoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Projetos.MedidasProjeto;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Projetos.V1.MedidasProjeto.CadastroAtualizacao;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Projetos.V1.MedidasProjeto
{
    /// <summary>
    /// Controller de medidas de projeto.
    /// </summary>
    public partial class MedidasProjetoController : BaseController
    {
        /// <summary>
        /// Cadastra uma medida de projeto.
        /// </summary>
        /// <param name="dadosParaCadastro">Objeto contendo dados para inserção de uma medida de projeto.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPost]
        [Route("")]
        [SwaggerResponse(201, "Medida de projeto cadastrado.", Type = typeof(CriadoDto<int>))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        public IHttpActionResult CadastrarMedidaProjeto([FromBody] CadastroAtualizacaoDto dadosParaCadastro)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    sessao.BeginTransaction();

                    var medidaProjeto = new ConverterCadastroAtualizacaoParaMedidaProjeto(dadosParaCadastro)
                        .ConverterParaMedidaProjeto();

                    var id = MedidaProjetoDAO.Instance.Insert(sessao, medidaProjeto);

                    sessao.Commit();

                    return this.Criado("Medida de projeto cadastrado com sucesso!", id);
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao cadastrar medida de projeto.", ex);
                }
            }
        }
    }
}
