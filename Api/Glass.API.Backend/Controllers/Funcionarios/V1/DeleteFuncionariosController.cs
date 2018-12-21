// <copyright file="DeleteFuncionariosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Funcionarios.V1
{
    /// <summary>
    /// Controller de Funcionários.
    /// </summary>
    public partial class FuncionariosController : BaseController
    {
        /// <summary>
        /// Exclui um funcionario, se possível.
        /// </summary>
        /// <param name="id">O identificador do funcionário que será excluído.</param>
        /// <returns>Um status HTTP indicando se o funcionário foi excluído.</returns>
        [HttpDelete]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Funcionário excluído.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de valor ou formato do campo id ou de validação na exclusão do funcionário.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Funcionário não encontrado para o filtro informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirFuncionario(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaFuncionario(sessao, id);

                if (validacao != null)
                {
                    return validacao;
                }

                try
                {
                    var funcionarioFluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Glass.Global.Negocios.IFuncionarioFluxo>();

                    var funcionario = funcionarioFluxo.ObtemFuncionario(id);

                    var resultado = funcionarioFluxo.ApagarFuncionario(funcionario);

                    return resultado ? this.Aceito(string.Format("Funcionário {0} excluído com sucesso!", id)) :
                        (IHttpActionResult)this.ErroValidacao($"Erro ao excluir o funcionário. {resultado.Message.Format()}");
                }
                catch (Exception e)
                {
                    sessao.Rollback();
                    return this.ErroValidacao("Erro ao excluir o funcionário.", e);
                }
            }
        }
    }
}
