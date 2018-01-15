using System;
using Colosoft;

namespace Glass.Negocios.Entidades.Seguranca
{
    /// <summary>
    /// Adaptação para a interface <see cref="IUser"/>.
    /// </summary>
    public class AdaptadorUsuario : Colosoft.Security.Authentication.IAutheticableUser
    {
        #region Variáveis Locais

        private Glass.Data.Model.Funcionario _funcionario;

        #endregion

        #region Propriedades

        /// <summary>
        /// Data de criação.
        /// </summary>
        public DateTimeOffset CreationDate
        {
            get { return _funcionario.DataCad; }
        }

        /// <summary>
        /// Email associado com o usuário.
        /// </summary>
        public string Email
        {
            get { return null; }
            set { }
        }

        /// <summary>
        /// Nome completo do usuário.
        /// </summary>
        public string FullName
        {
            get { return _funcionario.Nome; }
            set { }
        }

        /// <summary>
        /// Nome do provedor de identidade.
        /// </summary>
        public string IdentityProvider
        {
            get { return "Default"; }
            set { }
        }

        /// <summary>
        /// Identifica se é para ignorar a apresentação do captcha.
        /// </summary>
        public bool IgnoreCaptcha
        {
            get { return true; }
        }

        /// <summary>
        /// Identifica se o usuário está aprovado.
        /// </summary>
        public bool IsApproved
        {
            get { return IsActive; }
        }

        /// <summary>
        /// Data da última alteração de senha.
        /// </summary>
        public DateTimeOffset LastPasswordChangedDate
        {
            get { return DateTimeOffset.Now; }
            set { }
        }

        /// <summary>
        /// Pergunta para a senha.
        /// </summary>
        public string PasswordQuestion
        {
            get { return ""; }
            set { }
        }

        /// <summary>
        /// Resposta à pergunta para relembrar senha
        /// </summary>
        public string PasswordAnswer
        {
            get;
            set;
        }

        /// <summary>
        /// Chave do usuário.
        /// </summary>
        public string UserKey
        {
            get { return _funcionario.IdFunc.ToString(); }
        }

        /// <summary>
        /// Nome de acesso do usuário.
        /// </summary>
        public string UserName
        {
            get { return _funcionario.Login; }
            set { _funcionario.Login = value; }
        }

        /// <summary>
        /// Identificador do provedor de identidade associado.
        /// </summary>
        public int IdentityProviderId
        {
            get { return 0; }
            set { }
        }

        /// <summary>
        /// Identifica se o usuário está ativo.
        /// </summary>
        public bool IsActive
        {
            get
            {
                return _funcionario.Situacao == Situacao.Ativo;
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="funcionario"></param>
        public AdaptadorUsuario(Data.Model.Funcionario funcionario)
        {
            funcionario.Require("user").NotNull();
            _funcionario = funcionario;
        }

        #endregion
    }
}
