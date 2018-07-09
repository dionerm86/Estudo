// <copyright file="TotalDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.Data.Beneficiamentos.Total.Dto
{
    /// <summary>
    /// Classe que encapsula o resultado do cálculo de valor total de beneficiamento.
    /// </summary>
    public class TotalDto
    {
        /// <summary>
        /// Obtém o valor unitário do beneficiamento.
        /// </summary>
        public decimal ValorUnitario { get; internal set; }

        /// <summary>
        /// Obtém o valor total do beneficiamento.
        /// </summary>
        public decimal ValorTotal { get; internal set; }

        /// <summary>
        /// Obtém o custo total do beneficiamento.
        /// </summary>
        public decimal CustoTotal { get; internal set; }
    }
}
