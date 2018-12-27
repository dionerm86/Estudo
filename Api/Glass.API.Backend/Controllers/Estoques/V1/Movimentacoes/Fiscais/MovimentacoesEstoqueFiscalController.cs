// <copyright file="MovimentacoesEstoqueFiscalController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Estoques.V1.Movimentacoes.Fiscais.CadastroAtualizacao;
using Glass.Data.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Estoques.V1.Movimentacoes.Fiscais
{
    /// <summary>
    /// Controller de movimentação de estoque fiscal.
    /// </summary>
    [RoutePrefix("api/v1/estoques/movimentacoes/fiscais")]
    public partial class MovimentacoesEstoqueFiscalController : BaseController
    {
        private IHttpActionResult ValidarIdMovimentacaoEstoqueFiscal(int idMovimentacaoEstoqueFiscal)
        {
            if (idMovimentacaoEstoqueFiscal <= 0)
            {
                return this.ErroValidacao("Identificador da movimentação deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdMovimentacaoEstoqueFiscal(GDASession sessao, int idMovimentacaoEstoqueFiscal)
        {
            var validacao = this.ValidarIdMovimentacaoEstoqueFiscal(idMovimentacaoEstoqueFiscal);

            if (validacao == null && !MovEstoqueFiscalDAO.Instance.Exists(sessao, idMovimentacaoEstoqueFiscal))
            {
                return this.NaoEncontrado("Movimentação não encontrada.");
            }

            return validacao;
        }

        private IHttpActionResult ValidarCadastroMovimentacaoEstoqueFiscal(GDASession sessao, CadastroAtualizacaoDto dados)
        {
            var erros = new List<Lazy<IHttpActionResult>>();

            erros.Add(new Lazy<IHttpActionResult>(() =>
                this.ValidarCadastroAtualizacaoMovimentacaoEstoqueFiscal(dados, "cadastro")));

            return erros.FirstOrDefault(e => e.Value != null)?.Value;
        }

        private IHttpActionResult ValidarAtualizacaoMovimentacaoEstoqueFiscal(GDASession sessao, int id, CadastroAtualizacaoDto dados)
        {
            var erros = new List<Lazy<IHttpActionResult>>();

            erros.Add(new Lazy<IHttpActionResult>(() =>
                this.ValidarIdMovimentacaoEstoqueFiscal(id)));

            erros.Add(new Lazy<IHttpActionResult>(() =>
                this.ValidarCadastroAtualizacaoMovimentacaoEstoqueFiscal(dados, "atualização")));

            return erros.FirstOrDefault(e => e.Value != null)?.Value;
        }

        private IHttpActionResult ValidarCadastroAtualizacaoMovimentacaoEstoqueFiscal(CadastroAtualizacaoDto dados, string tipo)
        {
            if (dados == null)
            {
                return this.ErroValidacao($"É preciso informar os dados para {tipo} da movimentação de estoque.");
            }

            return null;
        }
    }
}
