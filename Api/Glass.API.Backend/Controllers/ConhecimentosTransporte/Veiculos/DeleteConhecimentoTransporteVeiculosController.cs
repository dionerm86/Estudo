// <copyright file="DeleteConhecimentoTransporteVeiculosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.ConhecimentosTransporte.Veiculos
{
    /// <summary>
    /// Controller de veículos para conhecimento de transporte.
    /// </summary>
    public partial class ConhecimentoTransporteVeiculosController : BaseController
    {
        /// <summary>
        /// Exclui uma associação entre proprietário e veículo.
        /// </summary>
        /// <param name="placa">O identificador do veículo.</param>
        /// <param name="idProprietario">O identificador do proprietário do veículo.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpDelete]
        [Route("{placa}/proprietarios/{idProprietario:int}/associacoes")]
        [SwaggerResponse(202, "Associação de proprietário com veículo excluída.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Associação de proprietário com veículo não encontrada para o idProprietario e placa informados.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirAssociacaoProprietarioVeiculo(string placa, int idProprietario)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExclusaoAssociacaoProprietarioVeiculo(sessao, idProprietario, placa);

                if (validacao != null)
                {
                    return validacao;
                }

                try
                {
                    sessao.BeginTransaction();

                    var associacaoProprietarioComVeiculo = Data.DAL.CTe.ProprietarioVeiculo_VeiculoDAO.Instance.GetElement(sessao, placa, (uint)idProprietario);

                    Data.DAL.CTe.ProprietarioVeiculo_VeiculoDAO.Instance.Delete(sessao, associacaoProprietarioComVeiculo);

                    sessao.Commit();
                    sessao.Close();

                    return this.Aceito(string.Format($"Associação do Proprietário de veículo '{idProprietario}' com o veículo de placa '{placa}' excluída com sucesso!"));
                }
                catch (Exception e)
                {
                    sessao.Rollback();
                    sessao.Close();
                    return this.ErroValidacao("Erro ao excluir a associação de proprietário com veículo.", e);
                }
            }
        }
    }
}