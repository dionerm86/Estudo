// <copyright file="CategoriasContaController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.PlanosConta.V1.CategoriasConta
{
    /// <summary>
    /// Controller de categorias de conta.
    /// </summary>
    [RoutePrefix("api/v1/planosConta/categorias")]
    public partial class CategoriasContaController : BaseController
    {
        private IHttpActionResult ValidarIdCategoriaConta(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador da categoria de conta deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdCategoriaConta(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdCategoriaConta(id);

            if (validacao == null && !CategoriaContaDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Categoria de conta não encontrada.");
            }

            return validacao;
        }
    }
}
