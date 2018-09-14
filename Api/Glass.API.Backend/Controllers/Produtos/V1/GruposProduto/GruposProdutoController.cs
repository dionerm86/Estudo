// <copyright file="GruposProdutoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Produtos.V1.GruposProduto
{
    /// <summary>
    /// Controller de grupos de produto.
    /// </summary>
    [RoutePrefix("api/v1/produtos/grupos")]
    public partial class GruposProdutoController : BaseController
    {
        private IHttpActionResult ValidarIdGrupoProduto(int idGrupoProduto)
        {
            if (idGrupoProduto <= 0)
            {
                return this.ErroValidacao("Identificador do grupo de produto deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdGrupoProduto(GDASession sessao, int idGrupoProduto)
        {
            var validacao = this.ValidarIdGrupoProduto(idGrupoProduto);

            if (validacao == null && !GrupoProdDAO.Instance.Exists(sessao, idGrupoProduto))
            {
                return this.NaoEncontrado("Grupo de produto não encontrado.");
            }

            return null;
        }
    }
}
