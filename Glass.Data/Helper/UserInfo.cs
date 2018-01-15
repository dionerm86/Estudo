using System;
using System.Collections.Generic;
using System.Web;
using Glass.Data.DAL;
using System.Web.Security;

namespace Glass.Data.Helper
{
    public static class UserInfo
    {
        private static object syncRoot = new object();
        internal static volatile List<LoginUsuario> _usuario = new List<LoginUsuario>();

        public static bool IsAdministrador(uint idFunc)
        {
            LoginUsuario l = FindUserInfo(idFunc, true);
            return l.IsAdministrador;
        }

        internal static LoginUsuario GetByIdFunc(uint idFunc)
        {
            return _usuario.Find(new Predicate<LoginUsuario>(
                delegate(LoginUsuario x)
                {
                    return x.CodUser == idFunc;
                }
            ));
        }

        internal static LoginUsuario GetByIdCliente(uint idCliente)
        {
            return _usuario.Find(new Predicate<LoginUsuario>(
                delegate(LoginUsuario x)
                {
                    return x.IdCliente == idCliente;
                }
            ));
        }

        public static void ZeraListaUsuariosLogados()
        {
            _usuario = new List<LoginUsuario>();
        }

        /// <summary>
        /// Captura as informações do usuário atualmente logado no sistema.
        /// </summary>
        internal static LoginUsuario FindUserInfo(uint codUser, bool adicionarNaoEncontrado)
        {
            return FindUserInfo(null, codUser, adicionarNaoEncontrado);
        }

        /// <summary>
        /// Captura as informações do usuário atualmente logado no sistema.
        /// </summary>
        internal static LoginUsuario FindUserInfo(GDA.GDASession sessao, uint codUser, bool adicionarNaoEncontrado)
        {
            try
            {
                if (HttpContext.Current == null || String.IsNullOrEmpty(HttpContext.Current.User.Identity.Name))
                    return null;

                if (_usuario == null)
                    _usuario = new List<LoginUsuario>();

                bool adicionar = false;
                LoginUsuario retorno = null;

                // Chamado 12793. Invertemos as condições para que, dessa forma, seja recuperado o login
                // do cliente somente se for preciso, caso não seja, recupera o login do funcionário. Suspeitamos que o
                // erro do chamado tenha ocorrido porque foi recuperado o login do cliente e por isso o funcionário ficou trocado.
                // Se for login de cliente, retorna classe de login com dados do cliente
                if (HttpContext.Current.User.Identity.Name.Contains("|cliente"))
                {
                    retorno = GetByIdCliente(codUser);

                    if (retorno == null && adicionarNaoEncontrado)
                    {
                        adicionar = true;
                        retorno = ClienteDAO.Instance.GetLogin(sessao, codUser);
                    }
                }
                else
                {
                    retorno = GetByIdFunc(codUser);

                    if (retorno == null && adicionarNaoEncontrado)
                    {
                        adicionar = true;
                        retorno = FuncionarioDAO.Instance.GetLogin(sessao, (int)codUser);
                    }
                }

                if (adicionar && retorno != null)
                    _usuario.Add(retorno);

                return retorno;
            }
            catch (Exception ex)
            {
                // Chamado 12793. Inserimos este log caso ocorra erro ao recuperar o login do usuário, para tentar resolver o 
                // problema de usuário trocado na produção, ao voltar o setor da peça, ao marcar a peça em um novo setor etc.
                ErroDAO.Instance.InserirFromException("Falha ao recuperar login (FindUserInfo).", ex, codUser);
                return null;
            }
        }

        /// <summary>
        /// Captura as informações do usuário atualmente logado no sistema.
        /// </summary>
        public static LoginUsuario GetUserInfo
        {
            get
            {
                if (HttpContext.Current == null || HttpContext.Current.User == null || HttpContext.Current.User.Identity == null || String.IsNullOrEmpty(HttpContext.Current.User.Identity.Name))
                    return null;

                var dadosLogin = HttpContext.Current.User.Identity.Name.Split(';')[0].Split('|');
                uint codUser = Conversoes.StrParaUint(dadosLogin[0]);

                var loginUsuario = FindUserInfo(codUser, true);

                if (dadosLogin.Length >= 3)
                {
                    loginUsuario.UsuarioSync = dadosLogin[2];

                    if (dadosLogin.Length >= 4)
                        loginUsuario.PodeAlterarConfiguracao = dadosLogin[3].ToLower() == "true";
                }
                // Os usuários que não são da Sync, devem poder alterar as configurações caso tenham acesso ao menu.
                else if (loginUsuario != null)
                    loginUsuario.PodeAlterarConfiguracao = Utils.IsLocalUrl(HttpContext.Current) || !loginUsuario.IsAdminSync;

                return loginUsuario;
            }
        }

        /// <summary>
        /// Inclui o usuário logado na lista de usuários logados ou caso o mesmo já esteja logado, 
        /// atualiza a data de última atividade
        /// </summary>
        public static void SetActivity()
        {
            if (HttpContext.Current == null || HttpContext.Current.Request.Url == null)
                return;

            LoginUsuario loginUsuario = GetUserInfo;

            if (loginUsuario == null)
            {
                FormsAuthentication.RedirectToLoginPage();
                HttpContext.Current.Response.End();
                return;
            }

            // Indica que o usuário fez o login, se necessário
            HttpContext.Current.Session["idUsuario"] = loginUsuario.CodUser;
            LoginSistemaDAO.Instance.Entrar(loginUsuario.CodUser, loginUsuario.UsuarioSync);

            // Se o usuário já estiver na lista de usuários, apenas atualiza a data de última atividade
            foreach (LoginUsuario login in _usuario)
                if ((!loginUsuario.IsCliente && login.CodUser == loginUsuario.CodUser) || (loginUsuario.IsCliente && login.IdCliente == loginUsuario.IdCliente))
                {
                    login.UltimaAtividade = DateTime.Now;
                    return;
                }

            // Se o usuário corrente não etiver na lista, atualiza a data da última atividade e inclui na lista
            loginUsuario.UltimaAtividade = DateTime.Now;
            _usuario.Add(loginUsuario);
        }

        /// <summary>
        /// Remove o usuário da lista de usuários logados
        /// </summary>
        public static void ClearActivity()
        {
            if (HttpContext.Current == null || HttpContext.Current.Request.Url == null)
                return;

            LoginUsuario loginUsuario = GetUserInfo;

            if (loginUsuario == null)
            {
                FormsAuthentication.RedirectToLoginPage();
                HttpContext.Current.Response.End();
                return;
            }

            // Indica que o usuário saiu do sistema, se necessário
            LoginSistemaDAO.Instance.Sair(loginUsuario.CodUser, loginUsuario.UsuarioSync, true);

            // Remove o usuário da lista de usuários
            for (int i = 0; i < _usuario.Count; i++)
                if (_usuario[i].CodUser == loginUsuario.CodUser)
                {
                    _usuario.RemoveAt(i);
                    return;
                }
        }
    }
}