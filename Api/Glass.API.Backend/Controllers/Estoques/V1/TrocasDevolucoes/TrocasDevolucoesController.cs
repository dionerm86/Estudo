// <copyright file="TrocasDevolucoesController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Estoques.V1.TrocasDevolucoes
{
    /// <summary>
    /// Controller de Troca/Devolucao.
    /// </summary>
    [RoutePrefix("api/v1/estoques/trocasDevolucoes")]
    public partial class TrocasDevolucoesController : BaseController
    {
        private IHttpActionResult ValidarIdTrocaDevolucao(int idTrocaDevolucao)
        {
            if (idTrocaDevolucao <= 0)
            {
                return this.ErroValidacao("Identificador do produto deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdTrocaDevolucao(GDASession sessao, int idTrocaDevolucao)
        {
            var validacao = this.ValidarIdTrocaDevolucao(idTrocaDevolucao);

            if (validacao == null && !TrocaDevolucaoDAO.Instance.Exists(sessao, idTrocaDevolucao))
            {
                return this.NaoEncontrado("Troca/Devolucao não encontrada.");
            }

            return null;
        }
    }
}
