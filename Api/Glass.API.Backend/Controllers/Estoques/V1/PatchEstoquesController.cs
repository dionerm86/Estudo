// <copyright file="PatchEstoquesController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Estoques;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Estoques.CadastroAtualizacao;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Estoques.V1
{
    /// <summary>
    /// Controller de estoques.
    /// </summary>
    public partial class EstoquesController : BaseController
    {
        /// <summary>
        /// Atualiza dados do estoque real do produto.
        /// </summary>
        /// <param name="idProduto">O identificador do produto que terá o estoque alterado.</param>
        /// <param name="idLoja">O identificador da loja que terá o estoque alterado.</param>
        /// <param name="dadosParaAlteracao">Os novos dados que serão alterados no estoque de produto indicado.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPatch]
        [Route("produto/{idProduto}/loja/{idLoja}/real")]
        [SwaggerResponse(202, "Dados de estoque do produto atualizados.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação ou de valor ou formato inválido do campo idProduto ou idLoja.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Estoque de produto não encontrado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AlterarEstoqueReal(int idProduto, int idLoja, [FromBody] CadastroAtualizacaoRealDto dadosParaAlteracao)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaIdProdutoIdLoja(sessao, idProduto, idLoja);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var estoque = ProdutoLojaDAO.Instance.GetElement(sessao, (uint)idLoja, (uint)idProduto);

                    if (estoque == null)
                    {
                        return this.NaoEncontrado($"Estoque de produto não encontrado.");
                    }

                    estoque = new ConverterCadastroAtualizacaoParaEstoqueReal(dadosParaAlteracao, estoque)
                        .ConverterParaEstoque();

                    ProdutoLojaDAO.Instance.AtualizaEstoque(sessao, estoque);
                    sessao.Commit();

                    return this.Aceito("Estoque do produto atualizado com sucesso!");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao("Erro ao atualizar estoque do produto.", ex);
                }
            }
        }

        /// <summary>
        /// Atualiza dados do estoque fiscal do produto.
        /// </summary>
        /// <param name="idProduto">O identificador do produto que terá o estoque alterado.</param>
        /// <param name="idLoja">O identificador da loja que terá o estoque alterado.</param>
        /// <param name="dadosParaAlteracao">Os novos dados que serão alterados no estoque de produto indicado.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPatch]
        [Route("produto/{idProduto}/loja/{idLoja}/fiscal")]
        [SwaggerResponse(202, "Dados de estoque do produto atualizados.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação ou de valor ou formato inválido do campo idProduto ou idLoja.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Estoque de produto não encontrado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AlterarEstoqueFiscal(int idProduto, int idLoja, [FromBody] CadastroAtualizacaoFiscalDto dadosParaAlteracao)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaIdProdutoIdLoja(sessao, idProduto, idLoja);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var estoque = ProdutoLojaDAO.Instance.GetElement(sessao, (uint)idLoja, (uint)idProduto);

                    if (estoque == null)
                    {
                        return this.NaoEncontrado($"Estoque de produto não encontrado.");
                    }

                    estoque = new ConverterCadastroAtualizacaoParaEstoqueFiscal(dadosParaAlteracao, estoque)
                        .ConverterParaEstoque();

                    ProdutoLojaDAO.Instance.AtualizaEstoque(sessao, estoque);
                    sessao.Commit();

                    return this.Aceito("Estoque do produto atualizado com sucesso!");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao("Erro ao atualizar estoque do produto.", ex);
                }
            }
        }

        /// <summary>
        /// Atualiza dados do estoque real do produto.
        /// </summary>
        /// <param name="idProduto">O identificador do produto que terá o estoque alterado.</param>
        /// <param name="idLoja">O identificador da loja que terá o estoque alterado.</param>
        /// <param name="dadosParaAlteracao">Os novos dados que serão alterados no estoque de produto indicado.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPatch]
        [Route("produto/{idProduto}/loja/{idLoja}/atualizarQuantidadeReal")]
        [SwaggerResponse(202, "Dados de estoque do produto atualizados.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação ou de valor ou formato inválido do campo idProduto ou idLoja.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Estoque de produto não encontrado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AtualizarQuantidadeReal(int idProduto, int idLoja, [FromBody] CadastroAtualizacaoRapidaRealDto dadosParaAlteracao)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaIdProdutoIdLoja(sessao, idProduto, idLoja);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var estoque = ProdutoLojaDAO.Instance.GetElement(sessao, (uint)idLoja, (uint)idProduto);

                    if (estoque == null)
                    {
                        return this.NaoEncontrado($"Estoque de produto não encontrado.");
                    }

                    estoque.QtdEstoque = (double)dadosParaAlteracao.QuantidadeEstoque.GetValueOrDefault();

                    ProdutoLojaDAO.Instance.AtualizaEstoque(sessao, estoque);
                    sessao.Commit();

                    return this.Aceito("Estoque do produto atualizado com sucesso!");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao("Erro ao atualizar estoque do produto.", ex);
                }
            }
        }

        /// <summary>
        /// Atualiza dados do estoque fiscal do produto.
        /// </summary>
        /// <param name="idProduto">O identificador do produto que terá o estoque alterado.</param>
        /// <param name="idLoja">O identificador da loja que terá o estoque alterado.</param>
        /// <param name="dadosParaAlteracao">Os novos dados que serão alterados no estoque de produto indicado.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPatch]
        [Route("produto/{idProduto}/loja/{idLoja}/atualizarQuantidadeFiscal")]
        [SwaggerResponse(202, "Dados de estoque do produto atualizados.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação ou de valor ou formato inválido do campo idProduto ou idLoja.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Estoque de produto não encontrado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AtualizarQuantidadeFiscal(int idProduto, int idLoja, [FromBody] CadastroAtualizacaoRapidaFiscalDto dadosParaAlteracao)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaIdProdutoIdLoja(sessao, idProduto, idLoja);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var estoque = ProdutoLojaDAO.Instance.GetElement(sessao, (uint)idLoja, (uint)idProduto);

                    if (estoque == null)
                    {
                        return this.NaoEncontrado($"Estoque de produto não encontrado.");
                    }

                    estoque.EstoqueFiscal = (double)dadosParaAlteracao.QuantidadeEstoqueFiscal.GetValueOrDefault();

                    ProdutoLojaDAO.Instance.AtualizaEstoque(sessao, estoque);
                    sessao.Commit();

                    return this.Aceito("Estoque do produto atualizado com sucesso!");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao("Erro ao atualizar estoque do produto.", ex);
                }
            }
        }
    }
}
