// <copyright file="PatchSeguradorasController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Helper.Seguradoras;
using Glass.API.Backend.Models.Seguradoras.V1.CadastroAtualizacao;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Seguradoras.V1
{
    /// <summary>
    /// Controller de seguradoras.
    /// </summary>
    public partial class SeguradorasController : BaseController
    {
        /// <summary>
        /// Atualiza uma seguradora.
        /// </summary>
        /// <param name="id">O identificador da seguradora que será alterada.</param>
        /// <param name="dadosParaAlteracao">Os novos dados que serão alterados na seguradora indicada.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPatch]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Seguradora alterada.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Seguradora não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AtualizarSeguradora(int id, [FromBody] CadastroAtualizacaoDto dadosParaAlteracao)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaIdSeguradora(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Fiscal.Negocios.ICTeFluxo>();

                    var seguradoraAtual = fluxo.ObtemSeguradora(id);

                    seguradoraAtual = new ConverterCadastroAtualizacaoParaSeguradora(dadosParaAlteracao, seguradoraAtual)
                        .ConverterParaSeguradora();

                    var resultado = fluxo.SalvarSeguradora(seguradoraAtual);

                    if (!resultado)
                    {
                        return this.ErroValidacao($"Falha ao atualizar seguradora. {resultado.Message.Format()}");
                    }

                    return this.Aceito($"Seguradora atualizado com sucesso!");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao atualizar seguradora.", ex);
                }
            }
        }
    }
}
