// <copyright file="DeleteFabricantesFerragemController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Projetos.V1.Ferragens.FabricantesFerragem
{
    /// <summary>
    /// Controller de fabricantes de ferragens.
    /// </summary>
    public partial class FabricantesFerragemController : BaseController
    {
        /// <summary>
        /// Exclui um fabricante de ferragem.
        /// </summary>
        /// <param name="id">O identificador do fabricante de ferragem que será excluído.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Fabricante de ferragem excluído.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Fabricante de ferragem não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirFabricanteFerragem(int id)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaIdFabricanteFerragem(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Projeto.Negocios.IFerragemFluxo>();

                    var fabricante = fluxo.ObterFabricanteFerragem(id);

                    var resultado = fluxo.ApagarFabricanteFerragem(fabricante);

                    if (!resultado)
                    {
                        return this.ErroValidacao($"Falha ao excluir fabricante de ferragem. {resultado.Message.Format()}");
                    }

                    return this.Aceito($"Fabricante de ferragem excluído.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao excluir fabricante de ferragem.", ex);
                }
            }
        }
    }
}
