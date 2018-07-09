// <copyright file="GetBeneficiamentosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Beneficiamentos.Configuracoes;
using Glass.API.Backend.Models.Beneficiamentos.Filtro;
using Glass.Data.DAL;
using Glass.Data.Model;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Beneficiamentos.V1
{
    /// <summary>
    /// Controller de beneficiamentos.
    /// </summary>
    public partial class BeneficiamentosController : BaseController
    {
        /// <summary>
        /// Recupera a lista de beneficiamentos para uso no controle.
        /// </summary>
        /// <param name="tipoBeneficiamentos">O tipo de beneficiamentos que o controle usará.</param>
        /// <returns>Uma lista JSON com os dados dos beneficiamentos.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Beneficiamentos encontrados.", Type = typeof(IEnumerable<BeneficiamentoDto>))]
        [SwaggerResponse(204, "Beneficiamentos não encontrados.", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterParaControle([FromUri] TipoBenef tipoBeneficiamentos)
        {
            using (var sessao = new GDATransaction())
            {
                var beneficiamentos = BenefConfigDAO.Instance.GetForControl(sessao, tipoBeneficiamentos, false);
                var precos = BenefConfigPrecoDAO.Instance.GetByIdBenefConfig(sessao, 0).ToList();

                var pais = beneficiamentos.Where(item => !item.IdParent.HasValue);

                return this.Lista(pais.Select(pai => this.ConverterBeneficiamentoParaDto(
                    pai,
                    beneficiamentos,
                    precos)));
            }
        }

        /// <summary>
        /// Recupera as configurações para o controle de beneficiamentos.
        /// </summary>
        /// <returns>Um objeto JSON com os dados dos tipos de controle.</returns>
        [HttpGet]
        [Route("configuracoes")]
        [SwaggerResponse(200, "Configurações encontradas.", Type = typeof(ControleDto))]
        [SwaggerResponse(204, "Configurações não encontradas.", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterConfiguracoes()
        {
            var configuracoes = new ControleDto();
            return this.Item(configuracoes);
        }

        private BeneficiamentoDto ConverterBeneficiamentoParaDto(
            BenefConfig beneficiamento,
            IEnumerable<BenefConfig> listaBeneficiamentos,
            IEnumerable<BenefConfigPreco> listaPrecos)
        {
            var filhos = listaBeneficiamentos.Where(filho => filho.IdParent == beneficiamento.IdBenefConfig);
            var preco = listaPrecos.FirstOrDefault(item => item.IdBenefConfig == beneficiamento.IdBenefConfig);

            return new BeneficiamentoDto
            {
                Id = beneficiamento.IdBenefConfig,
                Nome = beneficiamento.Descricao,
                TipoControle = beneficiamento.TipoControle,
                TipoCalculo = beneficiamento.TipoCalculo,
                CustoUnitario = preco?.Custo ?? 0,
                ValorAtacadoUnitario = preco?.ValorAtacado ?? 0,
                ValorBalcaoUnitario = preco?.ValorBalcao ?? 0,
                ValorObraUnitario = preco?.ValorObra ?? 0,
                PermitirCobrancaOpcional = beneficiamento.CobrancaOpcional,
                Filhos = filhos.Select(filho => this.ConverterBeneficiamentoParaDto(filho, listaBeneficiamentos, listaPrecos)),
            };
        }
    }
}
