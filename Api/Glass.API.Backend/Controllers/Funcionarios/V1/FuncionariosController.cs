// <copyright file="FuncionariosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Funcionarios.V1.CadastroAtualizacao;
using Glass.Data.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Funcionarios.V1
{
    /// <summary>
    /// Controller de funcionários.
    /// </summary>
    [RoutePrefix("api/v1/funcionarios")]
    public partial class FuncionariosController : BaseController
    {
        private IHttpActionResult ValidarIdFuncionario(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador do funcionário deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaFuncionario(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdFuncionario(id);

            if (validacao == null && !FuncionarioDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Funcionário não encontrado.");
            }

            return null;
        }

        private IHttpActionResult ValidarCadastroFuncionario(GDASession sessao, CadastroAtualizacaoDto dados)
        {
            var erros = new List<Lazy<IHttpActionResult>>();

            erros.Add(new Lazy<IHttpActionResult>(() =>
                this.ValidarCadastroAtualizacaoFuncionario(dados, "cadastro")));

            return erros.FirstOrDefault(e => e.Value != null)?.Value;
        }

        private IHttpActionResult ValidarAtualizacaoFuncionario(GDASession sessao, int id, CadastroAtualizacaoDto dados)
        {
            var erros = new List<Lazy<IHttpActionResult>>();

            erros.Add(new Lazy<IHttpActionResult>(() =>
                this.ValidarIdFuncionario(id)));

            erros.Add(new Lazy<IHttpActionResult>(() =>
                this.ValidarCadastroAtualizacaoFuncionario(dados, "atualização")));

            return erros.FirstOrDefault(e => e.Value != null)?.Value;
        }

        private IHttpActionResult ValidarCadastroAtualizacaoFuncionario(CadastroAtualizacaoDto dados, string tipo)
        {
            if (dados == null)
            {
                return this.ErroValidacao(string.Format("É preciso informar os dados para {0} do funcionário.", tipo));
            }

            return null;
        }
    }
}
