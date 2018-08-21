// <copyright file="ItemBeneficiamentoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Genericas
{
    /// <summary>
    /// Classe que encapsula os dados de um item de beneficiamento.
    /// </summary>
    [DataContract(Name = "ItemBeneficiamento")]
    public class ItemBeneficiamentoDto : Data.Beneficiamentos.Total.Dto.ItemBeneficiamentoDto
    {
    }
}
