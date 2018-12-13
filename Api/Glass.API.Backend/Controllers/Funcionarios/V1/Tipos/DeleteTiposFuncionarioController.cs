// <copyright file="DeleteTiposFuncionarioController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
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
        /// Exclui um tipo de funcionário.
        /// </summary>
        /// <param name="id">O identificador do tipo de funcionário que será excluído.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Tipo de funcionário excluído.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Tipo de funcionário não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirTipoFuncionario(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdTipoFuncionario(sessao, id);

                if (validacao != null)
                {
                    return validacao;
                }

                try
                {
                    sessao.BeginTransaction();

                    Microsoft.Practices.ServiceLocation.ServiceLocator.Current
                    .GetInstance<Glass.Global.Negocios.IFuncionarioFluxo>()
                    .ApagarTipoFuncionario(id);

                    sessao.Commit();

                    return this.Aceito(string.Format("Tipo de funcionário {0} excluído com sucesso!", id));
                }
                catch (Exception e)
                {
                    sessao.Rollback();
                    return this.ErroValidacao("Erro ao excluir o tipo de funcionário.", e);
                }
            }
        }
    }
}