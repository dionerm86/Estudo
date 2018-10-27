// <copyright file="PostcategoriasContaController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.PlanosConta.GruposConta.CategoriasConta;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.PlanosConta.V1.GruposConta.CategoriasConta.CadastroAtualizacao;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.PlanosConta.V1.GruposConta.CategoriasConta
{
    /// <summary>
    /// Controller de categorias de conta.
    /// </summary>
    public partial class CategoriasContaController : BaseController
    {
        /// <summary>
        /// Cadastra uma categoria de conta.
        /// </summary>
        /// <param name="dadosParaCadastro">Objeto contendo dados para inserção de uma categoria de conta.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPost]
        [Route("")]
        [SwaggerResponse(201, "Categoria de conta cadastrada.", Type = typeof(CriadoDto<int>))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        public IHttpActionResult CadastrarGrupoConta([FromBody] CadastroAtualizacaoDto dadosParaCadastro)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var grupoConta = new ConverterCadastroAtualizacaoParaCategoriaConta(dadosParaCadastro)
                        .ConverterParaCategoriaConta();

                    var resultado = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Financeiro.Negocios.IPlanoContasFluxo>()
                        .SalvarCategoriaConta(grupoConta);

                    if (!resultado)
                    {
                        return this.ErroValidacao($"Falha ao cadastrar categoria de conta. {resultado.Message.Format()}");
                    }

                    return this.Criado("Categoria de conta cadastrada com sucesso!", grupoConta.IdCategoriaConta);
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao cadastrar categoria de conta.", ex);
                }
            }
        }
    }
}
