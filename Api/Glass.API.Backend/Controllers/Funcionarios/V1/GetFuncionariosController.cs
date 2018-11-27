// <copyright file="GetFuncionariosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
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
        /// Recupera as configurações usadas pela tela de listagem de pedidos.
        /// </summary>
        /// <param name="id">O identificador do pedido.</param>
        /// <returns>Um objeto JSON com as configurações da tela.</returns>
        [HttpGet]
        [Route("{id}/configuracoes")]
        [SwaggerResponse(200, "Configurações recuperadas.", Type = typeof(Models.Funcionarios.V1.Configuracoes.DetalheDto))]
        public IHttpActionResult ObterConfiguracoesDetalhePedido(int id)
        {
            var configuracoes = new Models.Funcionarios.V1.Configuracoes.DetalheDto();
            return this.Item(configuracoes);
        }

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
        /// <param name="orcamento">O resultado deve considerar os emissores de orçamentos?.</param>
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
        /// Obtém uma lista de conferentes.
        /// </summary>
        /// <returns>Uma lista JSON com os dados básicos dos conferentes.</returns>
        [HttpGet]
        [Route("conferentes")]
        [SwaggerResponse(200, "Conferentes encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Conferentes não encontrados.")]
        public IHttpActionResult ObterConferentes()
        {
            using (var sessao = new GDATransaction())
            {
                var medidores = FuncionarioDAO.Instance.GetConferentesPCP()
                    .Select(f => new IdNomeDto
                    {
                        Id = f.IdFunc,
                        Nome = f.Nome,
                    });

                return this.Lista(medidores);
            }
        }

        /// <summary>
        /// Obtém uma lista de funcionários do caixa diário.
        /// </summary>
        /// <returns>Uma lista JSON com os dados básicos dos funcionários.</returns>
        [HttpGet]
        [Route("caixaDiario")]
        [SwaggerResponse(200, "Funcionários encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Funcionários não encontrados.")]
        public IHttpActionResult ObterFuncionariosCaixaDiario()
        {
            using (var sessao = new GDATransaction())
            {
                var funcionarios = FuncionarioDAO.Instance.GetCaixaDiario()
                    .Select(f => new IdNomeDto
                    {
                        Id = f.IdFunc,
                        Nome = f.Nome,
                    });

                return this.Lista(funcionarios);
            }
        }

        /// <summary>
        /// Obtém uma lista de funcionários do financeiro.
        /// </summary>
        /// <returns>Uma lista JSON com os dados básicos dos funcionários.</returns>
        [HttpGet]
        [Route("financeiros")]
        [SwaggerResponse(200, "Funcionários encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Funcionários não encontrados.")]
        public IHttpActionResult ObterFuncionariosFinanceiro()
        {
            using (var sessao = new GDATransaction())
            {
                var funcionarios = FuncionarioDAO.Instance.GetFinanceiros()
                    .Select(f => new IdNomeDto
                    {
                        Id = f.IdFunc,
                        Nome = f.Nome,
                    });

                return this.Lista(funcionarios);
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

        /// <summary>
        /// Recupera as configurações usadas pela tela de listagem de funcionários.
        /// </summary>
        /// <returns>Um objeto JSON com as configurações da tela.</returns>
        [HttpGet]
        [Route("configuracoes")]
        [SwaggerResponse(200, "Configurações encontradas.", Type = typeof(Models.Funcionarios.Configuracoes.ListaDto))]
        public IHttpActionResult ObterConfiguracoesListaFuncionarios()
        {
            using (var sessao = new GDATransaction())
            {
                var configuracoes = new Models.Funcionarios.Configuracoes.ListaDto();
                return this.Item(configuracoes);
            }
        }

        /// <summary>
        /// Recupera a lista de funcionários.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos funcionários.</param>
        /// <returns>Uma lista JSON com os dados dos funcionários.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Funcionários sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<Models.Funcionarios.Lista.ListaDto>))]
        [SwaggerResponse(204, "Funcionários não encontradas para o filtro informado.")]
        [SwaggerResponse(206, "Funcionários paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<Models.Funcionarios.Lista.ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaFuncionarios([FromUri] Models.Funcionarios.Lista.FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new Models.Funcionarios.Lista.FiltroDto();

                var funcionarios = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Global.Negocios.IFuncionarioFluxo>()
                    .PesquisarFuncionarios(
                        filtro.IdLoja,
                        filtro.Nome,
                        filtro.Situacao,
                        filtro.ApenasRegistrados,
                        filtro.IdTipoFuncionario,
                        filtro.IdSetor,
                        filtro.PeriodoDataNascimentoInicio,
                        filtro.PeriodoDataNascimentoFim);

                ((Colosoft.Collections.IVirtualList)funcionarios).Configure(filtro.NumeroRegistros);
                ((Colosoft.Collections.ISortableCollection)funcionarios).ApplySort(filtro.ObterTraducaoOrdenacao());

                return this.ListaPaginada(
                    funcionarios
                        .Skip(filtro.ObterPrimeiroRegistroRetornar())
                        .Take(filtro.NumeroRegistros)
                        .Select(f => new Models.Funcionarios.Lista.ListaDto(f)),
                    filtro,
                    () => funcionarios.Count);
            }
        }

        /// <summary>
        /// Recupera a lista de tipos de funcionários.
        /// </summary>
        /// <returns>Uma lista JSON com os dados básicos de tipos de funcionários.</returns>
        [HttpGet]
        [Route("tiposFuncionario")]
        [SwaggerResponse(200, "Tipos encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Tipos não encontrados.")]
        public IHttpActionResult ObterTipos()
        {
            using (var sessao = new GDATransaction())
            {
                var tipos = TipoFuncDAO.Instance.GetAll(sessao)
                    .Select(s => new IdNomeDto
                    {
                        Id = s.IdTipoFuncionario,
                        Nome = s.Descricao,
                    });

                return this.Lista(tipos);
            }
        }

        /// <summary>
        /// Recupera a lista de tipos de funcionários.
        /// </summary>
        /// <returns>Uma lista JSON com os dados básicos de tipos de funcionários.</returns>
        [HttpGet]
        [Route("estadoscivis")]
        [SwaggerResponse(200, "Estados Civis encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Estados Civis não encontrados.")]
        public IHttpActionResult ObterEstadosCivis()
        {
            using (var sessao = new GDATransaction())
            {
                var tipos = new ConversorEnum<EstadoCivil>()
                    .ObterTraducao();

                return this.Lista(tipos);
            }
        }

        /// <summary>
        /// Recupera os detalhes de um funcionário.
        /// </summary>
        /// <param name="id">O identificador do funcionário.</param>
        /// <returns>Um objeto JSON com os dados do funcionário.</returns>
        [HttpGet]
        [Route("{id:int}")]
        [SwaggerResponse(200, "Funcionário encontrado.", Type = typeof(Models.Funcionarios.Detalhe.DetalheDto))]
        [SwaggerResponse(400, "Erro de validação ou de valor ou formato inválido do campo id.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Funcionário não encontrado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterFuncionario(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarIdFuncionario(id);

                if (validacao != null)
                {
                    return validacao;
                }

                var funcionarioFluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Glass.Global.Negocios.IFuncionarioFluxo>();

                var funcionario = funcionarioFluxo.ObtemFuncionario(id);

                if (funcionario == null)
                {
                    return this.NaoEncontrado(string.Format("Funcionário {0} não encontrado.", id));
                }

                try
                {
                    return this.Item(new Models.Funcionarios.Detalhe.DetalheDto(funcionario));
                }
                catch (Exception e)
                {
                    return this.ErroInternoServidor("Erro ao recuperar o funcionário.", e);
                }
            }
        }
    }
}
