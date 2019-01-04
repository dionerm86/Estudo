// <copyright file="DeleteProprietariosDeVeiculosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.Data.DAL.CTe;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.ConhecimentosTransporte.Veiculos.Proprietarios
{
    /// <summary>
    /// Controller de proprietários de veiculos.
    /// </summary>
    public partial class ProprietariosDeVeiculosController : BaseController
    {
        /// <summary>
        /// Exclui um proprietário de veículo.
        /// </summary>
        /// <param name="id">O identificador do proprietário de veículo que será excluído.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Proprietário de veículo excluído.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Proprietário de veículo não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirProprietarioDeVeiculo(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdProprietarioVeiculo(sessao, id);

                if (validacao != null)
                {
                    return validacao;
                }

                try
                {
                    sessao.BeginTransaction();

                    ProprietarioVeiculoDAO.Instance.DeleteByPrimaryKey(sessao, id);
                    sessao.Commit();
                    sessao.Close();

                    return this.Aceito(string.Format("Proprietário de veículo {0} excluído com sucesso!", id));
                }
                catch (Exception e)
                {
                    sessao.Rollback();
                    sessao.Close();
                    return this.ErroValidacao("Erro ao excluir o proprietário de veículo.", e);
                }
            }
        }
    }
}