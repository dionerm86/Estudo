// <copyright file="DeleteParcelasController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Parcelas.V1
{
    /// <summary>
    /// Controller de parcelas.
    /// </summary>
    public partial class ParcelasController : BaseController
    {
        /// <summary>
        /// Exclui uma parcela.
        /// </summary>
        /// <param name="id">O identificador da parcela que será excluída.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Parcela excluída.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Parcela não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirParcela(int id)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaIdParcela(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Financeiro.Negocios.IParcelasFluxo>();

                    var parcela = fluxo.ObtemParcela(id);

                    var resultado = fluxo.ApagarParcela(parcela);

                    if (!resultado)
                    {
                        return this.ErroValidacao($"Falha ao excluir parcela. {resultado.Message.Format()}");
                    }

                    return this.Aceito($"Parcela {id} excluída.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao excluir parcela {id}.", ex);
                }
            }
        }
    }
}
