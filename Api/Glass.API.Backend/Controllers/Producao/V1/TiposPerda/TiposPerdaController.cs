// <copyright file="TiposPerdaController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Producao.V1.TiposPerda
{
    /// <summary>
    /// Controller de tipos de perda.
    /// </summary>
    [RoutePrefix("api/v1/producao/tiposPerda")]
    public partial class TiposPerdaController : BaseController
    {
        private IHttpActionResult ValidarIdTtipoPerda(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador do tipo de perda deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdTipoPerda(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdTtipoPerda(id);

            if (validacao == null && !TipoPerdaDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Tipo de perda não encontrado.");
            }

            return validacao;
        }
    }
}
