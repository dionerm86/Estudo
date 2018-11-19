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
        [SwaggerResponse(200, "Trocas/devoluções encontradas sem paginação (apenas uma página de retorno) ou última página retornada", Type = typeof(IEnumerable<Models.Estoques.V1.Lista.ListaDto>))]
        [SwaggerResponse(204, "Trocas/devoluções não encontradas.")]
        [SwaggerResponse(206, "Trocas/devoluções paginadas (qualquer página, exceto a última).", Type = typeof(IEnumerable<Models.Estoques.V1.Lista.ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaTrocasDevolucoes([FromUri] Models.Estoques.V1.TrocasDevolucoes.Lista.FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new Models.Estoques.V1.TrocasDevolucoes.Lista.FiltroDto();

                var idsFuncionario = filtro.IdsFuncionario.Any() ? string.Join(",", filtro.IdsFuncionario) : string.Empty;
                var idsFuncionarioAssociadoCliente = filtro.IdsFuncionarioAssociadoCliente.Any() ? string.Join(",", filtro.IdsFuncionarioAssociadoCliente) : string.Empty;

                var trocasDevolucoes = TrocaDevolucaoDAO.Instance.GetList(
                    (uint)filtro.Id,
                    (uint)filtro.IdPedido,
                    filtro.Tipo,
                    /*filtro.Situacao*/ 0,
                    (uint)filtro.Idcliente,
                    filtro.NomeCliente,
                    idsFuncionario,
                    idsFuncionarioAssociadoCliente,
                    filtro.DataInicio.ToString(),
                    filtro.DataFim.ToString(),
                    (uint)filtro.IdProduto,
                    filtro.AlturaMinima,
                    filtro.AlturaMaxima,
                    filtro.LarguraMinima,
                    filtro.LarguraMaxima,
                    (uint)filtro.IdOrigemTrocaDevolucao,
                    (uint)filtro.IdTipoPerda,
                    (int)filtro.IdSetor,
                    filtro.TipoPedido.ToString(),
                    (int)filtro.IdGrupoProduto,
                    (int)filtro.IdSubgrupoProduto,
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros);

                return this.ListaPaginada(
                    trocasDevolucoes.Select(o => new Models.Estoques.V1.TrocasDevolucoes.Lista.ListaDto(o)),
                    filtro,
                    () => TrocaDevolucaoDAO.Instance.GetCount(
                        (uint)filtro.Id,
                        (uint)filtro.IdPedido,
                        filtro.Tipo,
                        /*filtro.Situacao*/ 0,
                        (uint)filtro.Idcliente,
                        filtro.NomeCliente,
                        idsFuncionario,
                        idsFuncionarioAssociadoCliente,
                        filtro.DataInicio.ToString(),
                        filtro.DataFim.ToString(),
                        (uint)filtro.IdProduto,
                        filtro.AlturaMinima,
                        filtro.AlturaMaxima,
                        filtro.LarguraMinima,
                        filtro.LarguraMaxima,
                        (uint)filtro.IdOrigemTrocaDevolucao,
                        (uint)filtro.IdTipoPerda,
                        (int)filtro.IdSetor,
                        filtro.TipoPedido.ToString(),
                        (int)filtro.IdGrupoProduto,
                        (int)filtro.IdSubgrupoProduto));
            }
        }
    }
}
