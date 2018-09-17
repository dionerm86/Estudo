// <copyright file="SubgruposProdutoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Produtos.V1.SubgruposProduto
{
    /// <summary>
    /// Controller de subgrupos de produto.
    /// </summary>
    [RoutePrefix("api/v1/produtos/subgrupos")]
    public partial class SubgruposProdutoController : BaseController
    {
        private IHttpActionResult ValidarIdSubgrupoProduto(int idSubgrupoProduto)
        {
            if (idSubgrupoProduto <= 0)
            {
                return this.ErroValidacao("Identificador do subgrupo de produto deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdSubgrupoProduto(GDASession sessao, int idSubgrupoProduto)
        {
            var validacao = this.ValidarIdSubgrupoProduto(idSubgrupoProduto);

            if (validacao == null && !SubgrupoProdDAO.Instance.Exists(sessao, idSubgrupoProduto))
            {
                return this.NaoEncontrado("Subgrupo de produto não encontrado.");
            }

            return null;
        }
    }
}
