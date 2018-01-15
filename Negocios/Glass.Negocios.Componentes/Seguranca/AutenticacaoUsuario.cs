using System;

namespace Glass.Negocios.Componentes.Seguranca
{
    /// <summary>
    /// Implementação da classe de autenticação do sistema.
    /// </summary>
    public class AutenticacaoUsuario : Colosoft.Security.IAuthenticate
    {
        /// <summary>
        /// Verifica se pode criar um novo usuário.
        /// </summary>
        public bool CanCreateUser
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Identifica se pode resetar a senha.
        /// </summary>
        public bool CanResetPassword
        {
            get { throw new NotImplementedException(); }
        }

        public Colosoft.Security.ChangePasswordResult ChangePassword(string userName, string oldPassword, string newPassword, params Colosoft.Security.SecurityParameter[] parameters)
        {
            throw new NotImplementedException();
        }

        public Colosoft.Security.CreateUserResult CreateNewUser(Colosoft.Security.IUser user, string password, params Colosoft.Security.SecurityParameter[] parameters)
        {
            throw new NotImplementedException();
        }

        public Colosoft.Security.ResetPasswordProcessResult RequestPasswordReset(string userName)
        {
            throw new NotImplementedException();
        }

        public Colosoft.Security.IValidateUserResult ValidateToken(string token)
        {
            throw new NotImplementedException();
        }

        public Colosoft.Security.IValidateUserResult ValidateUser(string userName, string password, params Colosoft.Security.SecurityParameter[] parameters)
        {
            /*var user = SourceContext.Instance.CreateQuery()
                .From<Data.Models.User>()
                .Where("Email=?email")
                .Add("?email", userName)
                .Execute<Data.Models.User>()
                .Where(f => Colosoft.Security.PasswordHash.ValidatePassword(password, f.Hash))
                .Select(f => new Usuario
                {
                    UserId = f.UserId,
                    CreationDate = f.CreatedDate,
                    Email = f.Email,
                    FullName = f.Name,
                    IdentityProvider = "Default",
                    IgnoreCaptcha = true,
                    IsApproved = f.IsApproved,
                    LastPasswordChangedDate = DateTime.Now,
                    PasswordQuestion = null,
                    UserKey = f.UserId.ToString(),
                    UserName = f.Name
                })
                .FirstOrDefault();

            if (user != null)
            {
                return new ValidateUserResult
                {
                    Status = Colosoft.Security.AuthenticationStatus.Success,
                    User = user,
                    Token = Guid.NewGuid().ToString()
                };
            }
            else
                return new ValidateUserResult
                {
                    Status = Colosoft.Security.AuthenticationStatus.InvalidUserNameOrPassword
                };*/
            throw new NotImplementedException();
        }

        #region Nested Types

        /// <summary>
        /// Armazena os dados do usuário.
        /// </summary>
        class Usuario : Colosoft.Security.IUser
        {
            /// <summary>
            /// Identificador do usuário.
            /// </summary>
            public int UserId { get; set; }

            /// <summary>
            /// Data de criação.
            /// </summary>
            public DateTimeOffset CreationDate { get; set; }

            /// <summary>
            /// Email.
            /// </summary>
            public string Email { get; set; }

            /// <summary>
            /// Nome completo.
            /// </summary>
            public string FullName { get; set; }

            /// <summary>
            /// Nome do provedor de identidade.
            /// </summary>
            public string IdentityProvider { get; set; }

            /// <summary>
            /// Identifica se é para ignorar o captcha.
            /// </summary>
            public bool IgnoreCaptcha { get; set; }

            /// <summary>
            /// Identifica se está aprovado.
            /// </summary>
            public bool IsApproved { get; set; }

            /// <summary>
            /// Data da ultima alteração de senha.
            /// </summary>
            public DateTimeOffset LastPasswordChangedDate { get; set; }

            /// <summary>
            /// Pergunta para recuperar a senha.
            /// </summary>
            public string PasswordQuestion { get; set; }

            /// <summary>
            /// Chave do usuário.
            /// </summary>
            public string UserKey { get; set; }

            /// <summary>
            /// Login do usuário
            /// </summary>
            public string UserName { get; set; }

            /// <summary>
            /// Identifica se o usuário está ativo.
            /// </summary>
            public bool IsActive { get; set; }
        }

        /// <summary>
        /// Resultado da validação do usuário.
        /// </summary>
        class ValidateUserResult : Colosoft.Security.IValidateUserResult
        {
            /// <summary>
            /// Captcha.
            /// </summary>
            public Colosoft.Security.CaptchaSupport.CaptchaInfo Captcha { get; set; }

            /// <summary>
            /// Data de expiração.
            /// </summary>
            public DateTimeOffset? ExpireDate { get; set; }

            /// <summary>
            /// Identifica se é um processo.
            /// </summary>
            public bool IsProcess { get; set; }

            /// <summary>
            /// Mensagem.
            /// </summary>
            public string Message { get; set; }

            /// <summary>
            /// Situação da validação.
            /// </summary>
            public Colosoft.Security.AuthenticationStatus Status { get; set; }

            /// <summary>
            /// Token associado.
            /// </summary>
            public string Token { get; set; }

            /// <summary>
            /// Dados do usuário associado.
            /// </summary>
            public Colosoft.Security.IUser User { get; set; }
        }

        #endregion
    }
}
