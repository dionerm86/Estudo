// <copyright file="PostBeneficiamentosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Beneficiamentos.Filtro;
using Glass.API.Backend.Models.Beneficiamentos.Total;
using Glass.API.Backend.Models.Genericas;
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
        public IHttpActionResult CalcularTotalBeneficiamentos([FromBody] IEnumerable<DadosEntradaDto> dadosEntrada)
        {
            if (dadosEntrada == null)
            {
                return this.ErroValidacao("Os dados de entrada são obrigatórios.");
            }

            var validacao = this.ValidarBeneficiamentosSelecionados(dadosEntrada);

            if (validacao != null)
            {
                return validacao;
            }

            var calculado = dadosEntrada.SelectMany(item =>
                item.ItensSelecionados.Select(itemSelecionado =>
                {
                    var total = item.CobrarBeneficiamento
                        ? ValorTotal.Instance.Calcular(item.DadosCalculo, item.Beneficiamento, itemSelecionado)
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

        private IHttpActionResult ValidarBeneficiamentosSelecionados(IEnumerable<DadosEntradaDto> dadosEntrada)
        {
            foreach (var item in dadosEntrada)
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
