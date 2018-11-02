// <copyright file="PatchMedidasProjetoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Projetos.MedidasProjeto;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Projetos.V1.MedidasProjeto.CadastroAtualizacao;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Projetos.V1.MedidasProjeto
{
    /// <summary>
    /// Controller de medidas de projeto.
    /// </summary>
    public partial class MedidasProjetoController : BaseController
    {
        /// <summary>
        /// Atualiza ums medida de projeto.
        /// </summary>
        /// <param name="id">O identificador ds medida de projeto que será alterads.</param>
        /// <param name="dadosParaAlteracao">Os novos dados que serão alterados na medida de projeto indicada.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPatch]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Medidas de projeto alterado.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Medidas de projeto não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AtualizarMedidaProjeto(int id, [FromBody] CadastroAtualizacaoDto dadosParaAlteracao)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    sessao.BeginTransaction();

                    var validacao = this.ValidarExistenciaIdMedidaProjeto(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var medidaProjetoAtual = MedidaProjetoDAO.Instance.GetElementByPrimaryKey(sessao, id);

                    medidaProjetoAtual = new ConverterCadastroAtualizacaoParaMedidaProjeto(dadosParaAlteracao, medidaProjetoAtual)
                        .ConverterParaMedidaProjeto();

                    MedidaProjetoDAO.Instance.Update(sessao, medidaProjetoAtual);

                    sessao.Commit();

                    return this.Aceito($"Medidas de projeto atualizado com sucesso!");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao atualizar medida de projeto.", ex);
                }
            }
        }
    }
}
