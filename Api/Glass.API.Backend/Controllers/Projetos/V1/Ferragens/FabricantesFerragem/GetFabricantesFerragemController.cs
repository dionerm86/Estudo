// <copyright file="GetFabricantesFerragemController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.API.Backend.Models.Projetos.V1.Ferragens.FabricantesFerragem.Lista;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Projetos.V1.Ferragens.FabricantesFerragem
{
    /// <summary>
    /// Controller de fabricantes de ferragem.
    /// </summary>
    public partial class FabricantesFerragemController : BaseController
    {
        /// <summary>
        /// Recupera a lista de fabricantes de ferragem.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos fabricantes de ferragens.</param>
        /// <returns>Uma lista JSON com os dados dos fabricantes de ferragens.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Fabricantes de ferragens sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Fabricantes de ferragens não encontrados para o filtro informado.")]
        [SwaggerResponse(206, "Fabricantes de ferragens paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaSetores([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new FiltroDto();

                var setores = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Projeto.Negocios.IFerragemFluxo>()
                    .PesquisarFabricanteFerragem(null, null);

                ((Colosoft.Collections.IVirtualList)setores).Configure(filtro.NumeroRegistros);
                ((Colosoft.Collections.ISortableCollection)setores).ApplySort(filtro.ObterTraducaoOrdenacao());

                return this.ListaPaginada(
                    setores
                        .Skip(filtro.ObterPrimeiroRegistroRetornar())
                        .Take(filtro.NumeroRegistros)
                        .Select(c => new ListaDto(c)),
                    filtro,
                    () => setores.Count);
            }
        }

        /// <summary>
        /// Recupera os fabricantes de ferragem para os controles de filtro das telas.
        /// </summary>
        /// <returns>Uma lista JSON com os itens encontrados.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Fabricantes de ferragens encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Fabricantes de ferragens não encontrados.")]
        public IHttpActionResult ObterFabricantes()
        {
            using (var sessao = new GDATransaction())
            {
                var itens = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Projeto.Negocios.IFerragemFluxo>()
                    .ObterFabricantesFerragem()
                    .Select(f => new IdNomeDto
                    {
                        Id = f.Id,
                        Nome = f.Name,
                    });

                return this.Lista(itens);
            }
        }
    }
}
