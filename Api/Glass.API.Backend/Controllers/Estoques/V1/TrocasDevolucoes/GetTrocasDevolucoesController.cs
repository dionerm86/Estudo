// <copyright file="GetTrocasDevolucoesController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Estoques.V1.TrocasDevolucoes
{
    public partial class TrocasDevolucoesController : BaseController
    {
        /// <summary>
        /// Recupera a lista de estoques.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos estoques.</param>
        /// <returns>Uma lista JSON com os dados dos estoques.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Trocas/devoluções encontradas sem paginação (apenas uma página de retorno) ou última página retornada", Type = typeof(IEnumerable<Models.Estoques.V1.TrocasDevolucoes.Lista.ListaDto>))]
        [SwaggerResponse(204, "Trocas/devoluções não encontradas.")]
        [SwaggerResponse(206, "Trocas/devoluções paginadas (qualquer página, exceto a última).", Type = typeof(IEnumerable<Models.Estoques.V1.TrocasDevolucoes.Lista.ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaTrocasDevolucoes([FromUri] Models.Estoques.V1.TrocasDevolucoes.Lista.FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new Models.Estoques.V1.TrocasDevolucoes.Lista.FiltroDto();

                var idsFuncionario = filtro.IdsFuncionario != null ? string.Join(",", filtro.IdsFuncionario) : string.Empty;
                var idsFuncionarioAssociadoCliente = filtro.IdsFuncionario != null ? string.Join(",", filtro.IdsFuncionarioAssociadoCliente) : string.Empty;

                var trocasDevolucoes = TrocaDevolucaoDAO.Instance.GetList(
                    (uint)filtro.Id.GetValueOrDefault(),
                    (uint)filtro.IdPedido.GetValueOrDefault(),
                    filtro.Tipo.GetValueOrDefault(),
                    /*filtro.Situacao*/ 0,
                    (uint)filtro.Idcliente.GetValueOrDefault(),
                    filtro.NomeCliente ?? string.Empty,
                    idsFuncionario,
                    idsFuncionarioAssociadoCliente,
                    filtro.DataInicio != null ? filtro.DataInicio.ToString() : string.Empty,
                    filtro.DataFim != null ? filtro.DataInicio.ToString() : string.Empty,
                    (uint)filtro.IdProduto.GetValueOrDefault(),
                    filtro.AlturaMinima.GetValueOrDefault(),
                    filtro.AlturaMaxima.GetValueOrDefault(),
                    filtro.LarguraMinima.GetValueOrDefault(),
                    filtro.LarguraMaxima.GetValueOrDefault(),
                    (uint)filtro.IdOrigemTrocaDevolucao.GetValueOrDefault(),
                    (uint)filtro.IdTipoPerda.GetValueOrDefault(),
                    filtro.IdSetor.GetValueOrDefault(),
                    filtro.TipoPedido ?? string.Empty,
                    filtro.IdGrupoProduto,
                    filtro.IdSubgrupoProduto.GetValueOrDefault(),
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros);

                return this.ListaPaginada(
                    trocasDevolucoes.Select(o => new Models.Estoques.V1.TrocasDevolucoes.Lista.ListaDto(o)),
                    filtro,
                    () => TrocaDevolucaoDAO.Instance.GetCount(
                        (uint)filtro.Id.GetValueOrDefault(),
                        (uint)filtro.IdPedido.GetValueOrDefault(),
                        filtro.Tipo.GetValueOrDefault(),
                        /*filtro.Situacao*/ 0,
                        (uint)filtro.Idcliente.GetValueOrDefault(),
                        filtro.NomeCliente ?? string.Empty,
                        idsFuncionario,
                        idsFuncionarioAssociadoCliente,
                        filtro.DataInicio != null ? filtro.DataInicio.ToString() : string.Empty,
                        filtro.DataFim != null ? filtro.DataInicio.ToString() : string.Empty,
                        (uint)filtro.IdProduto.GetValueOrDefault(),
                        filtro.AlturaMinima.GetValueOrDefault(),
                        filtro.AlturaMaxima.GetValueOrDefault(),
                        filtro.LarguraMinima.GetValueOrDefault(),
                        filtro.LarguraMaxima.GetValueOrDefault(),
                        (uint)filtro.IdOrigemTrocaDevolucao.GetValueOrDefault(),
                        (uint)filtro.IdTipoPerda.GetValueOrDefault(),
                        filtro.IdSetor.GetValueOrDefault(),
                        filtro.TipoPedido ?? string.Empty,
                        filtro.IdGrupoProduto,
                        filtro.IdSubgrupoProduto.GetValueOrDefault()));
            }
        }
    }
}
