// <copyright file="CoresFerragemController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Produtos.V1.CoresFerragem
{
    /// <summary>
    /// Controller de cores de ferragem.
    /// </summary>
    [RoutePrefix("api/v1/produtos/cores/ferragem")]
    public partial class CoresFerragemController : BaseController
    {
        private IHttpActionResult ValidarIdCorFerragem(int idCorFerragem)
        {
            if (idCorFerragem <= 0)
            {
                return this.ErroValidacao("Identificador da cor de ferragem deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdCorFerragem(GDASession sessao, int idCorFerragem)
        {
            var validacao = this.ValidarIdCorFerragem(idCorFerragem);

            if (validacao == null && !CorFerragemDAO.Instance.Exists(sessao, idCorFerragem))
            {
                return this.NaoEncontrado("Cor de ferragem não encontrada.");
            }

            return null;
        }
    }
}
