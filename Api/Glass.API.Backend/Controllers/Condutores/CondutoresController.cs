// <copyright file="CondutoresController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Condutores.CadastroAtualizacao;
using Glass.Data.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Condutores.V1
{
    /// <summary>
    /// Controller de Condutores.
    /// </summary>
    [RoutePrefix("api/v1/Condutores")]
    public partial class CondutoresController : BaseController
    {
        private IHttpActionResult ValidarIdCondutor(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador do condutor deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdCondutor(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdCondutor(id);

            if (validacao == null && !CondutoresDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Condutor não encontrado.");
            }

            return null;
        }

        private IHttpActionResult ValidarCadastroCondutor(GDASession sessao, CadastroAtualizacaoDto dados)
        {
            var erros = new List<Lazy<IHttpActionResult>>();

            erros.Add(new Lazy<IHttpActionResult>(() =>
                this.ValidarCadastroAtualizacaoCondutor(dados, "cadastro")));

            return erros.FirstOrDefault(e => e.Value != null)?.Value;
        }

        private IHttpActionResult ValidarAtualizacaoCondutor(GDASession sessao, int id, CadastroAtualizacaoDto dados)
        {
            var erros = new List<Lazy<IHttpActionResult>>();

            erros.Add(new Lazy<IHttpActionResult>(() =>
                this.ValidarIdCondutor(id)));

            erros.Add(new Lazy<IHttpActionResult>(() =>
                this.ValidarCadastroAtualizacaoCondutor(dados, "atualização")));

            return erros.FirstOrDefault(e => e.Value != null)?.Value;
        }

        private IHttpActionResult ValidarCadastroAtualizacaoCondutor(CadastroAtualizacaoDto dados, string tipo)
        {
            if (dados == null)
            {
                return this.ErroValidacao($"É preciso informar os dados para {tipo} do condutor.");
            }

            return null;
        }
    }
}
