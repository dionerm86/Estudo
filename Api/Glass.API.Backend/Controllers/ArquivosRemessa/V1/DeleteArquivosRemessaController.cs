// <copyright file="DeleteArquivosRemessaController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.ArquivosRemessa.V1
{
    /// <summary>
    /// Controller de arquivos de remessa.
    /// </summary>
    public partial class ArquivosRemessaController : BaseController
    {
        /// <summary>
        /// Exclui um arquivo de remessa.
        /// </summary>
        /// <param name="id">O identificador do arquivo de remessa que será excluído.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Arquivo de remessa excluído.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Arquivo de remessa não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirArquivoRemessa(int id)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaIdArquivoRemessa(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var arquivoRemessa = ArquivoRemessaDAO.Instance.GetElementByPrimaryKey(sessao, id);

                    ArquivoRemessaDAO.Instance.Delete(arquivoRemessa);

                    sessao.Commit();
                    return this.Aceito($"Arquivo de remessa excluído.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao excluir arquivo de remessa.", ex);
                }
            }
        }
    }
}