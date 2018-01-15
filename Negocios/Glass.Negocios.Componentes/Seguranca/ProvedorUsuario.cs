using Colosoft.Security;
using System;
using System.Linq;

namespace Glass.Negocios.Componentes.Seguranca
{
    /// <summary>
    /// Recupera as informações do provider utilizando o objeto Queryable
    /// </summary>
    public class ProvedorUsuario : Colosoft.Security.Authentication.Authentication
    {
        #region Properties

        /// <summary>
        /// Identifica se está habilitado resetar a senha.
        /// </summary>
        public override bool EnablePasswordReset
        {
            get { return true; }
        }

        /// <summary>
        /// Identifica se a recuperação de senha está habilitada.
        /// </summary>
        public override bool EnablePasswordRetrieval
        {
            get { return false; }
        }

        /// <summary>
        /// Quantidade minima de caracteres não alfanuméricos.
        /// </summary>
        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return 0; }
        }

        /// <summary>
        /// Nome do provedor.
        /// </summary>
        public override string Name
        {
            get { return this.GetType().FullName; }
        }

        /// <summary>
        /// Formato da senha.
        /// </summary>
        public override Colosoft.Security.PasswordFormat PasswordFormat
        {
            get { return Colosoft.Security.PasswordFormat.Hashed; }
        }

        /// <summary>
        /// Expressão regular usada para varifica se a senha é forte.
        /// </summary>
        public override string PasswordStrengthRegularExpression
        {
            get { return String.Empty; }
        }

        /// <summary>
        /// Identifica se é requerido pergunta e resposta.
        /// </summary>
        public override bool RequiresQuestionAndAnswer
        {
            get { return false; }
        }

        /// <summary>
        /// Número máximo de senhas inválidas.
        /// </summary>
        public override int MaxInvalidPasswordAttempts
        {
            get { return 3; }
        }

        /// <summary>
        /// Comprimento minimo requerido para a senha.
        /// </summary>
        public override int MinRequiredPasswordLength
        {
            get { return 4; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public ProvedorUsuario()
        {
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Recupera os dados do usuário.
        /// </summary>
        /// <param name="funcionario"></param>
        /// <returns></returns>
        private IUser GetUser(Data.Model.Funcionario funcionario)
        {
            if (funcionario != null)
                return new Entidades.Seguranca.AdaptadorUsuario(funcionario);

            return null;
        }

        #endregion

        /// <summary>
        /// Recupera o número de usuário online.
        /// </summary>
        /// <returns></returns>
        public override int GetNumberOfUsersOnline()
        {
            return 0;
        }

        /// <summary>
        /// Recupera os dados do usuário.
        /// </summary>
        /// <param name="username">Nome do usuário.</param>
        /// <param name="userIsOnline">Identifica se o usuário é para estar online.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public override Colosoft.Security.IUser GetUser(string username, bool userIsOnline)
        {
            var user = SourceContext.Instance.CreateQuery()
                .From<Data.Model.Funcionario>()
                .Where("Login == ?login").Add("?login", username)
                .Execute<Data.Model.Funcionario>()
                .FirstOrDefault();

            return GetUser(user);
        }

        /// <summary>
        /// Recupera os dados do usuário pela a sua chave.
        /// </summary>
        /// <param name="userKey">Valor da chave do usuário.</param>
        /// <param name="userIsOnline">Identifica se o usuário é para estar online.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public override IUser GetUserByKey(string userKey, bool userIsOnline)
        {
            int userId = 0;

            if (!int.TryParse(userKey, out userId))
                return null;

            var user = SourceContext.Instance.CreateQuery()
                .From<Data.Model.Funcionario>()
                .Where("IdFunc == ?idFunc")
                .Add("?idFunc", userId)
                .Execute<Data.Model.Funcionario>().FirstOrDefault();

            return GetUser(user);
        }

        /// <summary>
        /// Persiste o usuário na base
        /// </summary>
        /// <param name="user">Usuário a persistir</param>
        /// <returns></returns>
        protected override bool InsertUser(Colosoft.Security.IUser user)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Atualiza os dados do usuário.
        /// </summary>
        /// <param name="user"></param>
        public override void UpdateUser(Colosoft.Security.IUser user)
        {
        }

        /// <summary>
        /// Recupera os nomes de todos os provedores do identidade cadastrados no sistema.
        /// </summary>
        /// <returns></returns>
        public override string[] GetIdentityProviders()
        {
            return new string[] { "Default" };
        }
    }
}
