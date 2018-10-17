// <copyright file="PostSubgruposProdutoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Produtos.SubgruposProduto;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Produtos.V1.SubgruposProduto.CadastroAtualizacao;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Produtos.V1.SubgruposProduto
{
    /// <summary>
    /// Controller de subgrupos de produto.
    /// </summary>
    public partial class SubgruposProdutoController : BaseController
    {
        /// <summary>
        /// Cadastra um subgrupo de produto.
        /// </summary>
        /// <param name="dadosParaCadastro">Objeto contendo dados para inserção de um subgrupo de produto.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPost]
        [Route("")]
        [SwaggerResponse(201, "Subgrupo de produto cadastrado.", Type = typeof(CriadoDto<int>))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        public IHttpActionResult CadastrarSubgrupo([FromBody] CadastroAtualizacaoDto dadosParaCadastro)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var subgrupo = new ConverterCadastroAtualizacaoParaSubgrupoProduto(dadosParaCadastro)
                        .ConverterParaSubgrupoProduto();

                    var resultado = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Global.Negocios.IGrupoProdutoFluxo>()
                        .SalvarSubgrupoProduto(subgrupo);

                    if (!resultado)
                    {
                        return this.ErroValidacao($"Falha ao cadastrar subgrupo de produto. {resultado.Message.Format()}");
                    }

                    return this.Criado("Subgrupo de produto cadastrado com sucesso!", 0);
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao cadastrar subgrupo de produto.", ex);
                }
            }
        }
    }
}
