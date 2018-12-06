// <copyright file="MovimentacaoEstoqueRealController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Estoques.V1.Movimentacoes.Reais.CadastroAtualizacao;
using Glass.Data.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Estoques.V1.Movimentacoes.Reais
{
    /// <summary>
    /// Controller de movimentação de estoque real.
    /// </summary>
    [RoutePrefix("api/v1/estoques/movimentacoes/reais")]
    public partial class MovimentacaoEstoqueRealController : BaseController
    {
        private IHttpActionResult ValidarIdMovimentacaoEstoqueReal(int idMovimentacaoEstoqueReal)
        {
            if (idMovimentacaoEstoqueReal <= 0)
            {
                return this.ErroValidacao("Identificador da movimentação deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdMovimentacaoEstoqueReal(GDASession sessao, int idMovimentacaoEstoqueReal)
        {
            var validacao = this.ValidarIdMovimentacaoEstoqueReal(idMovimentacaoEstoqueReal);

            if (validacao == null && !MovEstoqueDAO.Instance.Exists(sessao, idMovimentacaoEstoqueReal))
            {
                return this.NaoEncontrado("Movimentação não encontrada.");
            }

            return null;
        }

        private IHttpActionResult ValidarCadastroMovimentacaoEstoqueReal(GDASession sessao, CadastroAtualizacaoDto dados)
        {
            var erros = new List<Lazy<IHttpActionResult>>();

            erros.Add(new Lazy<IHttpActionResult>(() =>
                this.ValidarCadastroAtualizacaoMovimentacaoEstoqueReal(dados, "cadastro")));

            return erros.FirstOrDefault(e => e.Value != null)?.Value;
        }

        private IHttpActionResult ValidarAtualizacaoMovimentacaoEstoqueReal(GDASession sessao, int id, CadastroAtualizacaoDto dados)
        {
            var erros = new List<Lazy<IHttpActionResult>>();

            erros.Add(new Lazy<IHttpActionResult>(() =>
                this.ValidarIdMovimentacaoEstoqueReal(id)));

            erros.Add(new Lazy<IHttpActionResult>(() =>
                this.ValidarCadastroAtualizacaoMovimentacaoEstoqueReal(dados, "atualização")));

            return erros.FirstOrDefault(e => e.Value != null)?.Value;
        }

        private IHttpActionResult ValidarCadastroAtualizacaoMovimentacaoEstoqueReal(CadastroAtualizacaoDto dados, string tipo)
        {
            if (dados == null)
            {
                return this.ErroValidacao($"É preciso informar os dados para {tipo} da movimentação de estoque real.");
            }

            return null;
        }
    }
}
