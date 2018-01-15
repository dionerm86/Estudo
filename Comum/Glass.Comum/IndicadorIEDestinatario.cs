using System.ComponentModel;

namespace Glass
{
    /// <summary>
    /// Indicador da IE do Destinatário.
    /// </summary>
    public enum IndicadorIEDestinatario
    {
        /// <summary>
        /// Contribuinte ICMS (informar a IE do destinatário).
        /// </summary>
        [Description("Contribuinte ICMS")]
        ContribuinteICMS = 1,
        /// <summary>
        /// Contribuinte isento de Inscrição no cadastro de Contribuintes do ICMS.
        /// </summary>
        [Description("Contribuinte Isento")]
        ContribuinteIsento,
        /// <summary>
        /// Não Contribuinte, que pode ou não possuir Inscrição Estadual no Cadastro de Contribuintes do ICMS.
        /// </summary>
        [Description("Não Contribuinte")]
        NaoContribuinte = 9
    }
}
