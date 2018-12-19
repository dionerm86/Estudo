// <copyright file="OrdensCargaController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Carregamentos.V1.OrdensCarga.Lista
{
    /// <summary>
    /// Controller de ordem de carga.
    /// </summary>
    [RoutePrefix("api/v1/carregamentos/ordensCarga")]
    public partial class OrdensCargaController : BaseController
    {
        private IHttpActionResult ValidarIdOrdemCarga(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador da ordem de carga deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdOrdemCarga(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdOrdemCarga(id);

            if (validacao == null && !OrdemCargaDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Ordem de carga não encontrada.");
            }

            return validacao;
        }

        private IHttpActionResult ValidarIdPedido(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador do pedido deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdPedido(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdPedido(id);

            if (validacao == null && !PedidoDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Pedido não encontrado.");
            }

            return validacao;
        }

        private IHttpActionResult ValidarDesassociacaoPedidoOrdemCarga(GDASession sessao, int id, int idPedido)
        {
            var erros = new List<Lazy<IHttpActionResult>>();

            erros.Add(new Lazy<IHttpActionResult>(() =>
                this.ValidarExistenciaIdOrdemCarga(sessao, id)));

            erros.Add(new Lazy<IHttpActionResult>(() =>
                this.ValidarExistenciaIdPedido(sessao, idPedido)));

            return erros.FirstOrDefault(e => e.Value != null)?.Value;
        }
    }
}