// <copyright file="PatchParcelasController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Parcelas;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Parcelas.V1.CadastroAtualizacao;
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
        /// Atualiza uma parcela.
        /// </summary>
        /// <param name="id">O identificador da parcela que será alterada.</param>
        /// <param name="dadosParaAlteracao">Os novos dados que serão alterados na parcela indicada.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPatch]
        [Route("{id}")]
        [SwaggerResponse(202, "Parcela alterada.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação na alteração da parcela.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Parcela não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AlterarParcela(int id, [FromBody] CadastroAtualizacaoDto dadosParaAlteracao)
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

                    var parcela = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Financeiro.Negocios.IParcelasFluxo>().ObtemParcela(id);

                    parcela = new ConverterCadastroAtualizacaoParaParcelas(dadosParaAlteracao, parcela)
                        .ConverterParaParcela();

                    var resultado = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Financeiro.Negocios.IParcelasFluxo>().SalvarParcela(parcela);

                    if (!resultado)
                    {
                        return this.ErroValidacao($"Falha ao atualizar a parcela. {resultado.Message.Format()}");
                    }

                    return this.Aceito($"Parcela {id} atualizada com sucesso!");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao atualizar parcela.", ex);
                }
            }
        }
    }
}
