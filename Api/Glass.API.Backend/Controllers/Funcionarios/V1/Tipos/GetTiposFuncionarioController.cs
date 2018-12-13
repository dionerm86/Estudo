// <copyright file="GetTiposFuncionarioController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Funcionarios.V1.Tipos;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Global.Negocios.Componentes;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Funcionarios.V1.Tipos
{
    /// <summary>
    /// Controller de tipos de funcionário.
    /// </summary>
    public partial class TiposFuncionarioController : BaseController
    {
        /// <summary>
        /// Recupera a lista de tipos de funcionário.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos tipos de funcionário.</param>
        /// <returns>Uma lista JSON com os dados dos tipos de funcionário.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Tipos de funcionário encontrados sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<Models.Funcionarios.V1.Tipos.ListaDto>))]
        [SwaggerResponse(204, "Tipos de funcionário não encontrados para o filtro informado.")]
        [SwaggerResponse(206, "Tipos de funcionário (qualquer página, exceto a última).", Type = typeof(IEnumerable<Models.Funcionarios.V1.Tipos.ListaDto>))]
        public IHttpActionResult ObterListaTiposFuncionario([FromUri] Models.Funcionarios.V1.Tipos.FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new Models.Funcionarios.V1.Tipos.FiltroDto();

                var tiposFuncionario = Microsoft.Practices.ServiceLocation.ServiceLocator.Current
                                       .GetInstance<Glass.Global.Negocios.IFuncionarioFluxo>()
                                       .PesquisarTiposFuncionario();

                ((Colosoft.Collections.IVirtualList)tiposFuncionario).Configure(filtro.NumeroRegistros);
                ((Colosoft.Collections.ISortableCollection)tiposFuncionario).ApplySort(filtro.ObterTraducaoOrdenacao());

                return this.ListaPaginada(
                    tiposFuncionario
                                   .Skip(filtro.ObterPrimeiroRegistroRetornar())
                                   .Take(filtro.NumeroRegistros)
                                   .Select(g => new ListaDto(g)),
                    filtro,
                    () => tiposFuncionario.Count);
            }
        }
    }
}