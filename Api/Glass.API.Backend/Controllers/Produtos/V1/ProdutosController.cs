// <copyright file="ProdutosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Produtos.V1
{
    /// <summary>
    /// Controller de produtos.
    /// </summary>
    [RoutePrefix("api/v1/produtos")]
    public partial class ProdutosController : BaseController
    {
        private IHttpActionResult ValidarIdProduto(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador do produto deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdProduto(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdProduto(id);

            if (validacao == null && !ProdutoDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Produto não encontrado.");
            }

            return validacao;
        }
    }
}
