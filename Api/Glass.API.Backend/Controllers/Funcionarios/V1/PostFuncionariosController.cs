// <copyright file="PostFuncionariosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Funcionarios;
using Glass.API.Backend.Helper.Respostas;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Funcionarios.V1
{
    /// <summary>
    /// Controller de funcionarios.
    /// </summary>
    public partial class FuncionariosController : BaseController
    {
        /// <summary>
        /// Realiza a emissão de um pedido novo.
        /// </summary>
        /// <param name="dadosParaCadastro">Os dados necessários para o cadastro do funcionário.</param>
        /// <returns>Um status HTTP que indica se o funcionário foi inserido.</returns>
        [HttpPost]
        [Route("")]
        [SwaggerResponse(201, "Funcionário inserido.", Type = typeof(CriadoDto<int>))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        public IHttpActionResult CadastrarFuncionario([FromBody] Models.Funcionarios.CadastroAtualizacao.CadastroAtualizacaoDto dadosParaCadastro)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarCadastroFuncionario(sessao, dadosParaCadastro);

                if (validacao != null)
                {
                    return validacao;
                }

                try
                {
                    var funcionario = new ConverterCadastroAtualizacaoParaFuncionario(dadosParaCadastro)
                        .ConverterParaFuncionario();

                    var funcionarioFluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Glass.Global.Negocios.IFuncionarioFluxo>();

                    var resultado = funcionarioFluxo.SalvarFuncionario(funcionario);

                    return resultado ? this.Criado(string.Format("Funcinário {0} inserido com sucesso!"), funcionario.IdFunc) :
                        (IHttpActionResult)this.ErroValidacao($"Erro ao inserir o pedido. {resultado.Message.Format()}");
                }
                catch (Exception e)
                {
                    sessao.Rollback();
                    return this.ErroValidacao("Erro ao inserir o pedido.", e);
                }
            }
        }
    }
}
