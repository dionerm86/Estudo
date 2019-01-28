// <copyright file="ComprasMercadoriasController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Compras.V1.Mercadorias
{
    /// <summary>
    /// Controller de compras de mercadorias.
    /// </summary>
    [RoutePrefix("api/v1/compras/mercadorias")]
    public partial class ComprasMercadoriasController : BaseController
    {
        private IHttpActionResult ValidarIdCompraMercadorias(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador da compra deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdCompraMercadorias(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdCompraMercadorias(id);

            if (validacao == null && !CompraDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Compra não encontrada.");
            }

            return validacao;
        }
    }
}
