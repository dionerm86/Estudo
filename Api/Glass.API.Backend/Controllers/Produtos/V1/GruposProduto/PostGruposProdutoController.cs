// <copyright file="PostGruposProdutoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Produtos.GruposProduto;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Produtos.GruposProduto.CadastroAtualizacao;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Produtos.V1.GruposProduto
{
    /// <summary>
    /// Controller de grupos de produto.
    /// </summary>
    public partial class GruposProdutoController : BaseController
    {
        /// <summary>
        /// Cadastra um grupo de produto.
        /// </summary>
        /// <param name="dadosParaCadastro">Objeto contendo dados para inserção de um grupo de produto.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPost]
        [Route("")]
        [SwaggerResponse(201, "Grupo de produto cadastrado.", Type = typeof(CriadoDto<int>))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        public IHttpActionResult CadastrarGrupo([FromBody] CadastroAtualizacaoDto dadosParaCadastro)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var grupo = new ConverterCadastroAtualizacaoParaGrupoProduto(dadosParaCadastro)
                        .ConverterParaGrupoProduto();

                    var resultado = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Global.Negocios.IGrupoProdutoFluxo>()
                        .SalvarGrupoProduto(grupo);

                    if (!resultado)
                    {
                        return this.ErroValidacao($"Falha ao cadastrar grupo de produto. {resultado.Message.Format()}");
                    }

                    return this.Criado("Grupo de produto cadastrado com sucesso!", 0);
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao cadastrar grupo de produto.", ex);
                }
            }
        }
    }
}
