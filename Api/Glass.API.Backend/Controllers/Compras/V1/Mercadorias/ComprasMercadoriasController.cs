// <copyright file="ComprasMercadoriasController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Compras.V1.Mercadorias
{
    /// <summary>
    /// Controller de compras de marcadorias.
    /// </summary>
    [RoutePrefix("api/v1/compras/mercadorias")]
    public partial class ComprasMercadoriasController : BaseController
    {
        private IHttpActionResult ValidarIdCompra(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador da compra deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdCompra(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdCompra(id);

            if (validacao == null && !CompraDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Compra não encontrada.");
            }

            return validacao;
        }

        private IHttpActionResult ValidarIdNotaFiscal(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador da nota fiscal deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdNotaFiscal (GDASession sessao, int id)
        {
            var validacao = this.ValidarIdNotaFiscal(id);

            if (validacao == null && !NotaFiscalDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Nota fiscal não encontrada.");
            }

            return null;
        }
    }
}
