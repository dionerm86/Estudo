// <copyright file="PostCoresFerragemController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Produtos.CoresFerragem;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Produtos.CoresFerragem.CadastroAtualizacao;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Produtos.V1.CoresFerragem
{
    /// <summary>
    /// Controller de cores de ferragem.
    /// </summary>
    public partial class CoresFerragemController : BaseController
    {
        /// <summary>
        /// Cadastra uma cor de ferragem.
        /// </summary>
        /// <param name="dadosParaCadastro">Objeto contendo dados para inserção de uma cor de ferragem.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPost]
        [Route("")]
        [SwaggerResponse(201, "Cor de ferragem cadastrada.", Type = typeof(CriadoDto<int>))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        public IHttpActionResult CadastrarCorFerragem([FromBody] CadastroAtualizacaoDto dadosParaCadastro)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var corFerragem = new ConverterCadastroAtualizacaoParaCorFerragem(dadosParaCadastro)
                        .ConverterParaCorFerragem();

                    var idCorFerragem = CorFerragemDAO.Instance.Insert(sessao, corFerragem);
                    sessao.Commit();

                    return this.Criado("Cor de ferragem inserida com sucesso!", idCorFerragem);
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao inserir cor de ferragem.", ex);
                }
            }
        }
    }
}
