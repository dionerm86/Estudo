// <copyright file="PatchFerragensController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Projetos.Ferragens;
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
        /// Altera a situação de uma ferragem.
        /// </summary>
        /// <param name="id">O identificador da ferragem que terá a situação alterada.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPatch]
        [Route("{id:int}/situacao")]
        [SwaggerResponse(202, "Situação da ferragem alterada.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Ferragem não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AlterarSituacaoFerragem(int id)
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

                    var resultado = fluxo.AtivarInativarFerragem(fluxo.ObterFerragem(id));

                    if (!resultado)
                    {
                        return this.ErroValidacao($"Falha ao atualizar situação da ferragem. {resultado.Message.Format()}");
                    }

                    return this.Aceito($"Situação da ferragem alterada com sucesso!");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao atualizar situação da ferragem.", ex);
                }
            }
        }
    }
}
