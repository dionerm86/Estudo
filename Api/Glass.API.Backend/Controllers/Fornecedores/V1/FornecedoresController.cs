// <copyright file="FornecedoresController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Fornecedores.V1
{
    /// <summary>
    /// Controller de fornecedores.
    /// </summary>
    [RoutePrefix("api/v1/fornecedores")]
    public partial class FornecedoresController : BaseController
    {
        private IHttpActionResult ValidarIdFornecedor(int idFornecedor)
        {
            if (idFornecedor <= 0)
            {
                return this.ErroValidacao("Identificador do fornecedor deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdFornecedor(GDASession sessao, int idFornecedor)
        {
            var validacao = this.ValidarIdFornecedor(idFornecedor);

            if (validacao == null && !FornecedorDAO.Instance.Exists(sessao, idFornecedor))
            {
                return this.NaoEncontrado("Fornecedor não encontrado.");
            }

            return null;
        }
    }
}
