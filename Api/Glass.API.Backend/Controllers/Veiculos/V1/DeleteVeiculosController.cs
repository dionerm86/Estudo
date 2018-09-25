// <copyright file="DeleteVeiculosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Veiculos.V1
{
    /// <summary>
    /// Controller de veículos.
    /// </summary>
    public partial class VeiculosController : BaseController
    {
        /// <summary>
        /// Exclui um veículo.
        /// </summary>
        /// <param name="placa">O identificador do veículo que será excluído.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id}")]
        [SwaggerResponse(202, "Veículo excluído.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Veículo não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirVeiculo(string placa)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaPlacaVeiculo(sessao, placa);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Global.Negocios.IVeiculoFluxo>();

                    var veiculo = fluxo.ObtemVeiculo(placa);

                    fluxo.ApagarVeiculo(veiculo);

                    return this.Aceito($"Veículo excluído.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao excluir veículo.", ex);
                }
            }
        }
    }
}
