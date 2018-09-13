// <copyright file="FiltroLogCancelamentoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas;
using Glass.Data.Model;
using System;

namespace Glass.API.Backend.Models.Log.Cancelamento
{
    /// <summary>
    /// Classe com os dados do filtro para a lista de logs de cancelamento.
    /// </summary>
    public class FiltroLogCancelamentoDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroLogCancelamentoDto"/>.
        /// </summary>
        public FiltroLogCancelamentoDto()
            : base(item => null)
        {
        }

        /// <summary>
        /// Obtém ou define a tabela que contém o item.
        /// </summary>
        public LogCancelamento.TabelaCancelamento? Tabela { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do item no qual o log foi feito.
        /// </summary>
        public int IdItem { get; set; }

        /// <summary>
        /// Obtém ou define o nome do campo no qual o log foi feito.
        /// </summary>
        public string Campo { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do funcionário que realizou o cancelamento.
        /// </summary>
        public int? IdFuncionarioCancelamento { get; set; }

        /// <summary>
        /// Obtém ou define o período inicial que o cancelamento foi efetuado.
        /// </summary>
        public DateTime? PeriodoCancelamentoInicio { get; set; }

        /// <summary>
        /// Obtém ou define o período final que o cancelamento foi efetuado.
        /// </summary>
        public DateTime? PeriodoCancelamentoFim { get; set; }

        /// <summary>
        /// Obtém ou define o valor do campo cancelado.
        /// </summary>
        public string Valor { get; set; }
    }
}
