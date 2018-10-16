// <copyright file="PostTiposClienteController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Clientes.Tipos;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Clientes.V1.Tipos.CadastroAtualizacao;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Clientes.V1.Tipos
{
    /// <summary>
    /// Controller de tipos de cliente.
    /// </summary>
    public partial class TiposClienteController : BaseController
    {
        /// <summary>
        /// Cadastra um tipo de cliente.
        /// </summary>
        /// <param name="dadosParaCadastro">Objeto contendo dados para inserção de um tipo de cliente.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPost]
        [Route("")]
        [SwaggerResponse(201, "Tipo de cliente cadastrado.", Type = typeof(CriadoDto<int>))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        public IHttpActionResult CadastrarTipoCliente([FromBody] CadastroAtualizacaoDto dadosParaCadastro)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var tipo = new ConverterCadastroAtualizacaoParaTipo(dadosParaCadastro)
                        .ConverterParaTipo();

                    var idTipoCliente = TipoClienteDAO.Instance.Insert(sessao, tipo);
                    sessao.Commit();

                    return this.Criado("Tipo de cliente inserido com sucesso!", idTipoCliente);
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao inserir tipo de cliente.", ex);
                }
            }
        }
    }
}
