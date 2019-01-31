// <copyright file="GetMovimentacoesLiberacoesController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Liberacoes.V1.Movimentacoes.Lista;
using Glass.Data.RelDAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Liberacoes.V1.Movimentacoes
{
    /// <summary>
    /// Controller de movimentações de liberações.
    /// </summary>
    public partial class MovimentacoesLiberacoesController : BaseController
    {
        /// <summary>
        /// Recupera a lista de movimentações de liberações.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca das movimentações de liberações.</param>
        /// <returns>Uma lista JSON com os dados das movimentações de liberações.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Movimentações de liberações encontradas sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Movimentações de liberações encontradas não encontradas.")]
        [SwaggerResponse(206, "Movimentações de liberações paginadas (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        public IHttpActionResult ObterListaMovimentacoesLiberacoes([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new FiltroDto();

                var movimentacoes = LiberarPedidoMovDAO.Instance.GetList(
                    (uint)(filtro.IdCliente ?? 0),
                    filtro.NomeCliente,
                    (uint)(filtro.IdFuncionario ?? 0),
                    filtro.PeriodoCadastroInicio?.ToShortDateString(),
                    filtro.PeriodoCadastroFim?.ToShortDateString(),
                    filtro.Situacao ?? 0,
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros);

                return this.ListaPaginada(
                    movimentacoes.Select(ml => new ListaDto(ml)),
                    filtro,
                    () => LiberarPedidoMovDAO.Instance.GetListCount(
                        (uint)(filtro.IdCliente ?? 0),
                        filtro.NomeCliente,
                        (uint)(filtro.IdFuncionario ?? 0),
                        filtro.PeriodoCadastroInicio?.ToShortDateString(),
                        filtro.PeriodoCadastroFim?.ToShortDateString(),
                        filtro.Situacao ?? 0));
            }
        }
    }
}
