// <copyright file="GetVolumesController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Volumes.V1
{
    /// <summary>
    /// Controller de volumes.
    /// </summary>
    public partial class VolumesController : BaseController
    {
        /// <summary>
        /// Recupera a lista de volumes.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos itens.</param>
        /// <returns>Uma lista JSON com os dados dos itens.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Volumes encontrados sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<Models.Volumes.V1.Lista.ListaDto>))]
        [SwaggerResponse(204, "Volumes não encontrados para o filtro informado.")]
        [SwaggerResponse(206, "Volumes paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<Models.Volumes.V1.Lista.ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterVolumes([FromUri] Models.Volumes.V1.Lista.FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new Models.Volumes.V1.Lista.FiltroDto();

                var volumes = VolumeDAO.Instance.GetList(
                    (uint)(filtro.IdVolume ?? 0),
                    (uint)(filtro.IdPedido ?? 0),
                    filtro.SituacoesVolume != null && filtro.SituacoesVolume.Any() ? string.Join(",", filtro.SituacoesVolume.Select(t => (int)t)) : null,
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros);

                return this.ListaPaginada(
                    volumes.Select(o => new Models.Volumes.V1.Lista.ListaDto(o)),
                    filtro,
                    () => VolumeDAO.Instance.GetListCount(
                        (uint)(filtro.IdVolume ?? 0),
                        (uint)(filtro.IdPedido ?? 0),
                        filtro.SituacoesVolume != null && filtro.SituacoesVolume.Any() ? string.Join(",", filtro.SituacoesVolume.Select(t => (int)t)) : null));
            }
        }

        /// <summary>
        /// Recupera a lista de situações de volume.
        /// </summary>
        /// <returns>Uma lista JSON com os dados básicos das situações de volume.</returns>
        [HttpGet]
        [Route("situacoes")]
        [SwaggerResponse(200, "Situações de volume encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Situações de volume não encontradas.")]
        public IHttpActionResult ObterSituacoesPedidoVolumes()
        {
            using (var sessao = new GDATransaction())
            {
                var situacoes = new ConversorEnum<Data.Model.Volume.SituacaoVolume>()
                    .ObterTraducao();

                return this.Lista(situacoes);
            }
        }
    }
}
