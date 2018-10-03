// <copyright file="DeleteFornecedoresController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
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
        /// Exclui um fornecedor.
        /// </summary>
        /// <param name="id">O identificador do fornecedor que será excluído.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id}")]
        [SwaggerResponse(202, "Fornecedor excluído.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Fornecedor não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirFornecedor(int id)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaIdFornecedor(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Global.Negocios.IFornecedorFluxo>();

                    var fornecedor = fluxo.ObtemFornecedor(id);

                    var resultado = fluxo.ApagarFornecedor(fornecedor);

                    if (!resultado)
                    {
                        return this.ErroValidacao(resultado.Message.Format());
                    }

                    return this.Aceito($"Fornecedor excluído.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao excluir fornecedor.", ex);
                }
            }
        }
    }
}
