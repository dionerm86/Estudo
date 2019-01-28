// <copyright file="GetArquivosRemessaController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper;
using Glass.API.Backend.Models.ArquivosRemessa.V1.Lista;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Glass.Data.Model;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.ArquivosRemessa.V1
{
    /// <summary>
    /// Controller de arquivos de remessa.
    /// </summary>
    public partial class ArquivosRemessaController : BaseController
    {
        /// <summary>
        /// Recupera a lista de arquivos de remessa.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos arquivos de remessa.</param>
        /// <returns>Uma lista JSON com os dados dos arquivos de remessa.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Arquivos de remessa encontrados sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Arquivos de remessa não encontrados.")]
        [SwaggerResponse(206, "Arquivos de remessa paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        public IHttpActionResult ObterListaArquivosRemessa([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new FiltroDto();

                var arquivosRemessa = ArquivoRemessaDAO.Instance.GetList(
                    filtro.Id ?? 0,
                    (uint)(filtro.NumeroArquivoRemessa ?? 0),
                    filtro.PeriodoCadastroInicio?.ToShortDateString(),
                    filtro.PeriodoCadastroFim?.ToShortDateString(),
                    filtro.Tipo,
                    filtro.IdContaBanco ?? 0,
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros);

                return this.ListaPaginada(
                    arquivosRemessa.Select(ar => new ListaDto(ar)),
                    filtro,
                    () => ArquivoRemessaDAO.Instance.GetCount(
                        filtro.Id ?? 0,
                        (uint)(filtro.NumeroArquivoRemessa ?? 0),
                        filtro.PeriodoCadastroInicio?.ToShortDateString(),
                        filtro.PeriodoCadastroFim?.ToShortDateString(),
                        (int)(filtro.Tipo ?? 0),
                        filtro.IdContaBanco ?? 0));
            }
        }

        /// <summary>
        /// Recupera os tipos de arquivos de remessa para uso no controle.
        /// </summary>
        /// <returns>Um objeto JSON com os tipos de arquivos de remessa.</returns>
        [HttpGet]
        [Route("tipos")]
        [SwaggerResponse(200, "Tipos de arquivos de remessa encontrados.", Type = typeof(IdNomeDto))]
        [SwaggerResponse(204, "Tipos de arquivos de remessa não encontrados.", Type = typeof(IdNomeDto))]
        public IHttpActionResult ObterTiposArquivosRemessa()
        {
            using (var sessao = new GDATransaction())
            {
                var tipos = new ConversorEnum<ArquivoRemessa.TipoEnum>()
                    .ObterTraducao();

                return this.Item(tipos);
            }
        }
    }
}