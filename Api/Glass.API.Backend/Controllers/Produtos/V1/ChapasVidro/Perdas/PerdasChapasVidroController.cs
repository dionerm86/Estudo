// <copyright file="PerdasChapasVidroController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Produtos.V1.ChapasVidro.Perdas
{
    /// <summary>
    /// Controller de perdas de chapas de vidro.
    /// </summary>
    [RoutePrefix("api/v1/produtos/chapasVidro/perdas")]
    public partial class PerdasChapasVidroController : BaseController
    {
        private IHttpActionResult ValidarIdPerdaChapaVidro(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador da perda de chapa de vidro deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdPerdaChapaVidro(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdPerdaChapaVidro(id);

            if (validacao == null && !PerdaChapaVidroDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Perda de chapa de vidro não encontrada.");
            }

            return validacao;
        }
    }
}