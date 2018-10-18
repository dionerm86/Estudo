// <copyright file="ItemBeneficiamentoExtension.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Data.Model;
using System.Collections.Generic;
using System.Linq;

namespace Glass.API.Backend.Helper
{
    /// <summary>
    /// Classe com métodos de extensão para os itens de beneficiamento.
    /// </summary>
    internal static class ItemBeneficiamentoExtension
    {
        /// <summary>
        /// Obtém a lista de beneficiamentos convertida, a partir dos beneficiamentos da model.
        /// </summary>
        /// <param name="beneficiamentos">A lista de beneficiamentos da model.</param>
        /// <returns>A lista convertida de beneficiamentos.</returns>
        public static IEnumerable<ItemBeneficiamentoDto> ObterListaBeneficiamentos(this GenericBenefCollection beneficiamentos)
        {
            var lista = new List<ItemBeneficiamentoDto>();

            if (beneficiamentos != null)
            {
                foreach (var beneficiamento in beneficiamentos)
                {
                    lista.Add(new ItemBeneficiamentoDto
                    {
                        Id = (int)beneficiamento.IdBenefConfig,
                        Altura = beneficiamento.LapAlt + beneficiamento.BisAlt,
                        Largura = beneficiamento.LapLarg + beneficiamento.BisLarg,
                        Espessura = beneficiamento.EspBisote + beneficiamento.EspFuro,
                        Quantidade = beneficiamento.Qtd,
                        ValorUnitario = beneficiamento.ValorUnit,
                        ValorTotal = beneficiamento.Valor,
                        CustoTotal = beneficiamento.Custo,
                    });
                }
            }

            return lista;
        }

        /// <summary>
        /// Obtém a lista de beneficiamentos convertida, a partir dos dados do DTO.
        /// </summary>
        /// <param name="beneficiamentos">A lista de beneficiamentos do DTO.</param>
        /// <returns>A lista convertida de beneficiamentos.</returns>
        public static GenericBenefCollection ObterListaBeneficiamentos(this IEnumerable<ItemBeneficiamentoDto> beneficiamentos)
        {
            var configuracoesBeneficiamentos = BenefConfigDAO.Instance.GetForConfig();
            var lista = new GenericBenefCollection();

            if (beneficiamentos != null)
            {
                foreach (var beneficiamento in beneficiamentos)
                {
                    var configuracao = configuracoesBeneficiamentos
                        .FirstOrDefault(b => b.IdBenefConfig == beneficiamento.Id);

                    var convertido = new GenericBenef
                    {
                        IdBenefConfig = (uint)beneficiamento.Id,
                        Qtd = beneficiamento.Quantidade ?? 0,
                        ValorUnit = beneficiamento.ValorUnitario,
                        Valor = beneficiamento.ValorTotal,
                        Custo = beneficiamento.CustoTotal,
                    };

                    ConverterLapidacao(convertido, beneficiamento, configuracao);
                    ConverterBisote(convertido, beneficiamento, configuracao);

                    lista.Add(convertido);
                }
            }

            return lista;
        }

        private static void ConverterLapidacao(GenericBenef convertido, ItemBeneficiamentoDto beneficiamento, BenefConfig configuracao)
        {
            if (configuracao?.TipoControleParent == TipoControleBenef.Lapidacao)
            {
                convertido.LapAlt = beneficiamento.Altura ?? 0;
                convertido.LapLarg = beneficiamento.Largura ?? 0;
            }
        }

        private static void ConverterBisote(GenericBenef convertido, ItemBeneficiamentoDto beneficiamento, BenefConfig configuracao)
        {
            if (configuracao?.TipoControleParent == TipoControleBenef.Bisote)
            {
                convertido.BisAlt = beneficiamento.Altura ?? 0;
                convertido.BisLarg = beneficiamento.Largura ?? 0;
                convertido.EspBisote = (float?)beneficiamento.Espessura ?? 0;
            }
        }
    }
}
