// <copyright file="PatchTiposClienteController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.TabelasDescontoAcrescimoCliente;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.TabelasDescontoAcrescimoCliente.CadastroAtualizacao;
using Glass.Data.DAL;
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
        /// Atualiza uma tabela de desconto/acréscimo de cliente.
        /// </summary>
        /// <param name="id">O identificador da tabela de desconto/acréscimo de cliente que será alterada.</param>
        /// <param name="dadosParaAlteracao">Os novos dados que serão alterados na tabela de desconto/acréscimo de cliente indicada.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPatch]
        [Route("{id}")]
        [SwaggerResponse(202, "Tabela de desconto/acréscimo de cliente alterada.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Tabela de desconto/acréscimo de cliente não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AlterarTabelaDescontoAcrescimoCliente(int id, [FromBody] CadastroAtualizacaoDto dadosParaAlteracao)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaIdTabelaDescontoAcrescimoCliente(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Global.Negocios.IClienteFluxo>();

                    var tabelaAtual = fluxo.ObtemTabelaDescontoAcrescimoCliente(id);

                    var tabela = new ConverterCadastroAtualizacaoParaTabelaDescontoAcrescimoCliente(dadosParaAlteracao, tabelaAtual)
                        .ConverterParaTabelaDescontoAcrescimoCliente();

                    var resultado = fluxo.SalvarTabelaDescontoAcrescimo(tabela);

                    if (!resultado)
                    {
                        return this.ErroValidacao($"Falha ao atualizar tabela de desconto/acréscimo de cliente. {resultado.Message.Format()}");
                    }

                    return this.Aceito($"Tabela de desconto/acréscimo de cliente atualizada com sucesso!");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao atualizar tabela de desconto/acréscimo de cliente.", ex);
                }
            }
        }
    }
}
