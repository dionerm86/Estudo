// <copyright file="PatchProdutosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Produtos.V1
{
    /// <summary>
    /// Controller de produtos.
    /// </summary>
    public partial class ProdutosController : BaseController
    {
        /// <summary>
        /// Ativa/inativa produtos pelo grupo de produto.
        /// </summary>
        /// <param name="id">O identificador do grupo de produto que terá a situação dos produtos alterada.</param>
        /// <param name="situacao">Situação nova que terão os produtos do grupo.</param>
        /// <returns>Um status HTTP indicando se a situação dos produtos foi alterada.</returns>
        [HttpPatch]
        [Route("alterarSituacao/grupo/{id}")]
        [SwaggerResponse(202, "Situação dos produtos do grupo de produto alterada.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Grupo de produto não encontrado para o `id` informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AlterarSituacao(int id, Situacao situacao)
        {
            using (var sessao = new GDATransaction())
            {
                if (!GrupoProdDAO.Instance.Exists(id))
                {
                    return this.NaoEncontrado("Grupo de produto não encontrado");
                }

                try
                {
                    sessao.BeginTransaction();

                    ProdutoDAO.Instance.AlterarSituacaoProduto(situacao, id, null);

                    sessao.Commit();

                    return this.Aceito("Situação dos produtos alterada com sucesso.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao(ex.Message, ex);
                }
            }
        }
    }
}
