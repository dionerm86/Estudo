// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System;
using System.Collections.Generic;
using static Glass.Data.Model.Pedido;

namespace Glass.API.Backend.Models.Producao.V1.Lista
{
    /// <summary>
    /// 
    /// </summary>
    public class FiltroDto
    {
        public int? IdPedido { get; set; }

        public int? IdLiberacaoPedido { get; set; }

        public int? IdCarregamento { get; set; }

        public int? IdPedidoImportado { get; set; }

        public string CodigoPedidoCliente { get; set; }

        public IEnumerable<int> IdsRotas { get; set; }

        public int? IdCliente { get; set; }

        public string NomeCliente { get; set; }

        public int? IdImpressao { get; set; }

        public string NumeroEtiquetaPeca { get; set; }

        public IEnumerable<SituacaoProducaoEnum> SituacoesProducao { get; set; }

        public int? IdSetor { get; set; }

        public DateTime? PeriodoSetorInicio { get; set; }

        public DateTime? PeriodoSetorFim { get; set; }

        public int? IdLoja { get; set; }

        public int MyProperty { get; set; }
    }
}
