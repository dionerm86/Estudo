﻿// <copyright file="DeleteCoresFerragemController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Produtos.V1.CoresFerragem
{
    /// <summary>
    /// Controller de cores de ferragem.
    /// </summary>
    public partial class CoresFerragemController : BaseController
    {
        /// <summary>
        /// Exclui uma cor de ferragem.
        /// </summary>
        /// <param name="id">O identificador da cor de ferragem que será excluída.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id}")]
        [SwaggerResponse(202, "Cor de ferragem excluída.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Cor de ferragem não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirCorFerragem(int id)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaIdCorFerragem(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    sessao.BeginTransaction();

                    CorFerragemDAO.Instance.DeleteByPrimaryKey(sessao, id);
                    sessao.Commit();

                    return this.Aceito($"Cor de ferragem excluída.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao excluir cor de ferragem.", ex);
                }
            }
        }
    }
}
