// <copyright file="PatchContasBancariasController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.ContasBancarias;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.ContasBancarias.V1.CadastroAtualizacao;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.ContasBancarias.V1
{
    /// <summary>
    /// Controller de contas bancárias.
    /// </summary>
    public partial class ContasBancariasController : BaseController
    {
        /// <summary>
        /// Atualiza uma conta bancária.
        /// </summary>
        /// <param name="id">O identificador da conta bancária que será alterada.</param>
        /// <param name="dadosParaAlteracao">Os novos dados que serão alterados na conta bancária indicada.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPatch]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Contas bancária alterada.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Contas bancária não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AtualizarContaBancaria(int id, [FromBody] CadastroAtualizacaoDto dadosParaAlteracao)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaIdContaBancaria(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Financeiro.Negocios.IContaBancariaFluxo>();

                    var contaBancariaAtual = fluxo.ObtemContaBanco(id);

                    contaBancariaAtual = new ConverterCadastroAtualizacaoParaContaBancaria(dadosParaAlteracao, contaBancariaAtual)
                        .ConverterParaContaBancaria();

                    var resultado = fluxo.SalvarContaBanco(contaBancariaAtual);

                    if (!resultado)
                    {
                        return this.ErroValidacao($"Falha ao atualizar conta bancária. {resultado.Message.Format()}");
                    }

                    return this.Aceito($"Conta bancária atualizada com sucesso!");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao atualizar conta bancária.", ex);
                }
            }
        }
    }
}
