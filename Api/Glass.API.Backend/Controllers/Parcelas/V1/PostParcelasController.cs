// <copyright file="PostParcelasController.cs" company="Sync Softwares">
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
    /// Controller de Condutores.
    /// </summary>
    public partial class ParcelasController : BaseController
    {
        /// <summary>
        /// Cadastra um condutor.
        /// </summary>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPost]
        [Route("")]
        [SwaggerResponse(201, "Parcela cadastrada.", Type = typeof(CriadoDto<int>))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        public IHttpActionResult CadastrarParcela([FromBody] CadastroAtualizacaoDto dadosParaCadastro)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var parcela = new ConverterCadastroAtualizacaoParaParcelas(dadosParaCadastro)
                        .ConverterParaParcela();

                    var resultado = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Financeiro.Negocios.IParcelasFluxo>().SalvarParcela(parcela);

                    if (!resultado)
                    {
                        return this.ErroValidacao($"Falha ao cadastrar parcela. {resultado.Message.Format()}");
                    }

                    return this.Criado("Parcela cadastrado com sucesso!", parcela.IdParcela);
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao cadastrar parcela.", ex);
                }
            }
        }
    }
}
