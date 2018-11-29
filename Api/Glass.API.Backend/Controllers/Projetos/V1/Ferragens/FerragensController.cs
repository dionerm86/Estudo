// <copyright file="FerragensController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Projetos.V1.Ferragens
{
    /// <summary>
    /// Controller de ferragens.
    /// </summary>
    [RoutePrefix("api/v1/projetos/ferragens")]
    public partial class FerragensController : BaseController
    {
        private IHttpActionResult ValidarIdFerragem(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador da ferragem deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdFerragem(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdFerragem(id);

            var ferragemExiste = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<Projeto.Negocios.IFerragemFluxo>()
                .ObterFerragem(id) != null;

            if (validacao == null && !ferragemExiste)
            {
                return this.NaoEncontrado("Ferragem não encontrada.");
            }

            return validacao;
        }
    }
}
