// <copyright file="GetClientesController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Clientes;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Clientes.Filtro;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Clientes.V1
{
    /// <summary>
    /// Controller de clientes.
    /// </summary>
    public partial class ClientesController : BaseController
    {
        /// <summary>
        /// Recupera os clientes para o controle de pesquisa.
        /// </summary>
        /// <param name="id">O identificador do cliente para pesquisa.</param>
        /// <param name="nome">O nome do cliente para pesquisa.</param>
        /// <param name="tipoValidacao">O tipo de validação que será feita para a pesquisa.</param>
        /// <returns>Uma lista JSON com os dados dos clientes encontrados.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Clientes encontrados.", Type = typeof(IEnumerable<ClienteDto>))]
        [SwaggerResponse(204, "Clientes não encontrados.")]
        [SwaggerResponse(400, "Filtros não informados.", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterFiltro(int? id = null, string nome = null, string tipoValidacao = null)
        {
            if (id == null && string.IsNullOrWhiteSpace(nome))
            {
                return this.ErroValidacao("Pelo menos um filtro (id ou nome) deve ser informado.");
            }

            var estrategiaValidacao = ValidacaoFactory.ObterEstrategiaFiltro(this, tipoValidacao);

            using (var sessao = new GDATransaction())
            {
                var validacao = estrategiaValidacao.ValidarAntesBusca(sessao, id, nome);

                if (validacao != null)
                {
                    return validacao;
                }

                var clientes = ClienteDAO.Instance.ObterClientesPorIdENome(
                    sessao,
                    id.GetValueOrDefault(),
                    nome,
                    0,
                    10);

                clientes = clientes.ToList();
                validacao = estrategiaValidacao.ValidarDepoisBusca(sessao, id, nome, ref clientes);

                if (validacao != null)
                {
                    return validacao;
                }

                return this.Lista(clientes.Select(c => new ClienteDto(c, tipoValidacao)));
            }
        }
    }
}
