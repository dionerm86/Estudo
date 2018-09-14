// <copyright file="EstoquesController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Estoques.V1
{
    /// <summary>
    /// Controller de estoques.
    /// </summary>
    [RoutePrefix("api/v1/estoques")]
    public partial class EstoquesController : BaseController
    {
        private IHttpActionResult ValidarIdProdutoIdLoja(int idProduto, int idLoja)
        {
            if (idProduto <= 0)
            {
                return this.ErroValidacao("Identificador do produto deve ser um número maior que zero.");
            }

            if (idLoja <= 0)
            {
                return this.ErroValidacao("Identificador da loja deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdProdutoIdLoja(GDASession sessao, int idProduto, int idLoja)
        {
            var validacao = this.ValidarIdProdutoIdLoja(idProduto, idLoja);

            if (validacao == null && !ProdutoDAO.Instance.Exists(sessao, idProduto))
            {
                return this.NaoEncontrado("Produto não encontrado.");
            }

            if (validacao == null && !LojaDAO.Instance.Exists(sessao, idLoja))
            {
                return this.NaoEncontrado("Loja não encontrada.");
            }

            return null;
        }
    }
}
