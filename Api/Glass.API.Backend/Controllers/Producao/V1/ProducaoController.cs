// <copyright file="ProducaoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Producao.V1
{
    /// <summary>
    /// Controller de produção.
    /// </summary>
    [RoutePrefix("api/v1/producao")]
    public partial class ProducaoController : BaseController
    {
        private IHttpActionResult ValidarIdProdutoProducao(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador da peça de produção deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdProdutoProducao(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdProdutoProducao(id);

            if (validacao == null && !ProdutoPedidoProducaoDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Peça de produção não encontrada.");
            }

            return null;
        }
    }
}
