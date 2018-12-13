// <copyright file="DocumentosEDadosPessoaisDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Funcionarios.V1.Detalhe
{
    /// <summary>
    /// Classe com a implementação dos documentos para a busca de detalhe de funcionário.
    /// </summary>
    [DataContract(Name = "DocumentosEDadosPessoais")]
    public class DocumentosEDadosPessoaisDto : Comuns.DocumentosEDadosPessoaisDto<DocumentosEDadosPessoaisDto>
    {
    }
}
