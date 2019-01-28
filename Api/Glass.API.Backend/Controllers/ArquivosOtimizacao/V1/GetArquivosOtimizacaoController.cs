// <copyright file="GetArquivosOtimizacaoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper;
using Glass.API.Backend.Models.ArquivosOtimizacao.V1;
using Glass.API.Backend.Models.ArquivosOtimizacao.V1.Lista;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.ArquivosOtimizacao.V1
{
    /// <summary>
    /// Controller de arquivos de otimização.
    /// </summary>
    public partial class ArquivosOtimizacaoController : BaseController
    {
        /// <summary>
        /// Recupera as configurações usadas pela tela de listagem de arquivos de otimização.
        /// </summary>
        /// <returns>Um objeto JSON com as configurações da tela.</returns>
        [HttpGet]
        [Route("configuracoes")]
        [SwaggerResponse(200, "Configurações recuperadas.", Type = typeof(Models.ArquivosOtimizacao.V1.Configuracoes.ListaDto))]
        public IHttpActionResult ObterConfiguracoes()
        {
            using (var sessao = new GDATransaction())
            {
                var configuracoes = new Models.ArquivosOtimizacao.V1.Configuracoes.ListaDto();
                return this.Item(configuracoes);
            }
        }

        /// <summary>
        /// Recupera a lista de arquivos de otimização.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos arquivos de otimização.</param>
        /// <returns>Uma lista JSON com os dados dos arquivos de otimização.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Arquivos de otimização encontrados sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Arquivos de otimização não encontrados.")]
        [SwaggerResponse(206, "Arquivos de otimização paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        public IHttpActionResult ObterListaArquivosOtimizacao([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new FiltroDto();

                var arquivosOtimizacao = ArquivoOtimizacaoDAO.Instance.GetList(
                    (uint)(filtro.IdFuncionario ?? 0),
                    filtro.PeriodoCadastroInicio?.ToString(),
                    filtro.PeriodoCadastroFim?.ToString(),
                    filtro.Direcao ?? 0,
                    (uint)(filtro.IdPedido ?? 0),
                    filtro.CodigoEtiqueta,
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros);

                return this.ListaPaginada(
                    arquivosOtimizacao.Select(a => new ListaDto(a)),
                    filtro,
                    () => ArquivoOtimizacaoDAO.Instance.GetCount(
                        (uint)(filtro.IdFuncionario ?? 0),
                        filtro.PeriodoCadastroInicio?.ToString(),
                        filtro.PeriodoCadastroFim?.ToString(),
                        filtro.Direcao ?? 0,
                        (uint)(filtro.IdPedido ?? 0),
                        filtro.CodigoEtiqueta));
            }
        }

        /// <summary>
        /// Recupera a lista de direções para os arquivos de otimização.
        /// </summary>
        /// <returns>Uma lista JSON com os dados para uso no controle de direções.</returns>
        [HttpGet]
        [Route("direcoes")]
        [SwaggerResponse(200, "Direções encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Direções não encontradas.")]
        public IHttpActionResult ObterDirecao()
        {
            using (var sessao = new GDATransaction())
            {
                var direcoes = new ConversorEnum<Data.Model.ArquivoOtimizacao.DirecaoEnum>()
                    .ObterTraducao();

                return this.Lista(direcoes);
            }
        }
    }
}
