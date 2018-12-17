// <copyright file="PostTiposFuncionarioController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Funcionarios.Tipos;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Funcionarios.V1.Tipos.CadastroAtualizacao;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Funcionarios.V1.Tipos
{
    /// <summary>
    /// Controller de tipos de funcionário.
    /// </summary>
    public partial class TiposFuncionarioController : BaseController
    {
        /// <summary>
        /// Cadastra um tipo de funcionário.
        /// </summary>
        /// <param name="dados">Objeto contendo dados para inserção de um tipo de funcionário.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPost]
        [Route("")]
        [SwaggerResponse(201, "Tipo de funcionário cadastrado.", Type = typeof(CriadoDto<int>))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        public IHttpActionResult CadastrarTipoFuncionario([FromBody] CadastroAtualizacaoDto dados)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    sessao.BeginTransaction();

                    var tipoFuncionario = new ConverterCadastroAtualizacaoParaTipoFuncionario(dados)
                        .ConverterParaTipoFuncionario();

                    var id = Microsoft.Practices.ServiceLocation.ServiceLocator.Current
                        .GetInstance<Glass.Global.Negocios.IFuncionarioFluxo>()
                        .SalvarTipoFuncionario(tipoFuncionario);

                    sessao.Commit();

                    return this.Criado("Tipo de funcionário cadastrado com sucesso!", id);
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao cadastrar tipo de funcionário.", ex);
                }
            }
        }
    }
}