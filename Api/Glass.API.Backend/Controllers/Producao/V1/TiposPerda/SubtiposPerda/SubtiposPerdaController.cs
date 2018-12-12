// <copyright file="SubtiposPerdaController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Producao.V1.TiposPerda.SubtiposPerda
{
    /// <summary>
    /// Controller de subtipos de perda.
    /// </summary>
    [RoutePrefix("api/v1/producao/tiposPerda/{idTipoPerda}/subtiposPerda")]
    public partial class SubtiposPerdaController : BaseController
    {
        private IHttpActionResult ValidarIdTipoPerda(int idTipoPerda)
        {
            if (idTipoPerda <= 0)
            {
                return this.ErroValidacao("Identificador do tipo de perda deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdTipoPerda(GDASession sessao, int idTipoPerda)
        {
            var validacao = this.ValidarIdTipoPerda(idTipoPerda);

            if (validacao == null && !TipoPerdaDAO.Instance.Exists(sessao, idTipoPerda))
            {
                return this.NaoEncontrado("Tipo de perda não encontrado.");
            }

            return validacao;
        }

        private IHttpActionResult ValidarIdSubtipoPerda(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador do subtipo de perda deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdSubtipoPerda(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdSubtipoPerda(id);

            if (validacao == null && !SubtipoPerdaDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Subtipo de perda não encontrado.");
            }

            return validacao;
        }
    }
}
