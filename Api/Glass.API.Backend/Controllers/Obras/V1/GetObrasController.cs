// <copyright file="GetObrasController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Obras.Filtro;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Obras.V1
{
    /// <summary>
    /// Controller de obras.
    /// </summary>
    public partial class ObrasController : BaseController
    {
        /// <summary>
        /// Recupera as obras para o controle de pesquisa.
        /// </summary>
        /// <param name="dadosEntrada">Os dados para pesquisa das obras.</param>
        /// <returns>Uma lista JSON com os dados das obras encontradas.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Obras encontradas.", Type = typeof(IEnumerable<ObraDto>))]
        [SwaggerResponse(204, "Obras não encontradas.")]
        [SwaggerResponse(400, "Filtros não informados.", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterFiltro([FromUri] DadosEntradaDto dadosEntrada)
        {
            if (dadosEntrada == null)
            {
                return this.ErroValidacao("Os parâmetros do método são obrigatórios.");
            }

            string idsPedidosIgnorar = dadosEntrada.IdsPedidosIgnorar != null
                ? string.Join(",", dadosEntrada.IdsPedidosIgnorar)
                : null;

            bool? gerarCredito = this.ObterFiltroGerarCredito(dadosEntrada.TipoObras);

            using (var sessao = new GDATransaction())
            {
                var obras = ObraDAO.Instance.GetList(
                    (uint?)dadosEntrada.IdCliente,
                    null,
                    0,
                    0,
                    0,
                    ((int?)dadosEntrada.Situacao).GetValueOrDefault(),
                    null,
                    null,
                    null,
                    null,
                    gerarCredito,
                    idsPedidosIgnorar,
                    (uint)(dadosEntrada.Id ?? 0),
                    0,
                    dadosEntrada.Descricao,
                    null,
                    0,
                    10);

                return this.Lista(obras.Select(o => new ObraDto(o)));
            }
        }

        private bool? ObterFiltroGerarCredito(TipoObrasFiltradas? tipoObras)
        {
            switch (tipoObras.GetValueOrDefault(TipoObrasFiltradas.Todas))
            {
                case TipoObrasFiltradas.GerarCredito:
                    return true;

                case TipoObrasFiltradas.PagamentoAntecipado:
                    return false;

                default:
                    return null;
            }
        }
    }
}
