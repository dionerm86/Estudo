namespace Glass.Api.Seguranca
{
    /// <summary>
    /// Assinatura da entidade do usuario do sistema
    /// </summary>
    public interface IUsuario
    {
        /// <summary>
        /// Identificador do Usuario
        /// </summary>
        int IdUsuario { get; set; }

        /// <summary>
        /// Nome
        /// </summary>
        string Nome { get; set; }

        /// <summary>
        /// E-mail
        /// </summary>
        string Email { get; set; }
    }
}
