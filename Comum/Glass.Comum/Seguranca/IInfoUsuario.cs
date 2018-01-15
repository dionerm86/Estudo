namespace Glass.Seguranca
{
    /// <summary>
    /// Armazena as informações do usuário do sistema.
    /// </summary>
    public interface IInfoUsuario
    {
        #region Propriedades

        /// <summary>
        /// Código do usuário.
        /// </summary>
        uint CodUser { get; }

        /// <summary>
        /// Identificador do cliente associado.
        /// </summary>
        uint? IdCliente { get; }

        /// <summary>
        /// Identificador da loja associada.
        /// </summary>
        uint? IdLoja { get; }

        /// <summary>
        /// Identifica se é um administrador.
        /// </summary>
        bool IsAdministrador { get; }

        /// <summary>
        /// Identifica se é uma administrador da sync.
        /// </summary>
        bool IsAdminSync { get; }

        /// <summary>
        /// Identifica se é um caixa diário.
        /// </summary>
        bool IsCaixaDiario { get; }

        /// <summary>
        /// Identifica se é um cliente.
        /// </summary>
        bool IsCliente { get; }

        /// <summary>
        /// Identifica se é um financeiro geral.
        /// </summary>
        bool IsFinanceiroGeral { get; }

        bool IsFinanceiroPagto { get; }

        bool IsFinanceiroReceb { get; }

        /// <summary>
        /// Nome do usuário.
        /// </summary>
        string Nome { get; }

        /// <summary>
        /// Nome da loja associada.
        /// </summary>
        string NomeLoja { get; }

        /// <summary>
        /// Tipo do usuário.
        /// </summary>
        TipoFuncionario TipoUsuario { get; }

        /// <summary>
        /// Estado a loja associada.
        /// </summary>
        string UfLoja { get; set; }

        #endregion

        #region Métodos

        /// <summary>
        /// Verifica a permissão do usuário.
        /// </summary>
        /// <param name="modulo"></param>
        /// <returns></returns>
        bool VerificarPermissao(ModuloIndividual modulo);

        #endregion
    }
}
