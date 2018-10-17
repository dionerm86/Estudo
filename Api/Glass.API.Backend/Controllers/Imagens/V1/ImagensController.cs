// <copyright file="ImagensController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Imagens.V1.Exibicao;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Imagens.V1
{
    /// <summary>
    /// Controller para recuperação de dados de imagens.
    /// </summary>
    [RoutePrefix("api/v1/imagens")]
    public partial class ImagensController : BaseController
    {
        private IHttpActionResult ValidarIdItem(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("O identificador do item deve ser maior que 0.");
            }

            return null;
        }

        private IHttpActionResult ValidarDadosItem(int id, TipoItem? tipo)
        {
            var validacao = this.ValidarIdItem(id);

            if (validacao != null)
            {
                return validacao;
            }

            if (!tipo.HasValue)
            {
                return this.ErroValidacao("Tipo de item inválido.");
            }

            return null;
        }
    }
}
