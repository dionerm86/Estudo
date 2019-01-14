// <copyright file="DeletePerdasChapasVidroController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Produtos.V1.ChapasVidro.Perdas
{
    /// <summary>
    /// Controller de perdas de chapas de vidro.
    /// </summary>
    public partial class PerdasChapasVidroController : BaseController
    {
        /// <summary>
        /// Cancela uma perda de chapa de vidro.
        /// </summary>
        /// <param name="id">O identificador da perda de chapa de vidro que será cancelada.</param>
        /// <returns>Um status HTTP indicando se a perda de chapa de vidro foi cancelada.</returns>
        [HttpDelete]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Perda de chapa de vidro cancelada.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Perda de chapa de vidro não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult CancelarPerdaChapaVidro(int id)
        {
            using (var sessao = new GDATransaction())
            {
                sessao.BeginTransaction();

                var validacao = this.ValidarExistenciaIdPerdaChapaVidro(sessao, id);

                if (validacao != null)
                {
                    return validacao;
                }

                try
                {
                    var perdaChapaVidro = PerdaChapaVidroDAO.Instance.GetElementByPrimaryKey(sessao, id);

                    PerdaChapaVidroDAO.Instance.Cancelar(sessao, perdaChapaVidro);

                    sessao.Commit();
                    sessao.Close();

                    return this.Aceito("Perda de chapa de vidro cancelada com sucesso!");
                }
                catch (Exception e)
                {
                    sessao.Rollback();
                    sessao.Close();
                    return this.ErroValidacao("Erro ao cancelar a perda de chapa de vidro.", e);
                }
            }
        }
    }
}