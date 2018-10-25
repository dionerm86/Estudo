// <copyright file="PostCondutoresController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Condutores.CadastroAtualizacao;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Condutores.V1
{
    /// <summary>
    /// Controller de Condutores.
    /// </summary>
    public partial class CondutoresController : BaseController
    {
        /// <summary>
        /// Cadastra um condutor.
        /// </summary>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPost]
        [Route("")]
        [SwaggerResponse(201, "Condutor cadastrado.", Type = typeof(CriadoDto<int>))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        public IHttpActionResult CadastrarCondutor([FromBody] CadastroAtualizacaoDto dadosParaCadastro)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarCadastroCondutor(sessao, dadosParaCadastro);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var condutor = new Helper.Condutores.ConverterCadastroAtualizacaoParaCondutor(dadosParaCadastro)
                        .ConverterParaCondutor();

                    var idCondutor = CondutoresDAO.Instance.Insert(sessao, condutor);
                    sessao.Commit();

                    return this.Criado($"Condutor {idCondutor} inserido com sucesso!", idCondutor);
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao inserir condutor.", ex);
                }
            }
        }
    }
}
