using System.ComponentModel;

namespace Glass.Negocios.Financeiro
{
    /// <summary>
    /// Possíveis tipos de plano de conta.
    /// </summary>
    public enum TipoPlanosConta
    {
        /// <summary>
        /// Crédito.
        /// </summary>
        [Description("Crédito")]
        Credito,
        /// <summary>
        /// Débito.
        /// </summary>
        [Description("Débito")]
        Debito
    }
}
