// <copyright file="PostContabilistasController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Contabilistas;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Contabilistas.V1.CadastroAtualizacao;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Contabilistas.V1
{
    /// <summary>
    /// Controller de contabilistas.
    /// </summary>
    public partial class ContabilistasController : BaseController
    {
        /// <summary>
        /// Cadastra um contabilista.
        /// </summary>
        /// <param name="dadosParaCadastro">Objeto contendo dados para inserção de um contabilista.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPost]
        [Route("")]
        [SwaggerResponse(201, "Contabilista cadastrado.", Type = typeof(CriadoDto<int>))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        public IHttpActionResult CadastrarContabilista([FromBody] CadastroAtualizacaoDto dadosParaCadastro)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    sessao.BeginTransaction();

                    var contabilista = new ConverterCadastroAtualizacaoParaContabilista(dadosParaCadastro)
                        .ConverterParaContabilista();

                    var id = ContabilistaDAO.Instance.Insert(sessao, contabilista);

                    sessao.Commit();

                    return this.Criado("Contabilista cadastrado com sucesso!", id);
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao cadastrar contabilista.", ex);
                }
            }
        }
    }
}
