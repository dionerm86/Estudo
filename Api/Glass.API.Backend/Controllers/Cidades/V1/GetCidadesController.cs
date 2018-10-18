// <copyright file="GetCidadesController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Cidades.V1.Detalhes;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Cidades.V1
{
    /// <summary>
    /// Controller de cidades.
    /// </summary>
    public partial class CidadesController : BaseController
    {
        /// <summary>
        /// Recupera os detalhes de uma cidade.
        /// </summary>
        /// <param name="id">O identificador da cidade.</param>
        /// <returns>Um objeto JSON com os dados da cidade.</returns>
        [HttpGet]
        [Route("{id}")]
        [SwaggerResponse(200, "Cidade encontrada.", Type = typeof(DetalheDto))]
        [SwaggerResponse(400, "Erro de validação ou de formato do campo id.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Cidade não encontrada.", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterDetalhe(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarId(id);

                if (validacao != null)
                {
                    return validacao;
                }

                var cidade = CidadeDAO.Instance.GetElementByPrimaryKey(sessao, id);

                if (cidade == null)
                {
                    return this.NaoEncontrado(string.Format("Cidade {0} não encontrada.", id));
                }

                try
                {
                    return this.Item(new DetalheDto(cidade));
                }
                catch (Exception e)
                {
                    return this.ErroInternoServidor("Erro ao recuperar cidade.", e);
                }
            }
        }

        /// <summary>
        /// Recupera a lista de cidades, com filtro por UF e por nome da cidade.
        /// </summary>
        /// <param name="uf">A UF em que a busca será feita.</param>
        /// <param name="id">O identificador da cidade que está sendo buscada.</param>
        /// <param name="nome">O nome da cidade para o filtro.</param>
        /// <returns>Uma lista JSON com algumas das cidades encontradas.</returns>
        [HttpGet]
        [Route("porUf/{uf}")]
        [SwaggerResponse(200, "Cidades da UF encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Cidades da UF não encontradas.")]
        public IHttpActionResult ObterFiltro(string uf, int? id = null, string nome = null)
        {
            using (var sessao = new GDATransaction())
            {
                if (string.IsNullOrWhiteSpace(uf))
                {
                    return this.ErroValidacao("UF precisa ser informada.");
                }

                var cidades = id.HasValue
                    ? new[] { CidadeDAO.Instance.GetElementByPrimaryKey(sessao, id.Value) }
                    : CidadeDAO.Instance.GetList(sessao, uf, nome, "NomeCidade", 0, 10);

                return this.Lista(cidades.Select(c => new IdNomeDto
                {
                    Id = c.IdCidade,
                    Nome = c.NomeCidade,
                }));
            }
        }

        /// <summary>
        /// Recupera a lista de UFs cadastradas no sistema.
        /// </summary>
        /// <returns>Uma lista de strings com os códigos de UFs.</returns>
        [HttpGet]
        [Route("ufs")]
        [SwaggerResponse(200, "UFs encontradas.", Type = typeof(IEnumerable<string>))]
        [SwaggerResponse(204, "UFs não encontradas.")]
        public IHttpActionResult ObterUFs()
        {
            using (var sessao = new GDATransaction())
            {
                var ufs = CidadeDAO.Instance.GetUf(sessao)
                    .Select(uf => uf.Key);

                return this.Lista(ufs);
            }
        }
    }
}
