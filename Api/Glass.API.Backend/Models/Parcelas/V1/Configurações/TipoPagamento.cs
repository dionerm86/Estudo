// <copyright file="TipoPagamento.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System.ComponentModel;

namespace Glass.API.Backend.Models.Parcelas.V1.Configuracoes
{
    /// <summary>
    /// Enumerador com o tipo de pagamento da parcela.
    /// </summary>
    public enum TipoPagamento
    {
        /// <summary>
        /// À prazo.
        /// </summary>
        [Description("À prazo")]
        TipoPagamentoAPrazo = 0,

        /// <summary>
        /// À vista.
        /// </summary>
        [Description("À vista")]
        TipoPagamentoAVista = 1,
    }
}
