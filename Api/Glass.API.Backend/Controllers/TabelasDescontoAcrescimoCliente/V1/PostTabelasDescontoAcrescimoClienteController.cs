// <copyright file="PostTabelasDescontoAcrescimoClienteController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Helper.TabelasDescontoAcrescimoCliente;
using Glass.API.Backend.Models.TabelasDescontoAcrescimoCliente.CadastroAtualizacao;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.TabelasDescontoAcrescimoCliente.V1
{
    /// <summary>
    /// Controller de tabelas de desconto/acréscimo de cliente.
    /// </summary>
    public partial class TabelasDescontoAcrescimoClienteController : BaseController
    {
        /// <summary>
        /// Cadastra uma tabela de desconto/acréscimo de cliente.
        /// </summary>
        /// <param name="dadosParaCadastro">Objeto contendo dados para inserção de uma tabela de desconto/acréscimo de cliente.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPost]
        [Route("")]
        [SwaggerResponse(201, "Tabela de desconto/acréscimo de cliente cadastrada.", Type = typeof(CriadoDto<int>))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        public IHttpActionResult CadastrarTabelaDescontoAcrescimoCliente([FromBody] CadastroAtualizacaoDto dadosParaCadastro)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var tabela = new ConverterCadastroAtualizacaoParaTabelaDescontoAcrescimoCliente(dadosParaCadastro)
                        .ConverterParaTabelaDescontoAcrescimoCliente();

                    var resultado = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Global.Negocios.IClienteFluxo>()
                        .SalvarTabelaDescontoAcrescimo(tabela);

                    if (!resultado)
                    {
                        return this.ErroValidacao($"Falha ao cadastrar tabela de desconto/acréscimo de cliente. {resultado.Message.Format()}");
                    }

                    return this.Criado("Tabela de desconto/acréscimo de cliente inserida com sucesso!", 0);
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao inserir tabela de desconto/acréscimo de cliente.", ex);
                }
            }
        }
    }
}
