// <copyright file="AplicacoesController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Aplicacoes.V1.CadastroAtualizacao;
using Glass.Data.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Aplicacoes.V1
{
    /// <summary>
    /// Controller de aplicações (etiqueta).
    /// </summary>
    [RoutePrefix("api/v1/aplicacoes")]
    public partial class AplicacoesController : BaseController
    {
        private IHttpActionResult ValidarIdAplicacao(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador da aplicação de etiqueta deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdAplicacao(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdAplicacao(id);

            if (validacao == null && !EtiquetaAplicacaoDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Aplicação de etiqueta não encontrada.");
            }

            return null;
        }

        private IHttpActionResult ValidarCadastroAplicacao(GDASession sessao, CadastroAtualizacaoDto dados)
        {
            var erros = new List<Lazy<IHttpActionResult>>();

            erros.Add(new Lazy<IHttpActionResult>(() =>
                this.ValidarCadastroAtualizacaoAplicacao(dados, "cadastro")));

            return erros.FirstOrDefault(e => e.Value != null)?.Value;
        }

        private IHttpActionResult ValidarAtualizacaoAplicacao(GDASession sessao, int id, CadastroAtualizacaoDto dados)
        {
            var erros = new List<Lazy<IHttpActionResult>>();

            erros.Add(new Lazy<IHttpActionResult>(() =>
                this.ValidarIdAplicacao(id)));

            erros.Add(new Lazy<IHttpActionResult>(() =>
                this.ValidarCadastroAtualizacaoAplicacao(dados, "atualização")));

            return erros.FirstOrDefault(e => e.Value != null)?.Value;
        }

        private IHttpActionResult ValidarCadastroAtualizacaoAplicacao(CadastroAtualizacaoDto dados, string tipo)
        {
            if (dados == null)
            {
                return this.ErroValidacao($"É preciso informar os dados para {tipo} da aplicação de etiqueta.");
            }

            return null;
        }
    }
}
