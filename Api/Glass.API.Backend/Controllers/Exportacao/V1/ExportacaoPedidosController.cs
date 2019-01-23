// <copyright file="ExportacaoPedidosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Exportacao.V1
{
    /// <summary>
    /// Controller de exportação de pedidos.
    /// </summary>
    [RoutePrefix("api/v1/exportacao")]
    public partial class ExportacaoPedidosController : BaseController
    {
        private IHttpActionResult ValidarIdExportacao(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador da exportação deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdExportacao(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdExportacao(id);

            if (validacao == null && !ExportacaoDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Exportação não encontrada.");
            }

            return validacao;
        }
    }
}
