// <copyright file="PostBeneficiamentosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Beneficiamentos.V1.Filtro;
using Glass.API.Backend.Models.Beneficiamentos.V1.Total;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.Beneficiamentos;
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
        /// Calcula o valor total dos beneficiamentos selecionados no controle.
        /// </summary>
        /// <param name="dadosEntrada">Os dados de entrada do endpoint.</param>
        /// <returns>Um objeto JSON com o valor total calculado.</returns>
        [HttpPost]
        [Route("total")]
        [SwaggerResponse(200, "Valor total dos beneficiamentos calculado.", Type = typeof(IEnumerable<TotalDto>))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        public IHttpActionResult CalcularTotalBeneficiamentos([FromBody] DadosEntradaDto dadosEntrada)
        {
            if (dadosEntrada == null)
            {
                return this.ErroValidacao("Os dados de entrada são obrigatórios.");
            }

            dadosEntrada.Beneficiamentos = dadosEntrada.Beneficiamentos ?? new DadosBeneficiamentosDto[0];
            var validacao = this.ValidarBeneficiamentosSelecionados(dadosEntrada.Beneficiamentos);

            if (validacao != null)
            {
                return validacao;
            }

            var calculado = dadosEntrada.Beneficiamentos.SelectMany(item =>
                item.ItensSelecionados.Select(itemSelecionado =>
                {
                    var total = item.CobrarBeneficiamento
                        ? ValorTotal.Instance.Calcular(dadosEntrada.DadosCalculo, item.Beneficiamento, itemSelecionado)
                        : new Data.Beneficiamentos.Total.Dto.TotalDto();

                    return new TotalDto
                    {
                        IdItemSelecionado = itemSelecionado.Id,
                        ValorUnitario = total.ValorUnitario.Arredondar(2),
                        ValorTotal = total.ValorTotal.Arredondar(2),
                        CustoTotal = total.CustoTotal.Arredondar(2),
                    };
                }));

            return this.Lista(calculado);
        }

        private IHttpActionResult ValidarBeneficiamentosSelecionados(IEnumerable<DadosBeneficiamentosDto> dadosBeneficiamentos)
        {
            foreach (var item in dadosBeneficiamentos)
            {
                if (item.Beneficiamento == null)
                {
                    return this.ErroValidacao("Beneficiamento não foi informado.");
                }

                if (item.ItensSelecionados == null)
                {
                    return this.ErroValidacao("Item selecionado não foi informado.");
                }

                foreach (var itemSelecionado in item.ItensSelecionados)
                {
                    var validarItem = this.ValidarItemSelecionadoEBeneficiamento(
                        itemSelecionado,
                        item.Beneficiamento);

                    if (validarItem != null)
                    {
                        return validarItem;
                    }
                }
            }

            return null;
        }

        private IHttpActionResult ValidarItemSelecionadoEBeneficiamento(
            ItemBeneficiamentoDto itemSelecionado,
            Data.Beneficiamentos.Total.Dto.BeneficiamentoDto beneficiamento)
        {
            if (itemSelecionado.Id == beneficiamento.Id)
            {
                return null;
            }

            var filhos = beneficiamento.Filhos ?? new BeneficiamentoDto[0];

            foreach (var filho in filhos)
            {
                var validacao = this.ValidarItemSelecionadoEBeneficiamento(itemSelecionado, filho);

                if (validacao == null)
                {
                    return null;
                }
            }

            return this.ErroValidacao("Item selecionado não é do tipo do beneficiamento informado.");
        }
    }
}
