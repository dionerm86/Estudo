// <copyright file="GetParcelasController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.API.Backend.Models.Parcelas.V1.Filtro;
using Glass.API.Backend.Models.Parcelas.V1.Lista;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Parcelas.V1
{
    /// <summary>
    /// Controller de parcelas.
    /// </summary>
    public partial class ParcelasController : BaseController
    {
        /// <summary>
        /// Recupera as configurações usadas pela tela de listagem de parcelas.
        /// </summary>
        /// <returns>Um objeto JSON com as configurações da tela.</returns>
        [HttpGet]
        [Route("configuracoes")]
        [SwaggerResponse(200, "Configurações recuperadas.", Type = typeof(Models.Parcelas.V1.Parcelas.Configuracoes.ListaDto))]
        public IHttpActionResult ObterConfiguracoesListaParcelas()
        {
            using (var sessao = new GDATransaction())
            {
                var configuracoes = new Models.Parcelas.V1.Parcelas.Configuracoes.ListaDto();
                return this.Item(configuracoes);
            }
        }

        /// <summary>
        /// Recupera a lista de parcelas para a tela de listagem.
        /// </summary>
        /// <param name="filtro">O filtro para a busca de condutores.</param>
        /// <returns>Uma lista JSON com os dados dos condutores.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Parcelas encontrados sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Parcelas não encontradas para o filtro informado.")]
        [SwaggerResponse(206, "Parcelas paginadas (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaParcelas([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                var parcelas = ParcelasDAO.Instance.GetAll();

                return this.ListaPaginada(
                    parcelas.Select(dao => new ListaDto(dao)),
                    filtro,
                    () => parcelas.Count());
            }
        }

        /// <summary>
        /// Recupera as parcelas do sistema para os controles de filtro das telas.
        /// </summary>
        /// <returns>Uma lista JSON com as parcelas encontradas.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Parcelas encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Parcelas não encontradas.")]
        public IHttpActionResult ObterParcelas()
        {
            using (var sessao = new GDATransaction())
            {
                var parcelas = ParcelasDAO.Instance.GetAll()
                    .Select(p => new IdNomeDto
                    {
                        Id = p.IdParcela,
                        Nome = p.Descricao,
                    });

                return this.Lista(parcelas);
            }
        }

        /// <summary>
        /// Recupera as parcelas que o cliente tem acesso para os controles de filtro das telas.
        /// </summary>
        /// <param name="idCliente">Identificador do cliente que será usado para buscar as parcelas.</param>
        /// <returns>Uma lista JSON com as parcelas encontradas.</returns>
        [HttpGet]
        [Route("cliente")]
        [SwaggerResponse(200, "Parcelas encontradas.", Type = typeof(IEnumerable<ParcelaDto>))]
        [SwaggerResponse(204, "Parcelas não encontradas.")]
        public IHttpActionResult ObterParcelasCliente(int? idCliente)
        {
            using (var sessao = new GDATransaction())
            {
                string msgErro;

                var parcelas = ParcelasDAO.Instance.GetForControleSelecionar((uint)idCliente.GetValueOrDefault(), 0, null, false, ParcelasDAO.TipoConsulta.Prazo, out msgErro)
                    .Where(p => p.Situacao == Situacao.Ativo)
                    .Select(p => new ParcelaDto
                    {
                        Id = p.IdParcela,
                        Nome = p.Descricao,
                        NumeroParcelas = p.NumParcelas,
                        Dias = this.ObterDias(p.Dias),
                    });

                return this.Lista(parcelas);
            }
        }

        private IEnumerable<int> ObterDias(string dias)
        {
            if (string.IsNullOrWhiteSpace(dias))
            {
                return new int[0];
            }

            return dias.Split(',')
                .Select(dia => dia.Trim().StrParaInt());
        }
    }
}
