namespace Glass.Seguranca
{
    /// <summary>
    /// Assinatura do contexto do usuário logado no sistema.
    /// </summary>
    public interface IContextoUsuario
    {
        /// <summary>
        /// Instancia com os dados do usuário logado no sistema.
        /// </summary>
        IInfoUsuario UsuarioAtual { get; set; }

        /// <summary>
        /// Verifica a permissão do usuário.
        /// </summary>
        /// <param name="modulo"></param>
        /// <returns></returns>
        bool VerificarPermissao(ModuloIndividual modulo);

        /// <summary>
        /// Verifica a permissão do usuário.
        /// </summary>
        /// <param name="idFuncionario"></param>
        /// <param name="modulo"></param>
        /// <returns></returns>
        bool VerificarPermissao(uint idFuncionario, ModuloIndividual modulo);

        /// <summary>
        /// Recupera a instancia do atual usuário logado no sistema.
        /// </summary>
        /// <returns></returns>
        IInfoUsuario ObterUsuarioAtual();
    }
}
