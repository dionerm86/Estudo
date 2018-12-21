// <copyright file="TiposFuncionarioController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Funcionarios.V1.CadastroAtualizacao;
using Glass.Data.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Funcionarios.V1.Tipos
{
    /// <summary>
    /// Controller de tipos de funcionário.
    /// </summary>
    [RoutePrefix("api/v1/funcionarios/tipos")]
    public partial class TiposFuncionarioController : BaseController
    {
        private IHttpActionResult ValidarIdTipoFuncionario(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador do tipo de funcionário deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdTipoFuncionario(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdTipoFuncionario(id);

            if (validacao == null && !TipoFuncDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Tipo de funcionário não encontrado.");
            }

            return validacao;
        }
    }
}