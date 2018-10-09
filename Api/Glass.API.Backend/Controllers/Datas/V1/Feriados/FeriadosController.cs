// <copyright file="FeriadosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Datas.V1.Feriados
{
    /// <summary>
    /// Controller de feriados.
    /// </summary>
    [RoutePrefix("api/v1/datas/feriados")]
    public partial class FeriadosController : BaseController
    {
        private IHttpActionResult ValidarIdFeriado(int idFeriado)
        {
            if (idFeriado <= 0)
            {
                return this.ErroValidacao("Identificador do feriado deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdFeriado(GDASession sessao, int idFeriado)
        {
            var validacao = this.ValidarIdFeriado(idFeriado);

            if (validacao == null && !FeriadoDAO.Instance.Exists(sessao, idFeriado))
            {
                return this.NaoEncontrado("Feriado não encontrado.");
            }

            return null;
        }
    }
}
