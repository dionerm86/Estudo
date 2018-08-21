// <copyright file="LoginUsuarioExpiracao.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System;

namespace Glass.Data.Helper
{
    /// <summary>
    /// Classe de suporte para o método de autorização para API de backend.
    /// </summary>
    public class LoginUsuarioExpiracao
    {
        /// <summary>
        /// Obtém ou define a data de expiração do token de acesso.
        /// </summary>
        public DateTime DataExpiracao { get; set; }

        /// <summary>
        /// Obtém ou define os dados do login para o token de acesso.
        /// </summary>
        public LoginUsuario Login { get; set; }
    }
}
