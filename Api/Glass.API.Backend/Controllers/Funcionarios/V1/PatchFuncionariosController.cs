using GDA;
using Glass.API.Backend.Helper.Funcionarios;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Funcionarios.V1.CadastroAtualizacao;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Funcionarios.V1
{
    /// <summary>
    /// Controller de funcionários.
    /// </summary>
    public partial class FuncionariosController : BaseController
    {
        /// <summary>
        /// Atualiza os dados de um funciónário.
        /// </summary>
        /// <param name="id">O identificador do pedido.</param>
        /// <param name="dadosParaAtualizacao">Os dados que serão atualizados no funcionário.</param>
        /// <returns>Um status HTTP que indica se o funcionário foi atualizado.</returns>
        [HttpPatch]
        [Route("{id}")]
        [SwaggerResponse(202, "Funcionário atualizado.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação ou de valor ou formato inválido do campo id.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Funcionário não encontrado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AtualizarFuncionario(int id, [FromBody] CadastroAtualizacaoDto dadosParaAtualizacao)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarAtualizacaoFuncionario(sessao, id, dadosParaAtualizacao);

                if (validacao != null)
                {
                    return validacao;
                }

                var funcionarioFluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Glass.Global.Negocios.IFuncionarioFluxo>();

                var funcionario = funcionarioFluxo.ObtemFuncionario(id);

                if (funcionario == null)
                {
                    return this.NaoEncontrado(string.Format("Funcionário {0} não encontrado.", id));
                }

                try
                {
                    funcionario = new ConverterCadastroAtualizacaoParaFuncionario(funcionarioFluxo, dadosParaAtualizacao, funcionario)
                        .ConverterParaFuncionario();

                    var resultado = funcionarioFluxo.SalvarFuncionario(funcionario);

                    if (resultado)
                    {
                        if (!dadosParaAtualizacao.VerificarCampoInformado(c => c.DocumentosEDadosPessoais.Foto))
                        {
                            return this.Aceito(string.Format($"Funcionário {id} atualizado com sucesso!"));
                        }

                        byte[] bytes = Convert.FromBase64String(dadosParaAtualizacao.DocumentosEDadosPessoais.Foto);

                        var imagem = new System.IO.MemoryStream(bytes);

                        var repositorio = Microsoft.Practices.ServiceLocation.ServiceLocator
                            .Current.GetInstance<Global.Negocios.Entidades.IFuncionarioRepositorioImagens>();

                        repositorio.SalvarImagem(funcionario.IdFunc, imagem);

                        Microsoft.Practices.ServiceLocation.ServiceLocator
                                .Current.GetInstance<Global.Negocios.IMenuFluxo>().RemoveMenuFuncMemoria(funcionario.IdFunc);

                        return this.Aceito(string.Format($"Funcionário {id} atualizado com sucesso!"));
                    }

                    return this.ErroValidacao($"Erro ao atualizar o funcionário {id}. {resultado.Message.Format()}");
                }
                catch (Exception e)
                {
                    return this.ErroValidacao($"Erro ao atualizar o funcionário {id}.", e);
                }
            }
        }
    }
}
