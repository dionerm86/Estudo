// <copyright file="GetFuncionariosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Genericas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Funcionarios.V1
{
    /// <summary>
    /// Controller de funcionários.
    /// </summary>
    public partial class FuncionariosController : BaseController
    {
        /// <summary>
        /// Obtém uma lista de funcionários que realizaram finalização de pedidos.
        /// </summary>
        /// <returns>Uma lista JSON com os dados básicos dos funcionários que finalizaram pedidos.</returns>
        [HttpGet]
        [Route("finalizacao")]
        [SwaggerResponse(200, "Funcionários de finalização encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Funcionários de finalização não encontrados.")]
        public IHttpActionResult ObterFuncionariosFinalizacao()
        {
            using (var sessao = new GDATransaction())
            {
                var funcionarios = FuncionarioDAO.Instance.GetFuncFin()
                    .Select(f => new IdNomeDto
                    {
                        Id = f.IdFunc,
                        Nome = f.Nome,
                    });

                return this.Lista(funcionarios);
            }
        }

        /// <summary>
        /// Obtém uma lista de vendedores.
        /// </summary>
        /// <param name="idVendedorAtual">Identificador do vendedor já selecionado no pedido/orçamento/PCP.</param>
        /// <param name="orcamento">O resultado deve considerar os emissores de orçamentos?</param>
        /// <returns>Uma lista JSON com os dados básicos dos vendedores.</returns>
        [HttpGet]
        [Route("vendedores")]
        [SwaggerResponse(200, "Vendedores encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Vendedores não encontrados.")]
        public IHttpActionResult ObterVendedores(int? idVendedorAtual = null, bool? orcamento = null)
        {
            using (var sessao = new GDATransaction())
            {
                var funcionarios = !orcamento.HasValue || !orcamento.Value
                    ? FuncionarioDAO.Instance.GetVendedores()
                    : FuncionarioDAO.Instance.GetVendedoresOrcamento(sessao, idVendedorAtual);

                var vendedores = funcionarios
                    .Select(f => new IdNomeDto
                    {
                        Id = f.IdFunc,
                        Nome = f.Nome,
                    })
                    .ToList();

                if (idVendedorAtual > 0 && !vendedores.Any(v => v.Id == idVendedorAtual))
                {
                    vendedores.Add(new IdNomeDto()
                    {
                        Id = idVendedorAtual.GetValueOrDefault(),
                        Nome = FuncionarioDAO.Instance.GetNome((uint)idVendedorAtual),
                    });
                }

                return this.Lista(vendedores);
            }
        }

        /// <summary>
        /// Obtém uma lista de funcionários que podem comprar na empresa.
        /// </summary>
        /// <returns>Uma lista JSON com os dados básicos dos funcionários que podem comprar na empresa.</returns>
        [HttpGet]
        [Route("compradores")]
        [SwaggerResponse(200, "Funcionários compradores encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Funcionários compradores não encontrados.")]
        public IHttpActionResult ObterFuncionariosCompradores()
        {
            using (var sessao = new GDATransaction())
            {
                var compradores = FuncionarioDAO.Instance.GetOrdered()
                    .Select(f => new IdNomeDto
                    {
                        Id = f.IdFunc,
                        Nome = f.Nome,
                    });

                return this.Lista(compradores);
            }
        }

        /// <summary>
        /// Obtém uma lista de medidores.
        /// </summary>
        /// <returns>Uma lista JSON com os dados básicos dos medidores.</returns>
        [HttpGet]
        [Route("medidores")]
        [SwaggerResponse(200, "Medidores encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Medidores não encontrados.")]
        public IHttpActionResult ObterMedidores()
        {
            using (var sessao = new GDATransaction())
            {
                var medidores = FuncionarioDAO.Instance.GetMedidores()
                    .Select(f => new IdNomeDto
                    {
                        Id = f.IdFunc,
                        Nome = f.Nome,
                    });

                return this.Lista(medidores);
            }
        }

        /// <summary>
        /// Obtém uma lista de liberadores de pedido.
        /// </summary>
        /// <returns>Uma lista JSON com os dados básicos dos liberadores de pedido.</returns>
        [HttpGet]
        [Route("liberadores")]
        [SwaggerResponse(200, "Liberadores de pedido encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Liberadores de pedido não encontrados.")]
        public IHttpActionResult ObterLiberadoresDePedido()
        {
            using (var sessao = new GDATransaction())
            {
                var liberadores = FuncionarioDAO.Instance.GetFuncLiberacao()
                    .Select(f => new IdNomeDto
                    {
                        Id = f.IdFunc,
                        Nome = f.Nome,
                    });

                return this.Lista(liberadores);
            }
        }

        /// <summary>
        /// Obtém uma lista de funcionários ativos associados à clientes.
        /// </summary>
        /// <returns>Uma lista JSON com os dados básicos dos funcionários ativos associados à clientes.</returns>
        [HttpGet]
        [Route("ativosAssociadosAClientes")]
        [SwaggerResponse(200, "Funcionários encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Funcionários não encontrados.")]
        public IHttpActionResult ObterAtivosAssociadosAClientes()
        {
            using (var sessao = new GDATransaction())
            {
                var funcionarios = FuncionarioDAO.Instance.GetAtivosAssociadosCliente()
                    .Select(f => new IdNomeDto
                    {
                        Id = f.IdFunc,
                        Nome = f.Nome,
                    });

                return this.Lista(funcionarios);
            }
        }

        /// <summary>
        /// Obtem a data de trabalho a ser considerada no pedido para o funcionário passado.
        /// </summary>
        /// <param name="id">O identificador do funcionário.</param>
        /// <returns>A data de trabalho do funcionário.</returns>
        [HttpGet]
        [Route("{id}/dataTrabalho")]
        [SwaggerResponse(200, "Data de trabalho encontrada.", Type = typeof(DataDto))]
        public IHttpActionResult ObterDataTrabalhoFuncionario(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var dataTrabalho = new DataDto
                {
                    Data = FuncionarioDAO.Instance.ObtemDataAtraso(sessao, (uint)id),
                };

                return this.Item(dataTrabalho);
            }
        }
    }
}
