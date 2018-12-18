// <copyright file="PedidosOrdemCargaController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Pedidos.V1.CadastroAtualizacao;
using Glass.Configuracoes;
using Glass.Data.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Carregamentos.V1.OrdensCarga.Pedidos
{
    /// <summary>
    /// Controller de pedidos associados a uma ordem de carga.
    /// </summary>
    [RoutePrefix("api/v1/carregamentos/ordensCarga/{id:int}/pedidos")]
    public partial class PedidosOrdemCargaController : BaseController
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

            return null;
        }
    }
}