// <copyright file="EstadoCivil.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System.ComponentModel;

namespace Glass
{
    /// <summary>
    /// Enum que armazena os dados do estado civil.
    /// </summary>
   public enum EstadoCivil
    {
        /// <summary>
        /// Estado Civil Casado
        /// </summary>
        [Description("Casado(a)")]
        Casado = 1,

        /// <summary>
        /// Estado Civil Desquitado
        /// </summary>
        [Description("Desquitado(a)")]
        Desquitado,

        /// <summary>
        /// Estado Civil Viúvo
        /// </summary>
        [Description("Divorciado(a)")]
        Divociado,

        /// <summary>
        /// Estado Civil Solteiro
        /// </summary>
        [Description("Solteiro(a)")]
        Solteiro,

        /// <summary>
        /// Estado Civil Viúvo
        /// </summary>
        [Description("Viúvo(a)")]
        Viuvo,
    }
}
