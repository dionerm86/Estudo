// <copyright file="TabelasDescontoAcrescimoClienteController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.TabelasDescontoAcrescimoCliente.V1
{
    /// <summary>
    /// Controller de tabelas de desconto/acréscimo de cliente.
    /// </summary>
    [RoutePrefix("api/v1/tabelasDescontoAcrescimoCliente")]
    public partial class TabelasDescontoAcrescimoClienteController : BaseController
    {
        private IHttpActionResult ValidarIdTabelaDescontoAcrescimoCliente(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador da tabela de desconto/acréscimo de cliente deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdTabelaDescontoAcrescimoCliente(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdTabelaDescontoAcrescimoCliente(id);

            if (validacao == null && !TabelaDescontoAcrescimoClienteDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Tabela de desconto/acréscimo de cliente não encontrada.");
            }

            return null;
        }
    }
}
