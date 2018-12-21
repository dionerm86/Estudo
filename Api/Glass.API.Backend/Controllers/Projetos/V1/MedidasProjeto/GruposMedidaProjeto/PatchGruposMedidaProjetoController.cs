// <copyright file="PatchGruposMedidaProjetoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Projetos.Medidas.Grupos;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Projetos.V1.Medidas.Grupos.CadastroAtualizacao;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Projetos.V1.MedidasProjeto.GruposMedidaProjeto
{
    /// <summary>
    /// Controller de grupos de medida de projeto.
    /// </summary>
    public partial class GruposMedidaProjetoController : BaseController
    {
        /// <summary>
        /// Atualiza um grupo de medida de projeto.
        /// </summary>
        /// <param name="id">O identificador do grupo de medida de projeto que será alterado.</param>
        /// <param name="dados">Os novos dados que serão alterados no grupo de medida de projeto indicado.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPatch]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Grupo de medida de projeto alterado.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Grupo de medida de projeto não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AtualizarGrupoMedidaProjeto(int id, [FromBody] CadastroAtualizacaoDto dados)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdGrupoMedidaProjeto(sessao, id);

                if (validacao != null)
                {
                    return validacao;
                }

                try
                {
                    sessao.BeginTransaction();

                    var grupoMedidaProjetoAtual = GrupoMedidaProjetoDAO.Instance.GetElementByPrimaryKey(id);

                    grupoMedidaProjetoAtual = new ConverterCadastroAtualizacaoParaGrupoMedidaProjeto(dados, grupoMedidaProjetoAtual)
                        .ConverterParaGrupoMedidaProjeto();

                    LogAlteracaoDAO.Instance.LogGrupoMedidaProjeto(grupoMedidaProjetoAtual);

                    GrupoMedidaProjetoDAO.Instance.Update(sessao, grupoMedidaProjetoAtual);

                    sessao.Commit();

                    return this.Aceito($"Grupo de medida de projeto atualizado com sucesso!");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao atualizar grupo de medida de projeto.", ex);
                }
            }
        }
    }
}