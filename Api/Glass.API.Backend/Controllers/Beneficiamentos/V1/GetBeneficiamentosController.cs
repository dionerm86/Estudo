// <copyright file="GetBeneficiamentosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Beneficiamentos.V1.Configuracoes;
using Glass.API.Backend.Models.Beneficiamentos.V1.Filtro;
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
            var precos = listaPrecos.Where(item => item.IdBenefConfig == beneficiamento.IdBenefConfig);

            return new BeneficiamentoDto
            {
                Id = beneficiamento.IdBenefConfig,
                Nome = beneficiamento.Descricao,
                TipoControle = beneficiamento.TipoControle,
                TipoCalculo = beneficiamento.TipoCalculo,
                PermitirCobrancaOpcional = beneficiamento.CobrancaOpcional,
                CalculoPorEspessura = beneficiamento.TipoEspessura != TipoEspessuraBenef.ItemNaoPossui,
                Precos = precos.Select(preco => this.ConverterPrecosParaDto(preco)),
                Filhos = filhos.Select(filho => this.ConverterBeneficiamentoParaDto(filho, listaBeneficiamentos, listaPrecos)),
            };
        }

        private Data.Beneficiamentos.Total.Dto.PrecoBeneficiamentoDto ConverterPrecosParaDto(
            BenefConfigPreco preco)
        {
            return new Data.Beneficiamentos.Total.Dto.PrecoBeneficiamentoDto
            {
                IdSubgrupo = preco.IdSubgrupoProd,
                IdCor = preco.IdCorVidro,
                Espessura = preco.Espessura,
                CustoUnitario = preco.Custo,
                ValorAtacadoUnitario = preco.ValorAtacado,
                ValorBalcaoUnitario = preco.ValorBalcao,
                ValorObraUnitario = preco.ValorObra,
            };
        }
    }
}
