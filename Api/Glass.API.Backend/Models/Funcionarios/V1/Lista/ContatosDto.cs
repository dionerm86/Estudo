// <copyright file="ContatosDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Funcionarios.V1.Lista
{
    /// <summary>
    /// Classe que encapsula dados de contato do funcionário.
    /// </summary>
    [DataContract(Name = "Contatos")]
    public class ContatosDto : Comuns.BaseContatosDto<ContatosDto>
    {
    }
}
