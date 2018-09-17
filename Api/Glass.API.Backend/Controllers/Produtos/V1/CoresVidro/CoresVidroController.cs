// <copyright file="CoresVidroController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Produtos.V1.CoresVidro
{
    /// <summary>
    /// Controller de cores de vidro.
    /// </summary>
    [RoutePrefix("api/v1/produtos/cores/vidro")]
    public partial class CoresVidroController : BaseController
    {
        private IHttpActionResult ValidarIdCorVidro(int idCorVidro)
        {
            if (idCorVidro <= 0)
            {
                return this.ErroValidacao("Identificador da cor de vidro deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdCorVidro(GDASession sessao, int idCorVidro)
        {
            var validacao = this.ValidarIdCorVidro(idCorVidro);

            if (validacao == null && !CorVidroDAO.Instance.Exists(sessao, idCorVidro))
            {
                return this.NaoEncontrado("Cor de vidro não encontrada.");
            }

            return null;
        }
    }
}
