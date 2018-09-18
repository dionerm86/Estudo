// <copyright file="CaixaDiarioController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Caixas.Diario.V1
{
    /// <summary>
    /// Controller de caixa diário.
    /// </summary>
    [RoutePrefix("api/v1/caixas/diario")]
    public partial class CaixaDiarioController : BaseController
    {
        private IHttpActionResult ValidarIdLoja(int idLoja)
        {
            if (idLoja <= 0)
            {
                return this.ErroValidacao("Identificador da loja deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdLoja(GDASession sessao, int idLoja)
        {
            var validacao = this.ValidarIdLoja(idLoja);

            if (validacao == null && !LojaDAO.Instance.Exists(sessao, idLoja))
            {
                return this.NaoEncontrado("Loja não encontrada.");
            }

            return null;
        }
    }
}
