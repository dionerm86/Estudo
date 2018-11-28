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
        public IHttpActionResult CadastrarFuncionario([FromBody] Models.Funcionarios.V1.CadastroAtualizacao.CadastroAtualizacaoDto dadosParaCadastro)
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
                    var funcionarioFluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Glass.Global.Negocios.IFuncionarioFluxo>();

                    var funcionario = new ConverterCadastroAtualizacaoParaFuncionario(funcionarioFluxo, dadosParaCadastro)
                        .ConverterParaFuncionario();

                    var resultado = funcionarioFluxo.SalvarFuncionario(funcionario);

                    if (resultado)
                    {
                        if (!dadosParaCadastro.VerificarCampoInformado(c => c.DocumentosEDadosPessoais.Foto))
                        {
                            return this.Criado(string.Format($"Funcinário {funcionario.IdFunc} inserido com sucesso!"), funcionario.IdFunc);
                        }

                        byte[] bytes = System.Convert.FromBase64String(dadosParaCadastro.DocumentosEDadosPessoais.Foto);

                        var imagem = new System.IO.MemoryStream(bytes);

                        var repositorio = Microsoft.Practices.ServiceLocation.ServiceLocator
                            .Current.GetInstance<Glass.Global.Negocios.Entidades.IFuncionarioRepositorioImagens>();

                        repositorio.SalvarImagem(funcionario.IdFunc, imagem);

                        Microsoft.Practices.ServiceLocation.ServiceLocator
                                .Current.GetInstance<Global.Negocios.IMenuFluxo>().RemoveMenuFuncMemoria(funcionario.IdFunc);

                        return this.Criado(string.Format($"Funcinário {funcionario.IdFunc} inserido com sucesso!"), funcionario.IdFunc);
                    }

                    return this.ErroValidacao($"Erro ao inserir o funcionário. {resultado.Message.Format()}");

                }
                catch (Exception e)
                {
                    sessao.Rollback();
                    return this.ErroValidacao("Erro ao inserir o funcionário.", e);
                }
            }
        }
    }
}
