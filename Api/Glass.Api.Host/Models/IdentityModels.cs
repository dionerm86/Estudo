using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace Glass.Api.Host.Models
{
    /// <summary>
    /// Representa o usuário da aplicação.
    /// </summary>
    public class ApplicationUser : IUser
    {
        #region Variáveis Locais

        private Data.Helper.LoginUsuario _loginUsuario;

        #endregion

        #region Propriedades

        /// <summary>
        /// Dados do usuário do login.
        /// </summary>
        public Data.Helper.LoginUsuario Usuario
        {
            get { return _loginUsuario; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="loginUsuario"></param>
        public ApplicationUser(Data.Helper.LoginUsuario loginUsuario)
        {
            if (loginUsuario == null)
                throw new ArgumentNullException("loginUsuario");

            _loginUsuario = loginUsuario;
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Gera a identidade do usuário.
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="authenticationType"></param>
        /// <returns></returns>
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            userIdentity.AddClaim(new Claim(Glass.Seguranca.ClaimTypes.IdCliente, _loginUsuario.IdCliente.GetValueOrDefault().ToString(), System.Security.Claims.ClaimValueTypes.Integer32));
            userIdentity.AddClaim(new Claim(Glass.Seguranca.ClaimTypes.TipoUsuario, _loginUsuario.TipoUsuario.ToString(), System.Security.Claims.ClaimValueTypes.Integer32));
            userIdentity.AddClaim(new Claim(Glass.Seguranca.ClaimTypes.IdLoja, _loginUsuario.IdLoja.ToString(), System.Security.Claims.ClaimValueTypes.Integer32));

            return userIdentity;
        }

        #endregion

        #region IUser<string> Members

        /// <summary>
        /// Identificador do usuário.
        /// </summary>
        string IUser<string>.Id
        {
            get
            {
                return _loginUsuario.CodUser.ToString();
            }
        }

        /// <summary>
        /// Nome do usuário.
        /// </summary>
        public string UserName
        {
            get
            {
                return _loginUsuario.Nome;
            }

            set
            {
            }
        }

        #endregion
    }
}