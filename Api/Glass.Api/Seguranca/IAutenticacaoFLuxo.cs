namespace Glass.Api.Seguranca
{
    #region Entidades

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

    #endregion

    #region Fluxos

    /// <summary>
    /// Assinatura do fluxo de autenticação de usuários
    /// </summary>
    public interface IAutenticacaoFluxo
    {
        /// <summary>
        /// Autentica um usuário
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="senha"></param>
        /// <returns></returns>
        IUsuario Autenticar(string usuario, string senha);

        /// <summary>
        /// Verifica a conexão com o BD
        /// </summary>
        /// <returns></returns>
        bool TesteConexao();
    }

    #endregion
}
