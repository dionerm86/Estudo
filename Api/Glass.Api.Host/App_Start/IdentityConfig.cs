using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Glass.Api.Host.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System.Threading.Tasks;

namespace Glass.Api.Host
{
    /// <summary>
    /// Implementação do armazem de usuários da aplicação.
    /// </summary>
    public class ApplicationUserStore : IUserStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>
    {
        #region Variáveis Locais

        private IPasswordHasher _passworkHasher;
        private Data.DAL.ClienteDAO _clienteDAO;

        #endregion

        #region Constructors

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="clienteDAO"></param>
        /// <param name="passwordHasher"></param>
        public ApplicationUserStore(Data.DAL.ClienteDAO clienteDAO, PasswordHasher passwordHasher)
        {
            _clienteDAO = clienteDAO;
            _passworkHasher = passwordHasher;
        }

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Recupera os dados do usuário da aplicação com base nos dados
        /// do cliente carregado.
        /// </summary>
        /// <param name="cliente"></param>
        /// <returns></returns>
        private static ApplicationUser GetProxy(Data.Model.Cliente cliente)
        {
            if (cliente == null)
                return null;

            return GetProxy(new Data.Helper.LoginUsuario
            {
                IdCliente = (uint)cliente.IdCli,
                Nome = cliente.Nome
            });
        }

        /// <summary>
        /// Recupera o proxy do usuário de aplicação associado com o login do usuário.
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        private static ApplicationUser GetProxy(Data.Helper.LoginUsuario usuario)
        {
            if (usuario == null)
                return null;

            return new ApplicationUser(usuario);
        }

        #endregion

        #region IUserStore Members

        /// <summary>
        /// Cria o usuário de forma assincrona.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task CreateAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Apaga o usuário de forma assincrona.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task DeleteAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Localiza o usuário pelo identificador informado.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<ApplicationUser> FindByIdAsync(string userId)
        {
            uint id = 0;

            if (uint.TryParse(userId, out id))
                return Task.FromResult(GetProxy(_clienteDAO.GetElement(id)));

            return Task.FromResult<ApplicationUser>(null);
        }

        /// <summary>
        /// Recupera o usuário pelo nome informaod.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public Task<ApplicationUser> FindByNameAsync(string userName)
        {
            return Task.FromResult(GetProxy(_clienteDAO.GetLoginUsuario(userName)));
        }

        public Task UpdateAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Libera a instancia.
        /// </summary>
        public void Dispose()
        {

        }

        #endregion

        #region IUserPasswordStore Members

        /// <summary>
        /// Define o hash da senha do usuário.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="passwordHash"></param>
        /// <returns></returns>
        public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Recupera o hash da senha do usuário.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<string> GetPasswordHashAsync(ApplicationUser user)
        {
            var senha = _clienteDAO.GetSenha(user.Usuario.IdCliente.GetValueOrDefault());

            if (!string.IsNullOrEmpty(senha))
                senha = _passworkHasher.HashPassword(senha);

            return Task.FromResult(senha);
        }

        /// <summary>
        /// Verifica se o usuário possui senha.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> HasPasswordAsync(ApplicationUser user)
        {
            return Task.FromResult(!string.IsNullOrEmpty(_clienteDAO.GetSenha(user.Usuario.IdCliente.GetValueOrDefault())));
        }

        #endregion
    }

    /// <summary>
    /// Implementação do gerenciador de usuários.
    /// </summary>
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="store"></param>
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Cria a instancia do gerenciador.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var passwordHasher = new Microsoft.AspNet.Identity.PasswordHasher();
            var manager = new ApplicationUserManager(new ApplicationUserStore(Data.DAL.ClienteDAO.Instance, passwordHasher));
            manager.PasswordHasher = passwordHasher;

            // Configura a lógica de validação para os nomes de usuário
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configura a lógica de validação para as senhas
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }

            return manager;
        }

        #endregion
    }
}