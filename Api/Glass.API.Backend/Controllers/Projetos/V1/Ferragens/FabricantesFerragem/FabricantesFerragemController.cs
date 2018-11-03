// <copyright file="FabricantesFerragemController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Projetos.V1.Ferragens.FabricantesFerragens
{
    /// <summary>
    /// Controller de fabricantes de ferragens.
    /// </summary>
    [RoutePrefix("api/v1/projetos/ferragens/fabricantes")]
    public partial class FabricantesFerragemController : BaseController
    {
        private IHttpActionResult ValidarIdFabricanteFerragem(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador do fabricante de ferragem deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdFabricanteFerragem(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdFabricanteFerragem(id);

            var fabricanteFerragemExiste = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<Projeto.Negocios.IFerragemFluxo>()
                .ObterFabricanteFerragem(id) != null;

            if (validacao == null && !fabricanteFerragemExiste)
            {
                return this.NaoEncontrado("Fabricante de ferragem não encontrado.");
            }

            return validacao;
        }
    }
}
