// <copyright file="PatchCoresVidroController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Datas.Feriados;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Datas.Feriados.CadastroAtualizacao;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Datas.V1.Feriados
{
    /// <summary>
    /// Controller de feriados.
    /// </summary>
    public partial class FeriadosController : BaseController
    {
        /// <summary>
        /// Atualiza um feriado.
        /// </summary>
        /// <param name="id">O identificador do feriado que será alterada.</param>
        /// <param name="dadosParaAlteracao">Os novos dados que serão alterados no feriado indicado.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPatch]
        [Route("{id}")]
        [SwaggerResponse(202, "Feriado alterado.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Feriado não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AlterarFeriado(int id, [FromBody] CadastroAtualizacaoDto dadosParaAlteracao)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var feriado = FeriadoDAO.Instance.GetElementByPrimaryKey(sessao, id);

                    if (feriado == null)
                    {
                        return this.NaoEncontrado($"Feriado {id} não encontrado.");
                    }

                    feriado = new ConverterCadastroAtualizacaoParaFeriado(dadosParaAlteracao, feriado)
                        .ConverterParaFeriado();

                    FeriadoDAO.Instance.Update(sessao, feriado);
                    sessao.Commit();

                    return this.Aceito($"Feriado atualizado com sucesso!");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao atualizar feriado.", ex);
                }
            }
        }
    }
}
