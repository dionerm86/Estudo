// <copyright file="CoresAluminioController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Produtos.V1.CoresAluminio
{
    /// <summary>
    /// Controller de cores de alumínio.
    /// </summary>
    [RoutePrefix("api/v1/produtos/cores/aluminio")]
    public partial class CoresAluminioController : BaseController
    {
        private IHttpActionResult ValidarIdCorAluminio(int idCorAluminio)
        {
            if (idCorAluminio <= 0)
            {
                return this.ErroValidacao("Identificador da cor de alumínio deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdCorAluminio(GDASession sessao, int idCorAluminio)
        {
            var validacao = this.ValidarIdCorAluminio(idCorAluminio);

            if (validacao == null && !CorAluminioDAO.Instance.Exists(sessao, idCorAluminio))
            {
                return this.NaoEncontrado("Cor de alumínio não encontrada.");
            }

            return null;
        }
    }
}
