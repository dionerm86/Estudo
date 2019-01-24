// <copyright file="GetAcertosChequesController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.AcertosCheques.V1.Lista;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.AcertosCheques.V1
{
    /// <summary>
    /// Controller de acertos de cheques.
    /// </summary>
    public partial class AcertosChequesController : BaseController
    {
        /// <summary>
        /// Recupera a lista de acertos de cheques.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos itens.</param>
        /// <returns>Uma lista JSON com os dados dos itens.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Acertos de cheques encontrados sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Acertos de cheques não encontrados.")]
        [SwaggerResponse(206, "Acertos de cheques paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        public IHttpActionResult ObterAcertosCheques([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new FiltroDto();

                var acertosCheques = AcertoChequeDAO.Instance.GetList(
                    (uint)(filtro.Id ?? 0),
                    (uint)(filtro.IdFuncionario ?? 0),
                    (uint)(filtro.IdCliente ?? 0),
                    filtro.NomeCliente,
                    filtro.PeriodoCadastroInicio?.ToShortDateString(),
                    filtro.PeriodoCadastroFim?.ToShortDateString(),
                    filtro.BuscarAcertosChequesProprios,
                    filtro.BuscarAcertosCaixaDiario,
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros);

                return this.ListaPaginada(
                    acertosCheques.Select(o => new ListaDto(o)),
                    filtro,
                    () => AcertoChequeDAO.Instance.GetListCount(
                        (uint)(filtro.Id ?? 0),
                        (uint)(filtro.IdFuncionario ?? 0),
                        (uint)(filtro.IdCliente ?? 0),
                        filtro.NomeCliente,
                        filtro.PeriodoCadastroInicio?.ToShortDateString(),
                        filtro.PeriodoCadastroFim?.ToShortDateString(),
                        filtro.BuscarAcertosChequesProprios,
                        filtro.BuscarAcertosCaixaDiario));
            }
        }
    }
}