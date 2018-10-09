// <copyright file="PostFornecedoresController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Microsoft.Practices.ServiceLocation;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Fornecedores.V1
{
    /// <summary>
    /// Controller de fornecedores.
    /// </summary>
    public partial class FornecedoresController : BaseController
    {
        /// <summary>
        /// Altera a situação do fornecedor.
        /// </summary>
        /// <param name="id">O identificador do fornecedor que será ativado/inativado.</param>
        /// <returns>Um status HTTP indicando se o fornecedor foi ativado/inativado.</returns>
        [HttpPost]
        [Route("{id}/alterarSituacao")]
        [SwaggerResponse(202, "Situação do fornecedor alterada.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Fornecedor não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AlterarSituacao(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdFornecedor(sessao, id);

                if (validacao != null)
                {
                    return validacao;
                }

                try
                {
                    var fluxo = ServiceLocator.Current
                        .GetInstance<Global.Negocios.IFornecedorFluxo>();

                    var fornecedor = fluxo.ObtemFornecedor(id);

                    fornecedor.Situacao =
                        fornecedor.Situacao == Data.Model.SituacaoFornecedor.Ativo ?
                        Data.Model.SituacaoFornecedor.Inativo : Data.Model.SituacaoFornecedor.Ativo;

                    var resultado = fluxo.SalvarFornecedor(fornecedor);

                    if (!resultado)
                    {
                        this.ErroValidacao($"Não foi possível alterar a situação do fornecedor. Erro: {resultado.Message.Format()}");
                    }

                    return this.Aceito("Situação do fornecedor alterada com sucesso.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao(ex.Message, ex);
                }
            }
        }
    }
}
