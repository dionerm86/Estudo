// <copyright file="PatchCoresVidroController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Clientes.Tipos;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Clientes.Tipos.CadastroAtualizacao;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Clientes.V1.Tipos
{
    /// <summary>
    /// Controller de tipos de cliente.
    /// </summary>
    public partial class TiposClienteController : BaseController
    {
        /// <summary>
        /// Atualiza um tipo de cliente.
        /// </summary>
        /// <param name="id">O identificador do tipo de cliente que será alterado.</param>
        /// <param name="dadosParaAlteracao">Os novos dados que serão alterados no tipo de cliente indicado.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPatch]
        [Route("{id}")]
        [SwaggerResponse(202, "Tipo de cliente alterado.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Tipo de cliente não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AlterarTipo(int id, [FromBody] CadastroAtualizacaoDto dadosParaAlteracao)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var tipo = TipoClienteDAO.Instance.GetElementByPrimaryKey(sessao, id);

                    if (tipo == null)
                    {
                        return this.NaoEncontrado($"Tipo de cliente {id} não encontrado.");
                    }

                    tipo = new ConverterCadastroAtualizacaoParaTipo(dadosParaAlteracao, tipo)
                        .ConverterParaTipo();

                    TipoClienteDAO.Instance.Update(sessao, tipo);
                    sessao.Commit();

                    return this.Aceito($"Tipo de cliente atualizado com sucesso!");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao atualizar tipo de cliente.", ex);
                }
            }
        }
    }
}
