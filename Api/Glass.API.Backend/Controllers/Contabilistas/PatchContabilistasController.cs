// <copyright file="PatchContabilistasController.cs" company="Sync Softwares">
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
        /// Atualiza um contabilista.
        /// </summary>
        /// <param name="id">O identificador do contabilista que será alterado.</param>
        /// <param name="dadosParaAlteracao">Os novos dados que serão alterados no contabilista indicado.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPatch]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Contabilista alterado.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Contabilista não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AtualizarContabilista(int id, [FromBody] CadastroAtualizacaoDto dadosParaAlteracao)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    sessao.BeginTransaction();

                    var validacao = this.ValidarExistenciaIdContabilista(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var contabilistaAtual = ContabilistaDAO.Instance.GetElementByPrimaryKey(sessao, id);

                    contabilistaAtual = new ConverterCadastroAtualizacaoParaContabilista(dadosParaAlteracao, contabilistaAtual)
                        .ConverterParaContabilista();

                    ContabilistaDAO.Instance.Update(sessao, contabilistaAtual);

                    sessao.Commit();

                    return this.Aceito($"Contabilista atualizado com sucesso!");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao atualizar contabilista.", ex);
                }
            }
        }
    }
}
