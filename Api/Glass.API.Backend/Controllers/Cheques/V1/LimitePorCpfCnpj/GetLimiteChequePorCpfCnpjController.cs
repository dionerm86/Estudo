// <copyright file="GetLimiteChequePorCpfCnpjController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Cheques.V1.LimitePorCpfCnpj.Lista;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WebGlass.Business.Cheque.Fluxo;

namespace Glass.API.Backend.Controllers.Cheques.V1.LimitePorCpfCnpj
{
    /// <summary>
    /// Controller de limite de cheques.
    /// </summary>
    public partial class LimiteChequePorCpfCnpjController : BaseController
    {
        /// <summary>
        /// Recupera as configurações usadas pela tela de listagem de limites de cheque por cpf/cnpj.
        /// </summary>
        /// <returns>Um objeto JSON com as configurações da tela.</returns>
        [HttpGet]
        [Route("configuracoes")]
        [SwaggerResponse(200, "Configurações recuperadas.", Type = typeof(Models.Cheques.V1.LimitePorCpfCnpj.Configuracoes.ListaDto))]
        public IHttpActionResult ObterConfiguracoesListaEstoque()
        {
            using (var sessao = new GDATransaction())
            {
                var configuracoes = new Models.Cheques.V1.LimitePorCpfCnpj.Configuracoes.ListaDto();
                return this.Item(configuracoes);
            }
        }

        /// <summary>
        /// Recupera a lista de limites de cheques.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos limites de cheques.</param>
        /// <returns>Uma lista JSON com os dados dos limites de cheques.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Limites de cheques por CPF/CNPJ encontrados sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Limites de cheques por CPF/CNPJ não encontrados.")]
        [SwaggerResponse(206, "Limites de cheques por CPF/CNPJ encontrados paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        public IHttpActionResult ObterListaLimiteCheques([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new FiltroDto();

                var limiteCheques = LimiteCheque.Instance.ObtemItens(
                    filtro.CpfCnpj,
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros).ToList();

                return this.ListaPaginada(
                    limiteCheques.Select(lc => new ListaDto(lc)),
                    filtro,
                    () => LimiteCheque.Instance.ObtemNumeroItens(filtro.CpfCnpj));
            }
        }
    }
}