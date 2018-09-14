// <copyright file="ProcessosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Processos.CadastroAtualizacao;
using Glass.Data.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Processos.V1
{
    /// <summary>
    /// Controller de processos (etiqueta).
    /// </summary>
    [RoutePrefix("api/v1/processos")]
    public partial class ProcessosController : BaseController
    {
        private IHttpActionResult ValidarIdProcesso(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador do processo de etiqueta deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdProcesso(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdProcesso(id);

            if (validacao == null && !EtiquetaProcessoDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Processo de etiqueta não encontrado.");
            }

            return null;
        }

        private IHttpActionResult ValidarCadastroProcesso(GDASession sessao, CadastroAtualizacaoDto dados)
        {
            var erros = new List<Lazy<IHttpActionResult>>();

            erros.Add(new Lazy<IHttpActionResult>(() =>
                this.ValidarCadastroAtualizacaoProcesso(dados, "cadastro")));

            return erros.FirstOrDefault(e => e.Value != null)?.Value;
        }

        private IHttpActionResult ValidarAtualizacaoProcesso(GDASession sessao, int id, CadastroAtualizacaoDto dados)
        {
            var erros = new List<Lazy<IHttpActionResult>>();

            erros.Add(new Lazy<IHttpActionResult>(() =>
                this.ValidarIdProcesso(id)));

            erros.Add(new Lazy<IHttpActionResult>(() =>
                this.ValidarCadastroAtualizacaoProcesso(dados, "atualização")));

            return erros.FirstOrDefault(e => e.Value != null)?.Value;
        }

        private IHttpActionResult ValidarCadastroAtualizacaoProcesso(CadastroAtualizacaoDto dados, string tipo)
        {
            if (dados == null)
            {
                return this.ErroValidacao($"É preciso informar os dados para {tipo} do processo de etiqueta.");
            }

            return null;
        }
    }
}
