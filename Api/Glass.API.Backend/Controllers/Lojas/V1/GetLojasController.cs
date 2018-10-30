// <copyright file="GetLojasController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.API.Backend.Models.Lojas.V1.Certificado;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Lojas.V1
{
    /// <summary>
    /// Controller de lojas.
    /// </summary>
    public partial class LojasController : BaseController
    {
        /// <summary>
        /// Recupera as lojas para os controles de filtro das telas.
        /// </summary>
        /// <param name="ativas">Indica se apenas as lojas ativas devem ser retornadas.</param>
        /// <returns>Uma lista JSON com as lojas encontradas.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Lojas encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Lojas não encontradas.")]
        public IHttpActionResult ObterLojasParaFiltro(bool? ativas = null)
        {
            using (var sessao = new GDATransaction())
            {
                Situacao situacaoLojaBuscar = 0;

                if (ativas.HasValue)
                {
                    situacaoLojaBuscar = ativas.Value
                        ? Situacao.Ativo
                        : Situacao.Inativo;
                }

                var situacoes = LojaDAO.Instance.GetAll(sessao)
                    .Where(l => !ativas.HasValue || l.Situacao == situacaoLojaBuscar)
                    .Select(l => new IdNomeDto
                    {
                        Id = l.IdLoja,
                        Nome = l.NomeFantasia,
                    });

                return this.Lista(situacoes);
            }
        }

        /// <summary>
        /// Recupera a data de vencimento do certificado cadastrado na loja informada.
        /// </summary>
        /// <param name="id">O identificador da loja.</param>
        /// <returns>Data de vencimento do certificado.</returns>
        [HttpGet]
        [Route("{id}/obterDataVencimentoCertificado")]
        [SwaggerResponse(200, "Data de vencimento do certificado da loja recuperado.", Type = typeof(CertificadoDto))]
        [SwaggerResponse(204, "Loja não encontrada.")]
        [SwaggerResponse(400, "Erro de valor ou formato do campo id.", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterDataVencimentoCertificado(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarIdLoja(id);

                if (validacao != null)
                {
                    return validacao;
                }

                var loja = LojaDAO.Instance.GetElement(sessao, (uint)id);

                return this.Item(new CertificadoDto()
                {
                    DataVencimento = loja.DataVencimentoCertificado,
                    Vencido = loja.DataVencimentoCertificado?.Ticks < DateTime.Now.Ticks,
                    DiasParaVencimento = loja.DataVencimentoCertificado?.Subtract(DateTime.Now).Days ?? 0,
                });
            }
        }
    }
}
