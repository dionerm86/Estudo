namespace Glass.Api.Seguranca.Implementacao
{
    /// <summary>
    /// Implementação da entidade de usuário do sistema
    /// </summary>
    public class Usuario : IUsuario
    {
        public int IdUsuario { get; set; }

        public string Nome { get; set; }

        public string Email { get; set; }
    }

    /// <summary>
    /// Implementação do fluxo de negocio da autenticacao de usuário
    /// </summary>
    public class AutenticacaoFluxo : IAutenticacaoFluxo
    {
        /// <summary>
        /// Autentica um usuário
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="senha"></param>
        /// <returns></returns>
        public IUsuario Autenticar(string usuario, string senha, bool cliente)
        {
            if (cliente)
            {
                var c = Data.DAL.ClienteDAO.Instance.Autenticacao(usuario, senha, string.Empty);
                var clienteInfo = Data.DAL.ClienteDAO.Instance.GetElement(c.IdCliente.Value);

                return new Usuario()
                {
                    IdUsuario = clienteInfo.IdCli,
                    Nome = clienteInfo.Nome
                };
            }

            var u = Data.DAL.FuncionarioDAO.Instance.Autenticacao(usuario, senha);
            var func = Data.DAL.FuncionarioDAO.Instance.GetElement(u.CodUser);

            return new Usuario()
            {
                IdUsuario = func.IdFunc,
                Nome = func.Nome,
                Email = func.Email
            };
        }

        /// <summary>
        /// Verifica a conexão com o BD
        /// </summary>
        /// <returns></returns>
        public bool TesteConexao()
        {
            var t = Data.DAL.FuncionarioDAO.Instance.Autenticacao("", "");
            return true;
        }
    }
}
