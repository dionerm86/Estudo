using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Seguranca
{
    public static class ClaimTypes
    {
        /// <summary>
        /// Representa o identificador do cliente.
        /// </summary>
        public const string IdCliente = "http://syncsoftwares.com.br/ws/2017/10/identity/claims/idcliente";

        /// <summary>
        /// Representa o tipo de usuário.
        /// </summary>
        public const string TipoUsuario = "http://syncsoftwares.com.br/ws/2017/10/identity/claims/tipousuario";

        /// <summary>
        /// Representa o identificador da loja.
        /// </summary>
        public const string IdLoja = "http://syncsoftwares.com.br/ws/2017/10/identity/claims/idloja";
    }
}
