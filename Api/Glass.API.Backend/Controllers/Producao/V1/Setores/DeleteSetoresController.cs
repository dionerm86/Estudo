// <copyright file="DeleteSetoresController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Producao.V1.Setores
{
    /// <summary>
    /// Controller de setores.
    /// </summary>
    public partial class SetoresController : BaseController
    {
        /// <summary>
        /// Exclui um setor.
        /// </summary>
        /// <param name="id">O identificador do setor que será excluído.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Setor excluído.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Setor não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirSetor(int id)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaIdSetor(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<PCP.Negocios.ISetorFluxo>();

                    var setor = fluxo.ObtemSetor(id);

                    var resultado = fluxo.ApagarSetor(setor);

                    if (!resultado)
                    {
                        return this.ErroValidacao($"Falha ao excluir setor. {resultado.Message.Format()}");
                    }

                    return this.Aceito($"Setor excluído.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao excluir setor.", ex);
                }
            }
        }
    }
}
