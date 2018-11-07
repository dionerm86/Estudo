// <copyright file="DeleteFerragensController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Projetos.V1.Ferragens
{
    /// <summary>
    /// Controller de ferragens.
    /// </summary>
    public partial class FerragensController : BaseController
    {
        /// <summary>
        /// Exclui uma ferragem.
        /// </summary>
        /// <param name="id">O identificador da ferragem que será excluída.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Ferragem excluída.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Ferragem não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirFerragem(int id)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaIdFerragem(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Projeto.Negocios.IFerragemFluxo>();

                    var ferragem = fluxo.ObterFerragem(id);

                    var resultado = fluxo.ApagarFerragem(ferragem);

                    if (!resultado)
                    {
                        return this.ErroValidacao($"Falha ao excluir ferragem. {resultado.Message.Format()}");
                    }

                    return this.Aceito($"Ferragem excluída.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao excluir ferragem.", ex);
                }
            }
        }
    }
}
